
// ####################################################################################################
// # Arquivo: GaleraNaFila/Data/Repositories/IEventoRepository.cs
// # Descrição: Interface específica para o repositório de Evento.
// ####################################################################################################
using GaleraNaFila.Domain;
using GaleraNaFila.Domain.Repositories;
using System.Threading.Tasks;

namespace GaleraNaFila.Domain.Repositories
{
    public interface IEventoRepository : IBaseRepository<Evento>
    {
        Task<Evento> GetActiveEventAsync();
        Task SetActiveEventAsync(int eventId);
    }
}