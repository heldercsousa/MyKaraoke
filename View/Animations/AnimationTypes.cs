namespace MyKaraoke.View.Animations
{
    /// <summary>
    /// Tipos de animação para botões de navegação regulares
    /// Permite múltiplas animações através de flags - APENAS as 3 essenciais
    /// </summary>
    [Flags]
    public enum NavButtonAnimationType
    {
        None = 0,
        Fade = 1 << 0,      // 1 - Animação de fade in/out
        Translate = 1 << 1,  // 2 - Animação de slide up/down
        Pulse = 1 << 2,      // 4 - Animação de pulse (scale)

        // Combinações comuns
        ShowHide = Fade | Translate,      // 3 - Fade + Translate (padrão para show/hide)
        CallToAction = Pulse,             // 4 - Apenas pulse para call-to-action
        Full = Fade | Translate | Pulse   // 7 - Todas as animações
    }

    /// <summary>
    /// Tipos de animação para botões especiais
    /// Permite múltiplas animações através de flags - APENAS as 3 essenciais
    /// </summary>
    [Flags]
    public enum SpecialButtonAnimationType
    {
        None = 0,
        Fade = 1 << 0,      // 1 - Animação de fade in/out
        Translate = 1 << 1,  // 2 - Animação de slide up/down
        Pulse = 1 << 2,      // 4 - Animação de pulse (scale)

        // Combinações comuns para botões especiais
        ShowHide = Fade | Translate,      // 3 - Fade + Translate (padrão para show/hide)
        CallToAction = Pulse,             // 4 - Pulse forte para call-to-action
        Full = Fade | Translate | Pulse   // 7 - Todas as animações
    }

    /// <summary>
    /// Tipos de gradiente para botões especiais - APENAS 2 opções
    /// </summary>
    public enum SpecialButtonGradientType
    {
        Yellow,  // Amarelo para Laranja
        Purple   // Roxo para RoxoClaro
    }

    /// <summary>
    /// Helper para trabalhar com flags de animação
    /// </summary>
    public static class AnimationTypeHelper
    {
        /// <summary>
        /// Verifica se um tipo de animação contém uma flag específica
        /// </summary>
        public static bool HasFlag<T>(T value, T flag) where T : Enum
        {
            return value.HasFlag(flag);
        }

        /// <summary>
        /// Converte NavButtonAnimationType para lista de tipos individuais
        /// </summary>
        public static List<NavButtonAnimationType> GetIndividualTypes(NavButtonAnimationType animationType)
        {
            var types = new List<NavButtonAnimationType>();

            if (HasFlag(animationType, NavButtonAnimationType.Fade))
                types.Add(NavButtonAnimationType.Fade);

            if (HasFlag(animationType, NavButtonAnimationType.Translate))
                types.Add(NavButtonAnimationType.Translate);

            if (HasFlag(animationType, NavButtonAnimationType.Pulse))
                types.Add(NavButtonAnimationType.Pulse);

            return types;
        }

        /// <summary>
        /// Converte SpecialButtonAnimationType para lista de tipos individuais
        /// </summary>
        public static List<SpecialButtonAnimationType> GetIndividualTypes(SpecialButtonAnimationType animationType)
        {
            var types = new List<SpecialButtonAnimationType>();

            if (HasFlag(animationType, SpecialButtonAnimationType.Fade))
                types.Add(SpecialButtonAnimationType.Fade);

            if (HasFlag(animationType, SpecialButtonAnimationType.Translate))
                types.Add(SpecialButtonAnimationType.Translate);

            if (HasFlag(animationType, SpecialButtonAnimationType.Pulse))
                types.Add(SpecialButtonAnimationType.Pulse);

            return types;
        }

        /// <summary>
        /// Retorna o tipo de animação padrão para show/hide de botões nav
        /// Apenas se o hardware suportar animações
        /// </summary>
        public static NavButtonAnimationType GetDefaultShowHideType()
        {
            return HardwareDetector.SupportsAnimations
                ? NavButtonAnimationType.ShowHide
                : NavButtonAnimationType.None;
        }

        /// <summary>
        /// Retorna o tipo de animação padrão para show/hide de botões especiais
        /// Apenas se o hardware suportar animações
        /// </summary>
        public static SpecialButtonAnimationType GetDefaultSpecialShowHideType()
        {
            return HardwareDetector.SupportsAnimations
                ? SpecialButtonAnimationType.ShowHide
                : SpecialButtonAnimationType.None;
        }

        /// <summary>
        /// Retorna o tipo de animação padrão para call-to-action
        /// Apenas se o hardware suportar animações
        /// </summary>
        public static NavButtonAnimationType GetDefaultCallToActionType()
        {
            return HardwareDetector.SupportsAnimations
                ? NavButtonAnimationType.CallToAction
                : NavButtonAnimationType.None;
        }

        /// <summary>
        /// Retorna o tipo de animação padrão para call-to-action especial
        /// Apenas se o hardware suportar animações
        /// </summary>
        public static SpecialButtonAnimationType GetDefaultSpecialCallToActionType()
        {
            return HardwareDetector.SupportsAnimations
                ? SpecialButtonAnimationType.CallToAction
                : SpecialButtonAnimationType.None;
        }
    }
}