
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Localization : MonoBehaviour
    {
        public TextAsset[] Jsons;
        private Dictionary<string, string> Dict;

        public string Get(string key) {
            if (!Dict.TryGetValue(key, out string result)) {
                throw new Exception($"localization key not found: {key}");
            }
            return result;
        }
        public string Get(Type key) {
            if (!Dict.TryGetValue(key.Name, out string result)) {
                throw new Exception($"localization key not found: {key}");
            }
            return result;
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
                ActiveLanguage = "zh_cn";
            }
            SwitchLanguage(ActiveLanguage);
        }
    }
}

