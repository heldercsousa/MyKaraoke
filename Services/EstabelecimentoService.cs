using MyVocaList.Domain;
using MyVocaList.Infra.Data.Repositories;
using MyVocaList.Infra.Utils;
using MyVocaList.Services.Mappers;
using MyVocaList.Contracts.DTOs.List;

namespace MyVocaList.Services
{
    /// <summary>
    /// Serviço para operações de negócio relacionadas a estabelecimentos/locais
    /// </summary>
    public class EstabelecimentoService : IEstabelecimentoService
    {
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;
        private readonly IEventoRepository _eventoRepository;
        private readonly ITextNormalizer _textNormalizer;

        // Constantes para validação
        public int MaxInputLength => 30;  // Limite conforme configuração EF
        public int ShowCounterAt => 25;   // Quando mostrar contador

        public EstabelecimentoService(
            IEstabelecimentoRepository estabelecimentoRepository,
            IEventoRepository eventoRepository,
            ITextNormalizer textNormalizer)
        {
            _estabelecimentoRepository = estabelecimentoRepository;
            _eventoRepository = eventoRepository;
            _textNormalizer = textNormalizer;
        }

        #region Validações

        public (bool isValid, string message) ValidateNameInput(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Venue name is required");
            }

            name = name.Trim();

            if (name.Length > MaxInputLength)
            {
                return (false, $"Name is too long. Maximum {MaxInputLength} characters.");
            }

            if (name.Length < 2)
            {
                return (false, "Name is too short. Minimum is 2 characters.");
            }

            return (true, "");
        }

        #endregion

        #region Operações CRUD

        public async Task<(bool success, string message, Estabelecimento? estabelecimento)> CreateEstabelecimentoAsync(string nome)
        {
            // Validação
            var validation = ValidateNameInput(nome);
            if (!validation.isValid)
            {
                return (false, validation.message, null);
            }

            try
            {
                nome = nome.Trim();

                // Verifica duplicação
                var existing = await _estabelecimentoRepository.GetByNomeAsync(nome);
                if (existing != null)
                {
                    return (false, "There is another venue registered with this name", null);
                }

                // Cria novo estabelecimento
                var estabelecimento = new Estabelecimento { Nome = nome };

                await _estabelecimentoRepository.AddAsync(estabelecimento);
                await _estabelecimentoRepository.SaveChangesAsync();

                return (true, $"Venue '{nome}' successfuly created!", estabelecimento);
            }
            catch (Exception ex)
            {
                return (false, $"Error while creating venue: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message)> UpdateEstabelecimentoAsync(int id, string novoNome)
        {
            // Validação
            var validation = ValidateNameInput(novoNome);
            if (!validation.isValid)
            {
                return (false, validation.message);
            }

            try
            {
                novoNome = novoNome.Trim();

                // Busca estabelecimento
                var estabelecimento = await _estabelecimentoRepository.GetByIdAsync(id);
                if (estabelecimento == null)
                {
                    return (false, "Venue not found");
                }

                // Verifica duplicação (exceto o próprio)
                var existing = await _estabelecimentoRepository.GetByNomeAsync(novoNome);
                if (existing != null && existing.Id != id)
                {
                    return (false, "There is another venue registered with this name");
                }

                // Atualiza
                estabelecimento.Nome = novoNome;
                await _estabelecimentoRepository.UpdateAsync(estabelecimento);
                await _estabelecimentoRepository.SaveChangesAsync();

                return (true, $"Venue name successfully updated to '{novoNome}'!");
            }
            catch (Exception ex)
            {
                return (false, $"Error while updating venue: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> DeleteEstabelecimentosAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return (false, "None venue was selected for removal.");
            }

            try
            {
                // Query otimizada com EXISTS
                var estabelecimentosWithEvents = await _estabelecimentoRepository.GetByIdsWithHasEventsAsync(ids);
                var validationResults = new List<(int id, string nome, bool canDelete, string reason)>();

                foreach (var (estabelecimento, hasEvents) in estabelecimentosWithEvents)
                {
                    validationResults.Add((estabelecimento.Id, estabelecimento.Nome, !hasEvents,
                        hasEvents ? "has registered events" : ""));
                }

                var cannotDelete = validationResults.Where(v => !v.canDelete).ToList();
                var canDelete = validationResults.Where(v => v.canDelete).ToList();

                if (canDelete.Any())
                {
                    var entitiesToDelete = estabelecimentosWithEvents
                        .Where(x => canDelete.Any(c => c.id == x.estabelecimento.Id))
                        .Select(x => x.estabelecimento);

                    await _estabelecimentoRepository.DeleteRangeAsync(entitiesToDelete);
                    await _estabelecimentoRepository.SaveChangesAsync();
                }

                return BuildDeleteResultMessage(canDelete, cannotDelete);
            }
            catch (Exception ex)
            {
                return (false, $"Error while removing venue: {ex.Message}");
            }
        }

        /// <summary>
        /// Constrói mensagem de resultado da exclusão seguindo padrão MD3 (count-based, não lista de nomes)
        /// </summary>
        private (bool success, string message) BuildDeleteResultMessage(
            List<(int id, string nome, bool canDelete, string reason)> canDelete,
            List<(int id, string nome, bool canDelete, string reason)> cannotDelete)
        {
            // ✅ MD3 PATTERN: Mensagens concisas com contagens, não listagens de nomes

            if (cannotDelete.Count == 0 && canDelete.Count > 0)
            {
                // Todos os selecionados foram excluídos com sucesso
                var count = canDelete.Count;
                return (true, count == 1
                    ? "1 venue successfully removed!"
                    : $"{count} venues successfully removed!");
            }
            else if (cannotDelete.Count > 0 && canDelete.Count > 0)
            {
                // Exclusão parcial: alguns excluídos, outros bloqueados
                var deleted = canDelete.Count;
                var blocked = cannotDelete.Count;
                var total = deleted + blocked;

                return (true, $"{deleted} of {total} successfully {(total == 1 ? "removed venue" : "removed venues")}. " +
                             $"{blocked} {(blocked == 1 ? "venue couldn´t be removed" : "venue couldn´t be removed")} " +
                             $"({(blocked == 1 ? "has" : "have")} events).");
            }
            else
            {
                // Nenhum pôde ser excluído (todos bloqueados)
                var count = cannotDelete.Count;
                return (false, count == 1
                    ? "The venue couldn´t be removed (has events)."
                    : $"The {count} venues couldn´t be removed (have events).");
            }
        }

        /// <summary>
        /// REATORADO: O método de exclusão única agora reutiliza a lógica de exclusão em lote.
        /// Isso garante que a regra de negócio seja a mesma e evita duplicação de código.
        /// </summary>
        public async Task<(bool success, string message)> DeleteEstabelecimentoAsync(int id)
        {
            // Simplesmente chama o novo método com uma coleção contendo um único ID.
            return await DeleteEstabelecimentosAsync(new[] { id });
        }

        public async Task<IEnumerable<Estabelecimento>> GetAllEstabelecimentosAsync()
        {
            Console.WriteLine("📋 === GetAllEstabelecimentosAsync INICIADO ===");

            try
            {
                Console.WriteLine($"📋 Repository disponível: {_estabelecimentoRepository != null}");

                if (_estabelecimentoRepository == null)
                {
                    Console.WriteLine("❌ Repository é NULL!");
                    return new List<Estabelecimento>();
                }

                Console.WriteLine("📋 Chamando GetAllAsync...");
                var estabelecimentos = await _estabelecimentoRepository.GetAllAsync();

                var list = estabelecimentos?.ToList() ?? new List<Estabelecimento>();
                Console.WriteLine($"📋 Estabelecimentos encontrados: {list.Count}");

                foreach (var est in list)
                {
                    Console.WriteLine($"📋 Encontrado: {est.Id} - '{est.Nome}'");
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar estabelecimentos: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return new List<Estabelecimento>();
            }
            finally
            {
                Console.WriteLine("📋 === GetAllEstabelecimentosAsync FINALIZADO ===");
            }
        }

        public async Task<Estabelecimento?> GetEstabelecimentoByIdAsync(int id)
        {
            try
            {
                return await _estabelecimentoRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar estabelecimento: {ex.Message}");
                return null;
            }
        }

        #endregion

        public async Task<IEnumerable<EstabelecimentoListItemDto>> GetAllEstabelecimentosForListAsync()
        {
            try
            {
                Console.WriteLine("📋 === GetAllEstabelecimentosForListAsync INICIADO ===");

                var estabelecimentosWithEvents = await _estabelecimentoRepository.GetAllWithHasEventsAsync();

                var result = estabelecimentosWithEvents.Select(x =>
                    EstabelecimentoMapper.ToListDto(x.estabelecimento, x.hasEvents)).ToList();

                Console.WriteLine($"📋 Total mapeados: {result.Count}");
                foreach (var item in result)
                {
                    Console.WriteLine($"📋 Mapeado: {item.Id} - '{item.Nome}' (HasEvents: {item.HasEvents})");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar estabelecimentos para lista: {ex.Message}");
                return new List<EstabelecimentoListItemDto>();
            }
        }

        public async Task<IEnumerable<EstabelecimentoListItemDto>> SearchEstabelecimentosForListAsync(string query)
        {
            try
            {
                var searchResults = await _estabelecimentoRepository.SearchWithHasEventsAsync(query);

                return searchResults.Select(x =>
                    EstabelecimentoMapper.ToListDto(x.estabelecimento, x.hasEvents)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao pesquisar estabelecimentos: {ex.Message}");
                return new List<EstabelecimentoListItemDto>();
            }
        }

        public async Task<(IEnumerable<EstabelecimentoListItemDto> items, int totalCount)> GetPagedEstabelecimentosForListAsync(
            int pageNumber,
            int pageSize,
            string? query = null)
        {
            try
            {
                Console.WriteLine($"📄 GetPagedEstabelecimentosForListAsync - Page {pageNumber}, Size {pageSize}, Query: '{query ?? "null"}'");

                var (items, totalCount) = await _estabelecimentoRepository.GetPagedWithHasEventsAsync(pageNumber, pageSize, query);

                var dtos = items.Select(x => EstabelecimentoMapper.ToListDto(x.estabelecimento, x.hasEvents)).ToList();

                Console.WriteLine($"📄 Returned {dtos.Count} items out of {totalCount} total");

                return (dtos, totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in GetPagedEstabelecimentosForListAsync: {ex.Message}");
                return (new List<EstabelecimentoListItemDto>(), 0);
            }
        }


        #region Utilitários

        public bool ShouldShowCharacterCounter(int currentLength)
        {
            return currentLength > ShowCounterAt;
        }

        public (string text, bool isWarning, bool isError) GetCharacterCounterInfo(int currentLength)
        {
            string text = $"{currentLength}/{MaxInputLength}";
            bool isWarning = currentLength > 27;
            bool isError = currentLength >= MaxInputLength;

            return (text, isWarning, isError);
        }

        #endregion
    }
}