using System;
using Microsoft.Extensions.DependencyInjection;
using MyKaraoke.Services;

namespace MyKaraoke.View
{
    public abstract class ServicePage : ContentPage
    {
        protected IQueueService _QueueService { get; private set; }
        protected ILanguageService _LanguageService { get; private set; }

        // Construtor com serviços explícitos
        protected ServicePage(IQueueService queueService, ILanguageService languageService)
        {
            _QueueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _LanguageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        }

        // Método auxiliar para obter serviço do container DI quando necessário
        protected T GetService<T>() where T : class
        {
            return Handler?.MauiContext?.Services?.GetService<T>() 
                ?? throw new InvalidOperationException($"Não foi possível resolver o serviço {typeof(T).Name}");
        }
    }
}