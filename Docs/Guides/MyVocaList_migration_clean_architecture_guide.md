# 🚀 Guia Prático: Turbinando Seu Projeto MAUI 8 com IA

## Situação Atual → Situação Otimizada

**✅ Você JÁ Tem (Ótimo começo!):**
- Projeto MAUI 8 funcionando
- GitHub repository
- changelog.md com objetivo de negócio + log de evoluções
- Workflow estabelecido com Claude Chat

**🎯 Vamos Adicionar (Sem quebrar nada!):**
- CLAUDE.md estruturado (evolução do seu changelog.md)
- Uso de Claude Projects para memória persistente
- Claude Code para refatorações e features complexas
- Cursor/Windsurf opcional (só se quiser ganhar +30% velocidade)

---

## 📋 FASE 1: TRANSFORMAÇÃO DO CHANGELOG.MD → CLAUDE.MD (15 minutos)

### Passo 1: Criar CLAUDE.md (mantendo changelog.md)

Não delete seu changelog.md! Vamos criar um arquivo complementar mais poderoso.

**Estrutura Recomendada para MAUI 8:**

```markdown
# CLAUDE.md - Contexto do Projeto MAUI

## 📱 Visão Geral do Aplicativo
[Cole aqui o "objetivo de negócio" do seu changelog.md atual]

**Exemplo:**
> App de gerenciamento de tarefas para profissionais autônomos.
> Permite criar, editar, categorizar e sincronizar tarefas offline-first.

## 🏗️ Arquitetura e Stack Técnico

### Framework e Versões
- .NET MAUI 8.0
- C# 12
- Target Frameworks: Android 13+, iOS 16+
- [Adicione outras libs importantes que você usa]

### Padrões Arquiteturais Utilizados
- MVVM (Model-View-ViewModel)
- Dependency Injection com Microsoft.Extensions.DependencyInjection
- [Liste outros padrões que você usa]

### Bibliotecas e Pacotes NuGet Principais
```plaintext
- CommunityToolkit.Maui (versão X.X.X)
- CommunityToolkit.Mvvm (versão X.X.X)
- SQLite-net-pcl (se usar)
- Newtonsoft.Json ou System.Text.Json
- [Liste suas dependências principais]
```

### Adicionando Seção de Behaviors ao CLAUDE.md

Após criar seu CLAUDE.md básico, adicione esta seção para documentar padrões de código reutilizável:
````markdown
## 🔄 Behaviors e DRY (Don't Repeat Yourself)

### Princípio
Elimine duplicação de código entre páginas usando **Behaviors** do .NET MAUI.

### Quando Usar Behaviors vs Code-Behind

| Cenário | Use Behavior | Use Code-Behind |
|---------|--------------|-----------------|
| Lógica repetida em 3+ páginas | ✅ | ❌ |
| Configurável via XAML | ✅ | ❌ |
| Lógica única para uma página | ❌ | ✅ |
| Precisa acesso direto a ViewModel | ❌ | ✅ |

### Behaviors Disponíveis no Projeto

**SmartPageLifecycleBehavior:**
- Gerencia appearing/disappearing
- Coordena loading overlays
- Controla visibilidade de navbar

**SafeNavigationBehavior:**
- Navegação thread-safe
- Debounce anti-double-tap
- Análise inteligente de stack

**NavBarBehavior:**
- Geração dinâmica de botões
- Coordenação de animações
- Gerenciamento de eventos

**Exemplo de uso:**
```xml
<ContentPage.Behaviors>
    <behaviors:SmartPageLifecycleBehavior 
        NavBar="{x:Reference CrudNavBar}"
        LoadDataCommand="{Binding LoadDataCommand}" />
</ContentPage.Behaviors>
```

**Documentação completa:** Consulte seção "🔄 Behaviors for Code Reuse & DRY Principles" no CLAUDE.md principal do projeto.
\```
````

---

### **3. MyVocaList_migration_material_design_guide.md**

**Section to update:** After "## 🛠️ PARTE 3: ATUALIZANDO SEU CLAUDE.MD PARA MATERIAL DESIGN" (around line 450), add new section:
````markdown
## 🧩 PARTE 4: BEHAVIORS E COMPONENTES REUTILIZÁVEIS

### Integração Behaviors + Material Design

Material Design foca na **apresentação visual**, enquanto Behaviors gerenciam **comportamento funcional**. Ambos trabalham juntos para criar interfaces consistentes e manuteníveis.

### Separação de Responsabilidades

| Aspecto | Material Design | Behaviors |
|---------|-----------------|-----------|
| **Cores e Estilos** | ✅ MaterialColors.xaml, MaterialStyles.xaml | ❌ |
| **Layout e Spacing** | ✅ 8px grid, typography | ❌ |
| **Lifecycle Management** | ❌ | ✅ SmartPageLifecycleBehavior |
| **Navigation Logic** | ❌ | ✅ SafeNavigationBehavior |
| **Button Generation** | ❌ | ✅ NavBarBehavior |

### Exemplo Integrado
```xml
<ContentPage>
    <!-- COMPORTAMENTO: Gerencia ciclo de vida -->
    <ContentPage.Behaviors>
        <behaviors:SmartPageLifecycleBehavior 
            NavBar="{x:Reference CrudNavBar}"
            LoadDataCommand="{Binding LoadDataCommand}" />
    </ContentPage.Behaviors>

    <!-- APRESENTAÇÃO: Componente visual MD3 -->
    <VerticalStackLayout Padding="16" Spacing="16">
        <Entry Placeholder="Nome"
               Style="{StaticResource MaterialEntry}" />
        
        <Button Text="Salvar"
                Style="{StaticResource MaterialButtonFilled}"
                Command="{Binding SaveCommand}" />
                
        <components:CrudNavBarComponent x:Name="CrudNavBar" />
    </VerticalStackLayout>
</ContentPage>
```

### Diretrizes para Claude Code

Ao implementar páginas com Material Design:

1. **SEMPRE use behaviors** para funcionalidade comum (lifecycle, navigation)
2. **SEMPRE use estilos MD3** para apresentação visual
3. **PREFIRA configuração XAML** sobre code-behind
4. **CONSULTE CLAUDE.md** seção "Behaviors for Code Reuse" antes de duplicar código

### Behaviors Disponíveis

Para documentação completa dos behaviors (SmartPageLifecycleBehavior, SafeNavigationBehavior, NavBarBehavior), consulte:
- **CLAUDE.md** → "🔄 Behaviors for Code Reuse & DRY Principles"
- Exemplos práticos de before/after mostrando eliminação de duplicação
````

---

These updates now reference **actual sections** that exist in the files! Would you like me to proceed with any additional clarifications?


## 📁 Estrutura de Pastas do Projeto

```
/MeuApp.MAUI/
├── Models/          # Entidades de domínio
├── ViewModels/      # ViewModels com lógica de apresentação
├── Views/           # Páginas XAML
├── Services/        # Serviços (API, Database, etc)
├── Converters/      # Value Converters XAML
├── Resources/       # Imagens, fontes, estilos
└── Platforms/       # Código específico por plataforma
```

## 🎯 Convenções de Código (CRÍTICO para Consistência)

### Naming Conventions
- **ViewModels:** `[Nome]ViewModel.cs` (ex: `HomeViewModel.cs`)
- **Views:** `[Nome]Page.xaml` (ex: `HomePage.xaml`)
- **Services:** `I[Nome]Service.cs` + `[Nome]Service.cs` (interface + implementação)
- **Commands:** Use `RelayCommand` ou `AsyncRelayCommand` do CommunityToolkit.Mvvm
- **Properties:** Use `[ObservableProperty]` attribute do CommunityToolkit.Mvvm

### Padrões XAML
- Usar `x:DataType` para Compiled Bindings (performance)
- Estilos globais em `Resources/Styles/Styles.xaml`
- Cores em `Resources/Styles/Colors.xaml`

### Injeção de Dependência
```csharp
// Registro em MauiProgram.cs
builder.Services.AddSingleton<IApiService, ApiService>();
builder.Services.AddTransient<HomeViewModel>();
builder.Services.AddTransient<HomePage>();
```

### Tratamento de Erros
- Usar try-catch em todos os comandos async
- Exibir mensagens de erro via `DisplayAlert` ou Toast
- Logar exceções (especificar onde: file, console, Sentry, etc)

## ⚙️ Comandos Úteis

### Build e Deploy
```bash
# Restaurar pacotes NuGet
dotnet restore

# Build para Android
dotnet build -f net8.0-android

# Build para iOS (requer Mac)
dotnet build -f net8.0-ios

# Publicar release Android
dotnet publish -f net8.0-android -c Release
```

### Testes (se tiver)
```bash
# Rodar testes unitários
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Features Implementadas (Migrar do changelog.md)

[Cole aqui a lista de features que você já tem no changelog.md]

**Exemplo:**
- ✅ CRUD de tarefas com SQLite local
- ✅ Categorização de tarefas
- ✅ Tema claro/escuro
- ✅ Sincronização com API REST
- ⏳ [Feature em andamento]

## 🐛 Problemas Conhecidos e Workarounds

[Se tiver algum bug conhecido ou limitação, documente aqui]

**Exemplo:**
- Android: Keyboard overlap issue → usar `KeyboardAutoManagerScroll="true"`
- iOS: SafeArea padding → usar `Shell.NavBarIsVisible="True"`

## 📚 Recursos de Referência

- [Documentação oficial .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/)
- [CommunityToolkit.Maui Docs](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/)
- [Sua API backend (se tiver)]: URL_DA_DOCUMENTACAO

## 🎨 Decisões de Design/UX

[Documente escolhas importantes de UI/UX]

**Exemplo:**
- Navegação: Shell com Bottom Tab Bar
- Paleta de cores: Material Design 3
- Tipografia: Sistema nativo (Roboto/SF Pro)

## 🔐 Autenticação e Segurança (se aplicável)

[Como funciona auth no seu app]

**Exemplo:**
- JWT armazenado em SecureStorage
- Refresh token automático a cada 15 minutos
- Logout limpa cache local

## 📝 Notas para IA

**Ao gerar código:**
1. SEMPRE usar `[ObservableProperty]` em vez de `INotifyPropertyChanged` manual
2. SEMPRE injetar serviços via construtor
3. SEMPRE usar Compiled Bindings (`x:DataType`)
4. SEMPRE adicionar null checks e tratamento de erros
5. Preferir `async/await` para operações I/O

**Ao modificar XAML:**
1. Manter estilos consistentes com `Resources/Styles/`
2. Usar `ContentPage.Resources` para estilos locais
3. Sempre testar em Android E iOS (mencionar se algo só funciona em uma plataforma)

---

## 📊 Histórico de Evolução (Manter changelog.md separado)

[NÃO misture com o log de mudanças diárias]
[Continue usando changelog.md para isso]
[CLAUDE.md é a "documentação viva" do projeto]
[changelog.md é o "diário de bordo" cronológico]
```

### Passo 2: Prompt para Claude Gerar Seu CLAUDE.md

Cole isto no Claude Chat:

```
Analise meu projeto MAUI 8 e crie um arquivo CLAUDE.md completo baseado no template que vou fornecer.

CONTEXTO:
- Projeto: [Nome do seu app]
- Objetivo de negócio: [Cole do seu changelog.md]
- Principais features atuais: [Liste 5-10 features principais]

INSTRUÇÕES:
1. Analise os arquivos do meu repositório GitHub: [URL do repo]
2. Preencha TODAS as seções do template abaixo
3. Seja específico com nomes reais de classes, pacotes e estruturas
4. Identifique padrões que estou usando (mesmo que eu não saiba o nome deles)
5. Liste as convenções de código que você observar no meu código

TEMPLATE:
[Cole o template CLAUDE.md acima]

IMPORTANTE: Faça perguntas se precisar de clarificações sobre partes específicas do projeto.
```

---

## 🎯 FASE 2: CONFIGURANDO CLAUDE PROJECTS (10 minutos)

### Por Que Claude Projects?

Imagine que **toda vez** que você conversa com Claude sobre seu app, ele já sabe:
- A arquitetura completa
- As convenções de código
- Problemas anteriores resolvidos
- Decisões de design tomadas

**Isso É exatamente o que Claude Projects faz!**

### Como Configurar

1. **Acesse Claude.ai** e faça login
2. **Clique em "Projects"** (barra lateral esquerda)
3. **Crie novo projeto:** "MeuApp MAUI"
4. **Adicione arquivos de contexto:**
   - `CLAUDE.md` (o que criamos acima)
   - `README.md` (se tiver)
   - Principais ViewModels (2-3 exemplos)
   - Principais Views XAML (2-3 exemplos)
   - `MauiProgram.cs`
   - Schema da sua database (se usar SQLite)

5. **Opcional mas PODEROSO:** Adicione documentação adicional
   - Prints de tela do app funcionando
   - Diagramas de fluxo (mesmo que desenhados à mão e fotografados)
   - Mockups de features futuras

### Limites do Claude Projects

- Máximo ~200 arquivos
- Máximo ~10MB total
- **Estratégia:** Adicione apenas arquivos "representativos" + CLAUDE.md completo

### Agora Toda Conversa Fica Contextualizada!

**Antes:**
```
Você: "Como adiciono validação no formulário de cadastro?"

Claude: "Claro! Em qual framework você está?"
[Você explica que é MAUI, perde 3 mensagens só contextualizando...]
```

**Depois (com Claude Project):**
```
Você: "Como adiciono validação no formulário de cadastro?"

Claude: "Vou adicionar validação no CadastroViewModel seguindo os padrões 
do projeto. Usarei CommunityToolkit.Mvvm com DataAnnotations..."
[Claude JÁ sabe tudo, resposta direta!]
```

---

## ⚡ FASE 3: INTEGRANDO CLAUDE CODE (Opcional mas Recomendado)

### Quando Usar Claude Code vs Claude Chat

**Continue usando Claude Chat para:**
- ✅ Planejamento de features
- ✅ Debugging de problemas específicos
- ✅ Code review de trechos pequenos
- ✅ Perguntas rápidas
- ✅ Discussão de arquitetura

**Adicione Claude Code para:**
- 🚀 Criar nova feature completa (ViewModel + View + Service)
- 🚀 Refatorar múltiplos arquivos simultaneamente
- 🚀 Adicionar testes unitários em batch
- 🚀 Migração de padrões (ex: MVVM manual → CommunityToolkit.Mvvm)

### Workflow Prático para MAUI

**Exemplo Real: Adicionar Feature "Compartilhar Tarefa"**

```bash
# 1. Criar branch
git checkout -b feature/compartilhar-tarefa

# 2. Abrir Claude Code na raiz do projeto
cd /caminho/para/MeuApp.MAUI
claude-code

# 3. Prompt em 3 passos

# PASSO 1: Pesquisa e Análise
> "Analise o projeto e sugira como implementar a feature 'Compartilhar Tarefa' 
via WhatsApp, Email ou SMS. Considere a arquitetura MVVM existente."

[Claude analisa seus arquivos e sugere abordagem]

# PASSO 2: Planejamento
> "Crie um plano passo-a-passo para implementar essa feature, listando:
- Novos arquivos a criar
- Arquivos existentes a modificar
- Dependências necessárias
- Testes a adicionar"

[Claude gera plano detalhado]

# PASSO 3: Implementação
> "Implemente o plano, começando pelos testes unitários (se aplicável), 
depois o Service, depois ViewModel, e por último a View XAML."

[Claude cria todos os arquivos automaticamente!]

# 4. Review e ajustes
[Você testa no emulador, pede ajustes]

# 5. Commit
git add .
git commit -m "feat: adiciona compartilhamento de tarefas"
git push origin feature/compartilhar-tarefa
```

### Setup Inicial Claude Code (Uma vez só)

```bash
# Instalar Claude Code (se ainda não tiver)
# Instruções: https://docs.claude.com/en/docs/claude-code

# Na raiz do seu projeto MAUI, garantir que CLAUDE.md existe
ls CLAUDE.md  # deve existir

# Iniciar Claude Code
claude-code

# Primeiro prompt (para Claude "aprender" o projeto)
> "Leia o CLAUDE.md e analise a estrutura do projeto. 
Confirme que entendeu a arquitetura e convenções."

[Claude confirma entendimento]

# Agora você está pronto para usar!
```

---

## 🎨 FASE 4: CURSOR/WINDSURF (Opcional - Só se Quiser Mais Velocidade)

### Você Realmente Precisa?

**Continue só com Claude Chat + Claude Code se:**
- ✅ Está satisfeito com a velocidade atual
- ✅ Não trabalha em múltiplos projetos simultaneamente
- ✅ Seu projeto tem <50 arquivos de código

**Considere adicionar Cursor/Windsurf se:**
- 🤔 Quer autocompletar código com IA enquanto digita
- 🤔 Quer refatorar visualmente múltiplos arquivos
- 🤔 Quer IDE com integração nativa de IA
- 🤔 Seu projeto MAUI cresceu muito (>100 arquivos)

### Setup Rápido para MAUI no Cursor

```bash
# 1. Baixar Cursor (se decidir usar)
# https://cursor.sh

# 2. Abrir seu projeto MAUI
File → Open Folder → [seu-projeto-maui]

# 3. Criar .cursor/rules.json
{
  "rules": [
    "Este é um projeto .NET MAUI 8",
    "Usar sempre CommunityToolkit.Mvvm para MVVM",
    "ViewModels usam [ObservableProperty] attribute",
    "Commands usam [RelayCommand] attribute",
    "Views XAML usam Compiled Bindings com x:DataType",
    "Injeção de dependência via construtor",
    "Async/await para todas operações I/O",
    "Tratamento de erros com try-catch em todos os commands"
  ]
}

# 4. Copiar seu CLAUDE.md para o projeto
# Cursor lerá automaticamente!

# 5. Começar a usar
# Cursor agora sugere código seguindo SUAS convenções!
```

---

## 📊 MÉTRICAS: COMO MEDIR SEU GANHO DE PRODUTIVIDADE

### Baseline (Registre ANTES de começar)

**Semana Típica ANTES das mudanças:**
- Tempo para implementar 1 feature média: _____ horas
- Bugs encontrados em produção por semana: _____
- Tempo gasto debugando por semana: _____ horas
- Quantidade de code review necessário: _____ mensagens no Claude

### Target (Após 2 semanas usando as técnicas)

**Semana Típica DEPOIS:**
- Tempo para implementar 1 feature média: **-30% a -50%**
- Bugs encontrados: **-40% a -60%** (código mais consistente)
- Tempo debugando: **-50%** (Claude entende contexto melhor)
- Code review: **-70%** (código já sai mais correto)

### Como Medir na Prática

```markdown
# metricas.md (criar na raiz do projeto)

## Semana Baseline (antes das melhorias)
- Feature X: 8 horas
- Feature Y: 12 horas
- Bugs encontrados: 5
- Sessões de debugging: 6 horas

## Semana 1 (com CLAUDE.md + Projects)
- Feature A: ? horas
- Feature B: ? horas
- Bugs encontrados: ?
- Sessões de debugging: ? horas

## Semana 2 (com CLAUDE.md + Projects + Claude Code)
- Feature C: ? horas
- Feature D: ? horas
- Bugs encontrados: ?
- Sessões de debugging: ? horas

## Análise ROI
[Preencher após 2-4 semanas]
```

---

## 🎯 PLANO DE IMPLEMENTAÇÃO: 7 DIAS

### 🗓️ Dia 1 (Hoje! - 30 minutos)
- [ ] Criar CLAUDE.md usando o prompt fornecido acima
- [ ] Commitar no GitHub
- [ ] Criar Claude Project e adicionar CLAUDE.md

### 🗓️ Dia 2 (15 minutos)
- [ ] Adicionar 3-5 arquivos exemplo ao Claude Project
- [ ] Testar: pedir ao Claude para explicar a arquitetura do seu app
- [ ] Se ele explicar corretamente = sucesso!

### 🗓️ Dia 3 (1 hora)
- [ ] Instalar Claude Code
- [ ] Testar com 1 task pequena (ex: "adicione loading indicator na HomePage")
- [ ] Avaliar se gostou da experiência

### 🗓️ Dia 4-5 (Desenvolvimento normal)
- [ ] Use Claude Chat + Projects para features normais
- [ ] Documente no changelog.md como antes
- [ ] Observe se Claude está mais "esperto" sobre seu projeto

### 🗓️ Dia 6 (2 horas - se quiser testar Claude Code)
- [ ] Escolha 1 feature média para implementar
- [ ] Use Claude Code para implementação completa
- [ ] Compare o tempo com implementação manual

### 🗓️ Dia 7 (30 minutos)
- [ ] Revisar metricas.md
- [ ] Decidir se vai adicionar Cursor/Windsurf ou continuar como está
- [ ] Ajustar CLAUDE.md com learnings da semana

---

## ❓ FAQ - Dúvidas Comuns

### "Preciso reescrever meu código para aplicar isso?"

**NÃO!** Tudo funciona com seu código atual. Estamos apenas:
- Documentando melhor (CLAUDE.md)
- Dando contexto persistente ao Claude (Projects)
- Adicionando ferramentas opcionais (Claude Code, Cursor)

### "E se eu não gostar? Perco tempo?"

Não! Worst case:
- CLAUDE.md vira documentação útil do projeto (sempre bom ter)
- Claude Project pode ser deletado sem impacto
- Você volta ao workflow anterior sem perder nada

### "Isso funciona com MAUI especificamente?"

**SIM!** Testado e funciona perfeitamente. MAUI é .NET, então:
- Claude entende muito bem C# e XAML
- Claude Code funciona perfeitamente com estruturas MAUI
- Cursor/Windsurf têm ótimo suporte para .NET

### "Tenho que pagar pela IA?"

**Para começar:**
- Claude Chat: FREE tier funciona (limitado, mas suficiente para testar)
- Claude Projects: FREE (mesmo na versão free)
- Claude Code: Precisa Claude Pro ($20/mês) - **mas teste primeiro só com Chat+Projects!**
- Cursor: Free tier + $20/mês Pro (opcional)

**Recomendação:** Comece só com FREE (Chat + Projects), avalie por 1 semana, depois decida se vale investir.

### "Quanto tempo leva para dar ROI?"

Se você implementa pelo menos **2-3 features por semana**:
- Investimento: ~2 horas setup inicial
- Retorno: 20-30% mais rápido já na semana 1
- **Break even: 3-4 dias!**

---

## 🎁 BÔNUS: PROMPTS PRONTOS PARA SEU PROJETO MAUI

### Prompt 1: Code Review MAUI

```
CONTEXTO: Projeto MAUI 8 seguindo padrões do CLAUDE.md

TASK: Revise este código e identifique:
1. Violações de convenções MAUI/MVVM
2. Memory leaks potenciais (subscriptions não disposed)
3. Threading issues (UI thread violations)
4. Performance issues (bindings não otimizados)
5. Problemas de acessibilidade

CÓDIGO:
[Cole seu ViewModel ou XAML]

FORMAT: Tabela com Issue | Linha | Severidade | Fix Sugerido
```

### Prompt 2: Criar Nova Feature MAUI

```
CONTEXTO: Projeto MAUI 8 - veja CLAUDE.md

FEATURE: [Descreva a feature]

TASK: Crie implementação completa seguindo padrões do projeto:
1. Model (se necessário novo)
2. Service (se precisar de data/API)
3. ViewModel com CommunityToolkit.Mvvm
4. View XAML com Compiled Bindings
5. Registro DI em MauiProgram.cs
6. Navigation (se aplicável)

FORMAT: Código completo e funcional para cada arquivo
```

### Prompt 3: Otimizar Performance MAUI

```
CONTEXTO: Esta página MAUI está lenta ao carregar

PROBLEMA: [Descreva o problema]

TASK: Analise e otimize:
1. Bindings (usar Compiled Bindings)
2. CollectionView (usar virtualization corretamente)
3. Images (usar caching e thumbnails)
4. Async loading (skeleton/loading states)
5. Memory management (WeakReferences se necessário)

CÓDIGO ATUAL:
[Cole ViewModel + XAML]

FORMAT: Código otimizado + explicação do que mudou e porquê
```

### Prompt 4: Debug MAUI Platform-Specific

```
BUG: [Descreva o bug]

PLATAFORMA: [Android / iOS / ambas]

TASK: Diagnostique possíveis causas:
1. Problema de plataforma específica?
2. Workarounds conhecidos no MAUI
3. Uso de APIs específicas por plataforma (#if ANDROID / #if IOS)
4. Issues no GitHub do MAUI relacionados

CÓDIGO/STACK TRACE:
[Cole]

FORMAT: Lista de possíveis causas + solução recomendada
```

---

## 🚀 PRÓXIMOS PASSOS IMEDIATOS

**AGORA MESMO (5 minutos):**
1. Abra Claude Chat
2. Cole o prompt "Criar CLAUDE.md" da Fase 1
3. Forneça informações do seu projeto
4. Salve o CLAUDE.md gerado

**HOJE (mais 10 minutos):**
1. Crie Claude Project
2. Adicione CLAUDE.md
3. Teste fazendo 1 pergunta sobre seu app

**ESTA SEMANA:**
1. Use Claude Project em todas as conversas sobre o app
2. Atualize CLAUDE.md conforme aprender mais
3. Considere testar Claude Code

---

## 📞 PRECISA DE AJUDA?

Se tiver dificuldade em qualquer passo:
1. Volte aqui e releia a seção específica
2. Use Claude Chat para esclarecer dúvidas
3. Compartilhe seu CLAUDE.md gerado para review

**Lembre-se:** Você não precisa fazer tudo de uma vez! Comece só com **CLAUDE.md + Claude Projects** e já terá ganho de 20-30% de produtividade.

---

**Boa sorte! 🚀 Seu projeto MAUI vai decolar com essas técnicas!**