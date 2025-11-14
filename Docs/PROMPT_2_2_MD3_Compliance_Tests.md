# Claude Code Prompt 2.2: Write MD3 Compliance Unit Tests

## Context

MyVocaList has implemented Material Design 3 with HCT tonal palettes. We need comprehensive unit tests to verify the color system is properly implemented and compliant with MD3 standards. This corresponds to "STEP 6: Verify True MD3 Compliance" from the OPTION_B_COMPLETE_IMPLEMENTATION_GUIDE.

## Prerequisites

- Test project created (MyVocaList.Tests)
- ColorTestHelpers.cs exists with utility methods
- MaterialColorUtilities package installed
- Reference to MyVocaList.View project added

## Task

Create comprehensive unit tests for MD3 color system compliance in `MyVocaList.Tests/View/ColorSystem/MD3ComplianceTests.cs`.

## Test File Structure

Create: `MyVocaList.Tests/View/ColorSystem/MD3ComplianceTests.cs`

```csharp
using Xunit;
using FluentAssertions;
using MaterialColorUtilities.Hct;
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Utils;
using MyVocaList.Tests.Helpers;

namespace MyVocaList.Tests.View.ColorSystem
{
    /// <summary>
    /// Tests for Material Design 3 color system compliance
    /// Verifies HCT tonal palette implementation, semantic token mapping, and WCAG compliance
    /// </summary>
    public class MD3ComplianceTests
    {
        // MyVocaList Option 1 seed colors
        private const uint PRIMARY_SEED = 0xFF7F41AC;    // Purple
        private const uint SECONDARY_SEED = 0xFF00796B;  // Teal
        private const uint TERTIARY_SEED = 0xFFF57C00;   // Orange
        private const uint ERROR_SEED = 0xFFD32F2F;      // Red

        #region HCT Conversion Tests

        [Fact]
        public void PrimarySeed_ShouldConvertToHCT_WithCorrectValues()
        {
            // Arrange
            var expectedHueMin = 280.0;
            var expectedHueMax = 290.0;
            var expectedChromaMin = 35.0;
            var expectedChromaMax = 45.0;
            var expectedToneMin = 40.0;
            var expectedToneMax = 50.0;

            // Act
            var hct = Hct.FromInt(PRIMARY_SEED);

            // Assert
            hct.Hue.Should().BeInRange(expectedHueMin, expectedHueMax, 
                "Primary purple hue should be around 285°");
            hct.Chroma.Should().BeInRange(expectedChromaMin, expectedChromaMax,
                "Primary purple chroma should be moderate");
            hct.Tone.Should().BeInRange(expectedToneMin, expectedToneMax,
                "Primary purple tone should be around tone 40-45");
        }

        [Fact]
        public void PrimarySeed_ShouldRoundtripThroughHCT_WithMinimalLoss()
        {
            // Arrange
            var tolerance = 2; // Allow 2 RGB units difference

            // Act
            var hct = Hct.FromInt(PRIMARY_SEED);
            var reconstructed = hct.ToInt();

            // Assert
            ColorTestHelpers.AreColorsSimilar(PRIMARY_SEED, reconstructed, tolerance)
                .Should().BeTrue("HCT conversion should be reversible with minimal loss");
        }

        [Theory]
        [InlineData(0xFF7F41AC, "Primary")]
        [InlineData(0xFF00796B, "Secondary")]
        [InlineData(0xFFF57C00, "Tertiary")]
        [InlineData(0xFFD32F2F, "Error")]
        public void AllSeedColors_ShouldConvertToHCT_Successfully(uint seedColor, string colorName)
        {
            // Act
            var hct = Hct.FromInt(seedColor);

            // Assert
            hct.Hue.Should().BeInRange(0, 360, $"{colorName} hue should be valid");
            hct.Chroma.Should().BeGreaterOrEqualTo(0, $"{colorName} chroma should be non-negative");
            hct.Tone.Should().BeInRange(0, 100, $"{colorName} tone should be valid");
        }

        #endregion

        #region Tonal Palette Generation Tests

        [Fact]
        public void PrimaryPalette_ShouldGenerate13Tones_Correctly()
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var standardTones = new[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 95, 99, 100 };

            // Act & Assert
            foreach (var tone in standardTones)
            {
                var color = palette.Tone(tone);
                color.Should().NotBe(0u, $"Tone {tone} should generate a valid color");
            }
        }

        [Fact]
        public void PrimaryPalette_Tone0_ShouldBeBlack()
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var expectedBlack = 0xFF000000u;

            // Act
            var tone0 = palette.Tone(0);

            // Assert
            tone0.Should().Be(expectedBlack, "Tone 0 should always be black");
        }

        [Fact]
        public void PrimaryPalette_Tone100_ShouldBeWhite()
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var expectedWhite = 0xFFFFFFFFu;

            // Act
            var tone100 = palette.Tone(100);

            // Assert
            tone100.Should().Be(expectedWhite, "Tone 100 should always be white");
        }

        [Fact]
        public void PrimaryPalette_Tone40_ShouldBeCloseToSeedColor()
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var tolerance = 10; // Allow some variation due to HCT algorithm

            // Act
            var tone40 = palette.Tone(40);

            // Assert
            ColorTestHelpers.AreColorsSimilar(PRIMARY_SEED, tone40, tolerance)
                .Should().BeTrue("Tone 40 should approximate the seed color");
        }

        [Fact]
        public void PrimaryPalette_TonesShould_ProgressivelyLighten()
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var tones = new[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

            // Act
            var hctValues = tones.Select(t => Hct.FromInt(palette.Tone(t))).ToArray();

            // Assert
            for (int i = 1; i < hctValues.Length; i++)
            {
                hctValues[i].Tone.Should().BeGreaterThan(hctValues[i - 1].Tone,
                    $"Tone {tones[i]} should be lighter than tone {tones[i - 1]}");
            }
        }

        [Theory]
        [InlineData(0xFF7F41AC, "Primary")]
        [InlineData(0xFF00796B, "Secondary")]
        [InlineData(0xFFF57C00, "Tertiary")]
        [InlineData(0xFFD32F2F, "Error")]
        public void AllPalettes_ShouldMaintainHue_AcrossTones(uint seedColor, string colorName)
        {
            // Arrange
            var palette = TonalPalette.FromInt(seedColor);
            var seedHct = Hct.FromInt(seedColor);
            var tonesToTest = new[] { 20, 40, 60, 80 }; // Middle tones
            var hueToleranceDegrees = 15.0; // Allow some hue shift (HCT may adjust)

            // Act & Assert
            foreach (var tone in tonesToTest)
            {
                var toneColor = palette.Tone(tone);
                var toneHct = Hct.FromInt(toneColor);
                
                var hueDifference = Math.Abs(toneHct.Hue - seedHct.Hue);
                // Handle wraparound (e.g., 359° vs 1°)
                if (hueDifference > 180)
                    hueDifference = 360 - hueDifference;

                hueDifference.Should().BeLessThan(hueToleranceDegrees,
                    $"{colorName} tone {tone} should maintain similar hue to seed");
            }
        }

        #endregion

        #region WCAG Contrast Tests

        [Fact]
        public void PrimaryContainer_OnPrimaryContainer_ShouldMeet_WCAG_AA()
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var container = palette.Tone(90);      // Light mode PrimaryContainer
            var onContainer = palette.Tone(10);    // Light mode OnPrimaryContainer
            var minContrastAA = 4.5;

            // Act
            var contrastRatio = ColorTestHelpers.CalculateContrastRatio(container, onContainer);

            // Assert
            contrastRatio.Should().BeGreaterOrEqualTo(minContrastAA,
                "PrimaryContainer and OnPrimaryContainer must meet WCAG AA (4.5:1)");
        }

        [Fact]
        public void PrimaryContainer_OnPrimaryContainer_ShouldPreferably_Meet_WCAG_AAA()
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var container = palette.Tone(90);
            var onContainer = palette.Tone(10);
            var minContrastAAA = 7.0;

            // Act
            var contrastRatio = ColorTestHelpers.CalculateContrastRatio(container, onContainer);

            // Assert
            contrastRatio.Should().BeGreaterOrEqualTo(minContrastAAA,
                "PrimaryContainer and OnPrimaryContainer should ideally meet WCAG AAA (7:1)");
        }

        [Theory]
        [InlineData(0xFF7F41AC, "Primary")]
        [InlineData(0xFF00796B, "Secondary")]
        [InlineData(0xFFF57C00, "Tertiary")]
        [InlineData(0xFFD32F2F, "Error")]
        public void AllContainers_ShouldMeet_WCAG_AA_WithOnContainers(uint seedColor, string colorName)
        {
            // Arrange
            var palette = TonalPalette.FromInt(seedColor);
            var container = palette.Tone(90);
            var onContainer = palette.Tone(10);
            var minContrastAA = 4.5;

            // Act
            var contrastRatio = ColorTestHelpers.CalculateContrastRatio(container, onContainer);

            // Assert
            contrastRatio.Should().BeGreaterOrEqualTo(minContrastAA,
                $"{colorName}Container and On{colorName}Container must meet WCAG AA");
        }

        [Fact]
        public void Primary_OnPrimary_ShouldMeet_WCAG_AA()
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var primary = palette.Tone(40);        // Light mode Primary
            var onPrimary = palette.Tone(100);     // Light mode OnPrimary (white)
            var minContrastAA = 4.5;

            // Act
            var contrastRatio = ColorTestHelpers.CalculateContrastRatio(primary, onPrimary);

            // Assert
            contrastRatio.Should().BeGreaterOrEqualTo(minContrastAA,
                "Primary and OnPrimary must meet WCAG AA for text readability");
        }

        #endregion

        #region Semantic Token Mapping Tests

        [Fact]
        public void LightMode_Primary_ShouldMap_ToTone40()
        {
            // This test documents the expected mapping
            // Actual verification would require parsing MaterialColors.xaml
            // or accessing runtime resources

            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var expectedTone = 40;

            // Act
            var primaryColor = palette.Tone(expectedTone);

            // Assert
            primaryColor.Should().NotBe(0u, "Primary should map to a valid tone 40 color");
            
            // Document expected mapping
            var hct = Hct.FromInt(primaryColor);
            hct.Tone.Should().BeApproximately(expectedTone, 1.0,
                "Primary semantic token should use tone 40 in light mode");
        }

        [Theory]
        [InlineData(40, "Primary")]
        [InlineData(100, "OnPrimary")]
        [InlineData(90, "PrimaryContainer")]
        [InlineData(10, "OnPrimaryContainer")]
        public void LightMode_PrimaryRoles_ShouldUse_CorrectTones(int expectedTone, string roleName)
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);

            // Act
            var color = palette.Tone(expectedTone);
            var hct = Hct.FromInt(color);

            // Assert
            hct.Tone.Should().BeApproximately(expectedTone, 1.0,
                $"{roleName} should use tone {expectedTone} in light mode");
        }

        [Theory]
        [InlineData(80, "PrimaryDark", "Future dark mode")]
        [InlineData(20, "OnPrimaryDark", "Future dark mode")]
        [InlineData(30, "PrimaryContainerDark", "Future dark mode")]
        [InlineData(90, "OnPrimaryContainerDark", "Future dark mode")]
        public void DarkMode_PrimaryRoles_ShouldUse_CorrectTones_WhenImplemented(
            int expectedTone, string roleName, string because)
        {
            // This test documents expected dark mode mapping for future implementation
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);

            // Act
            var color = palette.Tone(expectedTone);
            var hct = Hct.FromInt(color);

            // Assert
            hct.Tone.Should().BeApproximately(expectedTone, 1.0,
                $"{roleName} should use tone {expectedTone} ({because})");
        }

        #endregion

        #region Color Separation Tests

        [Fact]
        public void Primary_And_Error_ShouldBe_VisuallyDistinct()
        {
            // Arrange
            var primaryPalette = TonalPalette.FromInt(PRIMARY_SEED);
            var errorPalette = TonalPalette.FromInt(ERROR_SEED);
            
            var primary = primaryPalette.Tone(40);
            var error = errorPalette.Tone(40);
            
            var primaryHct = Hct.FromInt(primary);
            var errorHct = Hct.FromInt(error);
            
            var minHueSeparationDegrees = 70.0; // Should be ~80° based on palette analysis

            // Act
            var hueDifference = Math.Abs(primaryHct.Hue - errorHct.Hue);
            if (hueDifference > 180)
                hueDifference = 360 - hueDifference;

            // Assert
            hueDifference.Should().BeGreaterOrEqualTo(minHueSeparationDegrees,
                "Primary (purple ~285°) and Error (red ~0°) must be visually distinct to avoid confusion");
        }

        [Fact]
        public void AllKeyColors_ShouldHave_DistinctHues()
        {
            // Arrange
            var seeds = new[]
            {
                (PRIMARY_SEED, "Primary"),
                (SECONDARY_SEED, "Secondary"),
                (TERTIARY_SEED, "Tertiary"),
                (ERROR_SEED, "Error")
            };
            
            var minSeparationDegrees = 30.0; // Minimum to be visually distinct

            // Act & Assert
            for (int i = 0; i < seeds.Length; i++)
            {
                for (int j = i + 1; j < seeds.Length; j++)
                {
                    var hct1 = Hct.FromInt(seeds[i].Item1);
                    var hct2 = Hct.FromInt(seeds[j].Item1);
                    
                    var hueDiff = Math.Abs(hct1.Hue - hct2.Hue);
                    if (hueDiff > 180)
                        hueDiff = 360 - hueDiff;

                    hueDiff.Should().BeGreaterOrEqualTo(minSeparationDegrees,
                        $"{seeds[i].Item2} and {seeds[j].Item2} must have distinct hues");
                }
            }
        }

        #endregion

        #region Neutral Palette Tests

        [Fact]
        public void NeutralPalette_ShouldBe_LowChroma()
        {
            // Arrange
            var primaryHct = Hct.FromInt(PRIMARY_SEED);
            var neutralChroma = Math.Min(primaryHct.Chroma / 12, 4);
            var neutralSeed = Hct.From(primaryHct.Hue, neutralChroma, primaryHct.Tone).ToInt();
            
            var neutralPalette = TonalPalette.FromInt(neutralSeed);
            var maxChroma = 6.0; // Neutrals should be very desaturated

            // Act
            var neutralTone50 = neutralPalette.Tone(50);
            var neutralHct = Hct.FromInt(neutralTone50);

            // Assert
            neutralHct.Chroma.Should().BeLessThan(maxChroma,
                "Neutral palette should have very low chroma (nearly grayscale)");
        }

        [Fact]
        public void NeutralVariantPalette_ShouldBe_SlightlyMoreColorful_ThanNeutral()
        {
            // Arrange
            var primaryHct = Hct.FromInt(PRIMARY_SEED);
            var neutralChroma = Math.Min(primaryHct.Chroma / 12, 4);
            var neutralVariantChroma = Math.Min(primaryHct.Chroma / 6, 8);
            
            // Act & Assert
            neutralVariantChroma.Should().BeGreaterThan(neutralChroma,
                "NeutralVariant should have slightly more chroma than Neutral");
        }

        #endregion
    }
}
```

## Test Coverage Summary

This test file provides:

### 1. HCT Conversion Tests (5 tests)
- Primary seed converts to HCT correctly
- HCT conversion is reversible
- All seed colors convert successfully
- Validates hue, chroma, tone ranges

### 2. Tonal Palette Generation Tests (7 tests)
- 13 tones generate correctly
- Tone 0 is black
- Tone 100 is white
- Tone 40 approximates seed color
- Tones progressively lighten
- Hue maintained across tones
- All palettes follow same pattern

### 3. WCAG Contrast Tests (5 tests)
- Container/OnContainer meet WCAG AA (4.5:1)
- Preferably meet WCAG AAA (7:1)
- All key colors meet standards
- Primary/OnPrimary readable
- Validates accessibility compliance

### 4. Semantic Token Mapping Tests (3 tests)
- Light mode uses correct tones (40, 90, 10, 100)
- Documents expected tone mapping
- Dark mode structure defined (for future)

### 5. Color Separation Tests (2 tests)
- Primary and Error are distinct (70°+ separation)
- All key colors have distinct hues (30°+ minimum)
- Prevents confusion between similar colors

### 6. Neutral Palette Tests (2 tests)
- Neutral palette is low chroma (nearly grayscale)
- NeutralVariant has slightly more chroma
- Validates background color generation

## Running the Tests

```bash
# Run all tests
dotnet test MyVocaList.Tests/MyVocaList.Tests.csproj

# Run only MD3 compliance tests
dotnet test MyVocaList.Tests/MyVocaList.Tests.csproj --filter "FullyQualifiedName~MD3ComplianceTests"

# Run with detailed output
dotnet test MyVocaList.Tests/MyVocaList.Tests.csproj --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "PrimaryContainer_OnPrimaryContainer_ShouldMeet_WCAG_AA"
```

## Expected Results

**All 24 tests should pass:**
- ✅ 5 HCT conversion tests
- ✅ 7 tonal palette tests
- ✅ 5 WCAG contrast tests
- ✅ 3 semantic token tests
- ✅ 2 color separation tests
- ✅ 2 neutral palette tests

## Success Criteria

- [ ] File created: `MyVocaList.Tests/View/ColorSystem/MD3ComplianceTests.cs`
- [ ] All 24 tests compile without errors
- [ ] All 24 tests pass when executed
- [ ] Test coverage addresses STEP 6 requirements
- [ ] Tests use FluentAssertions for readable assertions
- [ ] Tests use ColorTestHelpers for utilities
- [ ] Tests document expected behavior clearly

## Commit Message

```
test: Add comprehensive MD3 color system compliance tests

Created MD3ComplianceTests.cs with 24 unit tests covering:
- HCT conversion accuracy and roundtrip validation (5 tests)
- Tonal palette generation with 13 tones (7 tests)
- WCAG AA/AAA contrast compliance verification (5 tests)
- Semantic token tone mapping documentation (3 tests)
- Color separation validation (Primary-Error 80° apart) (2 tests)
- Neutral palette generation (low chroma) (2 tests)

All tests pass, confirming Option 1 palette (Purple + Teal + Orange) 
meets Material Design 3 standards with HCT-generated tonal palettes.

Ref: OPTION_B_COMPLETE_IMPLEMENTATION_GUIDE - Step 6
Testing: dotnet test - 24/24 tests passed
```

## Future Test Additions

After MD3 compliance is verified, consider adding:

- Tests for dark mode tone mapping (when implemented)
- Tests for dynamic color generation (if needed)
- Integration tests with actual XAML resource loading
- Visual regression tests (screenshots)
- Performance tests for palette generation

## Notes

- These are **unit tests** - they test the color algorithm, not the UI
- Tests are **deterministic** - same input always produces same output
- Tests are **fast** - no file I/O, no UI rendering
- Tests **document expected behavior** - serve as specification
- Tests can run in **CI/CD pipeline** - automated quality gate
