# MyVocaList Iconography Guideline

This document outlines the standard icons to be used for various sections and functions within the application. Adhering to this guide ensures a consistent, intuitive, and professional user experience, aligned with Material Design 3 principles.

---

##  Icon Reference Table

| Function / Section | Recommended Icon | Material Symbol Name | Rationale / Use Case |
| :--- | :---: | :--- | :--- |
| **Musicians/Bands** | 👥 | `group` | Represents a collection of people—the band or artist. |
| **Musics Catalog** | 🎶 | `library_music` | A collection or repertoire of songs associated with an artist. |
| **Venues (Karaoke)** | 🍸 | `nightlife` | Captures the social, bar, and event atmosphere of a karaoke venue. |
| **Events** | 📅 | `event` | The universal symbol for a scheduled event or a specific date/occasion. |
| **Singers** | 🎤 | `mic` | Defines the participant by their primary tool and action: singing. |
| **Event Queue** | 🔢 | `format_list_numbered` | Represents a numbered, ordered list of singers for the event. |
| **Menu** | ☰ | `menu` | For opening a side navigation menu or drawer. |
| **Theme** | 🌓 | `contrast` | For switching between light/dark application themes. |
| **Language** | 🌐 | `language` | For opening language selection options. |
| **Backup/Restore** | ☁️ | `cloud_sync` | For managing cloud data synchronization, backup, and restore. |
| **Quit App** | 🚪 | `logout` | For exiting the application or logging out. |

---

## Implementation Rules

* **Source:** All icons should be sourced from the **Google Material Symbols** library to maintain a consistent visual style.
* **Style:**
    * Use the **Outlined** style for inactive/unselected states and for decorative purposes (e.g., in empty state cards).
    * Use the **Filled** style to indicate active/selected states (e.g., the current tab in a navigation bar).
* **Format:** Icons should be used as **SVG** files. This ensures they are scalable and can be dynamically colored.
* **Coloring:** Icon colors must be applied dynamically in the application using theme resources (e.g., `{StaticResource Primary}`). Colors should **never** be hard-coded into the SVG file itself.