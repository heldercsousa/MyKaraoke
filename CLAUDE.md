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

### ⚠️ Material Design 3 Migration Guidelines (REQUIRES HELDER COORDINATION!)

## 🎨 UI/UX - Material Design 3 System

**Status**: In migration (v1.1)  
**Approach**: Full MD3 adoption maintaining brand identity  
**Colors**: Pink #E91E63 (Primary) + Purple gradients (Brand)  

---

### Color System

**CRITICAL: Never use hardcoded colors!**

**Color Roles (MD3 Standard):**

```xml
<!-- Primary (Your brand pink) -->
<Color x:Key="Primary">#E91E63</Color>
<Color x:Key="OnPrimary">#FFFFFF</Color>

<!-- Secondary (Your brand purple) -->
<Color x:Key="Secondary">#8B4CB8</Color>
<Color x:Key="OnSecondary">#FFFFFF</Color>

<!-- Tertiary (Your brand gold) -->
<Color x:Key="Tertiary">#FFD700</Color>
<Color x:Key="OnTertiary">#1A1024</Color>

<!-- Surface & Background -->
<Color x:Key="Background">#1A1024</Color>
<Color x:Key="OnBackground">#FFFFFF</Color>
<Color x:Key="Surface">#2D1B69</Color>
<Color x:Key="OnSurface">#FFFFFF</Color>
```

**Usage:**

```xml
<!-- ✅ CORRECT -->
<Button BackgroundColor="{StaticResource Primary}" 
        TextColor="{StaticResource OnPrimary}" />

<!-- ❌ WRONG -->
<Button BackgroundColor="#E91E63" 
        TextColor="White" />
```

**Why:** Enables theme switching, maintains accessibility, ensures consistency.

---

### Typography Hierarchy

**Always use predefined styles, never set FontSize directly!**

```xml
<!-- Page titles -->
<Label Text="Queue Management" Style="{StaticResource HeadlineLarge}" />

<!-- Section titles -->
<Label Text="Active Singers" Style="{StaticResource TitleLarge}" />

<!-- Card titles / List item titles -->
<Label Text="Singer Name" Style="{StaticResource TitleMedium}" />

<!-- Main body text -->
<Label Text="Description here..." Style="{StaticResource BodyLarge}" />

<!-- Secondary text / metadata -->
<Label Text="Last updated 2 hours ago" Style="{StaticResource BodySmall}" />

<!-- Field labels -->
<Label Text="Enter name:" Style="{StaticResource FieldLabel}" />

<!-- Captions / small text -->
<Label Text="Optional" Style="{StaticResource LabelSmall}" />
```

**Scale Reference:**
- **Display**: 36-57pt (Hero sections - rare)
- **Headline**: 24-32pt (Page titles)
- **Title**: 14-22pt (Section/card titles)
- **Body**: 12-16pt (Main content)
- **Label**: 11-14pt (Captions, metadata)

---

### Button Patterns

**Use correct button type for visual hierarchy!**

**Rule: Maximum 1 Filled Button per screen**

```xml
<!-- Primary action (most important) -->
<Button Text="Add to Queue" 
        Style="{StaticResource FilledButton}"
        Command="{Binding AddCommand}" />

<!-- Secondary action -->
<Button Text="Cancel" 
        Style="{StaticResource OutlinedButton}"
        Command="{Binding CancelCommand}" />

<!-- Tertiary action (low emphasis) -->
<Button Text="Learn More" 
        Style="{StaticResource TextButton}"
        Command="{Binding LearnMoreCommand}" />
```

**Button Decision Tree:**

1. **Is this the most important action on screen?** → FilledButton
2. **Is this a cancel/secondary action?** → OutlinedButton  
3. **Is this optional/low priority?** → TextButton

**Examples:**

- Save form: FilledButton
- Cancel: OutlinedButton or TextButton
- Edit/Delete in list: TextButton
- Navigation: TextButton
- Learn more/Help: TextButton

---

### Card Usage

**CRITICAL: Cards are for LIST ITEMS, not page wrappers!**

**✅ CORRECT - Card for list item:**

```xml
<CollectionView ItemsSource="{Binding Singers}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Frame Style="{StaticResource ElevatedCard}">
                <Grid>
                    <Label Text="{Binding Name}" Style="{StaticResource TitleMedium}" />
                    <Label Text="{Binding Position}" Style="{StaticResource BodySmall}" />
                </Grid>
            </Frame>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

**❌ WRONG - Card wrapping entire page:**

```xml
<!-- DON'T DO THIS! Wastes screen space -->
<ContentPage>
    <Frame Style="{StaticResource SomeCardStyle}">
        <VerticalStackLayout>
            <!-- Page content -->
        </VerticalStackLayout>
    </Frame>
</ContentPage>
```

**✅ CORRECT - Direct content on page:**

```xml
<ContentPage BackgroundColor="{StaticResource Background}">
    <ScrollView>
        <VerticalStackLayout Style="{StaticResource PageContainer}">
            <!-- Page content directly, no card wrapper -->
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

**Card Styles:**

- `ElevatedCard` - With shadow, for list items
- `FilledCard` - With tint, for grouped content
- `OutlinedCard` - With border, for subtle separation
- `GradientCard` - Your custom branded card (special cases)

---

### Spacing System (8px Grid)

**CRITICAL: All spacing must be multiples of 4px (prefer 8px)**

```xml
<!-- ✅ CORRECT -->
<VerticalStackLayout Padding="16" Spacing="16">

<!-- ❌ WRONG -->
<VerticalStackLayout Padding="15" Spacing="20">
```

**Standard Values:**

- **4dp**: Micro spacing (very close elements)
- **8dp**: Related elements (form fields, list items)
- **16dp**: Page padding, section spacing
- **24dp**: Major section breaks
- **32dp**: Large block separations

**Common Patterns:**

```xml
<!-- Page layout -->
<VerticalStackLayout Padding="16" Spacing="16">

<!-- Form fields -->
<VerticalStackLayout Spacing="8">
    <Label Text="Name" Style="{StaticResource FieldLabel}" />
    <Entry Style="{StaticResource MaterialEntry}" />
</VerticalStackLayout>

<!-- Card list -->
<CollectionView>
    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Frame Style="{StaticResource ElevatedCard}" Margin="0,4">
                <!-- 8dp total spacing between items (4dp each side) -->
            </Frame>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

---

### Input Fields

**Always use MaterialEntry with proper labeling:**

```xml
<!-- ✅ CORRECT Pattern -->
<VerticalStackLayout Spacing="8">
    <Label Text="Singer Name" Style="{StaticResource FieldLabel}" />
    <Entry Placeholder="Enter name" 
           Text="{Binding SingerName}"
           Style="{StaticResource MaterialEntry}" />
    <Label Text="Required field" Style="{StaticResource HelperText}" />
</VerticalStackLayout>

<!-- With error state -->
<VerticalStackLayout Spacing="8">
    <Label Text="Email" Style="{StaticResource FieldLabel}" />
    <Entry Placeholder="email@example.com" 
           Text="{Binding Email}"
           Style="{StaticResource MaterialEntry}" />
    <Label Text="Invalid email format" 
           Style="{StaticResource ErrorText}"
           IsVisible="{Binding HasEmailError}" />
</VerticalStackLayout>
```

**Input Field Structure:**
1. Field label (12pt, bold, OnSurfaceVariant color)
2. Input field (MaterialEntry style, 56dp height)
3. Helper text or error message (12pt, below field)

---

### Accessibility

**Contrast Requirements (WCAG 2.1):**

All color combinations are pre-validated:

- White (#FFFFFF) on Background (#1A1024): **12.7:1** ✅
- OnSurfaceVariant (#D1D5DB) on Background: **6.2:1** ✅  
- Primary (#E91E63) on Background: **5.4:1** ✅

**Always use On[Color] for text:**

- Text on Primary → OnPrimary
- Text on Surface → OnSurface
- Text on Background → OnBackground

**Touch Targets:**

- Minimum: 48x48dp (Android standard)
- Buttons: 40dp height minimum
- List items: 56dp height minimum
- FAB: 56x56dp

---

### Elevation System

**Use shadows for hierarchy, not borders:**

```xml
<!-- Elevated card (2dp elevation) -->
<Frame Style="{StaticResource ElevatedCard}">
    <Shadow Brush="{StaticResource Shadow}" Offset="0,1" Radius="3" Opacity="0.2" />
</Frame>

<!-- Floating Action Button (6dp elevation) -->
<Frame Style="{StaticResource FabContainer}">
    <Shadow Brush="#FFD700" Offset="0,6" Radius="20" Opacity="0.4" />
</Frame>
```

**Elevation Levels:**

- **0dp**: Flat elements (text, icons)
- **1-2dp**: Cards in lists
- **4-8dp**: Elevated buttons, floating elements
- **16dp+**: Modals, dialogs

---

### Page Structure Pattern

**Standard page structure:**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MyVocaList.View.MyPage"
             BackgroundColor="{StaticResource Background}"
             Shell.NavBarIsVisible="False">

    <ScrollView>
        <VerticalStackLayout Style="{StaticResource PageContainer}">
            
            <!-- Page Title -->
            <Label Text="Page Title" Style="{StaticResource HeadlineLarge}" />
            
            <!-- Content sections with 16dp spacing -->
            <VerticalStackLayout Spacing="16">
                
                <!-- Section 1 -->
                <VerticalStackLayout Spacing="8">
                    <Label Text="Section Title" Style="{StaticResource TitleMedium}" />
                    <!-- Section content -->
                </VerticalStackLayout>
                
                <!-- Section 2 -->
                <VerticalStackLayout Spacing="8">
                    <!-- More content -->
                </VerticalStackLayout>
                
            </VerticalStackLayout>
            
            <!-- Actions at bottom -->
            <HorizontalStackLayout Spacing="8" HorizontalOptions="End" Margin="0,24,0,0">
                <Button Text="Cancel" Style="{StaticResource TextButton}" />
                <Button Text="Save" Style="{StaticResource FilledButton}" />
            </HorizontalStackLayout>
            
        </VerticalStackLayout>
    </ScrollView>
    
</ContentPage>
```

---

## 🚨 Common Mistakes to Avoid

### ❌ **Don't:**

1. Use hardcoded colors: `BackgroundColor="#E91E63"`
2. Set FontSize directly: `FontSize="16"`
3. Use random spacing: `Padding="15,18,12,20"`
4. Wrap pages in cards: `<Frame><VerticalStackLayout>...</Frame>`
5. Mix button styles randomly (no visual hierarchy)
6. Ignore 8px spacing grid
7. Use more than 1 FilledButton per screen

### ✅ **Do:**

1. Use color resources: `BackgroundColor="{StaticResource Primary}"`
2. Use typography styles: `Style="{StaticResource BodyLarge}"`
3. Use 8px grid: `Padding="16" Spacing="16"`
4. Put content directly on page background
5. Follow button hierarchy (Filled > Outlined > Text)
6. Apply spacing consistently
7. Reserve FilledButton for primary action only

---

## 🎯 Quick Reference for Claude AI

**When generating new pages:**

1. Start with page structure template (see above)
2. Use `PageContainer` for outer layout
3. Apply typography styles (never FontSize)
4. Use color resources (never hex codes)
5. Follow 8px spacing grid
6. Button hierarchy: 1 Filled max, then Outlined/Text
7. Cards for list items only, NOT page wrappers
8. Include ScrollView for long content

**Testing checklist:**

- [ ] No hardcoded colors
- [ ] No hardcoded FontSize
- [ ] Spacing is multiples of 4/8
- [ ] Max 1 FilledButton per screen
- [ ] No card wrapping entire page
- [ ] Contrast ratios meet WCAG 2.1
- [ ] Touch targets ≥ 48x48dp

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
# CLAUDE.md Section - Changelog & Git Workflow

## 📝 Documentation & Version Control Workflow

**Status**: Mandatory for ALL development  
**Applies to**: Every enhancement, fix, refactoring, or migration  
**Philosophy**: "Document as you build, commit as you succeed"

---

## 🔄 Standard Development Cycle

Every implementation follows this **3-step cycle**:

```
1. IMPLEMENT → 2. VERIFY → 3. DOCUMENT & COMMIT
    ↓              ↓              ↓
  Code change   Test/confirm   changelog.md + git commit
```

**NEVER skip step 3!** Even "small" changes get documented and committed.

---

## 📋 When to Update changelog.md

### **ALWAYS Document These:**

✅ **Enhancements** - New features, UI improvements, refactorings  
✅ **Fixes** - Bug fixes, corrections, problem resolutions  
✅ **Migrations** - Architecture changes, library updates, pattern adoptions  
✅ **Optimizations** - Performance improvements, code cleanup  

### **DON'T Document These:**

❌ Work in progress (not yet functional)  
❌ Experimental code (not confirmed working)  
❌ Commits to feature branches before merging  
❌ Typo fixes in comments/docs (too granular)  

**Rule of Thumb:** If it changes user-visible behavior OR code structure, document it!

---

## ✍️ Changelog Entry Format

**Standard Format:**
```
- **MM/DD/YYYY** - [Type] - Succinct description in English
```

### **Entry Types:**

- **Enhancement** - New features, improvements, additions
- **Fix** - Bug fixes, corrections, problem resolutions
- **Refactor** - Code restructuring without behavior change
- **Migration** - Library/framework/pattern changes
- **Optimization** - Performance improvements

### **Description Guidelines:**

✅ **Do:**
- Be succinct but complete (1-3 sentences)
- Mention WHAT changed and WHY
- Include specific file/component names
- Explain user-visible impact if applicable
- Use technical terminology appropriately

❌ **Don't:**
- Write vague descriptions ("improved things")
- Skip the reasoning ("updated PersonPage" - why?)
- Use first-person ("I added..." - use "Added...")
- Include code snippets (save for commit messages)

### **Examples:**

**✅ Good Entries:**

```markdown
- **10/14/2025** - Migration - Migrated PersonPage to Material Design 3: removed Frame card container wrapper (gained 60px vertical space), applied MaterialEntry style to input field, used FilledButton for primary action. Page now follows MD3 spacing grid (16dp padding, 16dp spacing) and typography scale (FieldLabel, TitleMedium).

- **10/14/2025** - Enhancement - Implemented theme switching service: created IThemeService interface with Dark/Light/Auto modes, added ThemeService implementation with SQLite persistence, integrated with App.xaml resource dictionary switching. Users can now change themes via Settings page.

- **10/15/2025** - Fix - Fixed CollectionView item selection not triggering Command: added TapGestureRecognizer to Frame wrapper (Commands don't work on Frame directly in MAUI), moved Command binding from Frame to GestureRecognizer. Queue item selection now works correctly on Android.

- **10/15/2025** - Refactor - Extracted card selection logic from StackPage into reusable SelectableCardBehavior: created behavior in MyVocaList.View.Behaviors with IsSelected bindable property and visual state management. Reduced code duplication across 3 pages (StackPage, SpotPage, PersonPage).

- **10/16/2025** - Optimization - Improved queue list scrolling performance: enabled CollectionView virtualization with CachingStrategy="RecycleElement", reduced item template complexity by removing nested Frames. List now scrolls smoothly with 200+ items.
```

**❌ Bad Entries:**

```markdown
- **10/14/2025** - Enhancement - Updated PersonPage
  [Too vague - what changed? why?]

- **10/14/2025** - Fix - Fixed bug
  [What bug? where? how?]

- **10/14/2025** - Enhancement - I added Material Design to the app
  [Too broad, first-person, lacks specifics]

- **10/15/2025** - Enhancement - Changed colors and stuff
  [Unprofessional, vague, no detail]
```

---

## 🔧 Git Commit Workflow

### **Commit Frequency:**

**Small, Focused Commits > Large, Monolithic Commits**

**Commit after:**
- ✅ Each page migration (during MD3 migration)
- ✅ Each feature implementation
- ✅ Each bug fix
- ✅ Each refactoring that passes tests
- ✅ Any working state you might want to rollback to

**Don't commit:**
- ❌ Broken/non-compiling code
- ❌ Half-finished features (unless using feature flags)
- ❌ Commented-out code blocks (clean them up first)
- ❌ Debug logging you forgot to remove

### **Commit Message Format:**

**Standard Format:**
```
[Type] Brief description (50 chars max)

Detailed explanation if needed:
- What changed
- Why it changed
- Any breaking changes
- Related issue/task numbers
```

**Types:**
- `feat:` - New feature
- `fix:` - Bug fix
- `refactor:` - Code restructuring
- `style:` - UI/UX changes
- `docs:` - Documentation only
- `test:` - Test additions/changes
- `chore:` - Maintenance tasks

**Examples:**

```bash
# Simple commits (no body needed)
git commit -m "feat: Add MaterialColors.xaml with brand color system"
git commit -m "refactor: Migrate PersonPage to Material Design 3"
git commit -m "fix: Resolve Button Command not firing on Android"

# Complex commits (with body)
git commit -m "refactor: Migrate all pages to Material Design 3

- Removed card container wrappers from all 5 pages
- Applied MD3 typography scale and spacing grid
- Updated button hierarchy (Filled/Outlined/Text)
- Maintained all existing functionality and bindings
- Gained 10-15% more screen space across app

Pages migrated:
- PersonPage
- SpotPage
- SpotFormPage
- TonguePage
- StackPage

Breaking changes: None (visual only)
Testing: Verified on Android API 33"
```

---

## 🔄 Example Workflow: Page Migration

**Scenario:** Migrating PersonPage to Material Design 3

### **Step 1: Implement**

```bash
# Start Claude Code
claude-code

# Give prompt for PersonPage refactoring
# Claude Code shows changes
# Review changes
# Apply changes
```

### **Step 2: Verify**

```bash
# Build
dotnet build -f net8.0-android

# Run
dotnet run -f net8.0-android

# Manual testing checklist:
# ✅ Page loads without errors
# ✅ Layout looks correct (no card wrapper, proper spacing)
# ✅ Input field accepts text
# ✅ "Add to Queue" button responds to tap
# ✅ Command fires correctly
# ✅ Validation works as before
# ✅ Navigation works (back button)
```

### **Step 3: Document & Commit**

**3a. Ask Helder (If Working Together):**

```
Claude: "Helder, PersonPage migration is complete. I've:
- Removed Frame card container wrapper
- Applied MaterialEntry and FilledButton styles  
- Used 16dp padding and spacing
- Tested on Android - all functionality working

Can you confirm the implementation was successful?"

Helder: "Yes, looks good!"
```

**3b. Update changelog.md:**

```markdown
- **10/14/2025** - Migration - Migrated PersonPage to Material Design 3: removed Frame card container wrapper (gained 60px vertical space), applied MaterialEntry style to input field, used FilledButton for primary action, implemented 16dp padding and spacing following MD3 grid system. All functionality preserved, improved visual hierarchy with FieldLabel typography.
```

**3c. Git Commit:**

```bash
git add MyVocaList.View/PersonPage.xaml
git add changelog.md
git commit -m "refactor: Migrate PersonPage to Material Design 3

- Removed Frame card container (gained 60px vertical space)
- Applied MaterialEntry style to singer name input
- Applied FilledButton style to 'Add to Queue' button
- Implemented 16dp padding and spacing (MD3 grid)
- Applied FieldLabel typography for field label

Testing: Verified on Android API 33
All functionality preserved"
```

**3d. Push (If Ready):**

```bash
# Push to feature branch
git push origin feature/material-design-3
```

---

## 📊 Multi-Step Task Workflow

**For larger tasks (like full MD3 migration), commit incrementally:**

### **Example: MD3 Migration (5 pages)**

```bash
# Initial setup
git add Resources/Styles/MaterialColors.xaml
git add Resources/Styles/MaterialStyles.xaml
git add MyVocaList.View/App.xaml
git add changelog.md
git commit -m "feat: Add Material Design 3 style system"

# Page 1
# [implement → verify → document]
git add MyVocaList.View/PersonPage.xaml
git add changelog.md
git commit -m "refactor: Migrate PersonPage to MD3"

# Page 2
# [implement → verify → document]
git add MyVocaList.View/SpotFormPage.xaml
git add changelog.md
git commit -m "refactor: Migrate SpotFormPage to MD3"

# Page 3
# [implement → verify → document]
git add MyVocaList.View/SpotPage.xaml
git add changelog.md
git commit -m "refactor: Migrate SpotPage to MD3"

# Page 4
# [implement → verify → document]
git add MyVocaList.View/TonguePage.xaml
git add changelog.md
git commit -m "refactor: Rebuild TonguePage with MD3"

# Page 5
# [implement → verify → document]
git add MyVocaList.View/StackPage.xaml
git add changelog.md
git commit -m "refactor: Migrate StackPage to MD3"

# Final cleanup
git add Resources/Styles/CardStyles.xaml
git add CLAUDE.md
git add changelog.md
git commit -m "docs: Update CLAUDE.md with MD3 guidelines"

# Merge to main
git checkout main
git merge feature/material-design-3
git push origin main
```

**Result:** 7 clear commits, each representing a working state. Easy to:
- Review what changed when
- Rollback specific changes if needed
- Understand project evolution
- Generate release notes

---

## 🎯 Best Practices

### **DO:**

✅ **Commit early, commit often** - Small commits are easier to review and rollback  
✅ **Write descriptive commit messages** - "Fix button" → "fix: Resolve Command binding on PersonPage button"  
✅ **Test before committing** - Never commit broken code  
✅ **Update changelog.md in same commit** - Keeps history synchronized  
✅ **Use feature branches** - Isolate work, easy to abandon if needed  
✅ **Ask for verification** - When working with Helder, confirm success before documenting  
✅ **Group related changes** - Update .xaml + .cs in same commit if they're coupled  

### **DON'T:**

❌ **Commit without testing** - "git commit -m 'hopefully this works'"  
❌ **Batch unrelated changes** - Don't mix PersonPage migration + bug fix in one commit  
❌ **Skip changelog updates** - Future you will forget what you did  
❌ **Use vague messages** - "fix stuff", "update", "changes"  
❌ **Commit generated files** - bin/, obj/, .vs/ should be in .gitignore  
❌ **Force push to main** - Only force push to feature branches if absolutely necessary  
❌ **Forget to pull before pushing** - Always `git pull` first to avoid conflicts  

---

## 🚨 Troubleshooting

### **"I forgot to update changelog.md before committing"**

```bash
# Edit changelog.md
# Amend last commit
git add changelog.md
git commit --amend --no-edit

# If already pushed (and you're on feature branch):
git push --force-with-lease origin feature/material-design-3
```

### **"I committed broken code by mistake"**

```bash
# Undo last commit, keep changes
git reset --soft HEAD~1

# Fix the code
# Test again
# Commit properly
git add .
git commit -m "fix: Correct implementation of X"
```

### **"I need to split a large commit into smaller ones"**

```bash
# Undo last commit, keep changes
git reset HEAD~1

# Stage files individually
git add MyVocaList.View/PersonPage.xaml
git add changelog.md
git commit -m "refactor: Migrate PersonPage to MD3"

git add MyVocaList.View/SpotPage.xaml
git add changelog.md
git commit -m "refactor: Migrate SpotPage to MD3"
```

---

## 📚 Quick Reference

### **After Every Successful Implementation:**

1. ✅ Test thoroughly
2. ✅ Update changelog.md (succinct entry with date/type/description)
3. ✅ Git add relevant files
4. ✅ Git commit with clear message
5. ✅ Push to feature branch (if ready)

### **Changelog Entry Template:**

```markdown
- **MM/DD/YYYY** - [Type] - [What changed]: [details including file/component names, what was done, why it matters, impact]. [Technical specifics if relevant].
```

### **Git Commit Template:**

```bash
git commit -m "[type]: Brief description (50 chars)

Detailed explanation:
- What changed
- Why it changed
- Testing performed
- Any breaking changes"
```

---

**Remember:** Documentation and version control are not "extra work" - they're **essential parts of professional development**. Your future self (and Helder!) will thank you! 🚀

---

**Last Updated**: October 14, 2025  
**Applies to**: All development (MVP, post-MVP, features, fixes, refactoring)

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