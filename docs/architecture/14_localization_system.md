# Architectural Specification: Localization System

* **Status**: APPROVED
* **Date**: 2026-07-09
* **Engine Focus**: Unity 6 LTS
* **Library Selection**: **Unity Localization Package** (integrated with TextMeshPro)

---

## 1. Design Intent & Requirements Traceability

The Localization System translates gameplay text, subtitles, UI elements, and voiceover scripts. It directly implements our core accessibility and cultural standards:

* **No Machine Translation (Vision §8 & GDD §15.5)**: We reject basic machine-translated text. Localization must adapt cultural references, idioms, and voiceover tones to local standards (e.g., Tidewell Cove becomes *La Bahía de Marea* in Spanish, not a literal translation).
* **Mathematics Notation Calibration (GDD §1.4 & §2.4.1 & Ch. 10)**: Decimal points and fractions are written differently across target countries (e.g., `1.2` in the US/UK vs `1,2` in France/Germany). The localization system must format mathematical representations to match the local school curriculum.
* **Dyslexia Font Accessibility (Vision §8 & GDD §15.2)**: Tapping the accessibility panel must instantly swap all screen fonts to a dyslexia-optimized typeface (e.g., **OpenDyslexic**), adjusting line/character spacing dynamically to prevent visual overlap.

---

## 2. Localization Table Schema

QuestBit uses structured JSON localization tables. Keys are formatted hierarchically:
`[biome/system]_[context]_[sub_context]_[character/element]`

### 2.1 JSON Schema Structure (`local_table_en.json`)

```json
{
  "locale": "en-US",
  "data": {
    "cove_quest_washedshore_mara_01": "Welcome to Tidewell Cove, Wayfinder! The tide washed away our docks.",
    "cove_gameplay_tideglass_empty": "The gap is empty. Try placing a plank.",
    "cove_gameplay_fraction_plank_label": "{numerator}/{denominator}",
    "cove_quest_ferro_net_rope_compare": "This rope is too long. It looks like it's {fraction} of a meter long.",
    "global_ui_dyslexia_toggle": "Dyslexia-Optimized Font",
    "global_ui_calm_toggle": "Calm Visuals Mode"
  }
}
```

### 2.2 Regional Formatting & Decimal Calibration

The C# formatting pipeline parses localized numbers dynamically using `IFormatProvider` and current culture configurations.

```csharp
using System;
using System.Globalization;

namespace QuestBit.Systems.Localization
{
    public static class LocaleFormatter
    {
        /// <summary>
        /// Formats fractions or decimals to match local curriculum standards.
        /// US/UK: "1.5 meters" | France/Germany: "1,5 mètres"
        /// </summary>
        public static string FormatMeasurement(float value, CultureInfo cultureInfo)
        {
            // Standardizes on local decimal symbols
            return value.ToString("N1", cultureInfo); 
        }

        /// <summary>
        /// Formats fraction representation based on locale rules.
        /// </summary>
        public static string FormatFractionSymbols(int numerator, int denominator, string locale)
        {
            if (locale.StartsWith("ar")) // Arabic right-to-left layout adaptation
            {
                return $"{denominator}/{numerator}";
            }
            return $"{numerator}/{denominator}";
        }
    }
}
```

---

## 3. Localization System Implementation & Dynamic Font Swapper

The localization framework handles key matching, fallback overrides, and dynamic font family loading.

### 3.1 Interface Contracts

```csharp
using TMPro;
using Cysharp.Threading.Tasks;

namespace QuestBit.Systems.Localization
{
    public interface ILocalizationSystem
    {
        string CurrentLocale { get; }
        UniTask SetLocaleAsync(string localeCode);
        
        string GetString(string key);
        string GetString(string key, params object[] args);
        
        // Font accessibility controls
        TMP_FontAsset GetActiveFont(bool isDyslexicEnabled);
        void RegisterTextComponent(LocalizedTextComponent component);
        void UnregisterTextComponent(LocalizedTextComponent component);
    }
}
```

### 3.2 Dynamic Text Component (MonoBehaviour)

Every TextMeshPro component in our UI Canvas registers with this script to handle font swops and translation lookups.

```csharp
using UnityEngine;
using TMPro;
using VContainer;

namespace QuestBit.Systems.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextComponent : MonoBehaviour
    {
        [SerializeField] private string _localizationKey = null!;
        
        private TextMeshProUGUI _tmpText = null!;
        private ILocalizationSystem _localizationSystem = null!;

        [Inject]
        public void Construct(ILocalizationSystem localizationSystem)
        {
            _localizationSystem = localizationSystem;
        }

        private void Awake()
        {
            _tmpText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            _localizationSystem.RegisterTextComponent(this);
            UpdateText();
        }

        private void OnDisable()
        {
            _localizationSystem.UnregisterTextComponent(this);
        }

        public void UpdateText()
        {
            if (string.IsNullOrEmpty(_localizationKey)) return;

            // 1. Fetch translated string from active table
            _tmpText.text = _localizationSystem.GetString(_localizationKey);

            // 2. Query system to see if dyslexia overrides are active
            bool useDyslexic = PlayerPrefs.GetInt("Accessibility_DyslexicFont", 0) == 1;
            _tmpText.font = _localizationSystem.GetActiveFont(useDyslexic);

            // 3. Adjust spacing parameters to suit OpenDyslexic letter layouts
            if (useDyslexic)
            {
                _tmpText.lineSpacing = 25f; // Extra line spacing
                _tmpText.characterSpacing = 10f; // Extra letter spacing to prevent crowded visual read
            }
            else
            {
                _tmpText.lineSpacing = 0f; // Reset to default font settings
                _tmpText.characterSpacing = 0f;
            }
        }
    }
}
```

---

## 4. UI Reflow Budgets & Text Bounds

* **Text Size Scaling Limit**: Accessibility settings allow scaling text up to **200%** (GDD §15.2).
* **Text Truncation Guard**: UI developers must never set static bounds (pixel-fixed height/width) on text panels. Text panels must utilize the TextMeshPro **Auto-Sizing** boundaries and have Canvas layout constraints set to **Preferred Size** auto-expansion.
* **Layout Reflow Buffer**: UI layout groups (horizontal/vertical layout layers) must preserve a **30% size buffer** to handle the visual expansion of translated words (e.g. English "Play" -> German "Spielen", or standard font -> OpenDyslexic font footprint).

---

## 5. Failure Modes & Edge Cases

### 1. Missing Localization Keys
* **Symptom**: Dialogue boxes show blank spaces or display raw keys (e.g., `_cove_quest_missing_key`).
* **Mitigation**: Implement a **Fallback Locale Chain**. If `GetString(key)` fails to locate the key in the active locale (e.g., `fr-FR`), it attempts to resolve the key in our baseline fallback locale (`en-US`). If that also fails, it returns the raw key wrapped in brackets (e.g., `[KEY_NOT_FOUND: cove_quest_missing_key]`) to allow QA to flag the error, rather than crashing the text renderer.

### 2. RTL (Right-to-Left) Layout Clipping
* **Symptom**: Arabic text displays backward or letter connections break, clipping outside the UI panels.
* **Mitigation**: Integrate the TextMeshPro **RTL Creator** plug-in. When switching to an RTL locale (e.g., `ar-SA`), the UI manager dynamically flips the horizontal alignment anchors of all text panels.

### 3. Font Atlas Memory Overflow
* **Symptom**: WebGL build crashes due to memory overflow when switching languages.
* **Cause**: Pre-loading large dynamic font atlases for multiple non-latin languages (e.g., Chinese, Japanese, Korean, Arabic) simultaneously.
* **Mitigation**: Store regional font atlases in isolated Addressable groups. Download and load the CJK or Arabic font asset only when the user selects that locale, unloading the previous font atlas from RAM.

---

## 6. Verification & Automated QA

1. **RTL and Layout Verification**:
   Build an Editor integration test that cycles all active Canvas panels through French, German, Arabic, and Japanese.
   * *Pass Criteria*: Text must not clip outside the viewport boundaries, and no TextMeshPro container may trigger truncation (`...` ellipses) in core instructional screens.

2. **Dyslexia Toggle Assertions**:
   Verify that changing the global accessibility flag triggers `UpdateText()` on all registered `LocalizedTextComponent` instances within **1 frame**, swapping the active font asset to the target `OpenDyslexic.asset`.
