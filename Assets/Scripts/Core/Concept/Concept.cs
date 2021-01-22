
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Weathering
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConceptAttribute : Attribute
    {
        //public string Color { get; set; } // #ffffff
        //public string Name { get; set; }

        //public ConceptAttribute(string name, string color = "ffffff") {
        //    Name = name;
        //    Color = color;
        //}
    }

    //public interface IConcept
    //{
    //    string ColoredNameOf<T>();
    //    string ColoredNameOf(Type type);

    //    string Val(Type type, long quantity);
    //    string Val<T>(long quantity);
    //}

    //public class Concept : MonoBehaviour, IConcept
    //{
    //    public static IConcept Ins { get; private set; }

    //    private void Awake() {
    //        if (Ins != null) {
    //            throw new Exception();
    //        }
    //        Ins = this;

    //        Assembly assembly = Assembly.GetExecutingAssembly();

    //        Type valueTypeAttributeType = typeof(ConceptAttribute);
    //        foreach (var type in assembly.GetTypes()) {
    //            if (Attribute.GetCustomAttribute(type, valueTypeAttributeType) is ConceptAttribute valueTypeAttribute) {
    //                Concepts.Add(type, valueTypeAttribute);
    //            }
    //        }
    //    }

    //    public string ColoredNameOf(Type type) {
    //        return Localization.Ins.Get(type);
    //    }
    //    public string ColoredNameOf<T>() {
    //        return ColoredNameOf(typeof(T));
    //    }

    //    public string Val(Type type, long quantity) {
    //        return $"{quantity}{Localization.Ins.Get(type)}";
    //    }
    //    public string Val<T>(long quantity) {
    //        return Val(typeof(T), quantity);
    //    }

    //    public Dictionary<Type, ConceptAttribute> Concepts { get; private set; } = new Dictionary<Type, ConceptAttribute>();
    //}
}




