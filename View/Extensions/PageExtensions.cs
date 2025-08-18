using MyKaraoke.View.Components;
using MyKaraoke.View.Managers;
using System.Windows.Input;
using MauiView = Microsoft.Maui.Controls.View;

namespace MyKaraoke.View.Extensions
{
    /// <summary>
    /// ✅ EXTENSIONS: Métodos de extensão para facilitar uso nas páginas
    /// 🎯 REUTILIZÁVEL: Pode ser usado em qualquer ContentPage
    /// 🛡️ ROBUST: Com tratamento de erros e fallbacks
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// 🎯 BYPASS: Método de extensão para bypass padrão do PageLifecycleBehavior
        /// </summary>
        public static async Task ExecuteStandardBypass(this ContentPage page)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: ExecuteStandardBypass para {page.GetType().Name} (Hash: {page.GetHashCode()})");

                // ETAPA 1: Aguarda página estar totalmente carregada
                await Task.Delay(100);

                // ETAPA 2: Tenta encontrar e executar LoadDataCommand via reflexão
                var loadDataCommand = GetLoadDataCommand(page);
                if (loadDataCommand != null && loadDataCommand.CanExecute(null))
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: Executando LoadDataCommand via reflexão");

                    try
                    {
                        loadDataCommand.Execute(null);
                        await Task.Delay(300); // Aguarda execução
                        System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: LoadDataCommand executado com sucesso");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro executando LoadDataCommand: {ex.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: LoadDataCommand não disponível ou não executável");
                }

                // ETAPA 3: Força exibição de navbar se existir
                var navBar = FindNavBar(page);
                if (navBar != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: Forçando exibição de NavBar");
                    await ForceShowNavBar(navBar);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: NavBar não encontrada");
                }

                System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: Bypass padrão concluído para {page.GetType().Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro no bypass padrão: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔍 REFLEXÃO: Encontra LoadDataCommand na página via reflexão
        /// </summary>
        private static ICommand GetLoadDataCommand(ContentPage page)
        {
            try
            {
                var pageType = page.GetType();
                var property = pageType.GetProperty("LoadDataCommand");

                if (property != null)
                {
                    var command = property.GetValue(page) as ICommand;
                    System.Diagnostics.Debug.WriteLine($"🔍 PageExtensions: LoadDataCommand encontrado via propriedade: {command != null}");
                    return command;
                }

                // 🔍 FALLBACK: Tenta encontrar via campo
                var field = pageType.GetField("LoadDataCommand", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    var command = field.GetValue(page) as ICommand;
                    System.Diagnostics.Debug.WriteLine($"🔍 PageExtensions: LoadDataCommand encontrado via campo: {command != null}");
                    return command;
                }

                System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: LoadDataCommand não encontrado em {pageType.Name}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao obter LoadDataCommand: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 🔍 BUSCA: Encontra NavBar na página
        /// </summary>
        private static IAnimatableNavBar FindNavBar(ContentPage page)
        {
            try
            {
                // 🔍 ESTRATÉGIA 1: Busca por referências nomeadas comuns via reflexão
                var navBar = FindNavBarByFieldName(page, "CrudNavBar") ??
                           FindNavBarByFieldName(page, "bottomNav") ??
                           FindNavBarByFieldName(page, "navBar") ??
                           FindNavBarByFieldName(page, "navigationBar");

                if (navBar != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 PageExtensions: NavBar encontrada via campo nomeado");
                    return navBar;
                }

                // 🔍 ESTRATÉGIA 2: Busca recursiva no conteúdo da página
                navBar = FindNavBarInContent(page.Content);
                if (navBar != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 PageExtensions: NavBar encontrada via busca recursiva");
                    return navBar;
                }

                System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: NavBar não encontrada em {page.GetType().Name}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao encontrar NavBar: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 🔍 HELPER: Encontra NavBar por nome de campo
        /// </summary>
        private static IAnimatableNavBar FindNavBarByFieldName(ContentPage page, string fieldName)
        {
            try
            {
                var field = page.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(page) as IAnimatableNavBar;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao buscar campo {fieldName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 🔍 RECURSIVA: Busca NavBar no conteúdo da página recursivamente
        /// </summary>
        private static IAnimatableNavBar FindNavBarInContent(MauiView content)
        {
            try
            {
                if (content == null) return null;

                // 🎯 DIRETO: Se o próprio content é IAnimatableNavBar
                if (content is IAnimatableNavBar navBar)
                    return navBar;

                // 🔍 CONTENTVIEW: Busca dentro de ContentView
                if (content is ContentView contentView && contentView.Content != null)
                    return FindNavBarInContent(contentView.Content);

                // 🔍 LAYOUT: Busca dentro de Layout (Grid, StackLayout, etc.)
                if (content is Layout layout)
                {
                    foreach (var child in layout.Children)
                    {
                        if (child is MauiView childView)
                        {
                            var found = FindNavBarInContent(childView);
                            if (found != null) return found;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro na busca recursiva: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 🎯 FORÇA: Força exibição de NavBar com timeout e fallbacks
        /// </summary>
        private static async Task ForceShowNavBar(IAnimatableNavBar navBar)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: Iniciando ForceShowNavBar");

                // 🛡️ PROTEÇÃO 1: Verifica se NavBar é válida
                if (navBar == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: NavBar é null");
                    return;
                }

                // 🛡️ PROTEÇÃO 2: Se é ContentView, verifica se está configurada
                if (navBar is ContentView contentView)
                {
                    await EnsureNavBarIsConfigured(contentView);
                }

                // 🎯 TENTATIVA 1: ShowAsync com timeout
                try
                {
                    var showTask = navBar.ShowAsync();
                    var timeoutTask = Task.Delay(3000);

                    var completedTask = await Task.WhenAny(showTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: Timeout em ShowAsync - tentando fallback");
                        await ForceNavBarVisibilityFallback(navBar);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: ShowAsync executado com sucesso");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro em ShowAsync - tentando fallback: {ex.Message}");
                    await ForceNavBarVisibilityFallback(navBar);
                }

                // 🎯 VERIFICAÇÃO FINAL: Confirma se está visível
                await VerifyNavBarVisibility(navBar);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro geral em ForceShowNavBar: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔧 CONFIGURAÇÃO: Garante que NavBar está configurada para exibição
        /// </summary>
        private static async Task EnsureNavBarIsConfigured(ContentView navBarContentView)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // 🎯 FORÇA: Propriedades básicas de visibilidade
                    navBarContentView.IsVisible = true;
                    navBarContentView.Opacity = 1.0;

                    // 🎯 ESPECÍFICO: Para tipos conhecidos, força configuração adicional
                    if (navBarContentView is CrudNavBarComponent crudNav)
                    {
                        // Para CrudNavBarComponent, força SelectionCount = 0 para mostrar botão Adicionar
                        crudNav.SelectionCount = 0;
                        System.Diagnostics.Debug.WriteLine($"🔧 PageExtensions: CrudNavBarComponent configurado - SelectionCount=0");
                    }

                    System.Diagnostics.Debug.WriteLine($"🔧 PageExtensions: NavBar configurada para exibição");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao configurar NavBar: {ex.Message}");
            }
        }

        /// <summary>
        /// 🛡️ FALLBACK: Força visibilidade da NavBar como último recurso
        /// </summary>
        private static async Task ForceNavBarVisibilityFallback(IAnimatableNavBar navBar)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (navBar is VisualElement visualElement)
                    {
                        visualElement.IsVisible = true;
                        visualElement.Opacity = 1.0;
                        visualElement.TranslationY = 0;

                        System.Diagnostics.Debug.WriteLine($"🛡️ PageExtensions: Fallback aplicado - forçando visibilidade direta");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro no fallback de visibilidade: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ VERIFICAÇÃO: Confirma se NavBar está visível após todas as tentativas
        /// </summary>
        private static async Task VerifyNavBarVisibility(IAnimatableNavBar navBar)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (navBar is VisualElement visualElement)
                    {
                        var isVisible = visualElement.IsVisible;
                        var opacity = visualElement.Opacity;

                        System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: Verificação final - IsVisible: {isVisible}, Opacity: {opacity}");

                        if (!isVisible || opacity < 0.1)
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: NavBar ainda não está visível - aplicando correção final");
                            visualElement.IsVisible = true;
                            visualElement.Opacity = 1.0;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro na verificação final: {ex.Message}");
            }
        }

        /// <summary>
        /// 📝 REGISTRO: Registra página no PageInstanceManager automaticamente
        /// </summary>
        public static void RegisterInInstanceManager(this ContentPage page)
        {
            try
            {
                PageInstanceManager.Instance.RegisterPageInstance(page);
                System.Diagnostics.Debug.WriteLine($"📝 PageExtensions: Página registrada no InstanceManager: {page.GetType().Name} (Hash: {page.GetHashCode()})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao registrar no InstanceManager: {ex.Message}");
            }
        }

        /// <summary>
        /// 🗑️ REMOÇÃO: Remove página do PageInstanceManager automaticamente
        /// </summary>
        public static void UnregisterFromInstanceManager(this ContentPage page)
        {
            try
            {
                PageInstanceManager.Instance.UnregisterPageInstance(page);
                System.Diagnostics.Debug.WriteLine($"🗑️ PageExtensions: Página removida do InstanceManager: {page.GetType().Name} (Hash: {page.GetHashCode()})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao remover do InstanceManager: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 BYPASS ESPECÍFICO: Para SpotPage com lógica específica
        /// </summary>
        public static async Task ExecuteSpotPageBypass(this SpotPage spotPage)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: ExecuteSpotPageBypass para SpotPage (Hash: {spotPage.GetHashCode()})");

                // ETAPA 1: Executa bypass padrão
                await spotPage.ExecuteStandardBypass();

                // ETAPA 2: Lógica específica da SpotPage
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    try
                    {
                        // 🎯 FORÇA: SelectionCount = 0 para mostrar botão "Adicionar"
                        spotPage.SelectionCount = 0;

                        // 🎯 FORÇA: Dispara PropertyChanged
                        var propertyChangedMethod = spotPage.GetType().GetMethod("OnPropertyChanged",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                        if (propertyChangedMethod != null)
                        {
                            propertyChangedMethod.Invoke(spotPage, new object[] { "SelectionCount" });
                            System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: PropertyChanged(SelectionCount) disparado");
                        }

                        System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: Lógica específica da SpotPage aplicada - SelectionCount=0");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro na lógica específica da SpotPage: {ex.Message}");
                    }
                });

                System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: SpotPageBypass concluído com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro no SpotPageBypass: {ex.Message}");
            }
        }

        /// <summary>
        /// 📊 DIAGNÓSTICO: Retorna informações de debug sobre a página
        /// </summary>
        public static Dictionary<string, object> GetPageDiagnostics(this ContentPage page)
        {
            var diagnostics = new Dictionary<string, object>();

            try
            {
                diagnostics["PageType"] = page.GetType().Name;
                diagnostics["PageHash"] = page.GetHashCode();
                diagnostics["IsVisible"] = page.IsVisible;
                diagnostics["Title"] = page.Title ?? "NULL";
                diagnostics["StyleId"] = page.StyleId ?? "NULL";

                // 🔍 LOADDATA COMMAND
                var loadDataCommand = GetLoadDataCommand(page);
                diagnostics["HasLoadDataCommand"] = loadDataCommand != null;
                diagnostics["LoadDataCommandCanExecute"] = loadDataCommand?.CanExecute(null) ?? false;

                // 🔍 NAVBAR
                var navBar = FindNavBar(page);
                diagnostics["HasNavBar"] = navBar != null;
                diagnostics["NavBarType"] = navBar?.GetType().Name ?? "NULL";
                if (navBar is VisualElement navBarElement)
                {
                    diagnostics["NavBarIsVisible"] = navBarElement.IsVisible;
                    diagnostics["NavBarOpacity"] = navBarElement.Opacity;
                }

                // 🔍 INSTANCE MANAGER
                var instanceCount = PageInstanceManager.Instance.GetActiveInstanceCount(page.GetType());
                diagnostics["ActiveInstanceCount"] = instanceCount;

                // 🔍 BEHAVIORS
                var behaviorCount = page.Behaviors?.Count ?? 0;
                diagnostics["BehaviorCount"] = behaviorCount;

                if (behaviorCount > 0)
                {
                    var behaviorTypes = page.Behaviors.Select(b => b.GetType().Name).ToList();
                    diagnostics["BehaviorTypes"] = string.Join(", ", behaviorTypes);
                }

                System.Diagnostics.Debug.WriteLine($"📊 PageExtensions: Diagnósticos coletados para {page.GetType().Name}");
            }
            catch (Exception ex)
            {
                diagnostics["DiagnosticsError"] = ex.Message;
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao coletar diagnósticos: {ex.Message}");
            }

            return diagnostics;
        }

        /// <summary>
        /// 🎯 UTILIDADE: Força aplicação de todas as correções conhecidas para uma página
        /// </summary>
        public static async Task ApplyAllKnownFixes(this ContentPage page)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: Aplicando todas as correções conhecidas para {page.GetType().Name}");

                // 🔧 CORREÇÃO 1: Registra no InstanceManager se não estiver
                page.RegisterInInstanceManager();

                // 🔧 CORREÇÃO 2: Aplica bypass se for tipo conhecido problemático
                if (page is SpotPage spotPage)
                {
                    await spotPage.ExecuteSpotPageBypass();
                }
                else
                {
                    await page.ExecuteStandardBypass();
                }

                // 🔧 CORREÇÃO 3: Força visibilidade da página
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    page.IsVisible = true;
                    page.Opacity = 1.0;
                });

                // 🔧 CORREÇÃO 4: Aguarda um momento para tudo se estabilizar
                await Task.Delay(200);

                System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: Todas as correções aplicadas com sucesso");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao aplicar correções: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔧 UTILITÁRIO: Força correção específica para CrudNavBarComponent
        /// </summary>
        public static async Task FixCrudNavBarComponent(this ContentPage page)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔧 PageExtensions: Aplicando correção específica para CrudNavBarComponent");

                var crudNavBar = FindNavBar(page) as CrudNavBarComponent;
                if (crudNavBar != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        // 🎯 FORÇA: SelectionCount = 0 para mostrar botão Adicionar
                        crudNavBar.SelectionCount = 0;
                        crudNavBar.IsVisible = true;

                        System.Diagnostics.Debug.WriteLine($"🔧 PageExtensions: CrudNavBarComponent corrigido - SelectionCount=0");
                    });

                    // 🎯 FORÇA: ShowAsync
                    try
                    {
                        await crudNavBar.ShowAsync();
                        System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: CrudNavBarComponent.ShowAsync() executado");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: Erro em ShowAsync: {ex.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: CrudNavBarComponent não encontrado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao corrigir CrudNavBarComponent: {ex.Message}");
            }
        }

        /// <summary>
        /// 🚀 AVANÇADO: Força reinicialização completa de uma página problemática
        /// </summary>
        public static async Task ForcePageReinitialize(this ContentPage page)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🚀 PageExtensions: Forçando reinicialização completa de {page.GetType().Name}");

                // 🔧 ETAPA 1: Remove do InstanceManager e re-registra
                page.UnregisterFromInstanceManager();
                await Task.Delay(50);
                page.RegisterInInstanceManager();

                // 🔧 ETAPA 2: Força execução de LoadDataCommand
                var loadDataCommand = GetLoadDataCommand(page);
                if (loadDataCommand != null && loadDataCommand.CanExecute(null))
                {
                    try
                    {
                        loadDataCommand.Execute(null);
                        await Task.Delay(500);
                        System.Diagnostics.Debug.WriteLine($"🚀 PageExtensions: LoadDataCommand re-executado");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao re-executar LoadDataCommand: {ex.Message}");
                    }
                }

                // 🔧 ETAPA 3: Força configuração de NavBar
                if (page is SpotPage)
                {
                    await page.FixCrudNavBarComponent();
                }

                // 🔧 ETAPA 4: Força visibilidade geral
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    page.IsVisible = true;
                    page.Opacity = 1.0;
                });

                System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: Reinicialização completa concluída");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro na reinicialização completa: {ex.Message}");
            }
        }

        /// <summary>
        /// 📊 ADVANCED: Retorna estatísticas detalhadas de todas as páginas ativas
        /// </summary>
        public static Dictionary<string, object> GetGlobalPageStatistics()
        {
            try
            {
                var stats = new Dictionary<string, object>();
                var instanceStats = PageInstanceManager.Instance.GetInstanceStatistics();

                stats["TotalPageTypes"] = instanceStats.Count;
                stats["TotalActiveInstances"] = instanceStats.Values.Sum();
                stats["PageBreakdown"] = instanceStats;
                stats["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // 🔍 PROBLEMAS: Identifica páginas com múltiplas instâncias
                var problematicPages = instanceStats.Where(kvp => kvp.Value > 1).ToList();
                if (problematicPages.Any())
                {
                    stats["ProblematicPages"] = problematicPages.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    stats["HasProblems"] = true;
                }
                else
                {
                    stats["HasProblems"] = false;
                }

                System.Diagnostics.Debug.WriteLine($"📊 PageExtensions: Estatísticas globais coletadas");
                return stats;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao coletar estatísticas globais: {ex.Message}");
                return new Dictionary<string, object>
                {
                    { "Error", ex.Message },
                    { "Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                };
            }
        }
    }
}