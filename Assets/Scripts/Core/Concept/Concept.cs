
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Weathering
{
    public interface IConcept
    {
        string ColoredNameOf<T>();
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
            return Concepts[typeof(T)].Name;
        }

        public string ColoredNameOf<T>() {
            return Concepts[typeof(T)].ColoredName;
        }

        public string Val<T>(long quantity) {
            return Concepts[typeof(T)].WithVal(quantity);
        }

        public string Inc<T>(long quantity) {
            return Concepts[typeof(T)].WithInc(quantity);
        }

        public string Max<T>(long quantity) {
            return Concepts[typeof(T)].WithMax(quantity);
        }

        public Dictionary<Type, ConceptAttribute> Concepts { get; private set; } = new Dictionary<Type, ConceptAttribute>();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConceptAttribute : Attribute
    {
        public string Color { get; set; } // #ffffff
        public string Name { get; set; }
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




