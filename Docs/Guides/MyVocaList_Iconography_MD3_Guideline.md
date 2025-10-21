# Final Application Iconography Guideline

This document provides the official, validated SVG icons for the application. Each icon has a direct download link to the official Google Material Design Icons repository to ensure correct rendering and a consistent user experience.

## 📥 Icon Download Information

All icons are sourced from Google's official Material Design Icons repository:
**Repository:** https://github.com/google/material-design-icons

**URL Pattern for downloading icons:**
- **Outlined variant:** `https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/{icon_name}/materialsymbolsoutlined/{icon_name}_24px.svg`
- **Filled variant:** `https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/{icon_name}/materialsymbolsoutlined/{icon_name}_fill1_24px.svg`

**Important:** All icons use `viewBox="0 -960 960 960"` format for proper rendering in .NET MAUI.

---

## 🚀 IMPLEMENTATION GUIDE - READ THIS FIRST!

### ⚡ How to Use Icons in Code

**RULE #1: ALWAYS use StatefulIcon component**
```xml
<components:StatefulIcon IconName="nightlife" />

<Image Source="nightlife_outlined.svg" />
```

**RULE #2: Pass ONLY IconName property**
```xml
<components:StatefulIcon IconName="arrow_back" />

<components:StatefulIcon IconName="arrow_back"
                        WidthRequest="24"
                        HeightRequest="24"
                        ActiveColor="#E91E63" />
```

**RULE #3: Icon names are WITHOUT suffix**
```xml
IconName="nightlife"

IconName="nightlife_outlined"
IconName="nightlife_filled.svg"
```

### 📋 Common Implementation Patterns

**Pattern 1: HeaderComponent with icon**
```xml
<components:HeaderComponent
    Title="Venues"
    IconName="nightlife" />
```

**Pattern 2: Bottom navigation with icons**
```csharp
new NavButtonConfig
{
    Text = "Venues",
    IconName = "nightlife",  // StatefulIcon auto-configured
    Command = new Command(() => OnVenuesClicked())
}
```

**Pattern 3: Standalone icon in UI**
```xml
<components:StatefulIcon IconName="check_circle" IsSelected="True" />
```

### 🎯 Icon Selection Process

**When adding a new icon to your code:**

1. **Find the feature** in this document (Core Functions, User Actions, etc.)
2. **Read the rationale** to ensure it matches your use case
3. **Copy the icon name** (e.g., `nightlife`, `arrow_back`)
4. **Use in StatefulIcon** with only `IconName` property
5. **Let the component handle** sizing, colors, variants automatically

### ⚠️ Common Mistakes to Avoid

❌ Setting manual sizes (component auto-sizes based on context)
❌ Using Image instead of StatefulIcon
❌ Including file extensions in IconName
❌ Creating new icons without consulting this guide first
❌ Using PNG icons when SVG equivalent exists

---

## 🎨 Button Icon Emphasis Patterns (MD3 Guidelines)

### Primary Action Button Emphasis

**The Problem:**
Some icons (like `check`) have visually identical outlined and filled variants - both look the same to users. This means using `IsSelected="True"` to switch between variants provides NO visual distinction.

**MD3-Compliant Solution:**
According to Material Design 3 guidelines, **emphasis comes from the button container, NOT the icon variant**.

### ✅ Correct Pattern: Filled Background Button

```xml
<!-- CORRECT: Primary action with filled background -->
<Frame BackgroundColor="{StaticResource Primary}"
       CornerRadius="20"
       Padding="16,8">
    <Frame.GestureRecognizers>
        <TapGestureRecognizer Tapped="OnSaveClicked" />
    </Frame.GestureRecognizers>

    <!-- Standard 24dp icon on contrasting background -->
    <components:StatefulIcon IconName="check"
                            InactiveColor="{StaticResource OnPrimary}" />
</Frame>
```

**Visual Result:**
- Pink filled background (#E91E63 - Primary)
- White checkmark icon (24dp standard size)
- High emphasis through **container color**, not icon variant

### ❌ Incorrect Approaches

**Don't use different icon sizes for emphasis:**
```xml
<!-- ❌ WRONG: Increasing icon size is not documented in MD3 -->
<StatefulIcon IconName="check" Size="Large" />
```

**Don't rely on filled/outlined distinction when icons look identical:**
```xml
<!-- ❌ WRONG: No visual difference for checkmark icon -->
<StatefulIcon IconName="check" IsSelected="True" />  <!-- Filled -->
<StatefulIcon IconName="check" IsSelected="False" /> <!-- Outlined -->
<!-- Both look exactly the same to users! -->
```

### 📊 MD3 Button Emphasis Hierarchy

| Button Type | Emphasis Level | Background | Icon Color | Use Case |
|-------------|---------------|------------|------------|----------|
| **Filled** | Highest | Primary color | OnPrimary | Save, Confirm, Submit |
| **Filled Tonal** | High | SecondaryContainer | OnSecondary | Next, Continue |
| **Outlined** | Medium | Transparent | OnSurface | Edit, Options |
| **Text** | Low | Transparent | OnSurface | Cancel, Dismiss |

### 💡 When to Use Filled vs Outlined Icons

**Use FILLED icon variant when:**
- Icon has meaningful fill area (e.g., `delete`, `favorite`, `star`)
- Selected/active state needs visual distinction
- Icon shape changes when filled (e.g., trash can outline → solid trash)

**Use OUTLINED icon variant when:**
- Icon looks identical when filled (e.g., `check`, `close`, `add`)
- Icon is on colored background (better contrast)
- Default/neutral state (most common use case)

### 🔍 Icon Shape Examples

**Icons with meaningful filled variant:**
- `delete` - Outline trash can → Filled trash can ✅
- `favorite` - Outline heart → Solid heart ✅
- `star` - Outline star → Solid star ✅

**Icons where filled makes NO difference:**
- `check` - Checkmark outline → Checkmark outline ❌ (looks the same!)
- `close` - X outline → X outline ❌ (looks the same!)
- `add` - Plus outline → Plus outline ❌ (looks the same!)

---

## 🎵 Core App Functions

### Musicians/Bands
- **Symbol Name:** `music_note`
- **Rationale:** The most direct symbol for music. Defines the artists by their primary purpose: creating music.
- [**Download `music_note_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/music_note/materialsymbolsoutlined/music_note_24px.svg)
- [**Download `music_note_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/music_note/materialsymbolsoutlined/music_note_fill1_24px.svg)

### Musics Catalog
- **Symbol Name:** `library_music`
- **Rationale:** A collection or repertoire of songs.
- [**Download `library_music_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/library_music/materialsymbolsoutlined/library_music_24px.svg)
- [**Download `library_music_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/library_music/materialsymbolsoutlined/library_music_fill1_24px.svg)

### Venues (Karaoke)
- **Symbol Name:** `nightlife`
- **Rationale:** Captures the social, bar, and event atmosphere of a karaoke venue.
- [**Download `nightlife_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/nightlife/materialsymbolsoutlined/nightlife_24px.svg)
- [**Download `nightlife_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/nightlife/materialsymbolsoutlined/nightlife_fill1_24px.svg)

### Events
- **Symbol Name:** `event`
- **Rationale:** Universal symbol for a scheduled event or a specific date/occasion.
- [**Download `event_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/event/materialsymbolsoutlined/event_24px.svg)
- [**Download `event_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/event/materialsymbolsoutlined/event_fill1_24px.svg)

### Singers
- **Symbol Name:** `mic`
- **Rationale:** Defines the participant by their primary tool and action: singing.
- [**Download `mic_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/mic/materialsymbolsoutlined/mic_24px.svg)
- [**Download `mic_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/mic/materialsymbolsoutlined/mic_fill1_24px.svg)

### Event Queue
- **Symbol Name:** `format_list_numbered`
- **Rationale:** Represents a numbered, ordered list of singers for the event.
- [**Download `format_list_numbered_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/format_list_numbered/materialsymbolsoutlined/format_list_numbered_24px.svg)
- [**Download `format_list_numbered_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/format_list_numbered/materialsymbolsoutlined/format_list_numbered_fill1_24px.svg)

---

## 🎤 Singer Status in Queue

### Currently Singing
- **Symbol Name:** `graphic_eq`
- **Rationale:** The equalizer icon clearly indicates active music or sound.
- [**Download `graphic_eq_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/graphic_eq/materialsymbolsoutlined/graphic_eq_24px.svg)
- [**Download `graphic_eq_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/graphic_eq/materialsymbolsoutlined/graphic_eq_fill1_24px.svg)

### Sung (Turn Completed)
- **Symbol Name:** `check_circle`
- **Rationale:** A checkmark inside a circle is a universally recognized symbol for a completed task.
- [**Download `check_circle_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/check_circle/materialsymbolsoutlined/check_circle_24px.svg)
- [**Download `check_circle_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/check_circle/materialsymbolsoutlined/check_circle_fill1_24px.svg)

### Declined / Skipped Turn
- **Symbol Name:** `block`
- **Rationale:** A clear and unambiguous way to show that a singer declined their turn.
- [**Download `block_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/block/materialsymbolsoutlined/block_24px.svg)
- [**Download `block_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/block/materialsymbolsoutlined/block_fill1_24px.svg)

---

## 👥 User & CRUD Actions

### Person / Profile (Default)
- **Symbol Name:** `account_circle`
- **Rationale:** Standard icon for a user profile when no photo is available.
- [**Download `account_circle_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/account_circle/materialsymbolsoutlined/account_circle_24px.svg)
- [**Download `account_circle_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/account_circle/materialsymbolsoutlined/account_circle_fill1_24px.svg)

### Save / Confirm
- **Symbol Name:** `check`
- **Rationale:** Universal symbol for confirmation, completion, and saving.
- [**Download `check_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/check/materialsymbolsoutlined/check_24px.svg)
- [**Download `check_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/check/materialsymbolsoutlined/check_fill1_24px.svg)

### Cancel
- **Symbol Name:** `close`
- **Rationale:** Standard for dismissing or canceling an action.
- [**Download `close_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/close/materialsymbolsoutlined/close_24px.svg)
- [**Download `close_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/close/materialsymbolsoutlined/close_fill1_24px.svg)

### Add New
- **Symbol Name:** `add`
- **Rationale:** Simple plus sign for adding a new item.
- [**Download `add_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/add/materialsymbolsoutlined/add_24px.svg)
- [**Download `add_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/add/materialsymbolsoutlined/add_fill1_24px.svg)

### Delete
- **Symbol Name:** `delete`
- **Rationale:** The trash can is the intuitive icon for a destructive delete action.
- [**Download `delete_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/delete/materialsymbolsoutlined/delete_24px.svg)
- [**Download `delete_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/delete/materialsymbolsoutlined/delete_fill1_24px.svg)

---

## ⚙️ App Management & Miscellaneous

### Menu
- **Symbol Name:** `menu`
- **Rationale:** For opening a side navigation menu or drawer.
- [**Download `menu_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/menu/materialsymbolsoutlined/menu_24px.svg)
- [**Download `menu_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/menu/materialsymbolsoutlined/menu_fill1_24px.svg)

### Navigate Back
- **Symbol Name:** `arrow_back`
- **Rationale:** Standard for returning to the previous screen.
- [**Download `arrow_back_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/arrow_back/materialsymbolsoutlined/arrow_back_24px.svg)
- [**Download `arrow_back_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/arrow_back/materialsymbolsoutlined/arrow_back_fill1_24px.svg)

### Navigate Forward
- **Symbol Name:** `arrow_forward`
- **Rationale:** Standard for moving to the next step or screen.
- [**Download `arrow_forward_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/arrow_forward/materialsymbolsoutlined/arrow_forward_24px.svg)
- [**Download `arrow_forward_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/arrow_forward/materialsymbolsoutlined/arrow_forward_fill1_24px.svg)

### Config / Setup
- **Symbol Name:** `settings`
- **Rationale:** Universal icon for accessing application settings.
- [**Download `settings_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/settings/materialsymbolsoutlined/settings_24px.svg)
- [**Download `settings_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/settings/materialsymbolsoutlined/settings_fill1_24px.svg)

### Backup/Restore
- **Symbol Name:** `cloud_sync`
- **Rationale:** For managing cloud data synchronization, backup, and restore.
- [**Download `cloud_sync_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/cloud_sync/materialsymbolsoutlined/cloud_sync_24px.svg)
- [**Download `cloud_sync_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/cloud_sync/materialsymbolsoutlined/cloud_sync_fill1_24px.svg)

### Theme
- **Symbol Name:** `contrast`
- **Rationale:** For switching between light/dark application themes.
- [**Download `contrast_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/contrast/materialsymbolsoutlined/contrast_24px.svg)
- [**Download `contrast_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/contrast/materialsymbolsoutlined/contrast_fill1_24px.svg)

### Language
- **Symbol Name:** `language`
- **Rationale:** For opening language selection options.
- [**Download `language_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/language/materialsymbolsoutlined/language_24px.svg)
- [**Download `language_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/language/materialsymbolsoutlined/language_fill1_24px.svg)

### History
- **Symbol Name:** `history`
- **Rationale:** Excellent for showing a chronological record or audit trail.
- [**Download `history_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/history/materialsymbolsoutlined/history_24px.svg)
- [**Download `history_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/history/materialsymbolsoutlined/history_fill1_24px.svg)

### Reports
- **Symbol Name:** `bar_chart`
- **Rationale:** A bar chart is the most common icon for data analysis and reports.
- [**Download `bar_chart_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/bar_chart/materialsymbolsoutlined/bar_chart_24px.svg)
- [**Download `bar_chart_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/bar_chart/materialsymbolsoutlined/bar_chart_fill1_24px.svg)

### Birthday
- **Symbol Name:** `cake`
- **Rationale:** A friendly and clear way to represent a birthday.
- [**Download `cake_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/cake/materialsymbolsoutlined/cake_24px.svg)
- [**Download `cake_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/cake/materialsymbolsoutlined/cake_fill1_24px.svg)

### Quit App
- **Symbol Name:** `logout`
- **Rationale:** For exiting the application or logging out.
- [**Download `logout_outlined.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/logout/materialsymbolsoutlined/logout_24px.svg)
- [**Download `logout_filled.svg`**](https://raw.githubusercontent.com/google/material-design-icons/master/symbols/web/logout/materialsymbolsoutlined/logout_fill1_24px.svg)

---

## 📝 Form Field Typography & Spacing (MD3 Guidelines)

### Text Field Label Specifications

**The Problem:**
Form labels that are too small or have incorrect spacing create poor usability - users struggle to read them and associate them with their corresponding input fields.

**MD3 Solution:**
Material Design 3 defines specific typography scales and spacing for text field components.

### ✅ Correct Pattern: MD3 Text Field Labels

```xml
<!-- CORRECT: Field label with MD3 Label Large typography -->
<Label Text="Venue name"
       Style="{StaticResource FieldLabel}" />

<Entry Placeholder="Enter venue name"
       Style="{StaticResource MaterialEntry}" />

<!-- Error message with MD3 Body Small typography -->
<Label Text="Venue name is required"
       Style="{StaticResource ErrorText}"
       IsVisible="{Binding HasError}" />
```

### 📊 MD3 Typography Specifications for Forms

| Element | MD3 Type Scale | Font Size | Weight | Spacing |
|---------|---------------|-----------|--------|---------|
| **Field Label** | Label Large | 14px | Medium (Bold) | 8dp bottom margin |
| **Input Text** | Body Large | 16px | Regular | - |
| **Helper Text** | Body Small | 12px | Regular | 4dp top margin |
| **Error Text** | Body Small | 12px | Medium (Bold) | 4dp top margin |
| **Placeholder** | Body Large | 16px | Regular (italic) | - |

### 🎨 MaterialStyles.xaml Implementation

```xml
<!-- Field Label (Above input fields) - MD3 Label Large -->
<Style x:Key="FieldLabel" TargetType="Label">
    <Setter Property="FontSize" Value="14" />
    <Setter Property="TextColor" Value="{StaticResource OnSurfaceVariant}" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="Margin" Value="0,0,0,8" />
</Style>

<!-- Helper Text (Below input fields) - MD3 Body Small -->
<Style x:Key="HelperText" TargetType="Label">
    <Setter Property="FontSize" Value="12" />
    <Setter Property="TextColor" Value="{StaticResource OnSurfaceVariant}" />
    <Setter Property="Margin" Value="0,4,0,0" />
</Style>

<!-- Error Text (Validation messages) - MD3 Body Small -->
<Style x:Key="ErrorText" TargetType="Label">
    <Setter Property="FontSize" Value="12" />
    <Setter Property="TextColor" Value="{StaticResource Error}" />
    <Setter Property="FontAttributes" Value="Bold" />
    <Setter Property="Margin" Value="0,4,0,0" />
</Style>
```

### ❌ Common Mistakes

**Don't use too small labels:**
```xml
<!-- ❌ WRONG: 12px is too small for field labels -->
<Label Text="Venue name" FontSize="12" />
```

**Don't use incorrect spacing:**
```xml
<!-- ❌ WRONG: No spacing creates visual crowding -->
<Label Text="Venue name" Margin="0" />
<Entry Placeholder="Enter name" />
```

**Don't use inconsistent typography:**
```xml
<!-- ❌ WRONG: Hardcoded sizes instead of MD3 scales -->
<Label Text="Error" FontSize="11" TextColor="#FF0000" />
```

### 💡 Spacing Guidelines (MD3 Standard)

**Vertical spacing for form elements:**
- Label → Input: **8dp** (provides clear visual connection)
- Input → Helper/Error: **4dp** (tight relationship)
- Field Group → Field Group: **16-24dp** (visual separation)

**Why these specific values?**
- **8dp** = MD3 base spacing unit, provides clear association
- **4dp** = Half unit, for closely related content
- **16-24dp** = Full/double unit, for distinct groups

### 🔍 Visual Hierarchy

Form elements should create clear visual hierarchy:

1. **Input field** (largest, Body Large 16px) - Primary interaction
2. **Field label** (medium, Label Large 14px) - Clear identification  
3. **Helper/Error text** (smallest, Body Small 12px) - Supporting info

**Font weight emphasis:**
- Field labels: **Bold** (draws attention to what field is for)
- Error text: **Bold** (critical information needs emphasis)
- Helper text: **Regular** (non-critical supporting info)

### 📱 Implementation Example

```xml
<VerticalStackLayout Spacing="24">
    <!-- Field Group 1 -->
    <VerticalStackLayout Spacing="0">
        <Label Text="Venue name"
               Style="{StaticResource FieldLabel}" />
        
        <Entry x:Name="venueNameEntry"
               Placeholder="Enter venue name"
               Style="{StaticResource MaterialEntry}"
               MaxLength="30" />
        
        <Label Text="Venue name is required"
               Style="{StaticResource ErrorText}"
               IsVisible="{Binding HasError}" />
    </VerticalStackLayout>

    <!-- Field Group 2 -->
    <VerticalStackLayout Spacing="0">
        <Label Text="Email"
               Style="{StaticResource FieldLabel}" />
        
        <Entry Placeholder="youremail@example.com"
               Style="{StaticResource MaterialEntry}"
               Keyboard="Email" />
        
        <Label Text="We'll never share your email"
               Style="{StaticResource HelperText}" />
    </VerticalStackLayout>
</VerticalStackLayout>
```

### 🎯 Key Takeaways

1. **Always use MD3 type scales** - Don't invent custom font sizes
2. **Consistent spacing** - 8dp for labels, 4dp for helper/error text
3. **Visual hierarchy** - Input > Label > Helper/Error
4. **Bold for emphasis** - Labels and errors need to stand out
5. **Use theme colors** - OnSurfaceVariant for labels, Error for validation

---

**Last Updated:** $(date +%Y-%m-%d)  
**Version:** 2.3  
**Related Guides:** MyVocaList_migration_material_design_guide.md
