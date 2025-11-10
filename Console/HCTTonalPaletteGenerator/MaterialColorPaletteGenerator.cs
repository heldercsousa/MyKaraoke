using MaterialColorUtilities;
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Utils;
using MaterialColorUtilities.ColorAppearance;
using System;
using System.Text;

namespace MyVocaList.Console.HCTTonalPaletteGenerator
{
    /// <summary>
    /// Generates Material Design 3 tonal palettes from seed colors
    /// Outputs XAML ResourceDictionary with proper HCT-based color system
    /// </summary>
    public class MaterialColorPaletteGenerator
    {
        // MyVocaList Option 1 Colors
        private const uint PRIMARY_SEED = 0xFF7F41AC;    // Purple #7F41AC
        private const uint SECONDARY_SEED = 0xFF00796B;  // Teal #00796B
        private const uint TERTIARY_SEED = 0xFFF57C00;   // Orange #F57C00
        private const uint ERROR_SEED = 0xFFD32F2F;      // Red #D32F2F

        /// <summary>
        /// Standard Material Design 3 tones for tonal palettes
        /// </summary>
        private static readonly int[] STANDARD_TONES =
        {
            0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 95, 99, 100
        };

        public static string GenerateXamlResourceDictionary()
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.AppendLine("<ResourceDictionary");
            sb.AppendLine("    xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"");
            sb.AppendLine("    xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\">");
            sb.AppendLine();
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- MATERIAL DESIGN 3 COLOR SYSTEM - MyVocaList                                -->");
            sb.AppendLine("    <!-- OPTION 1: Purple-Primary Professional (HCT-GENERATED)                      -->");
            sb.AppendLine("    <!-- Generated using MaterialColorUtilities library                             -->");
            sb.AppendLine("    <!-- Split-Complementary Harmony: Purple + Teal + Orange                        -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();

            // Generate Primary Tonal Palette
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- PRIMARY TONAL PALETTE (Purple #7F41AC)                                     -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendTonalPalette(sb, "Primary", PRIMARY_SEED);

            // Generate Secondary Tonal Palette
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- SECONDARY TONAL PALETTE (Teal #00796B)                                     -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendTonalPalette(sb, "Secondary", SECONDARY_SEED);

            // Generate Tertiary Tonal Palette
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- TERTIARY TONAL PALETTE (Orange #F57C00)                                    -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendTonalPalette(sb, "Tertiary", TERTIARY_SEED);

            // Generate Error Tonal Palette
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- ERROR TONAL PALETTE (Red #D32F2F)                                          -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendTonalPalette(sb, "Error", ERROR_SEED);

            // Generate Neutral Palettes (algorithmically derived from Primary)
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- NEUTRAL TONAL PALETTES (Derived from Primary)                              -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();

            var hct = Hct.FromInt(PRIMARY_SEED);
            uint neutralSeed = Hct.From(hct.Hue, Math.Min(hct.Chroma / 12, 4), hct.Tone).ToInt();
            AppendTonalPalette(sb, "Neutral", neutralSeed);

            sb.AppendLine();
            uint neutralVariantSeed = Hct.From(hct.Hue, Math.Min(hct.Chroma / 6, 8), hct.Tone).ToInt();
            AppendTonalPalette(sb, "NeutralVariant", neutralVariantSeed);

            // Light Mode Semantic Tokens
            sb.AppendLine();
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- LIGHT MODE SEMANTIC TOKENS                                                 -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendLightModeTokens(sb);

            // Dark Mode Semantic Tokens (commented out for future)
            sb.AppendLine();
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- DARK MODE SEMANTIC TOKENS (Future)                                         -->");
            sb.AppendLine("    <!-- Uncomment when implementing dark mode                                      -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendDarkModeTokens(sb);

            // Gradients
            sb.AppendLine();
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- GRADIENTS (Using Tonal Palette Values)                                     -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendGradients(sb);

            // Semantic Colors
            sb.AppendLine();
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- SEMANTIC COLORS (For Specific Use Cases)                                   -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendSemanticColors(sb);

            // Footer
            sb.AppendLine("</ResourceDictionary>");

            return sb.ToString();
        }

        private static void AppendTonalPalette(StringBuilder sb, string colorName, uint seedColor)
        {
            var palette = TonalPalette.FromInt(seedColor);

            // Get HCT values for documentation
            var hct = Hct.FromInt(seedColor);
            sb.AppendLine($"    <!-- Base: {HexFromArgb(seedColor)} | Hue: {hct.Hue:F1}° | Chroma: {hct.Chroma:F1} | Tone: {hct.Tone:F1} -->");
            sb.AppendLine();

            foreach (var tone in STANDARD_TONES)
            {
                uint argb = palette.Tone((uint)tone);
                string hex = HexFromArgb(argb);
                string toneName = $"{colorName}{tone}";

                sb.AppendLine($"    <Color x:Key=\"{toneName}\">{hex}</Color>");
            }
            sb.AppendLine();
        }

        private static void AppendLightModeTokens(StringBuilder sb)
        {
            sb.AppendLine("    <!-- Primary Role Colors -->");
            sb.AppendLine("    <Color x:Key=\"Primary\">{StaticResource Primary40}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnPrimary\">{StaticResource Primary100}</Color>");
            sb.AppendLine("    <Color x:Key=\"PrimaryContainer\">{StaticResource Primary90}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnPrimaryContainer\">{StaticResource Primary10}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Secondary Role Colors -->");
            sb.AppendLine("    <Color x:Key=\"Secondary\">{StaticResource Secondary40}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSecondary\">{StaticResource Secondary100}</Color>");
            sb.AppendLine("    <Color x:Key=\"SecondaryContainer\">{StaticResource Secondary90}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSecondaryContainer\">{StaticResource Secondary10}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Tertiary Role Colors -->");
            sb.AppendLine("    <Color x:Key=\"Tertiary\">{StaticResource Tertiary40}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnTertiary\">{StaticResource Tertiary100}</Color>");
            sb.AppendLine("    <Color x:Key=\"TertiaryContainer\">{StaticResource Tertiary90}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnTertiaryContainer\">{StaticResource Tertiary10}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Error Role Colors -->");
            sb.AppendLine("    <Color x:Key=\"Error\">{StaticResource Error40}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnError\">{StaticResource Error100}</Color>");
            sb.AppendLine("    <Color x:Key=\"ErrorContainer\">{StaticResource Error90}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnErrorContainer\">{StaticResource Error10}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Background & Surface -->");
            sb.AppendLine("    <Color x:Key=\"Background\">{StaticResource Neutral99}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnBackground\">{StaticResource Neutral10}</Color>");
            sb.AppendLine("    <Color x:Key=\"Surface\">{StaticResource Neutral99}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSurface\">{StaticResource Neutral10}</Color>");
            sb.AppendLine("    <Color x:Key=\"SurfaceVariant\">{StaticResource NeutralVariant90}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSurfaceVariant\">{StaticResource NeutralVariant30}</Color>");
            sb.AppendLine("    <Color x:Key=\"SurfaceDim\">{StaticResource Neutral87}</Color>");
            sb.AppendLine("    <Color x:Key=\"SurfaceBright\">{StaticResource Neutral98}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Outline & Borders -->");
            sb.AppendLine("    <Color x:Key=\"Outline\">{StaticResource NeutralVariant50}</Color>");
            sb.AppendLine("    <Color x:Key=\"OutlineVariant\">{StaticResource NeutralVariant80}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Inverse Colors -->");
            sb.AppendLine("    <Color x:Key=\"InverseSurface\">{StaticResource Neutral20}</Color>");
            sb.AppendLine("    <Color x:Key=\"InverseOnSurface\">{StaticResource Neutral95}</Color>");
            sb.AppendLine("    <Color x:Key=\"InversePrimary\">{StaticResource Primary80}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Scrim & Shadow -->");
            sb.AppendLine("    <Color x:Key=\"Scrim\">{StaticResource Neutral0}</Color>");
            sb.AppendLine("    <Color x:Key=\"Shadow\">{StaticResource Neutral0}</Color>");
        }

        private static void AppendDarkModeTokens(StringBuilder sb)
        {
            sb.AppendLine("    <!-- Dark Mode (Future Implementation) -->");
            sb.AppendLine("    <!--");
            sb.AppendLine("    <Color x:Key=\"PrimaryDark\">{StaticResource Primary80}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnPrimaryDark\">{StaticResource Primary20}</Color>");
            sb.AppendLine("    <Color x:Key=\"PrimaryContainerDark\">{StaticResource Primary30}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnPrimaryContainerDark\">{StaticResource Primary90}</Color>");
            sb.AppendLine("    ...");
            sb.AppendLine("    -->");
        }

        private static void AppendGradients(StringBuilder sb)
        {
            sb.AppendLine("    <!-- Primary Purple Gradient -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"PrimaryGradient\" StartPoint=\"0,0\" EndPoint=\"1,0\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Primary40}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Primary10}\" Offset=\"1.0\" />");
            sb.AppendLine("    </LinearGradientBrush>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Secondary Teal Gradient -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"SecondaryGradient\" StartPoint=\"0,0\" EndPoint=\"1,0\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Secondary40}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Secondary10}\" Offset=\"1.0\" />");
            sb.AppendLine("    </LinearGradientBrush>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Tertiary Orange Gradient -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"TertiaryGradient\" StartPoint=\"0,0\" EndPoint=\"1,1\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Tertiary40}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Tertiary10}\" Offset=\"1.0\" />");
            sb.AppendLine("    </LinearGradientBrush>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Background Gradient -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"AppBackgroundGradient\" StartPoint=\"0,0\" EndPoint=\"1,0\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Neutral99}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource NeutralVariant95}\" Offset=\"1.0\" />");
            sb.AppendLine("    </LinearGradientBrush>");
        }

        private static void AppendSemanticColors(StringBuilder sb)
        {
            sb.AppendLine("    <!-- Success (for positive feedback) -->");
            sb.AppendLine("    <Color x:Key=\"Success\">#4CAF50</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSuccess\">#FFFFFF</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Warning (for caution states) -->");
            sb.AppendLine("    <Color x:Key=\"Warning\">#FF9800</Color>");
            sb.AppendLine("    <Color x:Key=\"OnWarning\">#000000</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Info (for informational elements) -->");
            sb.AppendLine("    <Color x:Key=\"Info\">#2196F3</Color>");
            sb.AppendLine("    <Color x:Key=\"OnInfo\">#FFFFFF</Color>");
        }

        // Helper to convert ARGB uint to hex string in #AARRGGBB format
        private static string HexFromArgb(uint argb)
        {
            return $"#{argb:X8}";
        }
    }
}
