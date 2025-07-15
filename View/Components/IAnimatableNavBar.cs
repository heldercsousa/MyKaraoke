namespace MyKaraoke.View.Components
{
    /// <summary>
    /// Define um contrato para componentes de barra de navegação
    /// que suportam animações de entrada e saída.
    /// </summary>
    public interface IAnimatableNavBar
    {
        /// <summary>
        /// Executa a animação de entrada (aparecer) do componente.
        /// </summary>
        Task ShowAsync();

        /// <summary>
        /// Executa a animação de saída (desaparecer) do componente.
        /// </summary>
        Task HideAsync();
    }
}