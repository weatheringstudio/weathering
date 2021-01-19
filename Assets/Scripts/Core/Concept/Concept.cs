
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Weathering
{
    public interface IConcept
    {
        string AddColor<T>(string text);
        string ColoredNameOf<T>();
        string ColoredNameOf(Type type);
        string Val<T>(long quantity);

        string Inc<T>(long quantity);
        string Max<T>(long quantity);
        Dictionary<Type, ConceptAttribute> Concepts { get; }
    }

    public class Concept : MonoBehaviour, IConcept
    {
        public static IConcept Ins { get; private set; }

        private void Awake() {
            if (Ins != null) {
                throw new Exception();
            }
            Ins = this;

            Assembly assembly = Assembly.GetExecutingAssembly();

            Type valueTypeAttributeType = typeof(ConceptAttribute);
            foreach (var type in assembly.GetTypes()) {
                if (Attribute.GetCustomAttribute(type, valueTypeAttributeType) is ConceptAttribute valueTypeAttribute) {
                    Concepts.Add(type, valueTypeAttribute);
                }
            }
        }

        public string NameOf<T>() {
            ConceptAttribute result;
            if (Concepts.TryGetValue(typeof(T), out result)) {
                return result.Name;
            } else {
                throw new Exception(typeof(T).FullName);
            }
        }

        public string AddColor<T>(string text) {
            ConceptAttribute result;
            if (Concepts.TryGetValue(typeof(T), out result)) {
                return result.AddColor(text);
            } else {
                throw new Exception(typeof(T).FullName);
            }
        }
        //public string ColorNameWith<T, U>() {
        //    ConceptAttribute nameSource;
        //    ConceptAttribute colorSource;
        //    if (Concepts.TryGetValue(typeof(T), out nameSource)) {
        //        if (Concepts.TryGetValue(typeof(U), out colorSource)) {
        //            return string.Format("<color=#{0}>【{1}】</color>", colorSource.Color, nameSource.Name);
        //        } else {
        //            throw new Exception(typeof(U).FullName);
        //        }
        //    } else {
        //        throw new Exception(typeof(T).FullName);
        //    }
        //}
        public string ColoredNameOf(Type type) {
            ConceptAttribute result;
            if (Concepts.TryGetValue(type, out result)) {
                return result.ColoredName;
            } else {
                throw new Exception(type.FullName);
            }
        }
        public string ColoredNameOf<T>() {
            return ColoredNameOf(typeof(T));
        }

        public string Val<T>(long quantity) {
            ConceptAttribute result;
            if (Concepts.TryGetValue(typeof(T), out result)) {
                return result.WithVal(quantity);
            } else {
                throw new Exception(typeof(T).FullName);
            }
        }

        public string Inc<T>(long quantity) {
            ConceptAttribute result;
            if (Concepts.TryGetValue(typeof(T), out result)) {
                return result.WithInc(quantity);
            } else {
                throw new Exception(typeof(T).FullName);
            }
        }

        public string Max<T>(long quantity) {
            ConceptAttribute result;
            if (Concepts.TryGetValue(typeof(T), out result)) {
                return result.WithMax(quantity);
            } else {
                throw new Exception(typeof(T).FullName);
            }
        }


        public Dictionary<Type, ConceptAttribute> Concepts { get; private set; } = new Dictionary<Type, ConceptAttribute>();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConceptAttribute : Attribute
    {
        public string Color { get; set; } // #ffffff
        public string Name { get; set; }

        public string AddColor(string text) {
            return string.Format("<color=#{0}>【{1}】</color>", Color, text);
        }
        public string ColoredName {
            get {
                string result = string.Format("<color=#{0}>【{1}】</color>", Color, Name);
                return result;
            }
        }
        public string WithVal(long quantity) {
            if (quantity > 0) {
                return string.Format("<color=#{0}>【</color>+{2}<color=#{0}>{1}】</color>", Color, Name, quantity);
            }
            else if (quantity < 0) {
                return string.Format("<color=#{0}>【</color>-{2}<color=#{0}>{1}】</color>", Color, Name, -quantity);
            }
            else {
                return string.Format("<color=#{0}>【</color>0<color=#{0}>{1}】</color>", Color, Name);
            }
        }

        public string WithInc(long quantity) {
            if (quantity > 0) {
                return string.Format("<color=#{0}>【</color>+{2}Δ<color=#{0}>{1}】</color>", Color, Name, quantity);
            } else if (quantity < 0) {
                return string.Format("<color=#{0}>【</color>-{2}Δ<color=#{0}>{1}】</color>", Color, Name, -quantity);
            } else {
                return string.Format("<color=#{0}>【</color>0<color=#{0}>{1}】</color>", Color, Name);
            }
        }

        public string WithMax(long quantity) {
            if (quantity > 0) {
                return string.Format("<color=#{0}>【</color>+{2}<color=#{0}>{1}-上限】</color>", Color, Name, quantity);
            } else if (quantity < 0) {
                return string.Format("<color=#{0}>【</color>-{2}<color=#{0}>{1}-上限】</color>", Color, Name, -quantity);
            } else {
                return string.Format("<color=#{0}>【</color>0<color=#{0}>{1}】</color>", Color, Name);
            }
        }

        public ConceptAttribute(string name, string color = "ffffff") {
            Name = name;
            Color = color;
        }
    }

}




