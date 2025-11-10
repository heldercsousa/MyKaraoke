# 🎨 Guia Definitivo: Material Design no MAUI + Soluções para Problemas de UI

## 🚀 PARTE 1: IMPLEMENTANDO MATERIAL DESIGN NO SEU MAUI

### Opção A: Material Design com CommunityToolkit.Maui (RECOMENDADO)

**Por quê?**
- Já está no MAUI ecosystem
- Mantido pela Microsoft
- Funciona bem no Android, iOS, Windows
- Fácil integração

## 🎨 PARTE 2: USANDO MATERIAL DESIGN NA PRÁTICA

### ✨ NOVO: Escolhendo o Botão "Salvar" Correto (Hierarquia e Posição)

A ênfase no botão "Salvar" depende crucialmente de **onde** ele está posicionado.

#### Cenário 1: Ação no Top App Bar (Header)

**REGRA:** Ações no header devem ser **Icon Buttons**, que não possuem um fundo preenchido visível. Usar botões com fundo sólido (circulares ou quadrados) nesta área é uma quebra do padrão MD3.

**Opção A: Icon Button Padrão (Ênfase Baixa)**
* **Descrição:** Apenas o ícone.
* **Uso:** Ações secundárias ou quando a tela é muito simples.
```xml
<components:StatefulIcon IconName="check" InactiveColor="{StaticResource OnSurface}" />
```

**Opção B: Icon Button Tinted (Ênfase Média - RECOMENDADO)**
* **Descrição:** O ícone é colorido com a cor Primary. É a forma mais comum e limpa de destacar a ação principal no header.
* **Uso:** Ação primária na maioria dos formulários.
```xml
<components:StatefulIcon IconName="check" InactiveColor="{StaticResource Primary}" />
```

**Opção C: Outlined/Tonal Icon Button (Ênfase Alta)**
* **Descrição:** O ícone fica dentro de um container circular com borda (Outlined) ou fundo sutil (Tonal).
* **Uso:** Quando a ação de salvar precisa de destaque visual máximo dentro do header.
```xml
<Frame Style="{StaticResource OutlinedIconButton}">
    <components:StatefulIcon IconName="check" InactiveColor="{StaticResource Primary}" VerticalOptions="Center" HorizontalOptions="Center" />
</Frame>
```

#### Cenário 2: Ação no Fim da Página

**REGRA:** Para formulários longos, o final da página é o local ideal para um Filled Button.
```xml
<HorizontalStackLayout Spacing="12" HorizontalOptions="End">
    <Button Text="Cancelar" Style="{StaticResource TextButton}" />
    <Button Text="Salvar" Style="{StaticResource FilledButton}" />
</HorizontalStackLayout>
```

#### Cenário 3: Ação Destrutiva (Ex: Excluir)

**REGRA:** Ações destrutivas em diálogos de confirmação devem usar um estilo que aplique a cor Error.
```xml
<Button Text="Excluir" Style="{StaticResource FilledDestructiveButton}" />
```

## 🎨 PARTE 3: UI/UX - Material Design System+

### Paleta de Cores
Este projeto usa Material Design 3 color system. NUNCA use cores hardcoded.

**Cores Principais:**
- `Primary`: Ações principais e elementos de destaque
- `Secondary`: Elementos secundários
- `Surface`: Backgrounds de cards e containers
- `Background`: Background das páginas
- `Error`: Estados de erro

**Uso:**
```xml
<!-- ✅ CORRETO -->
<Button BackgroundColor="{StaticResource Primary}" />

<!-- ❌ ERRADO -->
<Button BackgroundColor="#6750A4" />
```

### Componentes e Padrões
- **Páginas:** `BackgroundColor="{StaticResource Background}"`
- **Botão Principal (em página/dialog):** `Style="{StaticResource FilledButton}"`
- **Botão Secundário:** `Style="{StaticResource TextButton}"`
- **Botão Destrutivo (em dialog):** `Style="{StaticResource FilledDestructiveButton}"`
- **Ação Principal (no header):** `<StatefulIcon IconName="check" InactiveColor="{StaticResource Primary}" />`
- **Cards:** `Style="{StaticResource ElevatedCard}"`
- **Inputs:** `Style="{StaticResource MaterialEntry}"`

### Hierarquia de Tipografia

**Sempre use os estilos predefinidos:**
- `DisplayLarge`: Títulos principais (raro)
- `HeadlineLarge`: Títulos de seções
- `TitleLarge`: Títulos de cards/items
- `BodyLarge`: Texto principal
- `BodyMedium`: Texto secundário
- `LabelSmall`: Legendas e metadados

### Botões Material Design

**Use conforme importância:**

1. **Filled Button** (Primary action - page body/dialog): Ação mais importante
   ```xml
   <Button Text="Salvar" Style="{StaticResource FilledButton}" />
   ```


2. **Outlined Button** (Secondary action): Ação secundária
   ```xml
   <Button Text="Cancelar" Style="{StaticResource OutlinedButton}" />
   ```

3. **Text Button** (Tertiary action): Ação terciária
   ```xml
   <Button Text="Mais" Style="{StaticResource TextButton}" />
      ```

4. **Filled Destructive Button** (Destructive action in dialogs): Ação destrutiva
   ```xml
   <Button Text="Mais" Style="{StaticResource FilledDestructiveButton}" />
   ```
   
5. **Filled Tonal Button** (Ação Secundária Importante ): É um botão de **média-alta ênfase**. Exemplos de quando usar:
- Ação Secundária Importante: Em um diálogo onde "Excluir" é o `FilledDestructiveButton`, "Cancelar" é o `TextButton`, você poderia ter "Arquivar" como um `FilledTonalButton`. É uma ação importante, mas menos final que excluir.
- Ação Principal em um Card: Se você tem um card e o `FilledButton` rosa compete muito com outros elementos, o `FilledTonalButton` (roxo) é uma alternativa mais harmoniosa.
- Hierarquia na Página: Em uma página que tem vários "calls-to-action", você pode usar o `FilledTonalButton` para ações intermediárias, reservando o `FilledButton` principal apenas para a ação final da jornada do usuário.
   ```xml
   <Button Text="Salvar" Style="{StaticResource FilledTonalButton}" />
   ```


**REGRA:** Nunca use mais de 1 Filled Button na mesma tela. Hierarquia visual é crítica.

### Espaçamentos Padrão Material Design

**Use múltiplos de 4:**
- 4dp: Espaçamento mínimo
- 8dp: Espaçamento entre elementos relacionados
- 16dp: Padding interno de cards/containers
- 24dp: Padding de página
- 32dp: Seções grandes

```xml
<!-- ✅ CORRETO -->
<VerticalStackLayout Spacing="16" Padding="24">

<!-- ❌ ERRADO -->
<VerticalStackLayout Spacing="15" Padding="20">
```

### Elevações (Shadows)

Material Design usa elevação para hierarquia:
- 0dp: Flat (sem sombra)
- 1-2dp: Cards normais
- 4-8dp: Cards elevated / Botões floating
- 16dp+: Modals e overlays

### Cards e Containers

**Sempre use MaterialCard para agrupar conteúdo:**
```xml
<Frame Style="{StaticResource MaterialCard}">
    <!-- Conteúdo -->
</Frame>
```

**Para cards com mais destaque:**
```xml
<Frame Style="{StaticResource MaterialCardElevated}">
    <!-- Conteúdo importante -->
</Frame>
```

### Input Fields

**Sempre use MaterialEntry:**
```xml
<Entry Text="{Binding Campo}"
       Placeholder="Digite..."
       Style="{StaticResource MaterialEntry}" />
```

### Regras de Acessibilidade

1. **Contraste:** Sempre use cores `On[Color]` sobre `[Color]`
   - Texto em Surface: use `OnSurface`
   - Texto em Primary: use `OnPrimary`

2. **Tamanho mínimo de toque:** 48x48dp (já definido nos estilos de Button)

3. **Labels:** Todo Entry deve ter Label acima

### Estados Visuais

**Disabled:**
```xml
<Button IsEnabled="{Binding CanSave}"
        Style="{StaticResource MaterialButtonFilled}" />
<!-- MAUI automaticamente aplica opacidade -->
```

**Loading:**
```xml
<ActivityIndicator IsRunning="{Binding IsLoading}"
                   Color="{StaticResource Primary}" />
```

## 📝 Checklist para Claude ao Gerar UI

Quando gerar código XAML, SEMPRE:
- [ ] Usar estilos Material Design (não criar estilos inline)
- [ ] Usar cores do ResourceDictionary (não hardcode)
- [ ] Usar espaçamentos múltiplos de 4
- [ ] Máximo 1 MaterialButtonFilled por tela
- [ ] Labels acima de Entries
- [ ] TapGestureRecognizer com Command (não Clicked)
- [ ] Compiled Bindings (x:DataType)
```

---

## 🐛 PARTE 4: TROUBLESHOOTING - "BOTÕES NÃO FUNCIONAM"

### CHECKLIST COMPLETO (Cole isto no CLAUDE.md!)

```markdown
## 🔧 Troubleshooting UI - Botões MAUI

### Problema: "Botão não responde ao toque"

**CAUSAS COMUNS (teste nesta ordem):**

#### 1. Command está null
```csharp
// ❌ ERRADO - Command não foi inicializado
public ICommand SalvarCommand { get; set; }

// ✅ CORRETO - Command inicializado
[RelayCommand]
private async Task Salvar()
{
    // Implementação
}
// OU
public ICommand SalvarCommand { get; }
public MeuViewModel()
{
    SalvarCommand = new Command(Salvar);
}
```

**Como verificar:**
```xml
<!-- Adicione para debug -->
<Button Text="Teste"
        Command="{Binding SalvarCommand}"
        x:Name="MeuBotao"
        Clicked="MeuBotao_Clicked" />
```
```csharp
private void MeuBotao_Clicked(object sender, EventArgs e)
{
    var button = (Button)sender;
    var command = button.Command;
    Debug.WriteLine($"Command is null? {command == null}");
    Debug.WriteLine($"Command.CanExecute? {command?.CanExecute(null)}");
}
```

#### 2. IsEnabled = false (implícito ou explícito)
```xml
<!-- ❌ ERRADO - Binding pode retornar false -->
<Button IsEnabled="{Binding CanSave}"
        Command="{Binding SalvarCommand}" />
```

**Como verificar:**
```xml
<!-- Teste temporário com IsEnabled hard-coded -->
<Button IsEnabled="True"
        Command="{Binding SalvarCommand}" />
```

Se funcionar = problema no binding de IsEnabled.

#### 3. InputTransparent = True (acidental)
```xml
<!-- ❌ ERRADO -->
<Button InputTransparent="True"
        Command="{Binding SalvarCommand}" />
```

**Busque no XAML:**
- Verifique se Button tem `InputTransparent="True"`
- Verifique se container pai (StackLayout, Grid) tem `InputTransparent="True"`

#### 4. Elemento sobrepondo o Button (Z-index)
```xml
<!-- ❌ ERRADO - Frame pode estar sobre o Button -->
<Grid>
    <Button Grid.Row="0" Command="{Binding Salvar}" />
    <Frame Grid.Row="0" /> <!-- Sobrepõe o Button! -->
</Grid>

<!-- ✅ CORRETO - Use ZIndex -->
<Grid>
    <Frame Grid.Row="0" ZIndex="0" />
    <Button Grid.Row="0" ZIndex="1" Command="{Binding Salvar}" />
</Grid>
```

#### 5. TapGestureRecognizer conflitando
```xml
<!-- ❌ ERRADO - TapGesture no mesmo elemento do Button -->
<Frame>
    <Frame.GestureRecognizers>
        <TapGestureRecognizer Command="{Binding AbrirCommand}" />
    </Frame.GestureRecognizers>
    
    <Button Command="{Binding SalvarCommand}" />
    <!-- Button pode não funcionar! -->
</Frame>

<!-- ✅ CORRETO - Separar gestures ou usar apenas Button -->
<Frame>
    <VerticalStackLayout>
        <Label Text="Título">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding AbrirCommand}" />
            </Label.GestureRecognizers>
        </Label>
        <Button Command="{Binding SalvarCommand}" />
    </VerticalStackLayout>
</Frame>
```

#### 6. Android específico: Ripple effect bloqueando
```xml
<!-- Se problema só no Android, tente: -->
<Button Text="Salvar"
        android:TintMode="SrcIn"
        Command="{Binding SalvarCommand}">
    <Button.Behaviors>
        <toolkit:TouchBehavior />
    </Button.Behaviors>
</Button>
```

#### 7. BindingContext não configurado
```csharp
// ❌ ERRADO - Binding não encontra Command
public MinhaPage()
{
    InitializeComponent();
    // BindingContext não foi definido!
}

// ✅ CORRETO
public MinhaPage(MeuViewModel viewModel)
{
    InitializeComponent();
    BindingContext = viewModel;
}
```

#### 8. Command async sem await
```csharp
// ❌ ERRADO - Exception silenciosa
[RelayCommand]
private async Task Salvar()
{
    await _service.SalvarAsync();
    // Se falhar aqui, botão para de funcionar!
}

// ✅ CORRETO - Tratamento de erros
[RelayCommand]
private async Task Salvar()
{
    try
    {
        await _service.SalvarAsync();
    }
    catch (Exception ex)
    {
        await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
    }
}
```

### SCRIPT DE DEBUG UNIVERSAL

Adicione isto temporariamente ao Button problemático:

```xml
<Button Text="Debug Button"
        x:Name="DebugButton"
        Command="{Binding SalvarCommand}"
        Clicked="DebugButton_Clicked"
        BackgroundColor="Red"
        IsEnabled="True"
        InputTransparent="False"
        ZIndex="999" />
```

```csharp
private void DebugButton_Clicked(object sender, EventArgs e)
{
    var btn = (Button)sender;
    
    Debug.WriteLine("=== BUTTON DEBUG INFO ===");
    Debug.WriteLine($"IsEnabled: {btn.IsEnabled}");
    Debug.WriteLine($"IsVisible: {btn.IsVisible}");
    Debug.WriteLine($"InputTransparent: {btn.InputTransparent}");
    Debug.WriteLine($"ZIndex: {btn.ZIndex}");
    Debug.WriteLine($"Command: {btn.Command}");
    Debug.WriteLine($"Command.CanExecute: {btn.Command?.CanExecute(null)}");
    Debug.WriteLine($"BindingContext: {btn.BindingContext}");
    Debug.WriteLine($"BindingContext Type: {btn.BindingContext?.GetType().Name}");
    Debug.WriteLine("======================");
    
    // Tenta executar manualmente
    if (btn.Command?.CanExecute(null) == true)
    {
        btn.Command.Execute(null);
        Debug.WriteLine("Command executado manualmente!");
    }
    else
    {
        Debug.WriteLine("Command não pode ser executado!");
    }
}
```

**Analise o Output window e identifique qual das 8 causas é o problema.**
```

---

---

## 🧩 PARTE 4: BEHAVIORS E PADRÕES DE CÓDIGO REUTILIZÁVEL

### Por Que Behaviors São Importantes para Material Design?

Material Design foca na **apresentação visual consistente**, enquanto **Behaviors** gerenciam **comportamento funcional reutilizável**. Ambos trabalham juntos para criar interfaces que são:

- ✅ **Visualmente consistentes** (Material Design)
- ✅ **Funcionalmente consistentes** (Behaviors)
- ✅ **Fáceis de manter** (DRY - Don't Repeat Yourself)

### Separação Clara de Responsabilidades

| Aspecto | Material Design | Behaviors |
|---------|-----------------|-----------|
| **Cores, Tipografia, Espaçamentos** | ✅ MaterialColors.xaml, MaterialStyles.xaml | ❌ |
| **Layout Visual (8px grid)** | ✅ Padding, Spacing, Margins | ❌ |
| **Componentes UI (Buttons, Cards)** | ✅ MaterialButton, MaterialCard | ❌ |
| **Ciclo de Vida (Appearing/Disappearing)** | ❌ | ✅ SmartPageLifecycleBehavior |
| **Navegação Thread-Safe** | ❌ | ✅ SafeNavigationBehavior |
| **Geração Dinâmica de Botões** | ❌ | ✅ NavBarBehavior |
| **Coordenação Loading + NavBar** | ❌ | ✅ SmartPageLifecycleBehavior |

**Princípio:** Material Design cuida do "como as coisas aparecem", Behaviors cuidam de "como as coisas funcionam".

---

### Behaviors Disponíveis no MyVocaList

O projeto possui 3 behaviors principais que eliminam duplicação de código:

#### **1. SmartPageLifecycleBehavior**
**Responsabilidade:** Gerencia ciclo de vida das páginas (appearing/disappearing), coordena loading overlays e controla visibilidade de navbars.

**Elimina duplicação de:**
- Código de `OnAppearing()` e `OnDisappearing()` repetido em cada página
- Lógica de exibir/esconder loading indicators
- Coordenação entre carregamento de dados e aparição de navbar

**Uso típico:**
```xml
<ContentPage.Behaviors>
    <behaviors:SmartPageLifecycleBehavior 
        NavBar="{x:Reference CrudNavBar}"
        MainContent="{x:Reference mainContentGrid}"
        LoadDataCommand="{Binding LoadDataCommand}"
        UseGlobalLoading="True" />
</ContentPage.Behaviors>
```

---

#### **2. SafeNavigationBehavior**
**Responsabilidade:** Navegação thread-safe com debounce (anti-double-tap) e análise inteligente de pilha de navegação.

**Elimina duplicação de:**
- Timers de debounce em cada botão
- `MainThread.BeginInvokeOnMainThread()` em código de navegação
- Lógica para determinar para onde voltar (back navigation inteligente)
- Try-catch blocks para tratamento de erros de navegação

**Uso típico:**
```xml
<ContentPage.Behaviors>
    <!-- Navegação para página específica -->
    <behaviors:SafeNavigationBehavior 
        x:Name="FormNavigation"
        TargetPageType="{x:Type local:SpotFormPage}"
        DebounceMilliseconds="800" />

    <!-- Navegação "voltar" inteligente -->
    <behaviors:SafeNavigationBehavior 
        x:Name="BackNavigation"
        EnableSmartStackNavigation="True"
        DebounceMilliseconds="500" />
</ContentPage.Behaviors>
```

---

#### **3. NavBarBehavior**
**Responsabilidade:** Geração dinâmica de botões de navbar baseado em configurações, com animações coordenadas (quando habilitadas).

**Elimina duplicação de:**
- Loops de criação de botões
- Configuração de Grid columns
- Lógica de show/hide com animações
- Sistema de eventos para cliques em botões

**Uso típico (interno aos componentes):**
```xml
<!-- Usado internamente em CrudNavBarComponent -->
<Grid x:Name="ButtonsGrid">
    <Grid.Behaviors>
        <behaviors:NavBarBehavior 
            Buttons="{Binding ButtonConfigs}"
            IsAnimated="False" />
    </Grid.Behaviors>
</Grid>
```

---

### Exemplo Integrado: Página CRUD com Material Design + Behaviors

**SpotPage.xaml - Página de Lista de Locais:**
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:MyVocaList.View"
             xmlns:components="clr-namespace:MyVocaList.View.Components"
             xmlns:behaviors="clr-namespace:MyVocaList.View.Behaviors"
             x:Class="MyVocaList.View.SpotPage"
             x:DataType="local:SpotPage"
             Background="{StaticResource AppBackgroundGradient}">

    <!-- 🔄 BEHAVIORS: Gerenciam funcionalidade -->
    <ContentPage.Behaviors>
        <!-- Gerencia lifecycle + loading + navbar -->
        <behaviors:SmartPageLifecycleBehavior 
            NavBar="{x:Reference CrudNavBar}"
            MainContent="{x:Reference mainContentGrid}"
            LoadDataCommand="{Binding LoadDataCommand}"
            UseGlobalLoading="True" />

        <!-- Navegação segura para formulário -->
        <behaviors:SafeNavigationBehavior 
            x:Name="SpotFormNavigationBehavior"
            TargetPageType="{x:Type local:SpotFormPage}"
            DebounceMilliseconds="800" />

        <!-- Navegação "voltar" inteligente -->
        <behaviors:SafeNavigationBehavior 
            x:Name="BackNavigationBehavior"
            EnableSmartStackNavigation="True"
            DebounceMilliseconds="500" />
    </ContentPage.Behaviors>

    <!-- 🎨 MATERIAL DESIGN: Apresentação visual -->
    <Grid RowDefinitions="Auto,*,Auto">
        
        <!-- Header com estilo MD3 -->
        <components:HeaderComponent Grid.Row="0" Title="Venues" />

        <!-- Content com espaçamento MD3 (16dp) -->
        <Grid Grid.Row="1" x:Name="mainContentGrid" Padding="16">
            
            <!-- Card MD3 -->
            <components:CardWrapperComponent 
                TitleText="Venues" 
                IconPath="spot_purple.png">
                
                <components:CardWrapperComponent.CardContent>
                    <!-- CollectionView com MaterialListItem styles -->
                    <CollectionView ItemsSource="{Binding Spots}">
                        <CollectionView.ItemTemplate>
                            <DataTemplate>
                                <!-- Frame usa MaterialCard style -->
                                <Frame Style="{StaticResource MaterialCard}"
                                       Padding="16"
                                       Margin="0,0,0,8">
                                    
                                    <Grid ColumnDefinitions="*,Auto">
                                        <!-- Texto com tipografia MD3 -->
                                        <Label Grid.Column="0"
                                               Text="{Binding Name}"
                                               Style="{StaticResource BodyLarge}" />
                                        
                                        <Image Grid.Column="1"
                                               Source="chevron_right.png"
                                               WidthRequest="24"
                                               HeightRequest="24" />
                                    </Grid>
                                    
                                </Frame>
                            </DataTemplate>
                        </CollectionView.ItemTemplate>
                    </CollectionView>
                </components:CardWrapperComponent.CardContent>
                
            </components:CardWrapperComponent>
            
        </Grid>

        <!-- Navbar com NavBarBehavior interno -->
        <components:CrudNavBarComponent 
            Grid.Row="2" 
            x:Name="CrudNavBar" />

    </Grid>
</ContentPage>
```

**Resultado:**
- ✅ **Material Design:** Cores, tipografia, espaçamentos, cards consistentes
- ✅ **Behaviors:** Lifecycle, loading, navegação, navbar funcionando automaticamente
- ✅ **Zero duplicação:** Código reutilizável configurado via XAML

---

### Diretrizes para Claude Code ao Criar Páginas

Ao implementar novas páginas com Material Design, **SEMPRE** siga este checklist:

#### **1. Estrutura Base (XAML)**
```xml
<ContentPage Background="{StaticResource AppBackgroundGradient}">
    
    <!-- BEHAVIORS primeiro -->
    <ContentPage.Behaviors>
        <behaviors:SmartPageLifecycleBehavior ... />
        <!-- Adicionar SafeNavigationBehavior se necessário -->
    </ContentPage.Behaviors>
    
    <!-- Layout MD3 -->
    <Grid RowDefinitions="Auto,*,Auto">
        <components:HeaderComponent Grid.Row="0" ... />
        <Grid Grid.Row="1" Padding="16" Spacing="16">
            <!-- Conteúdo com styles MD3 -->
        </Grid>
        <components:NavBarComponent Grid.Row="2" ... />
    </Grid>
    
</ContentPage>
```

#### **2. Behaviors - Quando Usar**

| Cenário | Use Behavior |
|---------|--------------|
| Página carrega dados no appearing | ✅ SmartPageLifecycleBehavior |
| Página navega para outra página | ✅ SafeNavigationBehavior (com TargetPageType) |
| Página tem botão "voltar" | ✅ SafeNavigationBehavior (com EnableSmartStackNavigation) |
| Navbar dinâmica com botões | ✅ Usar CrudNavBarComponent (tem NavBarBehavior interno) |

#### **3. Material Design - Sempre Usar**

| Elemento | Style MD3 Obrigatório |
|----------|----------------------|
| Background da página | `BackgroundColor="{StaticResource Background}"` |
| Cards/Containers | `{StaticResource MaterialCard}` |
| Botão principal | `{StaticResource MaterialButtonFilled}` |
| Botão secundário | `{StaticResource MaterialButtonOutlined}` |
| Input fields | `{StaticResource MaterialEntry}` |
| Títulos | `{StaticResource TitleLarge}` ou `{StaticResource HeadlineLarge}` |
| Texto corpo | `{StaticResource BodyLarge}` ou `{StaticResource BodyMedium}` |
| Padding/Spacing | Múltiplos de 8: `"16"`, `"24"`, `"8"` |

---

### Anti-Patterns: O Que NÃO Fazer

❌ **NÃO duplique código que behaviors já resolvem:**
```xml
<!-- ERRADO: Código duplicado em code-behind -->
protected override void OnAppearing()
{
    base.OnAppearing();
    await LoadDataAsync();
    await _navbar.ShowAsync();
}

<!-- CERTO: Usar SmartPageLifecycleBehavior -->
<behaviors:SmartPageLifecycleBehavior 
    LoadDataCommand="{Binding LoadDataCommand}"
    NavBar="{x:Reference navbar}" />
```

---

❌ **NÃO misture Material Design com hardcoded styles:**
```xml
<!-- ERRADO: Mixing MD3 com valores hardcoded -->
<Button Text="Save" 
        BackgroundColor="#FF5722"    <!-- ❌ hardcoded -->
        Style="{StaticResource MaterialButtonFilled}" />  <!-- ✅ MD3 -->

<!-- CERTO: 100% Material Design -->
<Button Text="Save" 
        Style="{StaticResource MaterialButtonFilled}" />
```

---

❌ **NÃO crie behaviors para lógica de negócio:**
```csharp
// ERRADO: Lógica de negócio no behavior
public class SpotValidationBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        // ❌ Business logic não vai aqui!
        if (spotName.Length < 3) { ... }
    }
}

// CERTO: Lógica de negócio no ViewModel/Service
public class SpotViewModel
{
    public bool ValidateSpotName(string name)
    {
        return !string.IsNullOrEmpty(name) && name.Length >= 3;
    }
}
```

---

### Documentação Completa

Para referência detalhada sobre todos os behaviors, consulte:

**📄 CLAUDE.md → Seção "🔄 Behaviors for Code Reuse & DRY Principles"**

Inclui:
- Documentação completa de cada behavior
- Exemplos de uso (before/after)
- Padrões de interação entre behaviors
- Configuração de bindable properties
- Troubleshooting de problemas comuns
- Performance considerations

---

### Checklist: Página Material Design + Behaviors Completa

Antes de considerar uma página "pronta", verifique:

- [ ] Usa `AppBackgroundGradient` como background
- [ ] Usa styles MD3 (MaterialCard, MaterialButton, MaterialEntry)
- [ ] Spacing e padding em múltiplos de 8
- [ ] Tipografia MD3 (BodyLarge, TitleLarge, etc)
- [ ] SmartPageLifecycleBehavior configurado (se carrega dados)
- [ ] SafeNavigationBehavior configurado (se navega)
- [ ] Navbar usa componente com NavBarBehavior interno
- [ ] Zero código duplicado de lifecycle/navegação
- [ ] Testado no Android (plataforma prioritária)
- [ ] Changelog atualizado com as mudanças

---

## 💬 PARTE 5: PROMPTS MATERIAL DESIGN PARA CLAUDE

### Prompt 1: Gerar Tela Material Design

```
CONTEXTO: Projeto MAUI 8 usando Material Design (veja CLAUDE.md)

TASK: Crie uma tela [NOME] com:
- Material Design 3 components
- Espaçamentos padronizados (múltiplos de 4)
- Hierarquia de tipografia correta
- Botões com hierarquia visual (Filled > Outlined > Text)

ELEMENTOS:
[Liste os elementos da tela]

IMPORTANTE:
- Usar APENAS estilos do MaterialStyles.xaml
- Usar APENAS cores do MaterialColors.xaml
- Compiled Bindings (x:DataType)
- Commands (não Clicked events)

FORMAT: XAML completo + ViewModel se necessário
```

### Prompt 2: Converter Tela Existente para Material Design

```
CONTEXTO: Tenho uma tela MAUI que precisa ser convertida para Material Design

TASK: Refatore este XAML aplicando Material Design:
1. Substituir cores hardcoded por {StaticResource}
2. Aplicar estilos MaterialButton, MaterialCard, etc
3. Corrigir espaçamentos para múltiplos de 4
4. Aplicar hierarquia de tipografia
5. Melhorar hierarquia visual

XAML ATUAL:
[Cole seu XAML]

FORMAT: XAML refatorado com comentários explicando mudanças
```

### Prompt 3: Debug Botão Não Funciona

```
CONTEXTO: Botão MAUI não responde ao toque

SINTOMAS:
- [Descreva: não acontece nada, só funciona às vezes, etc]
- Plataforma: [Android / iOS / Windows]

CÓDIGO XAML:
[Cole o XAML do Button e container pai]

CÓDIGO VIEWMODEL:
[Cole o Command relacionado]

TASK: Analise usando o checklist de troubleshooting do CLAUDE.md e identifique a causa provável. Forneça solução específica.
```

---

## 🎯 RESUMO: SEU PLANO DE AÇÃO

### 📅 DIA 1 (1-2 horas)
1. ✅ Instalar CommunityToolkit.Maui
2. ✅ Criar MaterialColors.xaml (copiar do guia)
3. ✅ Criar MaterialStyles.xaml (copiar do guia)
4. ✅ Registrar em App.xaml
5. ✅ Testar: criar uma página de teste com botões Material Design

### 📅 DIA 2-3 (2-3 horas)
1. ✅ Atualizar CLAUDE.md com seção Material Design (copiar do guia)
2. ✅ Atualizar CLAUDE.md com Troubleshooting (copiar do guia)
3. ✅ Converter 1-2 telas existentes para Material Design
4. ✅ Testar no Android

### 📅 SEMANA 1 (desenvolvimento normal)
1. ✅ Todas novas telas usar Material Design
2. ✅ Converter gradualmente telas antigas (quando mexer nelas)
3. ✅ Usar prompts Material Design para gerar UI
4. ✅ Aplicar checklist de troubleshooting em botões problemáticos

### 📅 SEMANA 2+
1. ✅ Ajustar paleta de cores para seu brand (manter estrutura Material)
2. ✅ Adicionar Dark Mode (opcional)
3. ✅ Considerar animações Material (CommunityToolkit tem suporte)

---

## 🎨 ICONOGRAPHY: Material Design 3 SVG Icons

### 📖 IMPORTANT: Read the Iconography Guide First!

**Before implementing ANY icons, read:**
`MyVocaList_Iconography_MD3_Guideline.md`

This document contains:
- ✅ All official MD3 icon names and their meanings
- ✅ Complete SVG source code for each icon (outlined & filled variants)
- ✅ Icon usage guidelines and rationale
- ✅ Implementation patterns with StatefulIcon component

### ⚡ Quick Implementation Pattern

**ALWAYS follow this pattern when adding icons:**

1. **Check the iconography guide** for the correct icon name
2. **Use StatefulIcon component** (auto-handles sizing, colors, variants)
3. **Pass ONLY IconName** property (component auto-configures everything else)

**Example:**
```xml
<!-- ✅ CORRECT: Only IconName -->
<components:StatefulIcon IconName="nightlife" />

<!-- ❌ WRONG: Don't set WidthRequest, HeightRequest, colors, etc -->
<components:StatefulIcon IconName="nightlife"
                        WidthRequest="24"
                        HeightRequest="24"
                        ActiveColor="..." />
```

### 🔧 StatefulIcon Component Features

**Automatic behaviors:**
- ✅ **Auto-sizing**: Detects parent context (HeaderComponent=24dp, NavBar=24dp, Page=32dp)
- ✅ **Auto-variant**: Switches between `_outlined.svg` and `_filled.svg` based on `IsSelected`
- ✅ **Auto-tinting**: Theme-aware color tinting for dark mode support
- ✅ **Auto-aspect**: Maintains proper aspect ratio

**Properties (all optional except IconName):**
- `IconName` (required): Base name from iconography guide (e.g., "nightlife", "arrow_back")
- `IsSelected` (optional): `false` = outlined, `true` = filled variant
- `Size` (optional): Override auto-detection if needed
- `ActiveColor` / `InactiveColor` (optional): Override theme colors

### 📋 Common Icon Patterns

**Headers (with title icon):**
```xml
<components:HeaderComponent Title="Venues" IconName="nightlife" />
```

**Navigation buttons:**
```xml
<!-- Back: arrow_back, Cancel: close, Save: check -->
<components:HeaderComponent
    IconName="nightlife"
    ShowCancelButton="True"
    ShowSaveButton="True"
    UseCancelIcon="True"
    UseSaveIcon="True" />
```

**Bottom navigation:**
```xml
<components:InactiveQueueBottomNav>
    <!-- Icons auto-loaded from NavButtonConfig with IconName property -->
</components:InactiveQueueBottomNav>
```

### 🎯 Icon Selection Rules

**ALWAYS consult the iconography guide** (`MyVocaList_Iconography_MD3_Guideline.md`) to choose the correct icon:

- **Venues**: `nightlife` (karaoke atmosphere)
- **Queue**: `format_list_numbered` (numbered list)
- **Musicians**: `music_note` (music creation)
- **Singers**: `mic` (singing action)
- **Events**: `event` (scheduled occasions)
- **Back navigation**: `arrow_back`
- **Cancel action**: `close`
- **Confirm/Save**: `check`
- **Settings**: `settings`
- **History**: `history`

**See the full iconography guide for all available icons and their rationale!**

---

## 🎁 BÔNUS: Gerador Automático de Paleta Material Design

Se quiser customizar as cores para seu brand:

1. Acesse: https://m3.material.io/theme-builder
2. Coloque sua cor principal (hex code)
3. Ferramenta gera paleta completa automaticamente
4. Exporte e substitua em MaterialColors.xaml

---

## ✅ CHECKLIST FINAL

Antes de considerar "migrado para Material Design":

- [ ] MaterialColors.xaml criado e registrado
- [ ] MaterialStyles.xaml criado e registrado
- [ ] CLAUDE.md atualizado com seção Material Design
- [ ] CLAUDE.md atualizado com Troubleshooting
- [ ] Pelo menos 3 telas usando Material Design
- [ ] Testado no Android (plataforma prioritária)
- [ ] Claude gerando código correto com novos prompts
- [ ] Botões funcionando consistentemente

---

## 🚀 RESULTADO ESPERADO

**ANTES (sem padrão):**
- 30% do tempo decidindo estilos
- Claude gera código inconsistente
- Muitos bugs de UI
- Botões param de funcionar misteriosamente

**DEPOIS (com Material Design):**
- 5% do tempo em estilos (já definidos!)
- Claude gera 95% correto de primeira
- Bugs de UI raros
- Troubleshooting sistemático quando algo falha

**GANHO TOTAL: ~40-50% mais rápido no desenvolvimento de UI!**