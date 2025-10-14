using MyVocaList.Domain;
using MyVocaList.Contracts.DTOs.List;

namespace MyVocaList.Services.Mappers
{
    public static class EstabelecimentoMapper
    {
        public static EstabelecimentoListItemDto ToListDto(Estabelecimento entity, bool hasEvents = false)
        {
            return new EstabelecimentoListItemDto
            {
                Id = entity.Id,
                Nome = entity.Nome,
                HasEvents = hasEvents
            };
        }

        public static Estabelecimento ToEntity(EstabelecimentoListItemDto dto)
        {
            return new Estabelecimento
            {
                Id = dto.Id,
                Nome = dto.Nome
            };
        }
    }
}