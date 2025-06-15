
// ####################################################################################################
// # Arquivo: GaleraNaFila/Data/Repositories/IParticipacaoEventoRepository.cs
// # Descrição: Interface específica para o repositório de ParticipacaoEvento.
// ####################################################################################################
using GaleraNaFila.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GaleraNaFila.Domain.Repositories
{
    public interface IParticipacaoEventoRepository : IBaseRepository<ParticipacaoEvento>
    {
        Task<IEnumerable<ParticipacaoEvento>> GetParticipacoesByPessoaIdAndEventoIdAsync(int pessoaId, int eventoId);
    }
}