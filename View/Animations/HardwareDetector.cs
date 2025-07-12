namespace MyKaraoke.View.Animations
{
    /// <summary>
    /// Detecta capacidade de hardware para otimizar animações
    /// </summary>
    public static class HardwareDetector
    {
        private static bool? _animationsSupported;

        /// <summary>
        /// Verifica se o dispositivo suporta animações fluidas
        /// </summary>
        public static bool SupportsAnimations
        {
            get
            {
                if (_animationsSupported.HasValue)
                    return _animationsSupported.Value;

                _animationsSupported = DetectAnimationCapability();
                return _animationsSupported.Value;
            }
        }

        private static bool DetectAnimationCapability()
        {
            try
            {
                // Desktop sempre suporta
                if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
                    return true;

                // Tablets geralmente têm boa performance
                if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
                    return true;

                // Para phones, verificar especificações
                if (DeviceInfo.Idiom == DeviceIdiom.Phone)
                {
                    var displayInfo = DeviceDisplay.MainDisplayInfo;

                    // Hardware limitado: densidade baixa OU resolução muito baixa
                    bool lowDensity = displayInfo.Density < 2.0;
                    bool lowResolution = displayInfo.Width < 720;
                    bool veryLowResolution = displayInfo.Width < 480;

                    // Critérios para DESABILITAR animações:
                    if (veryLowResolution) // Muito limitado
                        return false;

                    if (lowDensity && lowResolution) // Combinação ruim
                        return false;

                    // Casos borderline - habilita mas com configuração sutil
                    return true;
                }

                // Outros dispositivos (TV, Watch, etc.) - conservador
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na detecção de hardware: {ex.Message}");
                // Em caso de erro, assume hardware limitado
                return false;
            }
        }

        /// <summary>
        /// Determina a configuração ideal baseada no hardware
        /// </summary>
        public static AnimationConfig GetOptimalConfig(AnimationConfig requestedConfig)
        {
            if (!SupportsAnimations)
                return null; // Sem animação

            try
            {
                var displayInfo = DeviceDisplay.MainDisplayInfo;

                // Hardware top: usa configuração solicitada
                if (displayInfo.Density >= 3.0 && displayInfo.Width >= 1080)
                    return requestedConfig;

                // Hardware médio: reduz intensidade
                if (displayInfo.Density >= 2.0 && displayInfo.Width >= 720)
                {
                    return new AnimationConfig
                    {
                        FromScale = requestedConfig.FromScale,
                        ToScale = Math.Min(requestedConfig.ToScale, 1.03), // Limita escala
                        PulseDuration = Math.Max(requestedConfig.PulseDuration, 500), // Mais lento
                        PulsePause = Math.Max(requestedConfig.PulsePause, 1000), // Mais pausa
                        PulseCount = Math.Min(requestedConfig.PulseCount, 2), // Menos pulses
                        InitialDelay = requestedConfig.InitialDelay,
                        CycleInterval = Math.Max(requestedConfig.CycleInterval, 15000), // Menos frequente
                        ExpandEasing = Easing.SinOut, // Easing mais simples
                        ContractEasing = Easing.SinIn,
                        AutoRepeat = requestedConfig.AutoRepeat
                    };
                }

                // Hardware básico: configuração muito sutil
                return AnimationConfig.Subtle;
            }
            catch
            {
                // Em caso de erro, usa configuração sutil
                return AnimationConfig.Subtle;
            }
        }

        /// <summary>
        /// Log de informações do hardware (para debug)
        /// </summary>
        public static void LogHardwareInfo()
        {
            try
            {
                var displayInfo = DeviceDisplay.MainDisplayInfo;
                System.Diagnostics.Debug.WriteLine($"=== HARDWARE INFO ===");
                System.Diagnostics.Debug.WriteLine($"Idiom: {DeviceInfo.Idiom}");
                System.Diagnostics.Debug.WriteLine($"Platform: {DeviceInfo.Platform}");
                System.Diagnostics.Debug.WriteLine($"Density: {displayInfo.Density}");
                System.Diagnostics.Debug.WriteLine($"Width: {displayInfo.Width}px");
                System.Diagnostics.Debug.WriteLine($"Height: {displayInfo.Height}px");
                System.Diagnostics.Debug.WriteLine($"Animations Supported: {SupportsAnimations}");
                System.Diagnostics.Debug.WriteLine($"===================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao logar hardware: {ex.Message}");
            }
        }
    }
}