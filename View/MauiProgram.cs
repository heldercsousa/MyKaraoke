using Microsoft.Extensions.Logging;
using MyKaraoke.Infra.Data;
using MyKaraoke.Domain.Repositories;
using MyKaraoke.Infra.Data.Repositories;
using MyKaraoke.Services;
using MyKaraoke.Infra.Utils;
using Microsoft.EntityFrameworkCore;
using MyKaraoke.View.Interceptors;

namespace MyKaraoke.View;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; } = null!;

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // === CONFIGURAÇÕES DE DEBUG ===
#if DEBUG
        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Information);
        });
#endif

        // === BANCO DE DADOS ===
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "mykaraoke.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}")
            .AddInterceptors(new DatabaseLoadingInterceptor());
#if DEBUG
            options.EnableSensitiveDataLogging();
            options.LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Information);
#endif
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

        try
        {
            var app = builder.Build();
            Services = app.Services;

            // ✅ INTERCEPTADOR DE NAVEGAÇÃO: Inicializa no startup
            NavigationLoadingInterceptor.Initialize();

            System.Diagnostics.Debug.WriteLine("[MauiProgram] Aplicação construída com interceptadores de loading automático");
            return app;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MauiProgram] ERRO ao construir aplicação: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[MauiProgram] Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}