using MyKaraoke.Domain;
using MyKaraoke.Domain.Repositories;
using MyKaraoke.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace MyKaraoke.Services;

/// <summary>
/// Serviço responsável apenas por operações de fila e eventos
/// Operações de pessoas delegadas para IPessoaService
/// </summary>
public class QueueService : IQueueService
{
    private readonly AppDbContext _dbContext; // Direto para migrações
    private readonly IEstabelecimentoRepository _estabelecimentoRepository;
    private readonly IEventoRepository _eventoRepository;
    private readonly IParticipacaoEventoRepository _participacaoEventoRepository;
    private readonly IPessoaService _pessoaService; // Nova dependência

    public QueueService(
        AppDbContext dbContext,
        IEstabelecimentoRepository estabelecimentoRepository,
        IEventoRepository eventoRepository,
        IParticipacaoEventoRepository participacaoEventoRepository,
        IPessoaService pessoaService)
    {
        _dbContext = dbContext;
        _estabelecimentoRepository = estabelecimentoRepository;
        _eventoRepository = eventoRepository;
        _participacaoEventoRepository = participacaoEventoRepository;
        _pessoaService = pessoaService;
    }

    // --- Operações de Fila (usando PessoaService para operações de pessoa) ---

    /// <summary>
    /// Adiciona pessoa à fila - delega criação para PessoaService
    /// </summary>
    public async Task<(bool success, string message, Pessoa? addedDomainPerson)> AddPersonToQueueAsync(
        string fullName, string birthday = null, string email = null)
    {
        try
        {
            // Verifica se há evento ativo
            var activeEvent = await GetActiveEventAsync();
            if (activeEvent == null || !activeEvent.FilaAtiva)
            {
                return (false, "Não há fila ativa no momento", null);
            }

            // Delega criação/busca da pessoa para PessoaService
            Pessoa? person = null;

            // Primeiro tenta buscar pessoa existente
            person = await _pessoaService.GetPersonByNameAsync(fullName);

            if (person == null)
            {
                // Se não existe, cria nova pessoa
                var createResult = await _pessoaService.CreatePersonAsync(fullName, birthday, email);
                if (!createResult.success)
                {
                    return (false, createResult.message, null);
                }
                person = createResult.person;
            }

            return (true, $"{person.NomeCompleto} adicionado à fila!", person);
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao adicionar à fila: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Registra participação no evento ativo
    /// </summary>
    public async Task RecordParticipationAsync(int pessoaId, ParticipacaoStatus status)
    {
        if (pessoaId == 0)
        {
            throw new ArgumentException("ID da Pessoa inválido para registrar participação.");
        }

        var activeEvent = await GetActiveEventAsync();
        if (activeEvent == null)
        {
            activeEvent = await GetOrCreateDefaultEventAsync();
        }

        var participacao = new ParticipacaoEvento
        {
            PessoaId = pessoaId,
            EventoId = activeEvent.Id,
            Timestamp = DateTime.Now,
            Status = status
        };

        await _participacaoEventoRepository.AddAsync(participacao);
        await _participacaoEventoRepository.SaveChangesAsync();
    }

    // --- Gerenciamento de Eventos ---

    public async Task<Evento?> GetActiveEventAsync()
    {
        return await _eventoRepository.GetActiveEventAsync();
    }

    public async Task SetActiveEventAsync(int eventId)
    {
        await _eventoRepository.SetActiveEventAsync(eventId);
    }

    public async Task<IEnumerable<Estabelecimento>> GetAllEstablishmentsAsync()
    {
        return await _estabelecimentoRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Evento>> GetAllEventsAsync()
    {
        return await _eventoRepository.GetAllAsync();
    }

    // --- Métodos privados ---

    private async Task<Evento> GetOrCreateDefaultEventAsync()
    {
        var activeEvent = await _eventoRepository.GetActiveEventAsync();
        if (activeEvent == null)
        {
            var defaultEstabelecimento = (await _estabelecimentoRepository.GetAllAsync()).FirstOrDefault();
            if (defaultEstabelecimento == null)
            {
                defaultEstabelecimento = new Estabelecimento { Nome = "Estabelecimento Padrão Criado Automaticamente" };
                await _estabelecimentoRepository.AddAsync(defaultEstabelecimento);
                await _estabelecimentoRepository.SaveChangesAsync();
            }

            activeEvent = new Evento
            {
                EstabelecimentoId = defaultEstabelecimento.Id,
                DataEvento = DateTime.Today,
                NomeEvento = $"Evento Automático {DateTime.Today.ToShortDateString()}",
                FilaAtiva = true
            };
            await _eventoRepository.AddAsync(activeEvent);
            await _eventoRepository.SaveChangesAsync();
        }
        return activeEvent;
    }
}