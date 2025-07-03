using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views.InputMethods;
using Android.Views;
using Android.Content;

namespace MyKaraoke.View
{
    [Activity(
        Theme = "@style/Maui.SplashTheme", 
        MainLauncher = true, 
        LaunchMode = LaunchMode.SingleTop, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Locale | ConfigChanges.KeyboardHidden | ConfigChanges.LayoutDirection,
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Força o suporte a entrada de texto internacional
            ConfigureInternationalInput();
            
            // Configura locale português brasileiro como padrão para entrada de texto
            SetApplicationLocale();
        }

        private void ConfigureInternationalInput()
        {
            try
            {
                // Obtém o InputMethodManager para configurar entrada de texto
                var inputMethodManager = (InputMethodManager?)GetSystemService(Context.InputMethodService);
                
                // Força o reconhecimento de caracteres Unicode independentemente do locale do sistema
                if (inputMethodManager != null)
                {
                    // Configurações para garantir suporte a caracteres acentuados
                    Window?.SetSoftInputMode(SoftInput.AdjustResize | SoftInput.StateHidden);
                }

                System.Diagnostics.Debug.WriteLine("Configuração internacional de entrada aplicada com sucesso");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar entrada internacional: {ex.Message}");
            }
        }

        private void SetApplicationLocale()
        {
            try
            {
                // Define português brasileiro como locale preferido para entrada de texto
                var locale = new Java.Util.Locale("pt", "BR");
                Java.Util.Locale.Default = locale;
                
                var config = new Android.Content.Res.Configuration(Resources?.Configuration);
                config.SetLocale(locale);
                
                // Aplica a configuração ao contexto da aplicação
                Resources?.UpdateConfiguration(config, Resources.DisplayMetrics);
                
                System.Diagnostics.Debug.WriteLine("Locale português brasileiro configurado como padrão");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao configurar locale: {ex.Message}");
            }
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            
            // Reaplica configurações quando a configuração do dispositivo muda
            ConfigureInternationalInput();
            SetApplicationLocale();
        }
    }
}
