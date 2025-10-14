# CLAUDE.md - MyVocaList Project Context

> **Living Documentation for AI-Assisted Development**  
> Last Updated: October 14, 2025  
> Version: 2.1 (Corrected)

---

## 📱 Application Overview

**MyVocaList** is a comprehensive .NET MAUI 8.0 mobile application designed for intelligent karaoke queue management with advanced features and future social network capabilities.

### Core Purpose
Manage karaoke participant queues with intelligent round-based organization, allowing administrators to track participation/absence, reorder singers, and provide real-time queue status information.

### MVP Key Features (Portuguese-Only)
- **Queue Management**: One active queue at a time with round-based progression
- **Participation Tracking**: Admin registers singers and marks participation/absence when reaching position 1
- **Round System**: Automatic round increment when all participants complete
- **Flexibility**: End rounds prematurely or revert to last state
- **Admin-Managed Registration**: Each singer registered by admin using standalone device
- **Queue Modes**: Mechanical karaoke and Bandokê (live instrumental)
- **Time Estimation**: Display estimated completion time based on pending singers
- **Multi-language Infrastructure**: 6 languages supported (MVP focuses on Portuguese only)

### Future Features (Post-MVP)
- **Singer Autonomy**: Self-registration capability with admin notifications
- **Facial Recognition**: Quick registration for returning singers
- **Song History**: Personalized song suggestions based on past performances
- **Social Network Integration**: Singer profiles, followers, interactions
- **Live Competitions**: Real-time voting, scoring, leaderboards
- **Cloud Synchronization**: Multi-device support with cloud backend

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

## 📐 Architecture & Patterns

### Architectural Style
**Multi-Project Clean Architecture** with Service-Oriented Design

### Solution Structure (~11-13 Projects)

```
MyVocaList.sln                                  # Visual Studio Solution
│
├── 📦 DOMAIN LAYER (Pure Entities - No Dependencies)
│   └── MyVocaList.Domain/                      
│       ├── Pessoa.cs                          # Person/Singer entity
│       ├── Estabelecimento.cs                 # Establishment/Venue entity
│       ├── Evento.cs                          # Event/Queue entity
│       ├── ParticipacaoEvento.cs             # Queue participation
│       └── ConfiguracaoSistema.cs            # System configuration
│
├── 📦 CONTRACTS LAYER (DTOs & ViewModels)
│   ├── MyVocaList.Contracts/                   
│   │   └── Models/                            # ViewModels for UI binding
│   │       └── PessoaListItemDto.cs          # Person list DTO
│   │
│   ├── MyVocaList.Contracts.DTOs/              # Data Transfer Objects
│   │   └── [Entity]Dto.cs                    
│   │
│   └── MyVocaList.Contracts.DTOs.List/        # List-specific DTOs
│       ├── PessoaListItemDto.cs              
│       ├── EstabelecimentoListItemDto.cs     
│       └── [Entity]ListItemDto.cs            
│
├── 📦 SERVICES LAYER (Business Logic)
│   ├── MyVocaList.Services/                    
│   │   ├── IDatabaseService.cs               # Interfaces
│   │   ├── ILanguageService.cs
│   │   ├── IPessoaService.cs
│   │   ├── IEstabelecimentoService.cs
│   │   ├── IQueueService.cs
│   │   ├── DatabaseService.cs                # Implementations
│   │   ├── LanguageService.cs
│   │   ├── PessoaService.cs
│   │   ├── EstabelecimentoService.cs
│   │   └── QueueService.cs
│   │
│   └── MyVocaList.Services.Mappers/           # Domain ↔ DTO Mappings
│       ├── PessoaMapper.cs
│       ├── EstabelecimentoMapper.cs
│       └── [Entity]Mapper.cs
│
├── 📦 INFRASTRUCTURE LAYER (Data Access)
│   ├── MyVocaList.Infra.Data/                 # EF Core DbContext
│   │   ├── AppDbContext.cs                   
│   │   └── AppDbContextFactory.cs            
│   │
│   ├── MyVocaList.Infra.Data.Config/          # Fluent API Configurations
│   │   ├── PessoaConfiguration.cs            
│   │   ├── EstabelecimentoConfiguration.cs   
│   │   ├── EventoConfiguration.cs            
│   │   └── [Entity]Configuration.cs          
│   │
│   ├── MyVocaList.Infra.Data.Repositories/    # Repository Pattern
│   │   ├── IPessoaRepository.cs              # Interfaces
│   │   ├── PessoaRepository.cs               # Implementations
│   │   ├── IEstabelecimentoRepository.cs
│   │   ├── EstabelecimentoRepository.cs
│   │   └── [Entity]Repository.cs
│   │
│   ├── MyVocaList.Infra.Migrations/           # EF Core Migrations
│   │   ├── [Timestamp]_InitialCreate.cs      
│   │   └── [Timestamp]_[Migration].cs        
│   │
│   └── MyVocaList.Infra.Utils/                # Infrastructure Utilities
│       ├── ITextNormalizer.cs                # Interface
│       └── TextNormalizer.cs                 # Implementation
│
└── 📦 PRESENTATION LAYER (MAUI App)
    ├── MyVocaList.View/                        # Main MAUI Project
    │   ├── App.xaml/App.xaml.cs              # Application entry
    │   ├── MauiProgram.cs                    # DI configuration
    │   ├── ServiceProvider.cs                # Service resolution helper
    │   │
    │   ├── Pages (in View root, no subfolder!)
    │   │   ├── SplashPage.xaml/.cs           # Splash screen
    │   │   ├── SplashLoadingPage.xaml/.cs    # Loading screen
    │   │   ├── TonguePage.xaml/.cs           # Language selection
    │   │   ├── StackPage.xaml/.cs            # Queue management
    │   │   ├── PersonPage.xaml/.cs           # Singer management
    │   │   ├── SpotPage.xaml/.cs             # Venue list
    │   │   ├── SpotFormPage.xaml/.cs         # Venue form
    │   │   └── EmergencyPage.xaml/.cs        # Emergency fallback
    │   │
    │   ├── Resources/
    │   │   ├── AppIcon/                      # App launcher icons
    │   │   │   ├── appicon.svg               # Vector source
    │   │   │   └── [generated PNGs]          # Platform-specific
    │   │   │
    │   │   ├── Splash/                       # Splash screen assets
    │   │   │   └── myvocalistsplashpage.jpg   # Splash image
    │   │   │
    │   │   ├── Styles/                       # Global XAML styles
    │   │   │   ├── Colors.xaml               # Color resources
    │   │   │   ├── Styles.xaml               # Base styles
    │   │   │   ├── CardStyles.xaml           # Card styles
    │   │   │   ├── NavBarStyles.xaml         # Navigation styles
    │   │   │   └── BottomNavBarStyles.xaml   # Bottom nav styles
    │   │   │
    │   │   ├── Images/                       # PNG/JPG/SVG assets
    │   │   │   ├── setaesquerda.png          # Back button
    │   │   │   ├── locais.png                # Venues icon
    │   │   │   ├── cantores.png              # Singers icon
    │   │   │   ├── novo.png                  # New queue icon
    │   │   │   ├── musicos.png               # Musicians icon
    │   │   │   ├── historico.png             # History icon
    │   │   │   └── [other icons]
    │   │   │
    │   │   ├── Fonts/                        # Custom fonts
    │   │   │   └── OpenSans-Regular.ttf
    │   │   │
    │   │   └── Strings/                      # Localization (Infrastructure ready, MVP = PT only)
    │   │       ├── AppResources.resx         # English (default/fallback)
    │   │       ├── AppResources_pt-BR.resx   # Portuguese (PRIMARY for MVP!)
    │   │       ├── AppResources_es.resx      # Spanish
    │   │       ├── AppResources_fr.resx      # French
    │   │       ├── AppResources_ja.resx      # Japanese
    │   │       └── AppResources_ko.resx      # Korean
    │   │
    │   ├── Platforms/                        # Platform-Specific Code
    │   │   └── Android/
    │   │       ├── MainActivity.cs           # Android activity
    │   │       ├── AndroidManifest.xml       # Manifest
    │   │       ├── CustomEntryHandler.cs     # Unicode input
    │   │       └── Resources/
    │   │           └── xml/
    │   │               └── locales_config.xml
    │   │
    │   └── Extensions/                       # Extension methods
    │
    ├── MyVocaList.View.Animations/            # UI Animations
    │   ├── PulseAnimation.cs
    │   ├── FadeAnimation.cs
    │   └── TranslateAnimation.cs
    │
    ├── MyVocaList.View.Behaviors/             # XAML Behaviors
    │   ├── SmartPageLifecycleBehavior.cs
    │   ├── SafeNavigationBehavior.cs
    │   └── [Behavior].cs
    │
    ├── MyVocaList.View.Converters/            # Value Converters
    │   └── [Converter].cs
    │
    ├── MyVocaList.View.Components/            # Reusable UI Components
    │   ├── HeaderComponent.xaml/.cs          # Page header
    │   ├── CardWrapperComponent.xaml/.cs     # Card wrapper
    │   ├── CrudNavBarComponent.xaml/.cs      # CRUD nav bar
    │   ├── BaseNavBarComponent.xaml/.cs      # Base nav bar
    │   ├── NavButtonComponent.xaml/.cs       # Nav button
    │   ├── SpecialNavButtonComponent.xaml/.cs # Special nav button
    │   └── InactiveQueueBottomNav.xaml/.cs   # Bottom navigation
    │
    ├── MyVocaList.View.Interceptors/          # Database Interceptors
    │   └── DatabaseLoadingInterceptor.cs     # Loading indicators
    │
    └── MyVocaList.View.Interfaces/            # View Interfaces
        ├── IManipulableDataPage.cs           # Data manipulation
        └── [Interface].cs

**NOTE**: No /Helpers folder exists. Helper logic is distributed across Extensions, Utilities, or integrated into Components/Services.
```

### Key Architectural Notes

**Project Count**: Approximately **11-13 core projects** (depends on whether you count View subprojects separately)

**Why This Structure?**
- **Separation of Concerns**: Each project has ONE responsibility
- **Testability**: Layers can be tested in isolation (future)
- **Scalability**: Easy to add features or extract to microservices
- **Maintainability**: Clear boundaries, easy to locate code
- **Team Collaboration**: Multiple developers can work on different layers

**Interfaces & Implementations**: 
- **Same folder**: Both interfaces and implementations live together in the same project folder (e.g., `MyVocaList.Services/` contains both `IQueueService.cs` and `QueueService.cs`)
- **No subfolder separation**: Unlike some architectures, we don't separate `/Interfaces` and `/Implementations` into subfolders

---

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
- **Dependencies**: Domain only
- **Purpose**: Data transfer and presentation abstractions

### **Services Layer** (`MyVocaList.Services.*`)
- **Contains**: ALL business logic and validation
- **Dependencies**: Domain, Contracts, Infrastructure.Data.Repositories
- **Purpose**: Implement use cases and business rules
- **Responsibilities**:
  - All validation and business rules
  - Coordinate multiple repositories
  - Transaction management
  - Domain ↔ DTO transformations

### **Infrastructure Layer** (`MyVocaList.Infra.*`)
- **Contains**: Data access, utilities
- **Dependencies**: Domain, Contracts
- **Purpose**: Technical concerns and database access

### **Presentation Layer** (`MyVocaList.View.*`)
- **Contains**: MAUI UI, pages, components
- **Dependencies**: Services, Contracts (NOT Domain directly!)
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
**Always Services layer**, never Domain or Repositories!

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

## 🌍 Internationalization & Localization

### Supported Languages (Infrastructure)

Based on `tongues.pdf` strategic analysis:

| Code | Language | Market Reasoning |
|------|----------|------------------|
| `en` | English | International standard |
| `pt` | Portuguese | Primary market (Brazil/Portugal) - **MVP FOCUS** |
| `es` | Spanish | Latin America expansion |
| `fr` | French | European market |
| `ja` | Japanese | Huge karaoke market |
| `ko` | Korean | K-pop explosion |

### Languages REMOVED
- `de` (German), `zh` (Chinese), `ar` (Arabic), `ru` (Russian), `hi` (Hindi)

### ⚠️ MVP Strategy: Portuguese-Only!

**CRITICAL FOR CLAUDE AI**:
- **MVP Phase**: Portuguese language ONLY
- **DO NOT** implement localization in MVP
- **DO NOT** create multi-language resources now
- **Localization = Future Phase** (after MVP validation)
- Focus on solid features in Portuguese first

### Localization Resources (Infrastructure Ready)
```
/Resources/Strings/
├── AppResources.resx           # English (fallback)
├── AppResources_pt-BR.resx     # Portuguese ⭐ PRIMARY
├── AppResources_es.resx        # Spanish (future)
├── AppResources_fr.resx        # French (future)
├── AppResources_ja.resx        # Japanese (future)
└── AppResources_ko.resx        # Korean (future)
```

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

## 🎨 UI/UX Design System

### ⚠️ Material Design Migration (Planned - REQUIRES HELDER COORDINATION!)

**Status**: Custom gradient-based design
**Future**: Material Design 3 (coordinated migration)
**Guide**: See `myvocalist_migration_material_design_guide.md`

### Current Design System

```xml
<!-- Background -->
<LinearGradientBrush x:Key="AppBackgroundGradient">
    <GradientStop Color="#221b3c" Offset="0"/>
    <GradientStop Color="#331e6e" Offset="1"/>
</LinearGradientBrush>

<!-- Colors -->
<Color x:Key="PrimaryColor">#e91e63</Color>
<Color x:Key="SecondaryColor">#904ab4</Color>
```

### Global Styles
- `BaseCardStyle`: Semi-transparent cards
- `GradientButtonStyle`: Pink→Purple gradient
- `FormButtonStyle`: Form-specific buttons
- `CompactBackButtonStyle`: 32x32px back button
- `InputEntryStyle`: Unicode-ready input fields

---

## 📋 Code Conventions

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

---

## 🎯 Critical Guidelines for Claude AI

### When to ASK Helder First
1. Architectural decisions
2. Material Design changes
3. Scalability approaches
4. Database schema changes
5. New external integrations

### When Autonomous Development is OK
1. Bug fixes
2. UI tweaks with existing styles
3. Business logic in existing services
4. CRUD operations
5. Validation rules

### Principle: **"When in Doubt, ASK Helder"**

---

## ❌ NEVER Do These

1. ❌ Use `localStorage`/`sessionStorage` (not supported)
2. ❌ Implement Material Design without Helder
3. ❌ Create localization in MVP (Portuguese only!)
4. ❌ Use emojis in XAML (use PNG images)
5. ❌ Add constructor parameters to Pages
6. ❌ Skip error handling
7. ❌ Make Singletons if they have state
8. ❌ Premature optimizations
9. ❌ Create Interface/Implementation subfolders (keep in same folder!)
10. ❌ Create /Helpers folder (use Extensions or Components instead!)

---

## 🌟 Future Growth Vision

### Phase Evolution
1. **MVP** (Current): Portuguese, core features, local database
2. **MVP Validation**: User feedback, optimization
3. **Localization**: 6 languages implementation
4. **Cloud**: API, authentication, synchronization
5. **Social Network**: Profiles, interactions
6. **Competitions**: Voting, scoring, leaderboards
7. **Global Scale**: Microservices, multi-region

### Scalability Preparation
- Structure allows future extraction to microservices
- Clean Architecture enables API creation
- Repository pattern allows database migration
- Service layer ready for CQRS if needed

**Philosophy**: "Build simplest thing NOW, but structure so it CAN scale LATER"

---

## 📚 Reference Documentation

### Project Guides
- `changelog.md` - Development history
- `myvocalist_migration_clean_architecture_guide.md` - Architecture guide
- `myvocalist_migration_material_design_guide.md` - Material Design plan
- `research_dotnet_ai_dev_guide_20251010_claudeSonnet4_5.md` - Advanced patterns
- `tongues.pdf` - Language strategy

---

## 💡 Quick Reference

### Creating New Page
1. Add `MyPage.xaml/.cs` to View root (no Pages/ subfolder!)
2. Use ServiceProvider pattern
3. Register in MauiProgram.cs as Transient
4. Navigate: `await Navigation.PushAsync(new MyPage());`

### Creating New Service
1. Add `IMyService.cs` and `MyService.cs` in same folder
2. Implement business logic
3. Register in MauiProgram.cs as Scoped
4. Use via ServiceProvider in pages

---

## 📊 Project Status

### Implementation
- ✅ Database: 100% (EF Core 9.0.6)
- ✅ Services: 80%
- ✅ UI Components: 70%
- ✅ Navigation: 100%
- 🚧 Material Design: 0% (planned)
- 🚧 Localization: 30% (MVP = PT only)
- 🚧 Testing: 0% (post-MVP)

### Technical Achievements
- ✅ Clean Architecture (~11-13 projects)
- ✅ ServiceProvider pattern
- ✅ Anti-crash pthread_mutex system
- ✅ Multilingual text search (6 languages)
- ✅ Accent-insensitive search
- ✅ Hardware-aware animations

---

## 🎓 Success Checklist for Claude AI

Before completing any task:
- [ ] Read current file implementations
- [ ] Follow ServiceProvider pattern
- [ ] Use existing styles
- [ ] Add error handling
- [ ] Include debug logging
- [ ] Maintain null safety
- [ ] Use async/await for I/O
- [ ] Check service scopes
- [ ] Verify parameterless constructors
- [ ] Remember MVP = Portuguese only
- [ ] ASK Helder for architectural decisions

---

## 📞 Final Notes

### Your Role (Claude AI)
Code Implementation Specialist under Helder's architectural guidance

### Success Formula
1. Always read before writing
2. Follow patterns religiously
3. Ask when uncertain
4. Test understanding
5. Document successes

### Remember
- MVP = Portuguese only (NO localization!)
- Material Design = Helder coordination required
- ServiceProvider pattern = Non-negotiable
- Styles = Reuse from ResourceDictionary
- When in doubt = ASK Helder

### Principle
> "Build correctly first time, following established patterns, deliver working code that integrates seamlessly."

---

**Last Updated**: October 14, 2025  
**Version**: 2.1 (Corrected)  
**Maintained by**: Helder (Architect) + Claude AI (Developer)