
// ####################################################################################################
// # Arquivo: GaleraNaFila/Data/Repositories/IPessoaRepository.cs
// # Descrição: Interface específica para o repositório de Pessoa.
// ####################################################################################################

namespace GaleraNaFila.Domain.Repositories
{
    public interface IPessoaRepository : IBaseRepository<Pessoa>
    {
        Task<Pessoa> GetByNomeCompletoAsync(string nomeCompleto);
    }
}