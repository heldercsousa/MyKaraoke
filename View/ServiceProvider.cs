using Microsoft.Extensions.DependencyInjection;
using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public class ServiceProvider
    {
        private readonly IServiceProvider _services;

        public ServiceProvider(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public T GetService<T>() where T : class
        {
            return _services.GetService<T>() ??
                throw new InvalidOperationException($"Não foi possível resolver o serviço {typeof(T).Name}");
        }

        public static ServiceProvider FromPage(Page page)
        {
            var services = page.Handler?.MauiContext?.Services;
            if (services == null)
                throw new InvalidOperationException("O contexto de serviços não está disponível");

            return new ServiceProvider(services);
        }
    }
}