using MyKaraoke.Domain;
using MyKaraoke.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyKaraoke.Infra.Data.Repositories
{
    public class PessoaRepository : BaseRepository<Pessoa>, IPessoaRepository
    {
        public PessoaRepository(AppDbContext context) : base(context) { }

        public async Task<Pessoa> GetByNomeCompletoAsync(string nomeCompleto)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.NomeCompleto == nomeCompleto);
        }
    }
}
