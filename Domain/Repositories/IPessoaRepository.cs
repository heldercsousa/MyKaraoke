namespace MyKaraoke.Domain.Repositories
{
    public interface IPessoaRepository : IBaseRepository<Pessoa>
    {
        Task<Pessoa> GetByNomeCompletoAsync(string nomeCompleto);
    }
}