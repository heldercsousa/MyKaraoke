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
        // Primary brand color from MaterialColors.xaml
        private const string PrimaryPurple = "#7F41AC";

        [Fact]
        public void Test1_HCT_Conversion_Roundtrip_ShouldPreserveColorAccurately()
        {
            // Arrange: Convert primary purple to ARGB
            var purpleArgb = ColorTestHelpers.HexToArgb(PrimaryPurple);

            // Act: Convert to HCT color space
            var hct = Hct.FromInt(purpleArgb);

            // Assert: Verify HCT values are within expected ranges for magenta-purple
            hct.Hue.Should().BeInRange(310, 320, "magenta-purple hue should be around 314°");
            hct.Chroma.Should().BeInRange(55, 65, "magenta-purple chroma should be ~59 for highly vibrant color");
            hct.Tone.Should().BeInRange(35, 55, "magenta-purple tone should be 40-50 for medium brightness");

            // Output for verification
            Console.WriteLine($"Primary Purple HCT Analysis:");
            Console.WriteLine($"  Hex: {PrimaryPurple}");
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
            // Arrange: Create tonal palette from primary purple
            var purpleArgb = ColorTestHelpers.HexToArgb(PrimaryPurple);
            var palette = TonalPalette.FromInt(purpleArgb);

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
            var hueBase = Hct.FromInt(purpleArgb).Hue;
            hct90.Hue.Should().BeApproximately(hueBase, 5, "tonal variants should maintain hue");
            hct10.Hue.Should().BeApproximately(hueBase, 5, "tonal variants should maintain hue");
        }

        [Fact]
        public void Test3_WCAG_Contrast_Primary40_On_Primary100_ShouldMeetAAStandard()
        {
            // Arrange: Generate Primary40 (default primary) and Primary100 (lightest background)
            var purpleArgb = ColorTestHelpers.HexToArgb(PrimaryPurple);
            var palette = TonalPalette.FromInt(purpleArgb);

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
            // Arrange: Generate container colors (MD3 pattern)
            var purpleArgb = ColorTestHelpers.HexToArgb(PrimaryPurple);
            var palette = TonalPalette.FromInt(purpleArgb);

            var primaryContainer = palette.Tone(90); // Light container background
            var onPrimaryContainer = palette.Tone(10); // Dark text on container

            // Act: Calculate contrast ratio
            var contrastRatio = ColorTestHelpers.CalculateContrastRatio(onPrimaryContainer, primaryContainer);

            // Assert: Must meet WCAG AA (4.5:1) for text on container
            contrastRatio.Should().BeGreaterOrEqualTo(4.5,
                "OnPrimaryContainer (Tone10) on PrimaryContainer (Tone90) must meet WCAG AA");

            Console.WriteLine($"\nWCAG Contrast Test 2:");
            Console.WriteLine($"  PrimaryContainer (Tone90): {StringUtils.HexFromArgb(primaryContainer)}");
            Console.WriteLine($"  OnPrimaryContainer (Tone10): {StringUtils.HexFromArgb(onPrimaryContainer)}");
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
            // Arrange: Test primary purple on typical surface colors
            var purpleArgb = ColorTestHelpers.HexToArgb(PrimaryPurple);
            var whiteSurface = ColorTestHelpers.HexToArgb("#FFFFFF");
            var lightGraySurface = ColorTestHelpers.HexToArgb("#F5F5F5");

            // Act: Calculate contrast ratios
            var contrastOnWhite = ColorTestHelpers.CalculateContrastRatio(purpleArgb, whiteSurface);
            var contrastOnLightGray = ColorTestHelpers.CalculateContrastRatio(purpleArgb, lightGraySurface);

            // Assert: Primary should be readable on common surface colors
            contrastOnWhite.Should().BeGreaterOrEqualTo(4.5,
                "Primary purple must be readable on white surface");
            contrastOnLightGray.Should().BeGreaterOrEqualTo(4.5,
                "Primary purple must be readable on light gray surface");

            Console.WriteLine($"\nWCAG Contrast Test 3:");
            Console.WriteLine($"  Primary ({PrimaryPurple}) on White (#FFFFFF):");
            Console.WriteLine($"    Contrast: {contrastOnWhite:F2}:1 - {(contrastOnWhite >= 4.5 ? "✓ PASS" : "✗ FAIL")} AA");
            Console.WriteLine($"  Primary ({PrimaryPurple}) on Light Gray (#F5F5F5):");
            Console.WriteLine($"    Contrast: {contrastOnLightGray:F2}:1 - {(contrastOnLightGray >= 4.5 ? "✓ PASS" : "✗ FAIL")} AA");
        }

        [Fact]
        public void Test6_FullTonalPalette_AllTonesGenerated_ShouldBeMonotonic()
        {
            // Arrange: Create palette and generate all standard tones
            var purpleArgb = ColorTestHelpers.HexToArgb(PrimaryPurple);
            var palette = TonalPalette.FromInt(purpleArgb);

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
            var purpleArgb = ColorTestHelpers.HexToArgb(PrimaryPurple);

            // Act: Convert to HCT and back
            var hct = Hct.FromInt(purpleArgb);
            var convertedArgb = hct.ToInt();

            // Assert: Colors should be visually identical (within tolerance)
            ColorTestHelpers.AreColorsSimilar(purpleArgb, convertedArgb, tolerance: 2)
                .Should().BeTrue("HCT roundtrip should preserve visual appearance");

            Console.WriteLine($"\nColor Similarity Test:");
            Console.WriteLine($"  Original:  {PrimaryPurple}");
            Console.WriteLine($"  Converted: {StringUtils.HexFromArgb(convertedArgb)}");
            Console.WriteLine($"  Match: {ColorTestHelpers.AreColorsSimilar(purpleArgb, convertedArgb, 2)}");
        }

        [Fact]
        public void Test8_MaterialColors_ExistingPalette_ShouldBeHCTCompliant()
        {
            // Test existing colors from MaterialColors.xaml to verify MD3 compliance
            var colorsToTest = new Dictionary<string, string>
            {
                { "Primary", "#7F41AC" },
                { "PrimaryDark", "#5A2D7A" },
                { "PrimaryLight", "#B47CD9" }
            };

            Console.WriteLine($"\nExisting MaterialColors.xaml Validation:");

            foreach (var (name, hex) in colorsToTest)
            {
                var argb = ColorTestHelpers.HexToArgb(hex);
                var hct = Hct.FromInt(argb);

                Console.WriteLine($"  {name} ({hex}):");
                Console.WriteLine($"    HCT: H:{hct.Hue:F1}° C:{hct.Chroma:F1} T:{hct.Tone:F1}");

                // Verify all magenta-purple variants have similar hue
                hct.Hue.Should().BeInRange(310, 320,
                    $"{name} should maintain magenta-purple hue family");
            }
        }
    }
}
