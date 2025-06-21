namespace MyKaraoke.Domain.Repositories
{
    public interface IParticipacaoEventoRepository : IBaseRepository<ParticipacaoEvento>
    {
        Task<IEnumerable<ParticipacaoEvento>> GetParticipacoesByPessoaIdAndEventoIdAsync(int pessoaId, int eventoId);
    }
}