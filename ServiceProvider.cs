using Microsoft.Extensions.DependencyInjection;
using MyVocaList.Services;

namespace MyVocaList.View
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
                throw new InvalidOperationException($"N�o foi poss�vel resolver o servi�o {typeof(T).Name}");
        }

        public static ServiceProvider FromPage(Page page)
        {
            var services = page.Handler?.MauiContext?.Services;
            if (services == null)
                throw new InvalidOperationException("O contexto de servi�os n�o est� dispon�vel");
            
            return new ServiceProvider(services);
        }
    }
}