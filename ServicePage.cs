using System;
using Microsoft.Extensions.DependencyInjection;
using MyVocaList.Services;

namespace MyVocaList.View
{
    public abstract class ServicePage : ContentPage
    {
        protected IQueueService _QueueService { get; private set; }
        protected ILanguageService _LanguageService { get; private set; }

        // Construtor com servi�os expl�citos
        protected ServicePage(IQueueService queueService, ILanguageService languageService)
        {
            _QueueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _LanguageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        }

        // M�todo auxiliar para obter servi�o do container DI quando necess�rio
        protected T GetService<T>() where T : class
        {
            return Handler?.MauiContext?.Services?.GetService<T>() 
                ?? throw new InvalidOperationException($"N�o foi poss�vel resolver o servi�o {typeof(T).Name}");
        }
    }
}