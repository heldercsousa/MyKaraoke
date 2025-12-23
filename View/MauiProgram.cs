using Microsoft.Extensions.Logging;
using MyVocaList.Infra.Data;
using MyVocaList.Infra.Data.Repositories;
using MyVocaList.Infra.Data.Interceptors;
using MyVocaList.Services;
using MyVocaList.Infra.Utils;
using Microsoft.EntityFrameworkCore;
using MyVocaList.View.Interceptors;
using CommunityToolkit.Maui;
using Serilog;
using Microsoft.Extensions.Configuration;
using MyVocaList.Services.Configuration;
using System.Reflection;

namespace MyVocaList.View;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; } = null!;

    public static MauiApp CreateMauiApp()
    {
        // Initialize global exception handler FIRST
        GlobalExceptionHandler.Initialize();

        var builder = MauiApp.CreateBuilder();

        // Load appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("MyVocaList.View.appsettings.json");

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream!)
            .Build();

        // Register configuration sections
        builder.Services.Configure<AppSettings>(appSettings => config.Bind(appSettings));
        builder.Services.Configure<PaginationSettings>(paginationSettings => config.GetSection("Pagination").Bind(paginationSettings));

        builder
            .UseMauiApp<App>()
            .ConfigureSerilog()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // === BANCO DE DADOS ===
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "myvocalist.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}")
            .AddInterceptors(
                new CollationInterceptor(),          // ✅ Register collation on every connection
                new DatabaseLoadingInterceptor());   // ✅ Auto-loading + string trimming
#if DEBUG
            // options.EnableSensitiveDataLogging();
            // options.LogTo(message => Console.WriteLine(message), LogLevel.Information);
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

            Log.Information("=== MyVocaList Started ===");
            Log.Information("Application built with automatic loading interceptors");

            return app;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to build application");
            throw;
        }
    }
}