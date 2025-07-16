using MyKaraoke.Domain;
using MyKaraoke.Domain.Repositories;
using MyKaraoke.Infra.Utils;

namespace MyKaraoke.Services
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
                var deletableIds = new List<int>();
                var nonDeletableCount = 0;

                foreach (var id in ids)
                {
                    var hasEvents = await _eventoRepository.HasEventsByEstabelecimentoAsync(id);
                    if (hasEvents)
                    {
                        nonDeletableCount++;
                    }
                    else
                    {
                        deletableIds.Add(id);
                    }
                }

                if (deletableIds.Any())
                {
                    // A busca das entidades para deletar precisa ser corrigida também,
                    // usando a sobrecarga que criamos anteriormente.
                    var entitiesToDelete = await _estabelecimentoRepository.GetAllAsync(e => deletableIds.Contains(e.Id));

                    if (entitiesToDelete.Any())
                    {
                        await _estabelecimentoRepository.DeleteRangeAsync(entitiesToDelete);
                        await _estabelecimentoRepository.SaveChangesAsync();
                    }
                }

                // CORREÇÃO: Declarar a variável 'message' aqui fora.
                string message;
                if (nonDeletableCount == 0 && deletableIds.Any())
                {
                    message = "Estabelecimento(s) excluído(s) com sucesso.";
                }
                else if (nonDeletableCount > 0 && deletableIds.Any())
                {
                    message = "Excluído(s) com sucesso. Alguns não puderam ser excluídos por terem outros dados associados.";
                }
                else // Apenas itens não deletáveis foram selecionados.
                {
                    message = "Não foi possível excluir o(s) registro(s) pois há outros dados associados.";
                }

                return (true, message);
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao excluir locais: {ex.Message}");
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
            try
            {
                return await _estabelecimentoRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar estabelecimentos: {ex.Message}");
                return new List<Estabelecimento>();
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