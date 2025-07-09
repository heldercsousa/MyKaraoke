using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyKaraoke.Domain.Repositories;
using MyKaraoke.Infra.Data;
using MyKaraoke.Infra.Data.Repositories;
using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    handlers.AddHandler<Entry, MyKaraoke.View.Platforms.Android.CustomEntryHandler>();
#endif
                });

            // **CONFIGURAÇÃO CORRIGIDA DO BANCO DE DADOS**
            ConfigureDatabase(builder.Services);

            // Registrar Repositórios existentes
            builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
            builder.Services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
            builder.Services.AddScoped<IEventoRepository, EventoRepository>();
            builder.Services.AddScoped<IParticipacaoEventoRepository, ParticipacaoEventoRepository>();

            // Registrar Serviços
            builder.Services.AddSingleton<IQueueService, QueueService>();
            builder.Services.AddSingleton<ILanguageService, LanguageService>();
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

            // Registrar Páginas
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<PersonPage>();
            builder.Services.AddTransient<StackPage>();
            builder.Services.AddTransient<TonguePage>();
            builder.Services.AddTransient<SplashLoadingPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void ConfigureDatabase(IServiceCollection services)
        {
            // Garantir que o diretório existe
            string appDataPath = FileSystem.AppDataDirectory;
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            string dbPath = Path.Combine(appDataPath, "MyKaraoke.db");
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Caminho do banco: {dbPath}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Diretório existe: {Directory.Exists(appDataPath)}");
#endif
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}", sqliteOptions =>
                {
                    sqliteOptions.CommandTimeout(60);
                });

#if DEBUG
                options.EnableSensitiveDataLogging();
                options.LogTo(message => System.Diagnostics.Debug.WriteLine($"[EF] {message}"));
#endif
            });
        }
    }
}