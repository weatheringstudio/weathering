
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Localization : MonoBehaviour
    {
        public string DefaultLanguage = "zh_cn";
        public TextAsset[] Jsons;
        private Dictionary<string, string> Dict;

        public string Get<T>() {
            return Get(typeof(T));
        }
        public string Get(Type key) {
            if (!Dict.TryGetValue(key.FullName, out string result)) {
                // throw new Exception($"localization key not found: {key}");
                return key.FullName;
            }
            return result;
        }
        public string Val<T>(long val) {
            return Val(typeof(T), val);
        }
        public string Val(Type key, long val) {
            return $"{val}{Get(key)}";
        }

        public string ActiveLanguage { get; private set; }
        public void SwitchLanguage(string language) {
            bool found = false;
            foreach (var jsonTextAsset in Jsons) {
                if (jsonTextAsset.name == ActiveLanguage) {
                    Dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonTextAsset.text);
                    found = true;
                }
            }
            if (!found) {
                throw new Exception(language);
            }
            ActiveLanguage = language;
            DataPersistence.Ins.WriteConfig(activeLanguageKey, new Dictionary<string, string> {
                { activeLanguageKey, ActiveLanguage }
            });
        }

        public static Localization Ins { get; private set; }
        private const string activeLanguageKey = "active_language";
        private void Awake() {
            if (Ins != null) {
                throw new Exception();
            }
            Ins = this;
            if (DataPersistence.Ins.HasConfig(activeLanguageKey)) {
                ActiveLanguage = DataPersistence.Ins.ReadConfig(activeLanguageKey)[activeLanguageKey];
            } else {
                ActiveLanguage = DefaultLanguage;
            }
            SwitchLanguage(ActiveLanguage);
        }
    }
}

