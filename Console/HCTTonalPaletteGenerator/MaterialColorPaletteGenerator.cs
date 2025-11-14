using MaterialColorUtilities;
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Utils;
using MaterialColorUtilities.ColorAppearance;
using System;
using System.Text;

namespace MyVocaList.Console.HCTTonalPaletteGenerator
{
    /// <summary>
    /// Generates Material Design 3 tonal palettes from seed colors (MyVocaList Brand: Pink/Purple/Gold)
    /// Outputs XAML ResourceDictionary with proper HCT-based color system
    /// </summary>
    public class MaterialColorPaletteGenerator
    {
        private const uint PRIMARY_SEED = 0xFFE91E63;    // Pink (your main brand color)
        private const uint SECONDARY_SEED = 0xFF8B4CB8;  // Purple (your secondary)
        private const uint TERTIARY_SEED = 0xFFFFD700;   // Gold (your FAB)
        private const uint ERROR_SEED = 0xFFF44336;      // Red (standard error)

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
            sb.AppendLine("    <!-- MyVocaList Brand Theme - Dark Mode (HCT-GENERATED)                         -->");
            sb.AppendLine("    <!-- Generated using MaterialColorUtilities library                             -->");
            sb.AppendLine("    <!-- Your Signature Colors: Pink + Purple + Gold                               -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();

            // Generate Primary Tonal Palette
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- PRIMARY TONAL PALETTE (Pink #E91E63)                                      -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendTonalPalette(sb, "Primary", PRIMARY_SEED);

            // Generate Secondary Tonal Palette
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- SECONDARY TONAL PALETTE (Purple #8B4CB8)                                     -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendTonalPalette(sb, "Secondary", SECONDARY_SEED);

            // Generate Tertiary Tonal Palette
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- TERTIARY TONAL PALETTE (Gold #FFD700)                                    -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendTonalPalette(sb, "Tertiary", TERTIARY_SEED);

            // Generate Error Tonal Palette
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- ERROR TONAL PALETTE (Red #F44336)                                          -->");
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

            // Dark Mode Semantic Tokens (PRIMARY THEME)
            sb.AppendLine();
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- DARK MODE SEMANTIC TOKENS (Primary Theme)                                  -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendDarkModeTokens(sb); 

            // Light Mode Semantic Tokens (FUTURE)
            sb.AppendLine();
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine("    <!-- LIGHT MODE SEMANTIC TOKENS (Future v2.0)                                   -->");
            sb.AppendLine("    <!-- Uncomment when implementing light mode                                     -->");
            sb.AppendLine("    <!-- ============================================================================ -->");
            sb.AppendLine();
            AppendLightModeTokens(sb); 

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
            sb.AppendLine("    <!-- ");
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
            sb.AppendLine("    --> ");
        }

        private static void AppendDarkModeTokens(StringBuilder sb)
        {
            sb.AppendLine("    <!-- Primary Role Colors (DARK MODE - Bright on Dark) -->");
            sb.AppendLine("    <Color x:Key=\"Primary\">{StaticResource Primary80}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnPrimary\">{StaticResource Primary20}</Color>");
            sb.AppendLine("    <Color x:Key=\"PrimaryContainer\">{StaticResource Primary30}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnPrimaryContainer\">{StaticResource Primary90}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Secondary Role Colors (DARK MODE) -->");
            sb.AppendLine("    <Color x:Key=\"Secondary\">{StaticResource Secondary80}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSecondary\">{StaticResource Secondary20}</Color>");
            sb.AppendLine("    <Color x:Key=\"SecondaryContainer\">{StaticResource Secondary30}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSecondaryContainer\">{StaticResource Secondary90}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Tertiary Role Colors (DARK MODE) -->");
            sb.AppendLine("    <Color x:Key=\"Tertiary\">{StaticResource Tertiary80}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnTertiary\">{StaticResource Tertiary20}</Color>");
            sb.AppendLine("    <Color x:Key=\"TertiaryContainer\">{StaticResource Tertiary30}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnTertiaryContainer\">{StaticResource Tertiary90}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Error Role Colors (DARK MODE) -->");
            sb.AppendLine("    <Color x:Key=\"Error\">{StaticResource Error80}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnError\">{StaticResource Error20}</Color>");
            sb.AppendLine("    <Color x:Key=\"ErrorContainer\">{StaticResource Error30}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnErrorContainer\">{StaticResource Error90}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Background & Surface (DARK MODE - Very Dark) -->");
            sb.AppendLine("    <Color x:Key=\"Background\">{StaticResource Neutral10}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnBackground\">{StaticResource Neutral90}</Color>");
            sb.AppendLine("    <Color x:Key=\"Surface\">{StaticResource Neutral10}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSurface\">{StaticResource Neutral90}</Color>");
            sb.AppendLine("    <Color x:Key=\"SurfaceVariant\">{StaticResource NeutralVariant30}</Color>");
            sb.AppendLine("    <Color x:Key=\"OnSurfaceVariant\">{StaticResource NeutralVariant80}</Color>");
            sb.AppendLine("    <Color x:Key=\"SurfaceDim\">{StaticResource Neutral6}</Color>");
            sb.AppendLine("    <Color x:Key=\"SurfaceBright\">{StaticResource Neutral24}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Outline & Borders (DARK MODE) -->");
            sb.AppendLine("    <Color x:Key=\"Outline\">{StaticResource NeutralVariant60}</Color>");
            sb.AppendLine("    <Color x:Key=\"OutlineVariant\">{StaticResource NeutralVariant30}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Inverse Colors (DARK MODE) -->");
            sb.AppendLine("    <Color x:Key=\"InverseSurface\">{StaticResource Neutral90}</Color>");
            sb.AppendLine("    <Color x:Key=\"InverseOnSurface\">{StaticResource Neutral20}</Color>");
            sb.AppendLine("    <Color x:Key=\"InversePrimary\">{StaticResource Primary40}</Color>");
            sb.AppendLine();

            sb.AppendLine("    <!-- Scrim & Shadow -->");
            sb.AppendLine("    <Color x:Key=\"Scrim\">{StaticResource Neutral0}</Color>");
            sb.AppendLine("    <Color x:Key=\"Shadow\">{StaticResource Neutral0}</Color>");
        }

        private static void AppendGradients(StringBuilder sb)
        {
            // Background Gradient (Dark Purple - Your Signature)
            sb.AppendLine("    <!-- App Background Gradient (Dark Purple) -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"AppBackgroundGradient\" StartPoint=\"0,0\" EndPoint=\"1,0\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Neutral10}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Primary10}\" Offset=\"1.0\" />");
            sb.AppendLine("    </LinearGradientBrush>");
            sb.AppendLine();

            // Card Gradient (Diagonal Purple)
            sb.AppendLine("    <!-- Card Background Gradient (Diagonal Purple) -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"CardBackgroundGradient\" StartPoint=\"0,0\" EndPoint=\"1,1\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Primary20}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Secondary30}\" Offset=\"1.0\" />");
            sb.AppendLine("    </LinearGradientBrush>");
            sb.AppendLine();

            // Selected Item Gradient (Pink to Purple)
            sb.AppendLine("    <!-- Selected Item Gradient (Pink to Purple) -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"SelectedGradient\" StartPoint=\"0,0\" EndPoint=\"1,0\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Primary30}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Secondary40}\" Offset=\"1.0\" />");
            sb.AppendLine("    </LinearGradientBrush>");
            sb.AppendLine();

            // Button Gradient (Bright Pink to Purple)
            sb.AppendLine("    <!-- Button Gradient (Bright Pink to Purple) -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"ButtonGradient\" StartPoint=\"0,0\" EndPoint=\"1,0\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Primary70}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Secondary70}\" Offset=\"1.0\" />");
            sb.AppendLine("    </LinearGradientBrush>");
            sb.AppendLine();

            // FAB Gradient (Gold to Pink - Your Signature)
            sb.AppendLine("    <!-- FAB Gradient (Gold to Pink) -->");
            sb.AppendLine("    <LinearGradientBrush x:Key=\"FabGradient\" StartPoint=\"0,0\" EndPoint=\"1,1\">");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Tertiary80}\" Offset=\"0.0\" />");
            sb.AppendLine("        <GradientStop Color=\"{StaticResource Primary70}\" Offset=\"1.0\" />");
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
            sb.AppendLine("    <Color x:Key=\"OnWarning\">#FFFFFF</Color>");
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
