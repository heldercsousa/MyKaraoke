using GaleraNaFila.Services;
using GaleraNaFila.Contracts;
using GaleraNaFila.Domain.Repositories;
using GaleraNaFila.Infra.Data;
using GaleraNaFila.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using System.IO;
using System.Reflection;

namespace GaleraNaFila
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
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "GaleraNaFila.db");
                options.UseSqlite($"Filename={dbPath}");
            });

            // Auto-registro de Repositórios
            builder.Services.AddRepositories();

            // Registrar Serviços
            builder.Services.AddScoped<QueueService>();

            // Registrar Páginas
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AdminPage>();

            // NOVO: Registrar a própria classe App como Singleton.
            // Isso permite que o construtor de App receba o IServiceProvider via DI.
            builder.Services.AddSingleton<App>(); // <--- ESTA LINHA É CRUCIAL E DEVE ESTAR AQUI!

            // Adicionar logging (opcional, útil para depuração)
#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build(); // Constrói o MauiApp e o ServiceProvider.

            // REMOVA A LINHA ABAIXO, POIS InitializeApp NÃO SERÁ MAIS USADO:
            // app.Services.GetRequiredService<App>().InitializeApp(app.Services);

            return app; // Retorna a instância do MauiApp.
        }

        // --- Método de Extensão para Auto-registro de Repositórios ---
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var infrastructureAssembly = typeof(BaseRepository<>).Assembly;

            var repositoryImplementations = infrastructureAssembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Repository"));

            foreach (var implementation in repositoryImplementations)
            {
                var interfaces = implementation.GetInterfaces();
                var repositoryInterface = interfaces.FirstOrDefault(
                    i => i.Name == $"I{implementation.Name}"
                );

                if (repositoryInterface != null)
                {
                    services.AddScoped(repositoryInterface, implementation);
                }
            }

            return services;
        }
    }
}