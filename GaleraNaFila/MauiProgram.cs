using GaleraNaFila.Services;
using GaleraNaFila.Domain.Repositories;
using GaleraNaFila.Infra.Data;
using GaleraNaFila.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

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

            // Registrar Repositórios existentes
            builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
            builder.Services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
            builder.Services.AddScoped<IEventoRepository, EventoRepository>();
            builder.Services.AddScoped<IParticipacaoEventoRepository, ParticipacaoEventoRepository>();

            // Registrar Serviços
            builder.Services.AddScoped<QueueService>();

            // Registrar Páginas
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AdminPage>();

            // Adicionar logging para debug
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}