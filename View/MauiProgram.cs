using Microsoft.Extensions.Logging;
using MyKaraoke.Infra.Data;
using MyKaraoke.Domain.Repositories;
using MyKaraoke.Infra.Data.Repositories;
using MyKaraoke.Services;
using MyKaraoke.Infra.Utils; // 🔄 NOVO: TextNormalizer
using Microsoft.EntityFrameworkCore;

namespace MyKaraoke.View;

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
            });

#if DEBUG
        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
        });
#endif

        // === BANCO DE DADOS ===
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mykaraoke.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        // === UTILITÁRIOS (SINGLETON - sem estado) ===
        builder.Services.AddSingleton<ITextNormalizer, TextNormalizer>();
        builder.Services.AddSingleton<ILanguageService, LanguageService>();

        // === REPOSITÓRIOS (SCOPED - com contexto) ===
        builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
        builder.Services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
        builder.Services.AddScoped<IEventoRepository, EventoRepository>();
        builder.Services.AddScoped<IParticipacaoEventoRepository, ParticipacaoEventoRepository>();

        // === SERVIÇOS DE NEGÓCIO (SCOPED - com estado) ===
        builder.Services.AddScoped<IPessoaService, PessoaService>();
        builder.Services.AddScoped<IQueueService, QueueService>();
        builder.Services.AddScoped<IDatabaseService, DatabaseService>();

        // === PÁGINAS (TRANSIENT - sempre nova instância) ===
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<TonguePage>();
        builder.Services.AddTransient<StackPage>();
        builder.Services.AddTransient<PersonPage>();

        return builder.Build();
    }
}
