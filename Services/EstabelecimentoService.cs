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
                return (false, "Nome do local é obrigatório");
            }

            name = name.Trim();

            if (name.Length > MaxInputLength)
            {
                return (false, $"Nome muito longo. Máximo {MaxInputLength} caracteres.");
            }

            if (name.Length < 2)
            {
                return (false, "Nome muito curto. Mínimo 2 caracteres.");
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
                    return (false, "Já existe um local com este nome", null);
                }

                // Cria novo estabelecimento
                var estabelecimento = new Estabelecimento { Nome = nome };

                await _estabelecimentoRepository.AddAsync(estabelecimento);
                await _estabelecimentoRepository.SaveChangesAsync();

                return (true, $"Local '{nome}' criado com sucesso!", estabelecimento);
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao criar local: {ex.Message}", null);
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
                    return (false, "Local não encontrado");
                }

                // Verifica duplicação (exceto o próprio)
                var existing = await _estabelecimentoRepository.GetByNomeAsync(novoNome);
                if (existing != null && existing.Id != id)
                {
                    return (false, "Já existe um local com este nome");
                }

                // Atualiza
                estabelecimento.Nome = novoNome;
                await _estabelecimentoRepository.UpdateAsync(estabelecimento);
                await _estabelecimentoRepository.SaveChangesAsync();

                return (true, $"Local alterado para '{novoNome}' com sucesso!");
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao atualizar local: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> DeleteEstabelecimentosAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return (false, "Nenhum local selecionado para exclusão.");
            }

            try
            {
                // Query otimizada com EXISTS
                var estabelecimentosWithEvents = await _estabelecimentoRepository.GetByIdsWithHasEventsAsync(ids);
                var validationResults = new List<(int id, string nome, bool canDelete, string reason)>();

                foreach (var (estabelecimento, hasEvents) in estabelecimentosWithEvents)
                {
                    validationResults.Add((estabelecimento.Id, estabelecimento.Nome, !hasEvents,
                        hasEvents ? "possui eventos registrados" : ""));
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
                return (false, $"Erro ao excluir locais: {ex.Message}");
            }
        }

        private (bool success, string message) BuildDeleteResultMessage(
            List<(int id, string nome, bool canDelete, string reason)> canDelete,
            List<(int id, string nome, bool canDelete, string reason)> cannotDelete)
        {
            if (cannotDelete.Count == 0 && canDelete.Count > 0)
            {
                var nomes = string.Join(", ", canDelete.Select(c => $"'{c.nome}'"));
                return (true, $"Local(is) {nomes} excluído(s) com sucesso!");
            }
            else if (cannotDelete.Count > 0 && canDelete.Count > 0)
            {
                var deletedNames = string.Join(", ", canDelete.Select(c => $"'{c.nome}'"));
                var blockedNames = string.Join(", ", cannotDelete.Select(c => $"'{c.nome}' ({c.reason})"));
                return (true, $"Excluídos: {deletedNames}.\nNão excluídos: {blockedNames}.");
            }
            else
            {
                var blockedNames = string.Join(", ", cannotDelete.Select(c => $"'{c.nome}' ({c.reason})"));
                return (false, $"Nenhum local pôde ser excluído:\n{blockedNames}");
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
            System.Diagnostics.Debug.WriteLine("📋 === GetAllEstabelecimentosAsync INICIADO ===");

            try
            {
                System.Diagnostics.Debug.WriteLine($"📋 Repository disponível: {_estabelecimentoRepository != null}");

                if (_estabelecimentoRepository == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Repository é NULL!");
                    return new List<Estabelecimento>();
                }

                System.Diagnostics.Debug.WriteLine("📋 Chamando GetAllAsync...");
                var estabelecimentos = await _estabelecimentoRepository.GetAllAsync();

                var list = estabelecimentos?.ToList() ?? new List<Estabelecimento>();
                System.Diagnostics.Debug.WriteLine($"📋 Estabelecimentos encontrados: {list.Count}");

                foreach (var est in list)
                {
                    System.Diagnostics.Debug.WriteLine($"📋 Encontrado: {est.Id} - '{est.Nome}'");
                }

                return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao buscar estabelecimentos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return new List<Estabelecimento>();
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("📋 === GetAllEstabelecimentosAsync FINALIZADO ===");
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
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar estabelecimento: {ex.Message}");
                return null;
            }
        }

        #endregion

        public async Task<IEnumerable<EstabelecimentoListItemDto>> GetAllEstabelecimentosForListAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("📋 === GetAllEstabelecimentosForListAsync INICIADO ===");

                var estabelecimentosWithEvents = await _estabelecimentoRepository.GetAllWithHasEventsAsync();

                var result = estabelecimentosWithEvents.Select(x =>
                    EstabelecimentoMapper.ToListDto(x.estabelecimento, x.hasEvents)).ToList();

                System.Diagnostics.Debug.WriteLine($"📋 Total mapeados: {result.Count}");
                foreach (var item in result)
                {
                    System.Diagnostics.Debug.WriteLine($"📋 Mapeado: {item.Id} - '{item.Nome}' (HasEvents: {item.HasEvents})");
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao buscar estabelecimentos para lista: {ex.Message}");
                return new List<EstabelecimentoListItemDto>();
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