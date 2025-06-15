// # Arquivo: FilaDeC.Application/Services/QueueService.cs
// # Descrição: Serviço de lógica de negócios, AGORA AGNOSTICO À PLATAFORMA (sem Preferences).
// ####################################################################################################
// FilaDeC.Application.Contracts NÃO É USADO DIRETAMENTE AQUI, APENAS PARA MAPPING (se fosse usar AutoMapper, por exemplo)

using GaleraNaFila.Domain;
using GaleraNaFila.Domain.Repositories;
using GaleraNaFila.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace GaleraNaFila.Services;
public class QueueService
{
    private readonly AppDbContext _dbContext; // Direto para migrações
    private readonly IPessoaRepository _pessoaRepository;
    private readonly IEstabelecimentoRepository _estabelecimentoRepository;
    private readonly IEventoRepository _eventoRepository;
    private readonly IParticipacaoEventoRepository _participacaoEventoRepository;

    public QueueService(
        AppDbContext dbContext,
        IPessoaRepository pessoaRepository,
        IEstabelecimentoRepository estabelecimentoRepository,
        IEventoRepository eventoRepository,
        IParticipacaoEventoRepository participacaoEventoRepository)
    {
        _dbContext = dbContext;
        _pessoaRepository = pessoaRepository;
        _estabelecimentoRepository = estabelecimentoRepository;
        _eventoRepository = eventoRepository;
        _participacaoEventoRepository = participacaoEventoRepository;
    }

    // --- Operações de Banco de Dados e Inicialização ---

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            await _dbContext.Database.MigrateAsync();
            await GetOrCreateDefaultEventAsync(); // Garante que há um evento padrão
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
            throw;
        }
    }

    // --- Operações de Pessoas (Domínio) ---
    // Este método agora retorna a entidade de domínio Pessoa, não o DTO de lista
    public async Task<(bool success, string message, Pessoa? addedDomainPerson)> AddPersonAsync(string fullName)
    {
        if (!Pessoa.ValidarNome(fullName))
        {
            return (false, "Nome inválido. Mínimo 2 caracteres no nome e 1 sobrenome com 2 caracteres.", null);
        }

        // Verifica se a pessoa já existe no banco de dados de pessoas mestre
        Pessoa existingDomainPerson = await _pessoaRepository.GetByNomeCompletoAsync(fullName);
        Pessoa personToReturn;

        if (existingDomainPerson == null)
        {
            personToReturn = new Pessoa(fullName);
            await _pessoaRepository.AddAsync(personToReturn);
            await _pessoaRepository.SaveChangesAsync(); // Salva a nova pessoa no DB para obter o Id
        }
        else
        {
            personToReturn = existingDomainPerson;
        }
        // A lógica de resetar Participacoes/Ausencias é da UI ou de um ViewModel para a fila ativa.

        return (true, $"{fullName} adicionado(a) ao banco de dados!", personToReturn);
    }

    // Este método agora aceita o ID da pessoa (do DTO) e o status, sem se preocupar com o DTO em si
    public async Task RecordParticipationAsync(int pessoaId, ParticipacaoStatus status)
    {
        if (pessoaId == 0)
        {
            throw new ArgumentException("ID da Pessoa inválido para registrar participação.");
        }

        var activeEvent = await GetActiveEventAsync(); // Pega o evento ativo do DB
        if (activeEvent == null)
        {
            activeEvent = await GetOrCreateDefaultEventAsync(); // Garante que há um evento ativo
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
        // A atualização dos contadores no DTO na fila ativa é de responsabilidade da UI.
    }

    // --- Gerenciamento de Eventos (Base para Evolutiva 1.3 - continua agnóstico à plataforma) ---

    public async Task<Evento?> GetActiveEventAsync()
    {
        // O EventoRepository já busca o evento ativo no DB
        return await _eventoRepository.GetActiveEventAsync();
    }

    public async Task SetActiveEventAsync(int eventId)
    {
        // O EventoRepository já lida com a ativação/desativação no DB
        await _eventoRepository.SetActiveEventAsync(eventId);
        // A atualização do ActiveEventIdKey nas Preferences e o reset dos contadores na fila ativa são da UI.
    }

    public async Task<IEnumerable<Estabelecimento>> GetAllEstablishmentsAsync()
    {
        return await _estabelecimentoRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Evento>> GetAllEventsAsync()
    {
        return await _eventoRepository.GetAllAsync();
    }

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
            // O registro do ID do evento ativo nas Preferences é da UI.
        }
        return activeEvent;
    }
}

