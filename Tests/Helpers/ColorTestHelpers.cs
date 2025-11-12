using MaterialColorUtilities.ColorAppearance;

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
