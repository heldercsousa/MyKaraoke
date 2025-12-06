# CLAUDE.md - MyVocaList Project Context

> **Living Documentation for AI-Assisted Development**  
> Last Updated: October 14, 2025  
> Version: 2.1 (Corrected)

---

## 📱 Application Overview

**MyVocaList** is a comprehensive .NET MAUI 8.0 mobile application designed for intelligent karaoke queue management with advanced features and future social network capabilities.

### Core Purpose
Manage karaoke participant queues with intelligent round-based organization, allowing administrators to track participation/absence, reorder singers, and provide real-time queue status information.


## Detailed Objectives

MyVocaList is a .NET MAUI 8.0 application for managing participant queues in karaoke rounds. It allows managing 1 queue at a time, 
enabling the user to register participation/absence of each singer as they reach position 1 in the queue! When all singers who 
entered the queue have participated or been absent, the round is incremented (round 1, round 2, etc). It allows the user to end a 
round even if there are singers in the queue who haven't participated in the round! It also allows enabling the last closed queue 
as a workaround when the user accidentally ends a queue! It allows reverting the round to its last state, in case the user realizes 
there was a registration error or accidentally ended a round! It also allows moving singers to any position in the queue. It enables 
singers to register in the queue autonomously, where the queue administrator receives notifications for each new singer registration! 
It displays the estimated queue completion time, based on the number of singers still pending to sing in the current round. There are 
also 2 queue modes (mechanical karaoke and bandokê - artist/band performs the instrumental). 

The code is being developed by Claude AI, with Helder serving as the architect and auditor of the work, guiding the AI on the 
technical approaches to be adopted and continuously monitoring performance optimization opportunities.

Key technical achievements include: multilingual support (11 languages), robust anti-crash pthread_mutex system, component-based 
architecture with dependency injection, performance-optimized animations with hardware detection, and SQLite database with Entity 
Framework integration.

### Bandokê Queue Mode
Artists/bands can optionally register their song catalog. Song lyrics can be stored in the MyVocaList database or obtained via 
third-party APIs like Genius.com. The app allows the administrator to register or change the song a participant sang at any 
time (as long as the queue is still active). If a singer is going to sing a song not registered in the artist/band catalog, 
the administrator can register the song the singer will perform just before the performance. If there's an internet connection 
and the song lyrics are not available in local data, it will fetch the lyrics via third-party APIs.

-
### MVP Key Features (English-Only)
- **Queue Management**: One active queue at a time with round-based progression
- **Participation Tracking**: Admin registers singers and marks participation/absence when reaching position 1
- **Round System**: Automatic round increment when all participants complete
- **Flexibility**: End rounds prematurely or revert to last state
- **Admin-Managed Registration**: Each singer registered by admin using standalone device
- **Queue Modes**: Mechanical karaoke and Bandokê (live instrumental)
- **Time Estimation**: Display estimated completion time based on pending singers
- **Multi-language Infrastructure**: 6 languages supported (MVP focuses on English only)

### Future Features (Post-MVP)
- **Singer Autonomy**: Self-registration capability with admin notifications
- **Facial Recognition**: Quick registration for returning singers
- **Song Medleys**: The app will allow bands to register song medleys in the catalog. In this case, lyrics will notbe displayed on screen unless the medley has been previously registered by the band/musician.
- **Song History**: Personalized song suggestions based on past performances
- **Social Network Integration**: Singer profiles, followers, interactions
- **Live Competitions**: Real-time voting, scoring, leaderboards
- **Cloud Synchronization**: Multi-device support with cloud backend

---

### Nice to Have & AI Roadmap (Future Vision)

**AI Engineering Roadmap Summary:**
A comprehensive roadmap for integrating AI features is detailed in `Docs/Guides/ai_engineering_roadmap.md`. Key areas include:
- **Singer Performance**: AI Score (Pitch/Timing), Real-time Feedback, Note Visualization, Lyrics Sync.
- **Host Efficiency**: Wait Time Prediction, Voice Commands, Intelligent Song Suggestions.
- **Social Engagement**: AI Persona Generation, Audience Voting, Band Score Generation.
- **Development**: MD3 Compliance Agents, Token Optimization.

**Refer to `Docs/Guides/ai_engineering_roadmap.md` for the complete detailed roadmap.**

---

## 👥 Development Team

### **Helder (Project Architect & Technical Auditor)**
- **Role**: Software Architect, Technical Leader, and Quality Auditor
- **Responsibilities**:
  - Defines technical approaches and architectural decisions
  - Guides AI development through strategic technical leadership
  - Conducts code reviews and quality audits
  - Monitors performance optimization opportunities continuously
  - Makes critical decisions on trade-offs (Scoped vs Singleton, architectural patterns)
  - Identifies and prioritizes technical debt and critical issues
  - Manages project complexity and implementation priorities
  - Ensures compliance with .NET MAUI best practices and mobile development standards

### **Claude AI (Code Developer)**
- **Role**: Code Implementation Specialist
- **Responsibilities**:
  - Implements code according to architectural guidelines provided by Helder
  - Develops features, fixes bugs, and creates technical solutions
  - Follows established patterns and coding standards
  - Provides technical analysis and implementation suggestions
  - Creates comprehensive documentation and code comments
  - Performs systematic debugging and troubleshooting
  - Ensures code quality and maintainability

### **Collaborative Process**
The development follows a structured approach where Helder provides strategic direction and technical oversight while Claude AI handles the detailed implementation work. This partnership combines human architectural vision with AI's systematic code development capabilities.

---

## 🏗️ Technical Stack

### Framework & Core Technologies
- **.NET MAUI 8.0**: Cross-platform mobile framework (net8.0-android)
- **C# 13**: Primary programming language (latest features enabled)
- **XAML**: UI markup language
- **SQLite**: Local database storage
- **Entity Framework Core 9.0.6**: ORM for database operations with migrations

### NuGet Packages
```plaintext
- net8.0-android
- net8.0-ios
- net8.0-maccatalyst
- net8.0-windows10.0.19041.0
- Microsoft.EntityFrameworkCore.Sqlite (versão 9.0.6)
- Microsoft.EntityFrameworkCore.Proxies (versão 9.0.6)
- Microsoft.EntityFrameworkCore.Design (versão 9.0.6)
- Microsoft.Maui.Controls (versão 8.0.100)
- Microsoft.Maui.Controls.Xaml (versão 8.0.100)
- Microsoft.Maui.Controls.Capability (versão 8.0.100)
- Microsoft.Extensions.DependencyInjection
```

### Platform Support
- **Primary Platform**: Android 13+
- **Future Platform**: iOS 16+ (planned)
- **Build System**: .NET CLI / Visual Studio

---

## 🏗️ Architecture Summary

**5-Project Clean Architecture:**
- **Domain** (entities) → **Contracts** (DTOs) → **Services** (business logic) → **Infra.Data** (repositories/EF Core) → **View** (MAUI UI)

**Key Rules:**
- Business logic ONLY in Services layer
- Use ServiceProvider pattern for DI in pages
- Interface + Implementation in SAME folder (no subfolders)
- Repository pattern with EF Core 9.0.6

**Full details:** See `MyVocaList_migration_clean_architecture_guide.md`

---

## 📁 Project Structure
```
MyVocaList.sln
├── MyVocaList.Domain/              # Pure entities
├── MyVocaList.Contracts/           # DTOs
├── MyVocaList.Services/            # Business logic
├── MyVocaList.Infra.Data/          # EF Core + repositories
└── MyVocaList.View/                # MAUI UI
    ├── Pages (at root, no subfolder!)
    ├── Components/
    ├── Behaviors/
    ├── Resources/Styles/
    └── Platforms/Android/
```

**Critical folders detail:**
- Pages at View ROOT (no Pages/ subfolder)
- Interface + Implementation in same folder
- No /Helpers folder (use Extensions or Components)

--

## 🎯 Current Layer Responsibilities

### **Domain Layer** (`MyVocaList.Domain`)
- **Contains**: Pure business entities (POCOs)
- **Dependencies**: NONE
- **Purpose**: Core business concepts
- **Rules**: 
  - No references to other projects
  - No framework dependencies
  - Only properties and navigation properties
  - **NO business logic** (business logic belongs in Services!)

### **Contracts Layer** (`MyVocaList.Contracts.*`)
- **Contains**: ViewModels, DTOs
- **Dependencies**: NONE
- **Purpose**: Data transfer and presentation abstractions

### **Services Layer** (`MyVocaList.Services.*`)
- **Contains**: ALL business logic and validation
- **Dependencies**: Domain, Contracts, Infrastructure
- **Purpose**: Implement use cases and business rules
- **Responsibilities**:
  - All validation and business rules
  - Coordinate multiple repositories
  - Transaction management
  - Domain ↔ DTO transformations

### **Infrastructure Layer** (`MyVocaList.Infra.*`)
- **Contains**: Data access, utilities
- **Dependencies**: Domain
- **Purpose**: Technical concerns and database access

### **Presentation Layer** (`MyVocaList.View.*`)
- **Contains**: MAUI UI, pages, components
- **Dependencies**: Infrastructure (MauiProgram DI repositories registration), Services (MauiProgram DI services registration and usage), Contracts
- **Purpose**: User interface and interaction

---

## 🔄 When to Modify Each Layer

### Adding New Entity (e.g., "Song")

```
Step 1: Domain Layer
├─ Add Song.cs to MyVocaList.Domain
├─ Define properties and relationships
└─ NO business logic!

Step 2: Infrastructure Layer  
├─ Add SongConfiguration.cs to MyVocaList.Infra.Data.Config
├─ Add ISongRepository.cs & SongRepository.cs (same folder!)
├─ Create migration: dotnet ef migrations add AddSongEntity
└─ Review migration

Step 3: Contracts Layer
├─ Add SongDto.cs to MyVocaList.Contracts.DTOs
└─ Add SongListItemDto.cs to MyVocaList.Contracts.DTOs.List

Step 4: Services Layer
├─ Add ISongService.cs & SongService.cs (same folder!)
├─ Add SongMapper.cs to MyVocaList.Services.Mappers
├─ Implement ALL business logic in service
└─ Register in MauiProgram.cs (DI)

Step 5: Presentation Layer
├─ Add SongPage.xaml/.cs (in View root, no Pages/ subfolder!)
├─ Use ServiceProvider pattern
└─ Bind to DTOs
```

### New Business Rule
**Always Services layer**, never Domain, Infrastructure (Infra.Data.Repositories) or View (directly in pages code-behind) layers!

### New UI Component
```
MyVocaList.View.Components/
├─ Add MyComponent.xaml
├─ Add MyComponent.xaml.cs
└─ Add styles to Resources/Styles/[Component]Styles.xaml
    (Only if styles are VERY specific to this component!)
    (Most styles should be in global Styles.xaml files)
```

---
## 🌍 Localization

**MVP: English-only** (no localization yet)

**Infrastructure supports 6 languages:**
- en (English) - MVP focus
- pt (Portuguese), es (Spanish), fr (French)
- ja (Japanese), ko (Korean)

**NEVER implement localization in MVP** - wait for post-MVP phase.

---

## 💼 Business Logic & Services

### Service Registration (MauiProgram.cs)

```csharp
// === UTILITIES (SINGLETON - stateless) ===
builder.Services.AddSingleton<ITextNormalizer, TextNormalizer>();
builder.Services.AddSingleton<ILanguageService, LanguageService>();

// === REPOSITORIES (SCOPED - database context) ===
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IParticipacaoEventoRepository, ParticipacaoEventoRepository>();

// === SERVICES (SCOPED - with state) ===
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<IEstabelecimentoService, EstabelecimentoService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

// === PAGES (TRANSIENT - new instance per navigation) ===
builder.Services.AddTransient<SplashPage>();
builder.Services.AddTransient<TonguePage>();
builder.Services.AddTransient<StackPage>();
builder.Services.AddTransient<PersonPage>();
builder.Services.AddTransient<SpotPage>();
builder.Services.AddTransient<SpotFormPage>();
```

---

## 🗂️ Database Architecture

### Entity Framework Core 9.0.6

**Connection String**: 
```csharp
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "myvocalist.db");
options.UseSqlite($"Data Source={dbPath}")
```

### Key Features
- **DatabaseLoadingInterceptor**: Automatic loading indicators
- **Text Normalization**: Multilingual search (6 languages)
- **Hybrid Validation**: Input (200 chars) + Database (250 chars)
- **Homonym Handling**: Birthday/Email for disambiguation

---

## 🎨 Material Design Quick Reference

**Always use styles (NEVER hardcoded values):**
- Colors: `{StaticResource Primary}`, `{StaticResource OnSurface}`
- Typography: `{StaticResource BodyLarge}`, `{StaticResource TitleMedium}`
- Spacing: Multiples of 8 (16, 24, 32)
- Buttons: MaterialButtonFilled (primary), MaterialButtonOutlined (secondary)

**Full MD3 guidelines:** See `Docs/Guides/MyVocaList_migration_material_design_guide.md`

**Common mistakes to avoid:**
❌ BackgroundColor="#E91E63"  
✅ BackgroundColor="{StaticResource Primary}"

❌ FontSize="16"  
✅ Style="{StaticResource BodyLarge}"

---

## 📋 Code Conventions - CRITICAL

### Language Standard: ENGLISH ONLY

**ABSOLUTE REQUIREMENT - NO EXCEPTIONS:**
- ✅ ALL variable names: English
- ✅ ALL function/method names: English
- ✅ ALL class/interface names: English
- ✅ ALL comments: English
- ✅ ALL UI strings: English
- ✅ ALL database fields: English
- ✅ ALL file names: English

**Current Status:**
- ❌ Legacy code is in Portuguese (being migrated)
- ✅ NEW code must be 100% English
- 🔄 When modifying existing Portuguese code, translate it to English

**Before writing ANY code, ask yourself:**
"Is this in English? If NO, rewrite in English."

**Examples:**

❌ WRONG (Portuguese):
```csharp
public void ValidarUsuario(string nomeUsuario)
{
    // Verifica se usuário existe
    if (string.IsNullOrEmpty(nomeUsuario))
        throw new Exception("Nome de usuário inválido");
}
```

✅ CORRECT (English):
```csharp
public void ValidateUser(string username)
{
    // Check if user exists
    if (string.IsNullOrEmpty(username))
        throw new Exception("Invalid username");
}
```
### ServiceProvider Pattern (Critical!)

```csharp
public partial class MyPage : ContentPage
{
    private ServiceProvider? _serviceProvider;
    private IMyService? _myService;

    public MyPage()  // MUST be parameterless!
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        if (Handler != null)
        {
            _serviceProvider = ServiceProvider.FromPage(this);
            _myService = _serviceProvider.GetService<IMyService>();
            _ = LoadDataAsync();
        }
    }
}
```
## 🔄 Behaviors for Code Reuse

**Avoid code duplication using Behaviors:**

**SmartPageLifecycleBehavior** - Page lifecycle + loading + navbar


```xml
<behaviors:SmartPageLifecycleBehavior 
    NavBar="{x:Reference CrudNavBar}"
    LoadDataCommand="{Binding LoadDataCommand}"
    UseGlobalLoading="True" />
```

**SafeNavigationBehavior** - Thread-safe navigation with debounce
```xml
<behaviors:SafeNavigationBehavior 
    TargetPageType="{x:Type local:SpotFormPage}"
    DebounceMilliseconds="800" />
```

**NavBarBehavior** - Dynamic button generation (used internally by components)

**When to use:** Repeated code in 3+ pages, configurable via XAML
**When NOT to use:** Page-specific logic, business rules

**Detailed documentation:** Full examples and troubleshooting in project knowledge base. 

---
## 📋 Documentation Standards
**Changelog & Git Workflow:** See `changelog.md` file header for complete workflow documentation.

---

**Last Updated**: October 17, 2025  
**Version**: 2.2
**Maintained by**: Helder (Architect) + Claude AI (Developer)