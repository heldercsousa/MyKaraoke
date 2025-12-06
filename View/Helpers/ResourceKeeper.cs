using System;
using System.Collections.Generic;

namespace MyVocaList.View.Helpers
{
    /// <summary>
    /// ✅ FIX: Explicitly references resources to prevent Linker/R8 from stripping them in Release mode.
    /// This is crucial for resources accessed via dynamic string interpolation.
    /// </summary>
    public static class ResourceKeeper
    {
        public static void Preserve()
        {
            // List of all dynamically loaded SVG icons
            var icons = new List<string>
            {
                // Nightlife (Venues)
                "nightlife_filled", "nightlife_outlined",
                
                // Music Note (Musicians)
                "music_note_filled", "music_note_outlined",
                
                // History
                "history_filled", "history_outlined",
                
                // Settings
                "settings_filled", "settings_outlined",
                
                // Common icons
                "account_circle_filled", "account_circle_outlined",
                "add_filled", "add_outlined",
                "arrow_back_filled", "arrow_back_outlined",
                "arrow_forward_filled", "arrow_forward_outlined",
                "bar_chart_filled", "bar_chart_outlined",
                "block_filled", "block_outlined",
                "cake_filled", "cake_outlined",
                "check_circle_filled", "check_circle_outlined",
                "check_filled", "check_outlined",
                "close_filled", "close_outlined",
                "cloud_sync_filled", "cloud_sync_outlined",
                "contrast_filled", "contrast_outlined",
                "delete_filled", "delete_outlined",
                "edit_filled", "edit_outlined",
                "event_filled", "event_outlined",
                "format_list_numbered_filled", "format_list_numbered_outlined",
                "graphic_eq_filled", "graphic_eq_outlined",
                "group_filled", "group_outlined",
                "language_filled", "language_outlined",
                "library_music_filled", "library_music_outlined",
                "logout_filled", "logout_outlined",
                "menu_filled", "menu_outlined",
                "mic_filled", "mic_outlined"
            };

            // Prevent optimization
            if (DateTime.Now.Ticks == 0)
            {
                Console.WriteLine(icons.Count);
            }
        }
    }
}
