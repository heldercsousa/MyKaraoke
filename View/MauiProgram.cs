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
                });

            // Configurar o AppDbContext com SQLite
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "MyKaraoke.db");
                options.UseSqlite($"Filename={dbPath}");
            });

            // Registrar Repositórios existentes
            builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
            builder.Services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
            builder.Services.AddScoped<IEventoRepository, EventoRepository>();
            builder.Services.AddScoped<IParticipacaoEventoRepository, ParticipacaoEventoRepository>();

            // Registrar Serviços
            builder.Services.AddSingleton<IQueueService, QueueService>();
            builder.Services.AddSingleton<ILanguageService, LanguageService>();


            // Registrar Páginas
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<PersonPage>();
            builder.Services.AddTransient<StackPage>();

            builder.Services.AddTransient<TonguePage>();

            // Adicionar logging para debug
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}