using MyKaraoke.View.Animations;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyKaraoke.View.Components
{
    #region Configuration Classes - MIGRADAS DO BASENAVBARCOMPONENT

    /// <summary>
    /// ✅ MIGRADO: Configuração de botão de navegação
    /// </summary>
    public class NavButtonConfig
    {
        public string Text { get; set; } = "";
        public string IconSource { get; set; } = "";
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public bool IsAnimated { get; set; } = true;
        public NavButtonAnimationType AnimationTypes { get; set; } = NavButtonAnimationType.ShowHide;

        // Para botões especiais
        public bool IsSpecial { get; set; } = false;
        public string CenterContent { get; set; } = "+";
        public string CenterIconSource { get; set; } = "";
        public SpecialButtonGradientType GradientStyle { get; set; } = SpecialButtonGradientType.Yellow;
        public SpecialButtonAnimationType SpecialAnimationTypes { get; set; } = SpecialButtonAnimationType.ShowHide;

        // Factory methods para facilitar criação
        public static NavButtonConfig Regular(string text, string iconSource, ICommand command = null)
        {
            return new NavButtonConfig
            {
                Text = text,
                IconSource = iconSource,
                Command = command,
                IsSpecial = false,
                AnimationTypes = HardwareDetector.SupportsAnimations ? NavButtonAnimationType.ShowHide : NavButtonAnimationType.None
            };
        }

        public static NavButtonConfig Special(string text, string centerContent = "+", SpecialButtonGradientType gradientStyle = SpecialButtonGradientType.Yellow, ICommand command = null)
        {
            return new NavButtonConfig
            {
                Text = text,
                CenterContent = centerContent,
                Command = command,
                IsSpecial = true,
                GradientStyle = gradientStyle,
                SpecialAnimationTypes = HardwareDetector.SupportsAnimations
                    ? (SpecialButtonAnimationType.ShowHide | SpecialButtonAnimationType.Pulse)
                    : SpecialButtonAnimationType.None,
                IsAnimated = true
            };
        }
    }

    #endregion

    #region Event Args - MIGRADAS DO BASENAVBARCOMPONENT

    /// <summary>
    /// ✅ MIGRADO: Event args para cliques de botão na navbar
    /// </summary>
    public class NavBarButtonClickedEventArgs : EventArgs
    {
        public NavButtonConfig ButtonConfig { get; }
        public object Parameter { get; }

        public NavBarButtonClickedEventArgs(NavButtonConfig buttonConfig, object parameter)
        {
            ButtonConfig = buttonConfig;
            Parameter = parameter;
        }
    }

    #endregion
}