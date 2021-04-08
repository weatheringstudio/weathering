
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
        string ValUnit(Type key);
        //string NoVal(Type key);
        //string NoVal<T>();
        string Val<T>(long val);
        string Val(Type key, long val);
        string ValPlus<T>(long val);
        string ValPlus(Type key, long val);
        string Inc<T>(long val);
        string Inc(Type key, long val);

        void SyncActiveLanguage(); // Globals.Ins.PlayerPreferences[activeLanguageKey]
        void SwitchNextLanguage();
    }

    public class Localization : MonoBehaviour, ILocalization
    {
        public static ILocalization Ins { get; private set; }
        public const string ACTIVE_LANGUAGE = "active_language";
        private void Awake() {
            if (Ins != null) {
                throw new Exception();
            }
            Ins = this;
            if (Globals.Ins.PlayerPreferences.ContainsKey(ACTIVE_LANGUAGE)) {

            } else {
                Globals.Ins.PlayerPreferences.Add(ACTIVE_LANGUAGE, DefaultLanguage);
            }
            SyncActiveLanguage();
        }


        private string DefaultLanguage = "zh_cn";


        public string[] SupporttedLanguages;

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
            return ValUnit(typeof(T));
        }
        public string ValUnit(Type key) {
            if (Dict.TryGetValue(key.FullName, out string result)) {
                return string.Format(result, "");
            }
            return key.FullName;
        }

        //public string NoVal(Type key) {
        //    if (Dict.TryGetValue(key.FullName, out string result)) {
        //        return string.Format(result, "");
        //    }
        //    return key.FullName;
        //}
        //public string NoVal<T>() {
        //    return NoVal(typeof(T));
        //}

        public string Val<T>(long val) {
            return Val(typeof(T), val);
        }
        public string Val(Type key, long val) {
            if (key == null) throw new Exception();
            if (Dict.TryGetValue(key.FullName, out string result)) {
                // throw new Exception($"localization key not found: {key}");
                if (val > 0) {
                    return string.Format(result, $" {val}");
                } else if (val < 0) {
                    return string.Format(result, $"-{-val}");
                } else {
                    return string.Format(result, " 0");
                }
            }
            return key.FullName;
        }
        public string ValPlus<T>(long val) {
            return ValPlus(typeof(T), val);
        }
        public string ValPlus(Type key, long val) {
            if (Dict.TryGetValue(key.FullName, out string result)) {
                // throw new Exception($"localization key not found: {key}");
                if (val > 0) {
                    return string.Format(result, $"+{val}");
                } else if (val < 0) {
                    return string.Format(result, $"-{-val}");
                } else {
                    return string.Format(result, " 0");
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
                    return string.Format(result, " 0");
                }
            }
            return key.FullName;
        }

        public void SyncActiveLanguage() {
            string activeLanguage = Globals.Ins.PlayerPreferences[ACTIVE_LANGUAGE];
            bool found = false;

            Dict = new Dictionary<string, string>();
            foreach (var jsonTextAsset in Jsons) {
                if (jsonTextAsset.name.StartsWith(activeLanguage)) {
                    Dictionary<string, string> subDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonTextAsset.text);
                    foreach (var pair in subDict) {
                        if (Dict.ContainsKey(pair.Key)) {
                            UIPreset.Throw($"出现了重复的key “{pair.Key}” in {jsonTextAsset.name}. 不知道另一个key在哪个文件");
                        }
                        else {
                            Dict.Add(pair.Key, pair.Value);
                        }
                    }
                    found = true;
                }
            }
            if (!found) {
                throw new Exception(activeLanguage);
            }
        }

        public void SwitchNextLanguage() {
            string activeLanguage = Globals.Ins.PlayerPreferences[ACTIVE_LANGUAGE];

            // 找到下一个语言，效率很低，但可以用
            bool found = false;
            int index = 0;
            foreach (var jsonTextAsset in SupporttedLanguages) {
                if (jsonTextAsset == activeLanguage) {
                    // Dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonTextAsset.text);
                    found = true;
                    break;
                }
                index++;
            }
            if (!found) throw new Exception();
            index++;
            if (index == Jsons.Length) {
                index = 0;
            }

            // Dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Jsons[index].text);

            Globals.Ins.PlayerPreferences[ACTIVE_LANGUAGE] = Jsons[index].name;
            SyncActiveLanguage();
        }


    }
}

