using MyVocaList.Domain;
using MyVocaList.Infra.Data.Repositories;
using MyVocaList.Infra.Utils;
using MyVocaList.Services.Mappers;
using MyVocaList.Contracts.DTOs.List;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MyVocaList.Services
{
    /// <summary>
    /// Service for business operations related to establishments/venues
    /// </summary>
    public class EstabelecimentoService : IEstabelecimentoService
    {
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;
        private readonly IEventoRepository _eventoRepository;
        private readonly ITextNormalizer _textNormalizer;
        private readonly ILogger<EstabelecimentoService> _logger;

        // Validation constants
        public int MaxInputLength => 30;  // Limit according to EF configuration
        public int ShowCounterAt => 25;   // When to show counter

        public EstabelecimentoService(
            IEstabelecimentoRepository estabelecimentoRepository,
            IEventoRepository eventoRepository,
            ITextNormalizer textNormalizer,
            ILogger<EstabelecimentoService> logger)
        {
            _estabelecimentoRepository = estabelecimentoRepository;
            _eventoRepository = eventoRepository;
            _textNormalizer = textNormalizer;
            _logger = logger;
        }

        #region Validation

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
            // Validation
            var validation = ValidateNameInput(nome);
            if (!validation.isValid)
            {
                return (false, validation.message, null);
            }

            nome = nome.Trim();

            // Check for duplicates
            var existing = await _estabelecimentoRepository.GetByNomeAsync(nome);
            if (existing != null)
            {
                return (false, "There is another venue registered with this name", null);
            }

            // Create new establishment
            var estabelecimento = new Estabelecimento { Nome = nome };

            await _estabelecimentoRepository.AddAsync(estabelecimento);
            await _estabelecimentoRepository.SaveChangesAsync();

            return (true, $"Venue '{nome}' successfully created!", estabelecimento);
        }

        public async Task<(bool success, string message)> UpdateEstabelecimentoAsync(int id, string novoNome)
        {
            Guard.AgainstNegativeOrZero(id, nameof(id));

            // Validation
            var validation = ValidateNameInput(novoNome);
            if (!validation.isValid)
            {
                return (false, validation.message);
            }

            novoNome = novoNome.Trim();

            // Find establishment
            var estabelecimento = await _estabelecimentoRepository.GetByIdAsync(id);
            if (estabelecimento == null)
            {
                return (false, "Venue not found");
            }

            // Check for duplicates (except itself)
            var existing = await _estabelecimentoRepository.GetByNomeAsync(novoNome);
            if (existing != null && existing.Id != id)
            {
                return (false, "There is another venue registered with this name");
            }

            // Update
            estabelecimento.Nome = novoNome;
            await _estabelecimentoRepository.UpdateAsync(estabelecimento);
            await _estabelecimentoRepository.SaveChangesAsync();

            return (true, $"Venue name successfully updated to '{novoNome}'!");
        }

        public async Task<(bool success, string message)> DeleteEstabelecimentosAsync(IEnumerable<int> ids)
        {
            Guard.AgainstNull(ids, nameof(ids));

            if (!ids.Any())
            {
                return (false, "No venue was selected for removal.");
            }

            // Optimized query with EXISTS
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

        private (bool success, string message) BuildDeleteResultMessage(
            List<(int id, string nome, bool canDelete, string reason)> canDelete,
            List<(int id, string nome, bool canDelete, string reason)> cannotDelete)
        {
            if (cannotDelete.Count == 0 && canDelete.Count > 0)
            {
                var count = canDelete.Count;
                return (true, count == 1
                    ? "1 venue successfully removed!"
                    : $"{count} venues successfully removed!");
            }
            else if (cannotDelete.Count > 0 && canDelete.Count > 0)
            {
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

        public async Task<(bool success, string message)> DeleteEstabelecimentoAsync(int id) => await DeleteEstabelecimentosAsync(new[] { id });

        public async Task<IEnumerable<Estabelecimento>> GetAllEstabelecimentosAsync() => await _estabelecimentoRepository.GetAllAsync();

        public async Task<Estabelecimento?> GetEstabelecimentoByIdAsync(int id)
        {
            Guard.AgainstNegativeOrZero(id, nameof(id));
            return await _estabelecimentoRepository.GetByIdAsync(id);
        }

        #endregion

        public async Task<IEnumerable<EstabelecimentoListItemDto>> GetAllEstabelecimentosForListAsync() =>
             (await _estabelecimentoRepository.GetAllWithHasEventsAsync())
            .Select(X => EstabelecimentoMapper.ToListDto(X.estabelecimento, X.hasEvents));

        public async Task<IEnumerable<EstabelecimentoListItemDto>> SearchEstabelecimentosForListAsync(string query) => 
            (await _estabelecimentoRepository.SearchWithHasEventsAsync(query))
            .Select(x => EstabelecimentoMapper.ToListDto(x.estabelecimento, x.hasEvents));

        public async Task<(IEnumerable<EstabelecimentoListItemDto> items, int totalCount)> GetPagedEstabelecimentosForListAsync(
            int pageNumber,
            int pageSize,
            string? query = null)
        {
            Guard.AgainstNegativeOrZero(pageNumber, nameof(pageNumber));
            Guard.AgainstNegativeOrZero(pageSize, nameof(pageSize));

            var (items, totalCount) = await _estabelecimentoRepository.GetPagedWithEventInfoAsync(pageNumber, pageSize, query);

            var dtos = items.Select(x => EstabelecimentoMapper.ToListDto(x.estabelecimento, x.hasEvents));

            return (dtos, totalCount);
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