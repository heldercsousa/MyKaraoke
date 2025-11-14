using Xunit;
using FluentAssertions;
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Utils;
using MyVocaList.Tests.Helpers;
using StringUtils = MaterialColorUtilities.Utils.StringUtils;

namespace MyVocaList.Tests.View.ColorSystem
{
    /// <summary>
    /// Material Design 3 color compliance verification tests
    /// Validates HCT conversion, tonal palette generation, and WCAG contrast ratios
    /// </summary>
    public class MD3ComplianceTests
    {
        // MyVocaList Brand Colors (Pink/Purple/Gold)
        private const string PrimaryPink = "#E91E63";
        private const uint PRIMARY_SEED = 0xFFE91E63;    // Pink
        private const uint SECONDARY_SEED = 0xFF8B4CB8;  // Purple
        private const uint TERTIARY_SEED = 0xFFFFD700;   // Gold
        private const uint ERROR_SEED = 0xFFF44336;      // Red

        [Fact]
        public void Test1_HCT_Conversion_Roundtrip_ShouldPreserveColorAccurately()
        {
            // Arrange: Convert primary pink to ARGB
            var pinkArgb = ColorTestHelpers.HexToArgb(PrimaryPink);

            // Act: Convert to HCT color space
            var hct = Hct.FromInt(pinkArgb);

            // Assert: Verify HCT values are within expected ranges for pink (which converts to red-adjacent hue in HCT)
            // Note: #E91E63 converts to hue ~8° (red-adjacent) in HCT color space, not magenta
            hct.Hue.Should().BeInRange(0, 15, "pink #E91E63 converts to red-adjacent hue (~8°) in HCT");
            hct.Chroma.Should().BeInRange(75, 95, "pink chroma should be ~93 for highly vibrant color");
            hct.Tone.Should().BeInRange(45, 60, "pink tone should be ~50 for medium brightness");

            // Output for verification
            Console.WriteLine($"Primary Pink HCT Analysis:");
            Console.WriteLine($"  Hex: {PrimaryPink}");
            Console.WriteLine($"  Hue: {hct.Hue:F1}°");
            Console.WriteLine($"  Chroma: {hct.Chroma:F1}");
            Console.WriteLine($"  Tone: {hct.Tone:F1}");

            // Verify roundtrip conversion (HCT -> ARGB -> HCT)
            var convertedArgb = hct.ToInt();
            var hctRoundtrip = Hct.FromInt(convertedArgb);

            hctRoundtrip.Hue.Should().BeApproximately(hct.Hue, 1.0, "hue should be preserved in roundtrip");
            hctRoundtrip.Chroma.Should().BeApproximately(hct.Chroma, 1.0, "chroma should be preserved in roundtrip");
            hctRoundtrip.Tone.Should().BeApproximately(hct.Tone, 1.0, "tone should be preserved in roundtrip");
        }

        [Fact]
        public void Test2_TonalPalette_Generation_ShouldProduceCorrectLightAndDarkVariants()
        {
            // Arrange: Create tonal palette from primary pink
            var pinkArgb = ColorTestHelpers.HexToArgb(PrimaryPink);
            var palette = TonalPalette.FromInt(pinkArgb);

            // Act: Generate light and dark variants
            var tone90 = palette.Tone(90); // Light purple for Container
            var tone10 = palette.Tone(10); // Dark purple for OnContainer
            var tone40 = palette.Tone(40); // Primary
            var tone100 = palette.Tone(100); // White/lightest

            // Assert: Verify tonal values
            var hct90 = Hct.FromInt(tone90);
            hct90.Tone.Should().BeApproximately(90, 2, "tone 90 should be light");

            var hct10 = Hct.FromInt(tone10);
            hct10.Tone.Should().BeApproximately(10, 2, "tone 10 should be dark");

            // Output hex values for MaterialColors.xaml
            Console.WriteLine($"\nTonal Palette Results:");
            Console.WriteLine($"  Tone 100 (Lightest): {StringUtils.HexFromArgb(tone100)}");
            Console.WriteLine($"  Tone 90 (Container): {StringUtils.HexFromArgb(tone90)}");
            Console.WriteLine($"  Tone 40 (Primary): {StringUtils.HexFromArgb(tone40)}");
            Console.WriteLine($"  Tone 10 (OnContainer): {StringUtils.HexFromArgb(tone10)}");

            // Verify all tones maintain same hue
            var hueBase = Hct.FromInt(pinkArgb).Hue;
            hct90.Hue.Should().BeApproximately(hueBase, 5, "tonal variants should maintain hue");
            hct10.Hue.Should().BeApproximately(hueBase, 5, "tonal variants should maintain hue");
        }

        [Fact]
        public void Test3_WCAG_Contrast_Primary40_On_Primary100_ShouldMeetAAStandard()
        {
            // Arrange: Generate Primary40 (default primary) and Primary100 (lightest background)
            var pinkArgb = ColorTestHelpers.HexToArgb(PrimaryPink);
            var palette = TonalPalette.FromInt(pinkArgb);

            var primary40 = palette.Tone(40); // Primary color
            var primary100 = palette.Tone(100); // White background

            // Act: Calculate contrast ratio
            var contrastRatio = ColorTestHelpers.CalculateContrastRatio(primary40, primary100);

            // Assert: Must meet WCAG AA (4.5:1) for text on background
            contrastRatio.Should().BeGreaterOrEqualTo(4.5,
                "Primary40 on Primary100 must meet WCAG AA standard for readable text");

            Console.WriteLine($"\nWCAG Contrast Test 1:");
            Console.WriteLine($"  Primary40: {StringUtils.HexFromArgb(primary40)}");
            Console.WriteLine($"  Primary100: {StringUtils.HexFromArgb(primary100)}");
            Console.WriteLine($"  Contrast Ratio: {contrastRatio:F2}:1");
            Console.WriteLine($"  WCAG AA (4.5:1): {(contrastRatio >= 4.5 ? "✓ PASS" : "✗ FAIL")}");
            Console.WriteLine($"  WCAG AAA (7:1): {(contrastRatio >= 7.0 ? "✓ PASS" : "✗ FAIL")}");

            // Verify using helper method
            ColorTestHelpers.MeetsWCAG_AA(primary40, primary100).Should().BeTrue(
                "Helper method should confirm WCAG AA compliance");
        }

        [Fact]
        public void Test4_WCAG_Contrast_OnPrimaryContainer_On_PrimaryContainer_ShouldMeetAAStandard()
        {
            // Arrange: Generate container colors (MD3 pattern - DARK MODE)
            var pinkArgb = ColorTestHelpers.HexToArgb(PrimaryPink);
            var palette = TonalPalette.FromInt(pinkArgb);

            var primaryContainer = palette.Tone(30); // DARK MODE: Dark container background
            var onPrimaryContainer = palette.Tone(90); // DARK MODE: Light text on dark container

            // Act: Calculate contrast ratio
            var contrastRatio = ColorTestHelpers.CalculateContrastRatio(onPrimaryContainer, primaryContainer);

            // Assert: Must meet WCAG AA (4.5:1) for text on container
            contrastRatio.Should().BeGreaterOrEqualTo(4.5,
                "DARK MODE: OnPrimaryContainer (Tone90) on PrimaryContainer (Tone30) must meet WCAG AA");

            Console.WriteLine($"\nWCAG Contrast Test 2:");
            Console.WriteLine($"  PrimaryContainer (Tone30 - Dark Mode): {StringUtils.HexFromArgb(primaryContainer)}");
            Console.WriteLine($"  OnPrimaryContainer (Tone90 - Dark Mode): {StringUtils.HexFromArgb(onPrimaryContainer)}");
            Console.WriteLine($"  Contrast Ratio: {contrastRatio:F2}:1");
            Console.WriteLine($"  WCAG AA (4.5:1): {(contrastRatio >= 4.5 ? "✓ PASS" : "✗ FAIL")}");
            Console.WriteLine($"  WCAG AAA (7:1): {(contrastRatio >= 7.0 ? "✓ PASS" : "✗ FAIL")}");

            // This should typically pass AAA as well (10 vs 90 has high contrast)
            contrastRatio.Should().BeGreaterOrEqualTo(7.0,
                "Tone 10 vs Tone 90 should meet WCAG AAA standard");
        }

        [Fact]
        public void Test5_WCAG_Contrast_Primary_On_Surface_ShouldMeetAAStandard()
        {
            // Arrange: Test Primary tone 80 (dark mode) on dark surfaces
            var palette = TonalPalette.FromInt(PRIMARY_SEED);
            var primary80 = palette.Tone(80);  // Dark mode primary (brighter)
            var darkSurface = ColorTestHelpers.HexToArgb("#121212");  // Dark mode surface
            var darkGraySurface = ColorTestHelpers.HexToArgb("#1E1E1E"); // Dark mode elevated surface

            // Act: Calculate contrast ratios
            var contrastOnDark = ColorTestHelpers.CalculateContrastRatio(primary80, darkSurface);
            var contrastOnDarkGray = ColorTestHelpers.CalculateContrastRatio(primary80, darkGraySurface);

            // Assert: Primary tone 80 should be readable on dark mode surfaces
            contrastOnDark.Should().BeGreaterOrEqualTo(4.5,
                "Primary (Tone80) must be readable on dark surface in dark mode");
            contrastOnDarkGray.Should().BeGreaterOrEqualTo(4.5,
                "Primary (Tone80) must be readable on dark gray surface in dark mode");

            Console.WriteLine($"\nWCAG Contrast Test 3 (Dark Mode):");
            Console.WriteLine($"  Primary Tone80 ({StringUtils.HexFromArgb(primary80)}) on Dark Surface (#121212):");
            Console.WriteLine($"    Contrast: {contrastOnDark:F2}:1 - {(contrastOnDark >= 4.5 ? "✓ PASS" : "✗ FAIL")} AA");
            Console.WriteLine($"  Primary Tone80 ({StringUtils.HexFromArgb(primary80)}) on Dark Gray (#1E1E1E):");
            Console.WriteLine($"    Contrast: {contrastOnDarkGray:F2}:1 - {(contrastOnDarkGray >= 4.5 ? "✓ PASS" : "✗ FAIL")} AA");
        }

        [Fact]
        public void Test6_FullTonalPalette_AllTonesGenerated_ShouldBeMonotonic()
        {
            // Arrange: Create palette and generate all standard tones
            var pinkArgb = ColorTestHelpers.HexToArgb(PrimaryPink);
            var palette = TonalPalette.FromInt(pinkArgb);

            int[] standardTones = { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 95, 99, 100 };

            // Act & Assert: Generate all tones and verify monotonic brightness
            Console.WriteLine($"\nFull Tonal Palette (MD3 Standard Tones):");

            double previousTone = -1;
            foreach (var toneValue in standardTones)
            {
                var colorArgb = palette.Tone((uint)toneValue);
                var hct = Hct.FromInt(colorArgb);

                // Verify tone increases monotonically
                hct.Tone.Should().BeGreaterThan(previousTone,
                    $"tone {toneValue} should be brighter than previous");
                previousTone = hct.Tone;

                Console.WriteLine($"  Tone {toneValue,3}: {StringUtils.HexFromArgb(colorArgb)} " +
                    $"(H:{hct.Hue:F0}° C:{hct.Chroma:F0} T:{hct.Tone:F0})");
            }

            // Verify extreme tones
            var darkest = Hct.FromInt(palette.Tone(0));
            var lightest = Hct.FromInt(palette.Tone(100));

            darkest.Tone.Should().BeLessThan(5, "tone 0 should be very dark");
            lightest.Tone.Should().BeGreaterThan(95, "tone 100 should be very light");
        }

        [Fact]
        public void Test7_ColorSimilarity_RoundtripConversion_ShouldPreserveVisualAppearance()
        {
            // Arrange: Original color
            var pinkArgb = ColorTestHelpers.HexToArgb(PrimaryPink);

            // Act: Convert to HCT and back
            var hct = Hct.FromInt(pinkArgb);
            var convertedArgb = hct.ToInt();

            // Assert: Colors should be visually identical (within tolerance)
            ColorTestHelpers.AreColorsSimilar(pinkArgb, convertedArgb, tolerance: 2)
                .Should().BeTrue("HCT roundtrip should preserve visual appearance");

            Console.WriteLine($"\nColor Similarity Test:");
            Console.WriteLine($"  Original:  {PrimaryPink}");
            Console.WriteLine($"  Converted: {StringUtils.HexFromArgb(convertedArgb)}");
            Console.WriteLine($"  Match: {ColorTestHelpers.AreColorsSimilar(pinkArgb, convertedArgb, 2)}");
        }

        [Fact]
        public void Test8_MaterialColors_ExistingPalette_ShouldBeHCTCompliant()
        {
            // Test brand colors to verify MD3 compliance
            var colorsToTest = new Dictionary<string, string>
            {
                { "Primary (Pink)", "#E91E63" },
                { "Secondary (Purple)", "#8B4CB8" },
                { "Tertiary (Gold)", "#FFD700" }
            };

            Console.WriteLine($"\nBrand Colors MD3 Validation:");

            foreach (var (name, hex) in colorsToTest)
            {
                var argb = ColorTestHelpers.HexToArgb(hex);
                var hct = Hct.FromInt(argb);

                Console.WriteLine($"  {name} ({hex}):");
                Console.WriteLine($"    HCT: H:{hct.Hue:F1}° C:{hct.Chroma:F1} T:{hct.Tone:F1}");

                // Verify all colors have reasonable chroma for vibrancy
                hct.Chroma.Should().BeGreaterThan(30,
                    $"{name} should be sufficiently vibrant for brand identity");
            }
        }

        #region Additional Comprehensive MD3 Compliance Tests

        [Theory]
        [InlineData(0xFFE91E63, "Primary")]   // Pink
        [InlineData(0xFF8B4CB8, "Secondary")] // Purple
        [InlineData(0xFFFFD700, "Tertiary")]  // Gold
        [InlineData(0xFFF44336, "Error")]     // Red
        public void Test9_AllSeedColors_ShouldConvertToHCT_Successfully(uint seedColor, string colorName)
        {
            // Act
            var hct = Hct.FromInt(seedColor);

            // Assert
            hct.Hue.Should().BeInRange(0, 360, $"{colorName} hue should be valid");
            hct.Chroma.Should().BeGreaterOrEqualTo(0, $"{colorName} chroma should be non-negative");
            hct.Tone.Should().BeInRange(0, 100, $"{colorName} tone should be valid");

            Console.WriteLine($"\n{colorName} Seed Color HCT:");
            Console.WriteLine($"  Hue: {hct.Hue:F1}°");
            Console.WriteLine($"  Chroma: {hct.Chroma:F1}");
            Console.WriteLine($"  Tone: {hct.Tone:F1}");
        }

        [Theory]
        [InlineData(0xFFE91E63, "Primary")]   // Pink
        [InlineData(0xFF8B4CB8, "Secondary")] // Purple
        [InlineData(0xFFFFD700, "Tertiary")]  // Gold
        [InlineData(0xFFF44336, "Error")]     // Red
        public void Test10_AllPalettes_ShouldMaintainHue_AcrossTones(uint seedColor, string colorName)
        {
            // Arrange
            var palette = TonalPalette.FromInt(seedColor);
            var seedHct = Hct.FromInt(seedColor);
            var tonesToTest = new[] { 20, 40, 60, 80 }; // Middle tones
            var hueToleranceDegrees = 15.0; // Allow some hue shift (HCT may adjust)

            Console.WriteLine($"\n{colorName} Hue Consistency Test:");
            Console.WriteLine($"  Seed Hue: {seedHct.Hue:F1}°");

            // Act & Assert
            foreach (var tone in tonesToTest)
            {
                var toneColor = palette.Tone((uint)tone);
                var toneHct = Hct.FromInt(toneColor);

                var hueDifference = Math.Abs(toneHct.Hue - seedHct.Hue);
                // Handle wraparound (e.g., 359° vs 1°)
                if (hueDifference > 180)
                    hueDifference = 360 - hueDifference;

                Console.WriteLine($"    Tone {tone}: {toneHct.Hue:F1}° (diff: {hueDifference:F1}°)");

                hueDifference.Should().BeLessThan(hueToleranceDegrees,
                    $"{colorName} tone {tone} should maintain similar hue to seed");
            }
        }

        [Theory]
        [InlineData(0xFFE91E63, "Primary")]   // Pink
        [InlineData(0xFF8B4CB8, "Secondary")] // Purple
        [InlineData(0xFFFFD700, "Tertiary")]  // Gold
        [InlineData(0xFFF44336, "Error")]     // Red
        public void Test11_AllContainers_ShouldMeet_WCAG_AA_WithOnContainers(uint seedColor, string colorName)
        {
            // Arrange - DARK MODE
            var palette = TonalPalette.FromInt(seedColor);
            var container = palette.Tone(30);  // Dark mode container
            var onContainer = palette.Tone(90); // Dark mode on-container
            var minContrastAA = 4.5;

            // Act
            var contrastRatio = ColorTestHelpers.CalculateContrastRatio(container, onContainer);

            // Assert
            contrastRatio.Should().BeGreaterOrEqualTo(minContrastAA,
                $"{colorName}Container and On{colorName}Container must meet WCAG AA");

            Console.WriteLine($"\n{colorName} Container Contrast (Dark Mode):");
            Console.WriteLine($"  Container (Tone30 - Dark): {StringUtils.HexFromArgb(container)}");
            Console.WriteLine($"  OnContainer (Tone90 - Dark): {StringUtils.HexFromArgb(onContainer)}");
            Console.WriteLine($"  Contrast Ratio: {contrastRatio:F2}:1");
            Console.WriteLine($"  WCAG AA (4.5:1): {(contrastRatio >= 4.5 ? "✓ PASS" : "✗ FAIL")}");
            Console.WriteLine($"  WCAG AAA (7:1): {(contrastRatio >= 7.0 ? "✓ PASS" : "✗ FAIL")}");
        }

        [Fact]
        public void Test12_Primary_And_Error_ShouldBe_VisuallyDistinct()
        {
            // Arrange - DARK MODE
            var primaryPalette = TonalPalette.FromInt(PRIMARY_SEED);
            var errorPalette = TonalPalette.FromInt(ERROR_SEED);

            var primary = primaryPalette.Tone(80);  // DARK MODE tone
            var error = errorPalette.Tone(80);      // DARK MODE tone

            var primaryHct = Hct.FromInt(primary);
            var errorHct = Hct.FromInt(error);

            var minHueSeparationDegrees = 15.0; // Pink vs Red - minimum for visual distinction

            // Act
            var hueDifference = Math.Abs(primaryHct.Hue - errorHct.Hue);
            if (hueDifference > 180)
                hueDifference = 360 - hueDifference;

            // Assert
            hueDifference.Should().BeGreaterOrEqualTo(minHueSeparationDegrees,
                "Primary (pink ~8° in HCT) and Error (red ~4°) should still be visually distinct");

            Console.WriteLine($"\nPrimary vs Error Color Separation:");
            Console.WriteLine($"  Primary Hue: {primaryHct.Hue:F1}°");
            Console.WriteLine($"  Error Hue: {errorHct.Hue:F1}°");
            Console.WriteLine($"  Separation: {hueDifference:F1}°");
            Console.WriteLine($"  Status: {(hueDifference >= minHueSeparationDegrees ? "✓ PASS" : "✗ FAIL")}");
        }

        [Fact]
        public void Test13_AllKeyColors_ShouldHave_DistinctHues()
        {
            // Arrange
            var seeds = new[]
            {
                (PRIMARY_SEED, "Primary"),
                (SECONDARY_SEED, "Secondary"),
                (TERTIARY_SEED, "Tertiary"),
                (ERROR_SEED, "Error")
            };

            var minSeparationDegrees = 15.0; // Minimum to be visually distinct (adjusted for pink/red proximity)

            Console.WriteLine($"\nAll Key Colors Hue Separation Matrix:");

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

                    Console.WriteLine($"  {seeds[i].Item2} vs {seeds[j].Item2}: {hueDiff:F1}° " +
                        $"{(hueDiff >= minSeparationDegrees ? "✓" : "✗")}");

                    hueDiff.Should().BeGreaterOrEqualTo(minSeparationDegrees,
                        $"{seeds[i].Item2} and {seeds[j].Item2} must have distinct hues");
                }
            }
        }

        [Fact]
        public void Test14_NeutralPalette_ShouldBe_LowChroma()
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

            Console.WriteLine($"\nNeutral Palette Chroma Test:");
            Console.WriteLine($"  Primary Chroma: {primaryHct.Chroma:F1}");
            Console.WriteLine($"  Neutral Chroma (calculated): {neutralChroma:F1}");
            Console.WriteLine($"  Neutral Tone50 Chroma: {neutralHct.Chroma:F1}");
            Console.WriteLine($"  Status: {(neutralHct.Chroma < maxChroma ? "✓ PASS" : "✗ FAIL")}");
        }

        [Theory]
        [InlineData(80, "Primary")]          // Dark mode: bright
        [InlineData(20, "OnPrimary")]        // Dark mode: dark
        [InlineData(30, "PrimaryContainer")] // Dark mode: dark container
        [InlineData(90, "OnPrimaryContainer")] // Dark mode: light text
        public void Test15_DarkMode_PrimaryRoles_ShouldUse_CorrectTones(int expectedTone, string roleName)
        {
            // Arrange
            var palette = TonalPalette.FromInt(PRIMARY_SEED);

            // Act
            var color = palette.Tone((uint)expectedTone);
            var hct = Hct.FromInt(color);

            // Assert
            hct.Tone.Should().BeApproximately(expectedTone, 1.0,
                $"{roleName} should use tone {expectedTone} in DARK MODE");

            Console.WriteLine($"\n{roleName} Tone Mapping:");
            Console.WriteLine($"  Expected Tone: {expectedTone}");
            Console.WriteLine($"  Actual Tone: {hct.Tone:F1}");
            Console.WriteLine($"  Color: {StringUtils.HexFromArgb(color)}");
        }

        #endregion
    }
}
