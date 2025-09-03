using MyKaraoke.View;
using MyKaraoke.View.Managers;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace MyKaraoke.View.Extensions
{
    /// <summary>
    /// ✅ EXTENSÕES COMPLETAS: Todos os métodos existentes + novos métodos genéricos
    /// 🚀 BACKWARD-COMPATIBLE: Mantém compatibilidade com código existente
    /// 🎯 REUTILIZÁVEL: Novos métodos genéricos para escalabilidade
    /// </summary>
    public static class PageExtensions
    {
        #region Métodos de Registro e Diagnóstico - PRESERVADOS

        /// <summary>
        /// 📊 DIAGNÓSTICO: Retorna informações de diagnóstico da página
        /// </summary>
        public static Dictionary<string, object> GetPageDiagnostics(this ContentPage page)
        {
            try
            {
                var diagnostics = new Dictionary<string, object>
                {
                    { "PageType", page.GetType().Name },
                    { "PageHash", page.GetHashCode() },
                    { "IsVisible", page.IsVisible },
                    { "IsEnabled", page.IsEnabled },
                    { "Title", page.Title ?? "NULL" },
                    { "BindingContext", page.BindingContext?.GetType().Name ?? "NULL" },
                    { "Handler", page.Handler != null },
                    { "Navigation", page.Navigation != null },
                    { "Behaviors", page.Behaviors?.Count ?? 0 }
                };

                // ✅ SIMPLIFICADO: Informações básicas do PageInstanceManager
                try
                {
                    var instanceManager = PageInstanceManager.Instance;
                    if (instanceManager != null)
                    {
                        diagnostics["PageInstanceManagerAvailable"] = true;
                        // Adicione outras informações básicas se necessário
                    }
                }
                catch
                {
                    diagnostics["PageInstanceManagerAvailable"] = false;
                }

                return diagnostics;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao obter diagnósticos: {ex.Message}");
                return new Dictionary<string, object> { { "Error", ex.Message } };
            }
        }

        #endregion

        #region Métodos de Bypass Originais - PRESERVADOS

        /// <summary>
        /// 🛡️ BYPASS PADRÃO: Execução de fallback padrão para qualquer página
        /// ✅ ORIGINAL: Método que já existia e deve ser preservado
        /// </summary>
        public static async Task ExecuteStandardBypass(this ContentPage page)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🛡️ PageExtensions: ExecuteStandardBypass INICIADO para {page.GetType().Name}");

                // ✅ GARANTE: Estado básico da página
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    page.IsVisible = true;
                    page.IsEnabled = true;
                });

                // ✅ PROCURA: Por propriedades conhecidas e as inicializa
                var pageType = page.GetType();

                // Inicializa SelectionCount se existir
                var selectionCountProp = pageType.GetProperty("SelectionCount");
                if (selectionCountProp != null && selectionCountProp.CanWrite)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        selectionCountProp.SetValue(page, 0);
                        System.Diagnostics.Debug.WriteLine($"✅ SelectionCount=0 definido para {page.GetType().Name}");
                    });
                }

                // Chama UpdateUIState se existir
                var updateMethod = pageType.GetMethod("UpdateUIState",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (updateMethod != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        updateMethod.Invoke(page, null);
                        System.Diagnostics.Debug.WriteLine($"✅ UpdateUIState chamado para {page.GetType().Name}");
                    });
                }

                System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: ExecuteStandardBypass CONCLUÍDO para {page.GetType().Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro no ExecuteStandardBypass: {ex.Message}");
            }
        }

        
        #endregion

        #region Métodos de Correção - PRESERVADOS

        /// <summary>
        /// 🔧 CORREÇÕES: Aplica todas as correções conhecidas para uma página
        /// ✅ ORIGINAL: Método que já existia
        /// </summary>
        public static async Task ApplyAllKnownFixes(this ContentPage page)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔧 PageExtensions: Aplicando correções conhecidas para {page.GetType().Name}");

                // ✅ CORREÇÃO 1: Força visibilidade e estado
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    page.IsVisible = true;
                    page.IsEnabled = true;
                });

                // ✅ CORREÇÃO 2: Verifica e corrige BindingContext
                if (page.BindingContext == null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        page.BindingContext = page;
                        System.Diagnostics.Debug.WriteLine($"🔧 BindingContext corrigido para {page.GetType().Name}");
                    });
                }

                // ✅ CORREÇÃO 3: Força chamada de métodos de correção específicos se existirem
                var applyFixesMethod = page.GetType().GetMethod("ApplyPageFixes",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                if (applyFixesMethod != null)
                {
                    try
                    {
                        if (applyFixesMethod.ReturnType == typeof(Task))
                        {
                            await (Task)applyFixesMethod.Invoke(page, null);
                        }
                        else
                        {
                            applyFixesMethod.Invoke(page, null);
                        }
                        System.Diagnostics.Debug.WriteLine($"🔧 Método específico ApplyPageFixes executado para {page.GetType().Name}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Erro ao executar ApplyPageFixes: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: Todas as correções aplicadas para {page.GetType().Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro ao aplicar correções: {ex.Message}");
            }
        }

        #endregion

        #region Novos Métodos Genéricos - ADICIONADOS

        /// <summary>
        /// 🎯 GENÉRICO: Bypass para páginas com lista (SpotPage, PersonPage, etc.)
        /// ✅ REUTILIZÁVEL: Funciona com qualquer página que tenha propriedade de coleção e SelectionCount
        /// </summary>
        public static async Task ExecuteListPageBypass(this ContentPage listPage)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: ExecuteListPageBypass INICIADO para {listPage.GetType().Name}");

                // ✅ DETECÇÃO AUTOMÁTICA: Encontra propriedades de coleção (Locais, Pessoas, etc.)
                var collectionProperty = FindCollectionProperty(listPage);
                var selectionCountProperty = FindSelectionCountProperty(listPage);

                if (collectionProperty != null && selectionCountProperty != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        // ✅ FORÇA: SelectionCount = 0 (mostra botão Adicionar)
                        selectionCountProperty.SetValue(listPage, 0);

                        // ✅ NOTIFICA: Se a página implementa INotifyPropertyChanged
                        if (listPage is INotifyPropertyChanged notifyPage)
                        {
                            var onPropertyChangedMethod = listPage.GetType().GetMethod("OnPropertyChanged",
                                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                            onPropertyChangedMethod?.Invoke(listPage, new object[] { "SelectionCount" });
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: {listPage.GetType().Name} - SelectionCount=0 definido");
                    });

                    // ✅ ATUALIZA: Estado da UI baseado na coleção
                    await UpdateListUIState(listPage, collectionProperty);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: Propriedades necessárias não encontradas em {listPage.GetType().Name}");

                    // 🛡️ FALLBACK: Executa bypass padrão
                    await listPage.ExecuteStandardBypass();
                }

                System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: ExecuteListPageBypass CONCLUÍDO para {listPage.GetType().Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro no ExecuteListPageBypass: {ex.Message}");

                // 🛡️ FALLBACK: Executa bypass padrão
                await listPage.ExecuteStandardBypass();
            }
        }

        /// <summary>
        /// 🎯 GENÉRICO: Bypass para páginas de formulário (SpotFormPage, PersonFormPage, etc.)
        /// ✅ REUTILIZÁVEL: Funciona com qualquer página que tenha campo de entrada e controle de salvamento
        /// </summary>
        public static async Task ExecuteFormPageBypass(this ContentPage formPage)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🎯 PageExtensions: ExecuteFormPageBypass INICIADO para {formPage.GetType().Name}");

                // ✅ DETECÇÃO AUTOMÁTICA: Encontra propriedades de controle de formulário
                var hasTextProperty = FindFormControlProperty(formPage);

                if (hasTextProperty != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        // ✅ FORÇA: Estado inicial sem texto para salvar
                        hasTextProperty.SetValue(formPage, false);
                        System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: {formPage.GetType().Name} - propriedade de controle=false definida");
                    });

                    // ✅ ESPECÍFICO: Verifica se já tem texto nos campos de entrada (caso de edição)
                    await CheckAndUpdateFormState(formPage, hasTextProperty);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ PageExtensions: Propriedade de controle não encontrada em {formPage.GetType().Name}");

                    // 🛡️ FALLBACK: Executa bypass padrão
                    await formPage.ExecuteStandardBypass();
                }

                System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: ExecuteFormPageBypass CONCLUÍDO para {formPage.GetType().Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PageExtensions: Erro no ExecuteFormPageBypass: {ex.Message}");

                // 🛡️ FALLBACK: Executa bypass padrão
                await formPage.ExecuteStandardBypass();
            }
        }

        #endregion

        #region Helper Methods - Detecção Automática de Propriedades

        /// <summary>
        /// 🔍 DETECÇÃO: Encontra propriedade de coleção (Locais, Pessoas, Items, etc.)
        /// </summary>
        private static System.Reflection.PropertyInfo FindCollectionProperty(ContentPage page)
        {
            var pageType = page.GetType();

            // 🎯 ESTRATÉGIA 1: Procura por nomes conhecidos
            var knownCollectionNames = new[] { "Locais", "Pessoas", "Items", "Data", "Collection" };
            foreach (var name in knownCollectionNames)
            {
                var prop = pageType.GetProperty(name);
                if (prop != null && IsCollectionType(prop.PropertyType))
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Coleção encontrada: {name}");
                    return prop;
                }
            }

            // 🎯 ESTRATÉGIA 2: Procura por qualquer propriedade que implemente IEnumerable
            var properties = pageType.GetProperties();
            foreach (var prop in properties)
            {
                if (IsCollectionType(prop.PropertyType) && prop.CanRead)
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Coleção detectada: {prop.Name}");
                    return prop;
                }
            }

            return null;
        }

        /// <summary>
        /// 🔍 DETECÇÃO: Encontra propriedade SelectionCount
        /// </summary>
        private static System.Reflection.PropertyInfo FindSelectionCountProperty(ContentPage page)
        {
            return page.GetType().GetProperty("SelectionCount");
        }

        /// <summary>
        /// 🔍 DETECÇÃO: Encontra propriedade de controle de formulário (HasTextToSave, CanSave, etc.)
        /// </summary>
        private static System.Reflection.PropertyInfo FindFormControlProperty(ContentPage page)
        {
            var pageType = page.GetType();

            // 🎯 ESTRATÉGIA 1: Procura por nomes conhecidos
            var knownControlNames = new[] { "HasTextToSave", "CanSave", "HasChanges", "IsDirty" };
            foreach (var name in knownControlNames)
            {
                var prop = pageType.GetProperty(name);
                if (prop != null && prop.PropertyType == typeof(bool) && prop.CanWrite)
                {
                    System.Diagnostics.Debug.WriteLine($"🔍 Propriedade de controle encontrada: {name}");
                    return prop;
                }
            }

            return null;
        }

        /// <summary>
        /// 🔍 HELPER: Verifica se é tipo de coleção
        /// </summary>
        private static bool IsCollectionType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) &&
                   type != typeof(string) &&
                   !type.IsPrimitive;
        }

        #endregion

        #region Helper Methods - Atualização de Estado

        /// <summary>
        /// 🔄 ATUALIZAÇÃO: Estado da UI para páginas de lista
        /// </summary>
        private static async Task UpdateListUIState(ContentPage listPage, System.Reflection.PropertyInfo collectionProperty)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    // ✅ CHAMA: Método UpdateUIState se existir
                    var updateUIStateMethod = listPage.GetType().GetMethod("UpdateUIState",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                    if (updateUIStateMethod != null)
                    {
                        updateUIStateMethod.Invoke(listPage, null);
                        System.Diagnostics.Debug.WriteLine($"✅ UpdateUIState chamado para {listPage.GetType().Name}");
                    }
                    else
                    {
                        // 🛡️ FALLBACK: Atualização básica de estado
                        var collection = collectionProperty.GetValue(listPage) as IEnumerable;
                        var hasItems = collection?.Cast<object>().Any() ?? false;

                        // Tenta encontrar e atualizar elementos de UI conhecidos
                        UpdateKnownUIElements(listPage, hasItems);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao atualizar estado da UI: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔄 ATUALIZAÇÃO: Estado para páginas de formulário
        /// </summary>
        private static async Task CheckAndUpdateFormState(ContentPage formPage, System.Reflection.PropertyInfo hasTextProperty)
        {
            try
            {
                // ✅ ESTRATÉGIA: Procura por campos de entrada com texto
                var entryFields = FindEntryFields(formPage);
                var hasText = false;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var entry in entryFields)
                    {
                        if (!string.IsNullOrWhiteSpace(entry.Text))
                        {
                            hasText = true;
                            break;
                        }
                    }

                    // ✅ ATUALIZA: Propriedade de controle baseado no conteúdo encontrado
                    hasTextProperty.SetValue(formPage, hasText);
                    System.Diagnostics.Debug.WriteLine($"✅ PageExtensions: {formPage.GetType().Name} - hasText={hasText} detectado automaticamente");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao verificar estado do formulário: {ex.Message}");
            }
        }

        /// <summary>
        /// 🔍 HELPER: Encontra campos Entry na página
        /// </summary>
        private static List<Entry> FindEntryFields(ContentPage page)
        {
            var entries = new List<Entry>();

            try
            {
                // 🎯 BUSCA: Recursiva por Entry fields
                FindEntriesRecursive(page.Content, entries);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao buscar Entry fields: {ex.Message}");
            }

            return entries;
        }

        /// <summary>
        /// 🔍 RECURSIVO: Busca Entry fields em toda a árvore visual
        /// </summary>
        private static void FindEntriesRecursive(Element element, List<Entry> entries)
        {
            if (element is Entry entry)
            {
                entries.Add(entry);
                return;
            }

            // ✅ CORRIGIDO: Casting apropriado para Layout
            if (element is Layout layout)
            {
                foreach (var child in layout.Children.OfType<Element>())
                {
                    FindEntriesRecursive(child, entries);
                }
            }
            else if (element is ContentView contentView && contentView.Content is Element contentElement)
            {
                FindEntriesRecursive(contentElement, entries);
            }
            else if (element is ScrollView scrollView && scrollView.Content is Element scrollElement)
            {
                FindEntriesRecursive(scrollElement, entries);
            }
        }

        /// <summary>
        /// 🔄 HELPER: Atualiza elementos de UI conhecidos
        /// </summary>
        private static void UpdateKnownUIElements(ContentPage page, bool hasItems)
        {
            try
            {
                // ✅ PROCURA: Por elementos de UI conhecidos
                var emptyStateFrame = page.FindByName<Frame>("emptyStateFrame");
                var collectionView = page.FindByName<CollectionView>("locaisCollectionView") ??
                                   page.FindByName<CollectionView>("pessoasCollectionView") ??
                                   page.FindByName<CollectionView>("itemsCollectionView");

                if (emptyStateFrame != null)
                {
                    emptyStateFrame.IsVisible = !hasItems;
                }

                if (collectionView != null)
                {
                    collectionView.IsVisible = hasItems;
                }

                System.Diagnostics.Debug.WriteLine($"✅ UI básica atualizada - hasItems: {hasItems}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao atualizar elementos de UI: {ex.Message}");
            }
        }

        #endregion
    }
}