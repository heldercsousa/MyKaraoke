# Claude Code Prompt 2.1: Create Unit Test Project for MyVocaList Solution

## Context

MyVocaList needs a comprehensive unit test project to verify MD3 color compliance and other functionality. The test project should be structured to:
1. Test MyVocaList.View initially
2. Be easily extensible to cover other projects in the solution (Services, Infrastructure, Domain, etc.)
3. Use modern .NET testing practices
4. Support testing color system compliance

## Task

Create a new xUnit test project in the MyVocaList solution with proper structure and dependencies.

## Solution Structure Context

Current MyVocaList solution has approximately these projects:
- MyVocaList.Domain
- MyVocaList.Contracts
- MyVocaList.Services
- MyVocaList.Infrastructure
- MyVocaList.View (Presentation layer - .NET MAUI)
- Others...

The test project should be able to reference and test any of these.

## Implementation Steps

### Step 1: Create Test Project

```bash
# Navigate to solution root
cd /path/to/MyVocaList

# Create xUnit test project
dotnet new xunit -n MyVocaList.Tests -f net8.0

# Add to solution
dotnet sln add MyVocaList.Tests/MyVocaList.Tests.csproj
```

### Step 2: Add Project References

```bash
cd MyVocaList.Tests

# Add reference to View project (primary focus initially)
dotnet add reference ../MyVocaList.View/MyVocaList.View.csproj

# Add references to other projects (for future test coverage)
dotnet add reference ../MyVocaList.Services/MyVocaList.Services.csproj
dotnet add reference ../MyVocaList.Infrastructure/MyVocaList.Infrastructure.csproj
dotnet add reference ../MyVocaList.Domain/MyVocaList.Domain.csproj
dotnet add reference ../MyVocaList.Contracts/MyVocaList.Contracts.csproj
```

### Step 3: Add Required NuGet Packages

```bash
# Testing framework (already included from template)
# - xunit
# - xunit.runner.visualstudio
# - Microsoft.NET.Test.Sdk

# Add FluentAssertions for better assertion syntax
dotnet add package FluentAssertions --version 6.12.0

# Add MaterialColorUtilities for HCT color testing
dotnet add package MaterialColorUtilities --version 0.3.0

# Add Moq for mocking (future use)
dotnet add package Moq --version 4.20.70
```

### Step 4: Create Folder Structure

Create this folder hierarchy inside `MyVocaList.Tests/`:

```
MyVocaList.Tests/
├── View/
│   ├── ColorSystem/          ← MD3 color tests (start here)
│   │   ├── MD3ComplianceTests.cs
│   │   └── TonalPaletteTests.cs
│   ├── Behaviors/            ← Future: behavior tests
│   ├── Components/           ← Future: component tests
│   └── Pages/                ← Future: page tests
├── Services/                 ← Future: service layer tests
├── Infrastructure/           ← Future: infrastructure tests
├── Domain/                   ← Future: domain logic tests
├── Helpers/                  ← Test helper classes
│   └── ColorTestHelpers.cs
└── TestData/                 ← Test data files
    └── SampleMaterialColors.xaml
```

### Step 5: Create Test Project File (MyVocaList.Tests.csproj)

The project file should look like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="MaterialColorUtilities" Version="0.3.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="xunit" Version="2.6.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.4">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MyVocaList.View\MyVocaList.View.csproj" />
    <ProjectReference Include="..\MyVocaList.Services\MyVocaList.Services.csproj" />
    <ProjectReference Include="..\MyVocaList.Infrastructure\MyVocaList.Infrastructure.csproj" />
    <ProjectReference Include="..\MyVocaList.Domain\MyVocaList.Domain.csproj" />
    <ProjectReference Include="..\MyVocaList.Contracts\MyVocaList.Contracts.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="View\ColorSystem\" />
    <Folder Include="View\Behaviors\" />
    <Folder Include="View\Components\" />
    <Folder Include="View\Pages\" />
    <Folder Include="Services\" />
    <Folder Include="Infrastructure\" />
    <Folder Include="Domain\" />
    <Folder Include="Helpers\" />
    <Folder Include="TestData\" />
  </ItemGroup>

</Project>
```

### Step 6: Create Helper Class

Create `MyVocaList.Tests/Helpers/ColorTestHelpers.cs`:

```csharp
using MaterialColorUtilities.Hct;
using MaterialColorUtilities.Utils;

namespace MyVocaList.Tests.Helpers
{
    /// <summary>
    /// Helper methods for color testing and validation
    /// </summary>
    public static class ColorTestHelpers
    {
        /// <summary>
        /// Converts hex color string to ARGB uint
        /// </summary>
        public static uint HexToArgb(string hex)
        {
            hex = hex.Replace("#", "");
            
            if (hex.Length == 6)
                hex = "FF" + hex; // Add alpha if not present
            
            return Convert.ToUInt32(hex, 16);
        }

        /// <summary>
        /// Calculates WCAG contrast ratio between two colors
        /// </summary>
        public static double CalculateContrastRatio(uint color1, uint color2)
        {
            double lum1 = GetRelativeLuminance(color1);
            double lum2 = GetRelativeLuminance(color2);
            
            double lighter = Math.Max(lum1, lum2);
            double darker = Math.Min(lum1, lum2);
            
            return (lighter + 0.05) / (darker + 0.05);
        }

        /// <summary>
        /// Gets relative luminance for WCAG calculations
        /// </summary>
        private static double GetRelativeLuminance(uint color)
        {
            double r = ((color >> 16) & 0xFF) / 255.0;
            double g = ((color >> 8) & 0xFF) / 255.0;
            double b = (color & 0xFF) / 255.0;
            
            r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
            g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
            b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);
            
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        /// <summary>
        /// Verifies if contrast ratio meets WCAG AA standard (4.5:1)
        /// </summary>
        public static bool MeetsWCAG_AA(uint color1, uint color2)
        {
            return CalculateContrastRatio(color1, color2) >= 4.5;
        }

        /// <summary>
        /// Verifies if contrast ratio meets WCAG AAA standard (7:1)
        /// </summary>
        public static bool MeetsWCAG_AAA(uint color1, uint color2)
        {
            return CalculateContrastRatio(color1, color2) >= 7.0;
        }

        /// <summary>
        /// Converts ARGB uint to HCT color space
        /// </summary>
        public static Hct ToHct(uint argb)
        {
            return Hct.FromInt(argb);
        }

        /// <summary>
        /// Checks if two colors are visually similar (within tolerance)
        /// </summary>
        public static bool AreColorsSimilar(uint color1, uint color2, int tolerance = 5)
        {
            int r1 = (int)((color1 >> 16) & 0xFF);
            int g1 = (int)((color1 >> 8) & 0xFF);
            int b1 = (int)(color1 & 0xFF);
            
            int r2 = (int)((color2 >> 16) & 0xFF);
            int g2 = (int)((color2 >> 8) & 0xFF);
            int b2 = (int)(color2 & 0xFF);
            
            return Math.Abs(r1 - r2) <= tolerance &&
                   Math.Abs(g1 - g2) <= tolerance &&
                   Math.Abs(b1 - b2) <= tolerance;
        }
    }
}
```

### Step 7: Create Sample Test (Verification)

Create `MyVocaList.Tests/View/ColorSystem/SampleTest.cs`:

```csharp
using Xunit;
using FluentAssertions;

namespace MyVocaList.Tests.View.ColorSystem
{
    /// <summary>
    /// Sample test to verify test project setup
    /// </summary>
    public class SampleTest
    {
        [Fact]
        public void TestProject_ShouldBeConfiguredCorrectly()
        {
            // Arrange
            var expected = 42;
            
            // Act
            var actual = 40 + 2;
            
            // Assert
            actual.Should().Be(expected);
        }
    }
}
```

### Step 8: Verify Setup

```bash
# Build test project
dotnet build MyVocaList.Tests/MyVocaList.Tests.csproj

# Run tests
dotnet test MyVocaList.Tests/MyVocaList.Tests.csproj

# Expected: 1 test passed (SampleTest)
```

## Project Structure Overview

After completion, the solution should have:

```
MyVocaList/
├── MyVocaList.Domain/
├── MyVocaList.Services/
├── MyVocaList.Infrastructure/
├── MyVocaList.View/
├── MyVocaList.Tests/              ← NEW
│   ├── View/
│   │   └── ColorSystem/
│   │       └── SampleTest.cs
│   ├── Helpers/
│   │   └── ColorTestHelpers.cs
│   └── MyVocaList.Tests.csproj
└── MyVocaList.sln
```

## Success Criteria

- [ ] Test project created with net8.0 target
- [ ] xUnit framework configured
- [ ] FluentAssertions package added
- [ ] MaterialColorUtilities package added
- [ ] References to all solution projects added
- [ ] Folder structure created (View/ColorSystem, Services, etc.)
- [ ] ColorTestHelpers.cs created with utility methods
- [ ] SampleTest.cs created and passes
- [ ] `dotnet test` runs successfully
- [ ] Project added to solution file

## Future Extensibility

This structure supports:

**For View Layer:**
- ColorSystem tests (MD3 compliance) ← Start here
- Behavior tests (NavBarBehavior, etc.)
- Component tests (HeaderComponent, etc.)
- Page tests (ViewModel interactions)

**For Other Layers:**
- Services/ folder for service layer tests
- Infrastructure/ folder for repository/data tests
- Domain/ folder for domain logic tests

**Test Organization:**
- Each folder mirrors the solution structure
- Easy to find tests for specific components
- Supports parallel test development

## Commit Message

```
test: Create unit test project structure for MyVocaList solution

- Created MyVocaList.Tests project with xUnit framework
- Added FluentAssertions for better test readability
- Added MaterialColorUtilities for HCT color testing
- Created folder structure mirroring solution (View, Services, Domain, etc.)
- Created ColorTestHelpers with WCAG contrast calculation utilities
- Added references to all solution projects for future coverage
- Verified setup with sample passing test

Ready for MD3 compliance test implementation.
Testing: dotnet test - 1 test passed
```

## Notes

- Delete `UnitTest1.cs` if it was created by template
- Keep folder structure even if folders are empty initially
- Add `.gitkeep` files to empty folders if needed for version control
- Consider adding code coverage tools later (coverlet already included)
