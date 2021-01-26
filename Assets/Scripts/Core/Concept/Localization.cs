
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface ILocalization
    {
        string Get<T>();
        string Get(Type key);

        string ValUnit<T>();
        string NoVal(Type key);
        string NoVal<T>();
        string Val<T>(long val);
        string Val(Type key, long val);
        string Inc<T>(long val);
        string Inc(Type key, long val);

        string ActiveLanguage { get; }
        void SwitchLanguage(string language);
    }

    public class Localization : MonoBehaviour, ILocalization
    {
        public static ILocalization Ins { get; private set; }
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


        [SerializeField]
        private string DefaultLanguage = "zh_cn";
        [SerializeField]
        private TextAsset[] Jsons;
        private Dictionary<string, string> Dict;

        public string Get<T>() {
            return Get(typeof(T));
        }
        public string Get(Type key) {
            if (Dict.TryGetValue(key.FullName, out string result)) {
                // throw new Exception($"localization key not found: {key}");
                // return string.Format(result, "");
                return result;
            }
            return key.FullName;
        }

        public string ValUnit<T>() {
            return NoVal(typeof(T));
        }
        public string NoVal(Type key) {
            if (Dict.TryGetValue(key.FullName, out string result)) {
                return string.Format(result, "");
            }
            return key.FullName;
        }
        public string NoVal<T>() {
            return NoVal(typeof(T));
        }

        public string Val<T>(long val) {
            return Val(typeof(T), val);
        }
        public string Val(Type key, long val) {
            if (Dict.TryGetValue(key.FullName, out string result)) {
                // throw new Exception($"localization key not found: {key}");
                if (val > 0) {
                    return string.Format(result, $" {val}");
                } else if (val < 0) {
                    return string.Format(result, $"-{-val}");
                } else {
                    return string.Format(result, 0);
                }
            }
            return key.FullName;
        }
        public string Inc<T>(long val) {
            return Inc(typeof(T), val);
        }
        public string Inc(Type key, long val) {
            if (Dict.TryGetValue(key.FullName, out string result)) {
                // throw new Exception($"localization key not found: {key}");
                if (val > 0) {
                    return string.Format(result, $" Δ{val}");
                } else if (val < 0) {
                    return string.Format(result, $"-Δ{-val}");
                } else {
                    return string.Format(result, 0);
                }
            }
            return key.FullName;
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


    }
}

