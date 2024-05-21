using System;
using System.Collections.Generic;

namespace Lab_21
{
    public enum Language
    {
        English,
        Ukrainian
    }

    public static class LocalizationManager
    {
        public static Language CurrentLanguage { get; set; } = Language.English;

        private static Dictionary<string, Dictionary<Language, string>> translations = new Dictionary<string, Dictionary<Language, string>>
        {
            ["File"] = new Dictionary<Language, string>
            {
                [Language.English] = "File",
                [Language.Ukrainian] = "Файл"
            },
            ["New"] = new Dictionary<Language, string>
            {
                [Language.English] = "New",
                [Language.Ukrainian] = "Новий"
            },
            ["Open"] = new Dictionary<Language, string>
            {
                [Language.English] = "Open",
                [Language.Ukrainian] = "Відкрити"
            },
            ["Save"] = new Dictionary<Language, string>
            {
                [Language.English] = "Save",
                [Language.Ukrainian] = "Зберегти"
            },
            ["Save As"] = new Dictionary<Language, string>
            {
                [Language.English] = "Save As",
                [Language.Ukrainian] = "Зберегти як"
            },
            ["Settings"] = new Dictionary<Language, string>
            {
                [Language.English] = "Settings",
                [Language.Ukrainian] = "Налаштування"
            },
            ["Exit"] = new Dictionary<Language, string>
            {
                [Language.English] = "Exit",
                [Language.Ukrainian] = "Вихід"
            },
            ["Format"] = new Dictionary<Language, string>
            {
                [Language.English] = "Format",
                [Language.Ukrainian] = "Формат"
            },
            ["Font"] = new Dictionary<Language, string>
            {
                [Language.English] = "Font",
                [Language.Ukrainian] = "Шрифт"
            },
            ["Color"] = new Dictionary<Language, string>
            {
                [Language.English] = "Color",
                [Language.Ukrainian] = "Колір"
            },
            ["Help"] = new Dictionary<Language, string>
            {
                [Language.English] = "Help",
                [Language.Ukrainian] = "Допомога"
            },
            ["About"] = new Dictionary<Language, string>
            {
                [Language.English] = "About",
                [Language.Ukrainian] = "Про програму"
            },
            ["Language"] = new Dictionary<Language, string>
            {
                [Language.English] = "Language",
                [Language.Ukrainian] = "Мова"
            },
            ["Cut"] = new Dictionary<Language, string>
            {
                [Language.English] = "Cut",
                [Language.Ukrainian] = "Вирізати"
            },
            ["Copy"] = new Dictionary<Language, string>
            {
                [Language.English] = "Copy",
                [Language.Ukrainian] = "Копіювати"
            },
            ["Paste"] = new Dictionary<Language, string>
            {
                [Language.English] = "Paste",
                [Language.Ukrainian] = "Вставити"
            },
            ["Align Left"] = new Dictionary<Language, string>
            {
                [Language.English] = "Align Left",
                [Language.Ukrainian] = "Вирівняти ліворуч"
            },
            ["Align Center"] = new Dictionary<Language, string>
            {
                [Language.English] = "Align Center",
                [Language.Ukrainian] = "Вирівняти по центру"
            },
            ["Align Right"] = new Dictionary<Language, string>
            {
                [Language.English] = "Align Right",
                [Language.Ukrainian] = "Вирівняти праворуч"
            },
            ["Justify"] = new Dictionary<Language, string>
            {
                [Language.English] = "Justify",
                [Language.Ukrainian] = "Вирівняти"
            }
        };

        public static string T(string key)
        {
            if (translations.TryGetValue(key, out var langDict) && langDict.TryGetValue(CurrentLanguage, out var value))
            {
                return value;
            }
            return key;
        }
    }
}