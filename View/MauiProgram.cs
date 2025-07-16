using Microsoft.Extensions.Logging;
using MyKaraoke.Infra.Data;
using MyKaraoke.Domain.Repositories;
using MyKaraoke.Infra.Data.Repositories;
using MyKaraoke.Services;
using MyKaraoke.Infra.Utils;
using Microsoft.EntityFrameworkCore;

namespace MyKaraoke.View;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }

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
        builder.Services.AddScoped<IEstabelecimentoService, EstabelecimentoService>();
        builder.Services.AddScoped<IQueueService, QueueService>();
        builder.Services.AddScoped<IDatabaseService, DatabaseService>();

        // === PÁGINAS (TRANSIENT - sempre nova instância) ===
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<TonguePage>();
        builder.Services.AddTransient<StackPage>();
        builder.Services.AddTransient<PersonPage>();
        builder.Services.AddTransient<SpotPage>();
        builder.Services.AddTransient<SpotFormPage>();

        var app = builder.Build();

        Services = app.Services;

        return app;

    }
}