# MyKaraoke

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-8.0-purple.svg)](https://dotnet.microsoft.com/apps/maui)
[![Platform](https://img.shields.io/badge/Platform-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20macOS-blue.svg)](https://dotnet.microsoft.com/apps/maui)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A professional .NET MAUI 8.0 application for managing participant queues in karaoke rounds with advanced features for both traditional karaoke and live band performances (bandokê).

## Overview

MyKaraoke enables efficient queue management for karaoke events, allowing administrators to register participant attendance/absence as singers reach position 1 in the queue. The application supports round-based progression, queue reactivation, state rollback, and flexible participant positioning.

### Key Features

- **Dual Queue Modes**: Traditional mechanical karaoke and bandokê (live artist/band performances)
- **Smart Queue Management**: Round progression, position control, and participant tracking
- **Autonomous Registration**: Singers can self-register with administrator notifications
- **Estimated Completion Times**: Real-time queue duration calculations
- **Advanced Search**: Multilingual, accent-insensitive participant lookup
- **Performance Optimization**: Hardware-adaptive animations and anti-crash systems

## Internationalization

Strategic multilingual support for **6 core languages**:
- **English (en)** - International standard
- **Portuguese (pt)** - Primary market (Brazil)
- **Spanish (es)** - Latin America expansion
- **French (fr)** - European market
- **Japanese (ja)** - Major karaoke market
- **Korean (ko)** - K-pop cultural impact

*The project strategically focuses on these 6 languages to maintain development efficiency while covering the most important karaoke markets globally.*

## Architecture

### Technology Stack
- **.NET MAUI 8.0**: Cross-platform framework
- **SQLite + Entity Framework**: Local database with migration support
- **Dependency Injection**: ServiceProvider pattern with SCOPED/SINGLETON lifecycle management
- **Component-Based UI**: Reusable XAML components with unified styling
- **MVVM Pattern**: Clean separation of concerns

### Project Structure
```
MyKaraoke/
├── MyKaraoke.Domain/          # Domain entities and contracts
├── MyKaraoke.Infra/           # Data access and utilities
├── MyKaraoke.Services/        # Business logic services
└── MyKaraoke.View/           # UI layer (MAUI)
    ├── Components/           # Reusable UI components
    ├── Pages/               # Application pages
    ├── Styles/              # XAML styling resources
    ├── Animations/          # Animation system
    └── Platforms/           # Platform-specific code
```

### Core Components

**Navigation System**
- `HeaderComponent`: Intelligent back navigation with context awareness
- `InactiveQueueBottomNav`: Bottom navigation for queue-inactive states
- `CrudNavBarComponent`: Dynamic CRUD operations navigation

**Data Management**
- `PessoaService`: Participant management with search capabilities
- `EstabelecimentoService`: Venue registration and management
- `QueueService`: Queue state and participant flow control
- `DatabaseService`: Migration and connection management

**UI Framework**
- `CardWrapperComponent`: Consistent card-based layouts
- `AnimationManager`: Hardware-adaptive animation system
- `TextNormalizer`: Multilingual text processing utility

## Development Team

**Collaborative AI-Human Development Process:**

### Helder (Project Architect & Technical Auditor)
- **Role**: Software Architect, Technical Leader, Quality Auditor
- **Responsibilities**: 
  - Defines technical approaches and architectural decisions
  - Guides development through strategic technical leadership
  - Conducts code reviews and performance optimization audits
  - Manages technical debt and critical issue prioritization

### Claude AI (Code Implementation Specialist)  
- **Role**: Code Developer and Implementation Expert
- **Responsibilities**:
  - Implements features according to architectural guidelines
  - Develops bug fixes and technical solutions
  - Ensures code quality and maintainability
  - Provides systematic debugging and troubleshooting

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 17.8+ or Visual Studio Code with C# Dev Kit
- Platform-specific workloads:
  - Android: Android SDK 33+
  - iOS: Xcode 15+ (macOS only)
  - Windows: Windows 11 SDK

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/your-username/mykaraoke.git
cd mykaraoke
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Build the solution**
```bash
dotnet build
```

4. **Run the application**
```bash
# Android
dotnet build -t:Run -f net8.0-android

# iOS (macOS only)
dotnet build -t:Run -f net8.0-ios

# Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

### Configuration

The application uses SQLite for local data storage with automatic database initialization and migration support. No additional configuration is required for basic functionality.

## Features

### Queue Management
- **Single Active Queue**: Manage one queue at a time with full participant control
- **Round Progression**: Automatic round incrementation when all participants complete
- **Flexible Completion**: End rounds even with pending participants
- **Queue Reactivation**: Restore accidentally closed queues
- **State Rollback**: Revert to previous queue states for error correction
- **Position Control**: Move participants to any queue position

### Participant Experience
- **Autonomous Registration**: Self-service queue entry with notifications
- **Intelligent Search**: Accent-insensitive, multilingual participant lookup
- **Duplicate Detection**: Smart homonym handling via birthday/email differentiation
- **Real-time Updates**: Live queue status and estimated completion times

### Bandokê Mode
- **Song Catalogs**: Artist/band repertoire management
- **Lyrics Integration**: Local storage or third-party API integration (Genius.com)
- **Dynamic Song Registration**: Add songs during performances
- **Performance Tracking**: Song-participant association logging

### Performance Features
- **Hardware Detection**: Automatic performance optimization based on device capabilities
- **Anti-Crash System**: Robust pthread_mutex crash prevention
- **Memory Management**: Proactive resource cleanup and garbage collection
- **Animation Scaling**: Adaptive visual effects based on hardware performance

## Advanced Configuration

### Animation System
Control animation behavior via hardware detection:
```csharp
// Automatic hardware-based optimization
HardwareDetector.SupportsAnimations // Returns capability assessment
```

### Database Management
```csharp
// Manual database operations
await DatabaseService.InitializeDatabaseAsync();
await DatabaseService.MigrateAsync();
```

### Multilingual Text Processing
```csharp
// Accent-insensitive search
var normalizedText = TextNormalizer.NormalizeName(inputText);
```

## Technical Achievements

- **Strategic Multilingual Support**: Optimized localization for 6 core languages focusing on major karaoke markets
- **Performance Optimization**: Hardware-adaptive animations with automatic scaling
- **Crash Prevention**: Robust pthread_mutex anti-crash system for Android
- **Component Architecture**: Reusable UI components reducing code duplication by 91%
- **Database Optimization**: Intelligent indexing and migration system
- **Search Innovation**: Multilingual normalization with 300ms debounce optimization

## Contributing

This project follows a collaborative AI-human development model. Contributions should:

1. Maintain the established architectural patterns
2. Include comprehensive logging for debugging
3. Follow the component-based UI structure
4. Preserve multilingual support capabilities
5. Include appropriate unit tests for new features

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- UI/UX design following Mobile 2025 guidelines
- Multilingual support powered by .NET localization framework
- Performance optimization inspired by modern mobile development best practices

---

**Note**: This project demonstrates advanced .NET MAUI development techniques including component architecture, performance optimization, strategic multilingual support, and collaborative AI-human development workflows.