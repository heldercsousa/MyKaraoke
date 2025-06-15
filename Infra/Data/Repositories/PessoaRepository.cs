using GaleraNaFila.Domain;
using GaleraNaFila.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleraNaFila.Infra.Data.Repositories
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
