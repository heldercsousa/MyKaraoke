using MyKaraoke.Domain;
using MyKaraoke.Domain.Repositories;

namespace MyKaraoke.Infra.Data.Repositories
{
    public class EstabelecimentoRepository : BaseRepository<Estabelecimento>, IEstabelecimentoRepository
    {
        public EstabelecimentoRepository(AppDbContext context) : base(context) { }
    }
}
