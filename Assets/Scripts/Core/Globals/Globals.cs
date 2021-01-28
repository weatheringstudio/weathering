
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IGlobals
    {
        IValues Values { get; }
        IRefs Refs { get; }
        Dictionary<string, string> PlayerPreferences { get; }
        string GetPreference(string pref);
        void SetPreference(string pref, string content);

        bool Bool<T>();
        void Bool<T>(bool val);
    }

    public interface IGlobalsDefinition : IGlobals
    {
        IValues ValuesInternal {  set; }
        IRefs RefsInternal { set; }
        Dictionary<string, string> PlayerPreferencesInternal {  set; }
    }

    public class Globals : MonoBehaviour, IGlobalsDefinition
    {
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        public string GetPreference(string pref) {
            PlayerPreferences.TryGetValue(pref, out string value);
            return value;
        }

        public void SetPreference(string pref, string content) {
            if (content == null) {
                if (PlayerPreferences.ContainsKey(pref)) {
                    PlayerPreferences.Remove(pref);
                }
            }
            else {
                if (PlayerPreferences.ContainsKey(pref)) {
                    PlayerPreferences[pref] = content;
                } else {
                    PlayerPreferences.Add(pref, content);
                }
            }
        }

        public bool Bool<T>() {
            return PlayerPreferences.ContainsKey(typeof(T).Name);
        }

        public void Bool<T>(bool val) {
            if (val) {
                if (!PlayerPreferences.ContainsKey(typeof(T).Name)) {
                    PlayerPreferences.Add(typeof(T).Name, null);
                }
            }
            else {
                if (PlayerPreferences.ContainsKey(typeof(T).Name)) {
                    PlayerPreferences.Remove(typeof(T).Name);
                }
            }
        }

        public static IGlobals Ins;

        private static IValue sanity;
        public static IValue Sanity {
            get {
                if (sanity == null) sanity = Ins.Values.Get<Sanity>();
                return sanity;
            }
        }

        public IValues ValuesInternal { get; set; }
        // public void SetValues(IValues values) => ValuesInternal = values;
        public IRefs RefsInternal { get; set; }
        // public void SetRefs(IRefs refs) => RefsInternal = refs;
        public Dictionary<string, string> PlayerPreferencesInternal { get; set; }


        public IValues Values => ValuesInternal;
        public IRefs Refs => RefsInternal;
        public Dictionary<string, string> PlayerPreferences { get => PlayerPreferencesInternal; }
    }
}

