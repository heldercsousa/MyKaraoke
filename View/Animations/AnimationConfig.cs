namespace MyKaraoke.View.Animations
{
    /// <summary>
    /// Configurações para animações
    /// </summary>
    public class AnimationConfig
    {
        /// <summary>
        /// Escala mínima (padrão: 1.0)
        /// </summary>
        public double FromScale { get; set; } = 1.0;

        /// <summary>
        /// Escala máxima (padrão: 1.05 - sutil)
        /// </summary>
        public double ToScale { get; set; } = 1.05;

        /// <summary>
        /// Duração de cada pulse em ms (padrão: 400ms)
        /// </summary>
        public uint PulseDuration { get; set; } = 400;

        /// <summary>
        /// Pausa entre pulses em ms (padrão: 800ms)
        /// </summary>
        public int PulsePause { get; set; } = 800;

        /// <summary>
        /// Número de pulses por ciclo (padrão: 3)
        /// </summary>
        public int PulseCount { get; set; } = 3;

        /// <summary>
        /// Delay inicial antes de começar (padrão: 2000ms)
        /// </summary>
        public int InitialDelay { get; set; } = 2000;

        /// <summary>
        /// Intervalo entre ciclos em ms (padrão: 10000ms)
        /// </summary>
        public int CycleInterval { get; set; } = 10000;

        /// <summary>
        /// Easing para expansão (padrão: CubicOut)
        /// </summary>
        public Easing ExpandEasing { get; set; } = Easing.CubicOut;

        /// <summary>
        /// Easing para contração (padrão: CubicIn)
        /// </summary>
        public Easing ContractEasing { get; set; } = Easing.CubicIn;

        /// <summary>
        /// Se deve repetir automaticamente (padrão: true)
        /// </summary>
        public bool AutoRepeat { get; set; } = true;

        /// <summary>
        /// Configuração padrão para botões de call-to-action
        /// </summary>
        public static AnimationConfig CallToAction => new AnimationConfig
        {
            ToScale = 1.05,
            PulseDuration = 400,
            PulsePause = 800,
            PulseCount = 3,
            InitialDelay = 2000,
            CycleInterval = 10000
        };

        /// <summary>
        /// Configuração sutil para elementos secundários
        /// </summary>
        public static AnimationConfig Subtle => new AnimationConfig
        {
            ToScale = 1.02,
            PulseDuration = 600,
            PulsePause = 1200,
            PulseCount = 2,
            InitialDelay = 3000,
            CycleInterval = 15000
        };

        /// <summary>
        /// Configuração intensa para alertas/urgências
        /// </summary>
        public static AnimationConfig Intense => new AnimationConfig
        {
            ToScale = 1.08,
            PulseDuration = 300,
            PulsePause = 400,
            PulseCount = 5,
            InitialDelay = 1000,
            CycleInterval = 5000
        };
    }
}