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

                    // ✅ CRITÉRIOS MAIS RIGOROSOS: Só desabilita em hardware MUITO ruim
                    bool veryLowResolution = displayInfo.Width < 480;      // Muito antigo (Android 2.x era)
                    bool veryLowDensity = displayInfo.Density < 1.5;       // Telas muito antigas
                    bool terribleCombo = displayInfo.Width < 720 && displayInfo.Density < 2.0; // Combo muito ruim

                    // BYPASS (sem animação) apenas para hardware MUITO limitado
                    if (veryLowResolution || veryLowDensity || terribleCombo)
                    {
                        System.Diagnostics.Debug.WriteLine("🚫 Hardware muito limitado - animações desabilitadas para economia de recursos");
                        return false;
                    }

                    // ✅ TODOS os outros phones modernos suportam animação
                    // Inclui: Pixel 5, iPhone 8+, Galaxy S8+, etc.
                    return true;
                }

                // Outros dispositivos (TV, Watch, etc.) - conservador
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na detecção de hardware: {ex.Message}");
                // Em caso de erro, assume hardware limitado por segurança
                return false;
            }
        }

        /// <summary>
        /// Determina a configuração ideal baseada no hardware
        /// ✅ LÓGICA CORRETA: Só otimiza em hardware MUITO ruim
        /// </summary>
        public static AnimationConfig GetOptimalConfig(AnimationConfig requestedConfig)
        {
            // ✅ BYPASS = Hardware muito ruim → SEM animação
            if (!SupportsAnimations)
                return null; // BYPASS total - sem animação para poupar recursos

            try
            {
                var displayInfo = DeviceDisplay.MainDisplayInfo;

                // 🔥 DEFINIÇÃO CORRETA: Hardware muito ruim precisa de BYPASS
                bool isVeryLowEndHardware =
                    displayInfo.Width < 480 ||           // Resolução muito antiga
                    displayInfo.Density < 1.5 ||         // Densidade muito baixa (telas antigas)
                    (displayInfo.Width < 720 && displayInfo.Density < 2.0); // Combinação ruim

                if (isVeryLowEndHardware)
                {
                    System.Diagnostics.Debug.WriteLine("🚫 Hardware MUITO ruim detectado - BYPASS ativado (sem animação)");
                    return null; // BYPASS = sem animação para economizar recursos
                }

                // ✅ TODOS OS OUTROS HARDWARES: Usa configuração original
                // Isso inclui:
                // - Hardware TOP (iPhone Pro, Galaxy S, etc.)
                // - Hardware BOM (Pixel 5, maioria dos smartphones modernos)  
                // - Hardware MÉDIO (smartphones de 2-3 anos atrás)

                System.Diagnostics.Debug.WriteLine("✅ Hardware adequado detectado - usando configuração ORIGINAL");
                System.Diagnostics.Debug.WriteLine($"   Resolução: {displayInfo.Width}x{displayInfo.Height}");
                System.Diagnostics.Debug.WriteLine($"   Densidade: {displayInfo.Density}");
                System.Diagnostics.Debug.WriteLine($"   Classificação: {GetHardwareClass()}");

                return requestedConfig; // 🎯 Usa EXATAMENTE sua configuração!
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na detecção de hardware: {ex.Message}");
                // Em caso de erro, assume hardware limitado e ativa BYPASS
                return null;
            }
        }

        /// <summary>
        /// Classificação do hardware para debug
        /// </summary>
        public static string GetHardwareClass()
        {
            try
            {
                if (!SupportsAnimations)
                    return "Muito Ruim (BYPASS - sem animações)";

                var displayInfo = DeviceDisplay.MainDisplayInfo;

                // ✅ LÓGICA CORRETA: Só hardware muito ruim precisa de bypass
                bool isVeryLowEndHardware =
                    displayInfo.Width < 480 ||
                    displayInfo.Density < 1.5 ||
                    (displayInfo.Width < 720 && displayInfo.Density < 2.0);

                if (isVeryLowEndHardware)
                    return "Muito Ruim (BYPASS - sem animações)";

                // TODOS os outros hardwares são considerados adequados
                if (displayInfo.Density >= 3.0 && displayInfo.Width >= 1080)
                    return "TOP (configuração original)";

                if (displayInfo.Density >= 2.0 && displayInfo.Width >= 720)
                    return "BOM (configuração original)"; // Pixel 5 fica aqui!

                return "MÉDIO (configuração original)";
            }
            catch
            {
                return "Desconhecido (assume BYPASS)";
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
                System.Diagnostics.Debug.WriteLine($"Hardware Class: {GetHardwareClass()}");
                System.Diagnostics.Debug.WriteLine($"===================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao logar hardware: {ex.Message}");
            }
        }
    }
}