# MD3 HCT Implementation - Testing & Verification Guide

## 🎯 Purpose

This guide provides comprehensive test cases to verify your HCT tonal palette implementation is correct.

---

## 🧪 TEST SUITE 1: Color Generation Validation

### Test 1.1: HCT Conversion Accuracy

**Purpose:** Verify seed colors convert to HCT and back correctly

**Test Code:**
```csharp
using MaterialColorUtilities.Hct;
using MaterialColorUtilities.Utils;

public void TestHCTConversion()
{
    // Test Primary Purple
    uint primarySeed = 0xFF7F41AC;
    var hct = Hct.FromInt(primarySeed);
    
    Console.WriteLine("Primary Purple #7F41AC:");
    Console.WriteLine($"  Hue: {hct.Hue:F1}°");
    Console.WriteLine($"  Chroma: {hct.Chroma:F1}");
    Console.WriteLine($"  Tone: {hct.Tone:F1}");
    
    // Roundtrip conversion
    uint reconstructed = hct.ToInt();
    string reconstructedHex = ColorUtils.HexFromArgb(reconstructed);
    
    Console.WriteLine($"  Reconstructed: {reconstructedHex}");
    Console.WriteLine($"  Match: {reconstructedHex == "#FF7F41AC"}");
}
```

**Expected Results:**
```
Primary Purple #7F41AC:
  Hue: ~285°
  Chroma: ~38-42
  Tone: ~43-47
  Reconstructed: #FF7F41AC (or very close)
  Match: True
```

**Pass Criteria:**
- [ ] Hue between 280-290°
- [ ] Chroma between 35-45
- [ ] Tone between 40-50
- [ ] Reconstructed hex matches original (±2 in RGB values acceptable)

---

### Test 1.2: Tonal Palette Generation

**Purpose:** Verify 13 tones generate correctly with proper progression

**Test Code:**
```csharp
using MaterialColorUtilities.Palettes;
using MaterialColorUtilities.Utils;

public void TestTonalPalette()
{
    uint primarySeed = 0xFF7F41AC;
    var palette = TonalPalette.FromInt(primarySeed);
    
    int[] tones = { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 95, 99, 100 };
    
    Console.WriteLine("Primary Tonal Palette:");
    foreach (var tone in tones)
    {
        uint argb = palette.Tone(tone);
        string hex = ColorUtils.HexFromArgb(argb);
        Console.WriteLine($"  Tone {tone,3}: {hex}");
    }
}
```

**Expected Results:**
```
Primary Tonal Palette:
  Tone   0: #FF000000 (black)
  Tone  10: #FF2E0048 (very dark purple)
  Tone  20: #FF4B0070 (dark purple)
  Tone  30: #FF680099 (medium-dark purple)
  Tone  40: #FF8500C3 (purple - close to #7F41AC)
  Tone  50: #FFA020E2 (bright purple)
  Tone  60: #FFB84FFF (light purple)
  Tone  70: #FFCD80FF (lighter purple)
  Tone  80: #FFE2B0FF (very light purple)
  Tone  90: #FFF1DFFF (almost white purple)
  Tone  95: #FFF9EFFF (barely purple white)
  Tone  99: #FFFFFBFF (near white)
  Tone 100: #FFFFFFFF (white)
```

**Pass Criteria:**
- [ ] Tone 0 is #FF000000 (black)
- [ ] Tone 100 is #FFFFFFFF (white)
- [ ] Tone 40 is close to seed color (~#7F41AC)
- [ ] Colors progressively lighten from 0 to 100
- [ ] Purple hue maintained across all tones
- [ ] No sudden jumps in lightness

---

### Test 1.3: Container Contrast Verification

**Purpose:** Verify container and onContainer combinations meet WCAG AA

**Test Code:**
```csharp
public void TestContainerContrast()
{
    var palette = TonalPalette.FromInt(0xFF7F41AC);
    
    // Light mode: Container=90, OnContainer=10
    uint container = palette.Tone(90);
    uint onContainer = palette.Tone(10);
    
    double contrast = CalculateContrast(container, onContainer);
    
    Console.WriteLine($"Container: {ColorUtils.HexFromArgb(container)}");
    Console.WriteLine($"OnContainer: {ColorUtils.HexFromArgb(onContainer)}");
    Console.WriteLine($"Contrast Ratio: {contrast:F2}:1");
    Console.WriteLine($"WCAG AA Pass (4.5:1): {contrast >= 4.5}");
}

// Helper method
double CalculateContrast(uint color1, uint color2)
{
    // Extract RGB
    double r1 = ((color1 >> 16) & 0xFF) / 255.0;
    double g1 = ((color1 >> 8) & 0xFF) / 255.0;
    double b1 = (color1 & 0xFF) / 255.0;
    
    double r2 = ((color2 >> 16) & 0xFF) / 255.0;
    double g2 = ((color2 >> 8) & 0xFF) / 255.0;
    double b2 = (color2 & 0xFF) / 255.0;
    
    // Calculate relative luminance
    double lum1 = GetRelativeLuminance(r1, g1, b1);
    double lum2 = GetRelativeLuminance(r2, g2, b2);
    
    // Contrast ratio
    double lighter = Math.Max(lum1, lum2);
    double darker = Math.Min(lum1, lum2);
    
    return (lighter + 0.05) / (darker + 0.05);
}

double GetRelativeLuminance(double r, double g, double b)
{
    r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
    g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
    b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);
    
    return 0.2126 * r + 0.7152 * g + 0.0722 * b;
}
```

**Expected Results:**
```
Container: #FFF1DFFF (light purple)
OnContainer: #FF2E0048 (dark purple)
Contrast Ratio: ~12-16:1
WCAG AA Pass (4.5:1): True
```

**Pass Criteria:**
- [ ] Contrast ratio ≥ 4.5:1 (WCAG AA)
- [ ] Preferably ≥ 7:1 (WCAG AAA)
- [ ] Container is visibly lighter than OnContainer

---

## 🧪 TEST SUITE 2: XAML Resource Validation

### Test 2.1: Resource Dictionary Structure

**Manual Inspection:** Open `MaterialColors.xaml` and verify:

**Tonal Palette Keys:**
- [ ] Primary0 through Primary100 (13 values)
- [ ] Secondary0 through Secondary100 (13 values)
- [ ] Tertiary0 through Tertiary100 (13 values)
- [ ] Error0 through Error100 (13 values)
- [ ] Neutral0 through Neutral100 (13 values)
- [ ] NeutralVariant0 through NeutralVariant100 (13 values)

**Total:** 78 tonal palette values

**Semantic Token Keys:**
- [ ] Primary, OnPrimary, PrimaryContainer, OnPrimaryContainer
- [ ] Secondary, OnSecondary, SecondaryContainer, OnSecondaryContainer
- [ ] Tertiary, OnTertiary, TertiaryContainer, OnTertiaryContainer
- [ ] Error, OnError, ErrorContainer, OnErrorContainer
- [ ] Background, OnBackground
- [ ] Surface, OnSurface, SurfaceVariant, OnSurfaceVariant
- [ ] SurfaceDim, SurfaceBright
- [ ] Outline, OutlineVariant
- [ ] InverseSurface, InverseOnSurface, InversePrimary
- [ ] Scrim, Shadow

**Gradients:**
- [ ] PrimaryGradient
- [ ] SecondaryGradient
- [ ] TertiaryGradient
- [ ] AppBackgroundGradient

**Semantic Colors:**
- [ ] Success, OnSuccess
- [ ] Warning, OnWarning
- [ ] Info, OnInfo

---

### Test 2.2: StaticResource References

**Purpose:** Verify semantic tokens reference tonal palettes correctly

**Check in XAML:**
```xml
<!-- ✅ CORRECT -->
<Color x:Key="Primary">{StaticResource Primary40}</Color>

<!-- ❌ WRONG -->
<Color x:Key="Primary">#FF7F41AC</Color>
```

**Verification Checklist:**
- [ ] All semantic tokens use `{StaticResource ...}` syntax
- [ ] No hardcoded hex values in semantic token definitions
- [ ] References point to existing tonal palette keys
- [ ] No typos in resource key names

---

### Test 2.3: Build-Time Resource Resolution

**Test Command:**
```bash
dotnet build -f net8.0-android -v detailed 2>&1 | grep -i "resource"
```

**Expected:** No resource resolution errors

**Pass Criteria:**
- [ ] Build succeeds
- [ ] No "StaticResource not found" errors
- [ ] No "Key not found in ResourceDictionary" warnings

---

## 🧪 TEST SUITE 3: Visual Regression Testing

### Test 3.1: Color Consistency Check

**Purpose:** Verify all purples use same shade throughout app

**Manual Test:**
1. Launch app
2. Navigate through all pages
3. Take screenshots of each page
4. Use eyedropper tool to sample purple colors

**Sample Points:**
- [ ] Navigation bar border
- [ ] Primary buttons
- [ ] Headers
- [ ] Icons in primary color
- [ ] Links (if any)

**Expected:** All samples should be #7F41AC (or tone variations from same palette)

**Pass Criteria:**
- [ ] All purples are from Primary tonal palette
- [ ] No random purple shades (#E91E63, #8B4CB8, etc.)
- [ ] Visual consistency across all pages

---

### Test 3.2: Navigation Bar Verification

**Purpose:** Verify nav bar uses new HCT colors

**Test:**
1. Launch app
2. Go to page with bottom navigation
3. Inspect visually

**Checkpoints:**
- [ ] Border color is purple #7F41AC (Primary40)
- [ ] Separator line is purple #7F41AC
- [ ] Button labels are gray (OnSurfaceVariant)
- [ ] Special button (Nova Fila) has orange gradient
- [ ] Plus symbol is white on orange background
- [ ] No gold (#FFD700) colors visible
- [ ] No old purple (#533682) visible

---

### Test 3.3: Contrast & Readability

**Purpose:** Ensure all text is readable with new colors

**Test Pages:**
- [ ] StackPage
- [ ] SpotPage
- [ ] SpotFormPage
- [ ] PersonPage
- [ ] TonguePage

**For Each Page:**
- [ ] All text is readable
- [ ] No color clashes
- [ ] Disabled states are distinguishable
- [ ] Error states are clearly visible
- [ ] Success feedback is noticeable

**Readability Scale:** 1-5 (5 = perfect)
- StackPage: ___
- SpotPage: ___
- SpotFormPage: ___
- PersonPage: ___
- TonguePage: ___

**Pass Criteria:** All pages rated ≥4

---

## 🧪 TEST SUITE 4: Functional Testing

### Test 4.1: Dynamic Resource Access

**Purpose:** Verify GetMD3Color() helper works correctly

**Test Scenario 1:** Resource Available
```csharp
// Should return Primary color from resources
var color = GetMD3Color("Primary", "#7F41AC");
// Expected: Color from MaterialColors.xaml Primary key
```

**Test Scenario 2:** Resource Not Found
```csharp
// Should return fallback
var color = GetMD3Color("NonExistentKey", "#FF0000");
// Expected: Color #FF0000 (fallback)
```

**Manual Test:**
1. Temporarily remove MaterialColors.xaml from App.xaml
2. Run app
3. Navigate to page with navigation bar

**Expected:** Nav bar displays with fallback purple (#7F41AC)

**Pass Criteria:**
- [ ] No crashes when resources unavailable
- [ ] Fallback colors display correctly
- [ ] App remains functional (even if colors wrong)

---

### Test 4.2: Theme Switching (Future)

**Purpose:** Verify foundation for dark mode is correct

**Inspection Check:**
In MaterialColors.xaml, verify dark mode tokens are commented out but structured correctly:

```xml
<!-- ✅ CORRECT STRUCTURE (commented) -->
<!--
<Color x:Key="PrimaryDark">{StaticResource Primary80}</Color>
<Color x:Key="OnPrimaryDark">{StaticResource Primary20}</Color>
-->
```

**Pass Criteria:**
- [ ] Dark mode tokens present (commented)
- [ ] Use correct tone mapping (Primary=80, OnPrimary=20, Container=30, OnContainer=90)
- [ ] All semantic tokens have dark equivalents
- [ ] Structure mirrors light mode exactly

---

## 🧪 TEST SUITE 5: Performance Testing

### Test 5.1: Resource Loading Time

**Purpose:** Verify MaterialColors.xaml doesn't slow down app startup

**Test:**
```csharp
var stopwatch = System.Diagnostics.Stopwatch.StartNew();
Application.Current.Resources.MergedDictionaries.Add(
    new ResourceDictionary 
    { 
        Source = new Uri("Resources/Styles/MaterialColors.xaml", UriKind.Relative) 
    }
);
stopwatch.Stop();
Console.WriteLine($"MaterialColors.xaml load time: {stopwatch.ElapsedMilliseconds}ms");
```

**Expected:** < 100ms

**Pass Criteria:**
- [ ] Load time < 100ms
- [ ] No noticeable delay in app startup
- [ ] Memory usage acceptable

---

### Test 5.2: Runtime Resource Lookup

**Purpose:** Verify {StaticResource} lookups don't cause performance issues

**Test:** Navigate rapidly between pages

**Pass Criteria:**
- [ ] No lag when navigating
- [ ] Smooth page transitions
- [ ] No frame drops
- [ ] CPU usage normal

---

## 🧪 TEST SUITE 6: Integration Testing

### Test 6.1: Complete User Flow

**Scenario:** Add a new singer to queue

**Steps:**
1. Launch app → StackPage
2. Tap "Nova Fila" button (special orange button)
3. Navigate to PersonPage
4. Enter singer name
5. Tap "Adicionar à Fila" (FilledButton)
6. Return to StackPage
7. Verify singer appears in queue

**Checkpoints:**
- [ ] All colors correct throughout flow
- [ ] Buttons respond to taps
- [ ] Visual feedback on interactions
- [ ] No crashes or exceptions
- [ ] Queue updates correctly

---

### Test 6.2: Form Validation Flow

**Scenario:** Validate error colors work correctly

**Steps:**
1. Go to SpotFormPage
2. Try to save without filling required fields
3. Observe error messages

**Checkpoints:**
- [ ] Error messages use Error color (red)
- [ ] Error color is clearly distinct from Primary (purple)
- [ ] Error color is visually urgent
- [ ] No confusion with warning or success colors

---

## 🧪 TEST SUITE 7: Edge Case Testing

### Test 7.1: Missing Resource Handling

**Test:** Comment out one semantic token in MaterialColors.xaml

**Steps:**
1. Backup MaterialColors.xaml
2. Comment out `<Color x:Key="Primary">`
3. Try to build

**Expected:** Build error referencing Primary key

**Restore:** Uncomment the line

**Pass Criteria:**
- [ ] Build fails with clear error message
- [ ] Error indicates which key is missing
- [ ] No silent failures

---

### Test 7.2: Circular Reference Check

**Test:** Verify no circular references in resource definitions

**Check:**
```xml
<!-- ❌ BAD: Circular reference -->
<Color x:Key="Primary">{StaticResource Primary}</Color>

<!-- ✅ GOOD: References different key -->
<Color x:Key="Primary">{StaticResource Primary40}</Color>
```

**Pass Criteria:**
- [ ] No circular references found
- [ ] All references point to different keys
- [ ] Resource graph is acyclic

---

## 📊 Test Results Summary

### Suite 1: Color Generation
- Test 1.1: HCT Conversion → [ ] Pass / [ ] Fail
- Test 1.2: Tonal Palette → [ ] Pass / [ ] Fail
- Test 1.3: Container Contrast → [ ] Pass / [ ] Fail

### Suite 2: XAML Resources
- Test 2.1: Structure → [ ] Pass / [ ] Fail
- Test 2.2: References → [ ] Pass / [ ] Fail
- Test 2.3: Build-Time → [ ] Pass / [ ] Fail

### Suite 3: Visual Regression
- Test 3.1: Color Consistency → [ ] Pass / [ ] Fail
- Test 3.2: Navigation Bar → [ ] Pass / [ ] Fail
- Test 3.3: Contrast → [ ] Pass / [ ] Fail

### Suite 4: Functional
- Test 4.1: Dynamic Resources → [ ] Pass / [ ] Fail
- Test 4.2: Theme Foundation → [ ] Pass / [ ] Fail

### Suite 5: Performance
- Test 5.1: Loading Time → [ ] Pass / [ ] Fail
- Test 5.2: Runtime Lookup → [ ] Pass / [ ] Fail

### Suite 6: Integration
- Test 6.1: User Flow → [ ] Pass / [ ] Fail
- Test 6.2: Validation → [ ] Pass / [ ] Fail

### Suite 7: Edge Cases
- Test 7.1: Missing Resource → [ ] Pass / [ ] Fail
- Test 7.2: Circular Reference → [ ] Pass / [ ] Fail

---

## ✅ Final Sign-Off

**All Tests Passed:** [ ] Yes / [ ] No

**Issues Found:** _______________________________________________

**Resolved:** [ ] Yes / [ ] No / [ ] N/A

**Ready for Production:** [ ] Yes / [ ] No

**Tested By:** _________________

**Date:** _________________

**Notes:**
_______________________________________________________________
_______________________________________________________________
_______________________________________________________________

