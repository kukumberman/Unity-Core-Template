using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Managers
{
    public interface ILocalizationContainer
    {
        string PreferredLanguage { get; set; }
        void Save();
    }

    public sealed class LocalizationManager
    {
        public event Action OnLanguageChanged;

        private ILocalizationContainer _container;

        private Dictionary<string, Dictionary<string, string>> _languagesMap = new();

        public string CurrentLanguage =>
            _container != null ? _container.PreferredLanguage : "undefined";

        public List<string> SupportedLanguages => _languagesMap.Keys.ToList();

        public LocalizationManager(ILocalizationContainer container)
        {
            _container = container;
        }

        public LocalizationManager CreateFromJson(string json)
        {
            _languagesMap = JsonConvert.DeserializeObject<
                Dictionary<string, Dictionary<string, string>>
            >(json);

            PreValidateMap();

            SetInitialLanguage();

            return this;
        }

        public bool TrySetLanguage(string language)
        {
            if (_container == null)
            {
                return false;
            }

            if (_container.PreferredLanguage == language)
            {
                return false;
            }

            if (_languagesMap.ContainsKey(language))
            {
                _container.PreferredLanguage = language;
                _container.Save();

                OnLanguageChanged.SafeInvoke();

                return true;
            }

            return false;
        }

        public string GetValue(string key)
        {
            if (
                _languagesMap.TryGetValue(CurrentLanguage, out var map)
                && map.TryGetValue(key, out var value)
            )
            {
                if (value.Length == 0)
                {
                    return key;
                }

                return value;
            }

            return key;
        }

        private void SetInitialLanguage()
        {
            if (_container == null)
            {
                return;
            }

            var systemLanguage = Application.systemLanguage.ToString();

            if (_container.PreferredLanguage == null)
            {
                _container.PreferredLanguage = systemLanguage;
            }

            if (SupportedLanguages.Contains(_container.PreferredLanguage))
            {
                OnLanguageChanged.SafeInvoke();
                return;
            }

            if (!TrySetLanguage(_container.PreferredLanguage) && !TrySetLanguage(systemLanguage))
            {
                if (!TrySetLanguage(SupportedLanguages[0]))
                {
                    Debug.LogError("Failed to set intiial language");
                }
            }
        }

        private void PreValidateMap()
        {
            var tempKeys = new List<string>();

            foreach (var langAsKey in _languagesMap.Keys)
            {
                var languageMap = _languagesMap[langAsKey];

                tempKeys.AddRange(languageMap.Keys);

                foreach (var key in tempKeys)
                {
                    var value = languageMap[key];
                    if (
                        string.IsNullOrEmpty(value)
                        || string.IsNullOrWhiteSpace(value)
                        || value.Trim().Length == 0
                    )
                    {
                        languageMap[key] = string.Empty;
                    }
                }

                tempKeys.Clear();
            }
        }
    }
}
