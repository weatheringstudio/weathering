
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface IGlobals
    {
        IValues Values { get; }
        IRefs Refs { get; }
        IInventory Inventory { get; }
        Dictionary<string, string> PlayerPreferences { get; }


        bool Bool<T>();
        void Bool<T>(bool val);
        bool Bool(Type type);
        void Bool(Type type, bool val);
    }

    public interface IGlobalsDefinition : IGlobals
    {
        IValues ValuesInternal { set; }
        IRefs RefsInternal { set; }
        Dictionary<string, string> PlayerPreferencesInternal { set; }
        IInventory InventoryInternal { get; set; }
    }

    public class Globals : MonoBehaviour, IGlobalsDefinition
    {
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        //public string GetPreference(string pref) {
        //    PlayerPreferences.TryGetValue(pref, out string value);
        //    return value;
        //}

        //public void SetPreference(string pref, string content) {
        //    if (content == null) {
        //        if (PlayerPreferences.ContainsKey(pref)) {
        //            PlayerPreferences.Remove(pref);
        //        }
        //    }
        //    else {
        //        if (PlayerPreferences.ContainsKey(pref)) {
        //            PlayerPreferences[pref] = content;
        //        } else {
        //            PlayerPreferences.Add(pref, content);
        //        }
        //    }
        //}

        public bool Bool<T>() {
            return Bool(typeof(T));
        }
        public bool Bool(Type type) {
            return PlayerPreferences.ContainsKey(type.Name);
        }

        public void Bool<T>(bool val) {
            Bool(typeof(T), val);
        }
        public void Bool(Type type, bool val) {
            if (val) {
                if (!PlayerPreferences.ContainsKey(type.Name)) {
                    PlayerPreferences.Add(type.Name, null);
                }
            } else {
                if (PlayerPreferences.ContainsKey(type.Name)) {
                    PlayerPreferences.Remove(type.Name);
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
        public static bool SanityCheck(long cost = 1) {
            if (sanity == null) sanity = Ins.Values.Get<Sanity>();
            if (sanity.Val < cost) {
                UI.Ins.ShowItems("操作太快了，慢一点吧", UIItem.CreateSeparator());
                return false;
            }
            sanity.Val -= cost;
            return true;
        }
        private static IValue cooldown;
        public static IValue CoolDown {
            get {
                if (cooldown == null) cooldown = Ins.Values.Get<CoolDown>();
                return cooldown;
            }
        }
        public static bool IsCool { get => cooldown.Maxed; }
        public static long SetCooldown {
            set {
                cooldown.Del = value * Value.Second;
                cooldown.Val = 0;
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

        public IInventory InventoryInternal { get; set; }
        public IInventory Inventory => InventoryInternal;
    }
}

