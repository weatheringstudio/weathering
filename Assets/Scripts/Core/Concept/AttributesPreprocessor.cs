
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Weathering
{
    public class DependAttribute : Attribute
    {
        public Type[] Types { get; private set; }
        public DependAttribute(params Type[] types) {
            Types = types;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConceptAttribute : Attribute
    {
    }

    public class AttributesPreprocessor : MonoBehaviour
    {
        [ContextMenu("生成本地化文件")]
        private void GenerateLocalizationFile() {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes()) {
                Attribute[] attributes = Attribute.GetCustomAttributes(type);
                foreach (var attribtue in attributes) {
                    if (attribtue is ConceptAttribute) {
                        result.Add(type.FullName, "");
                        break;
                    }
                }
            }
            string fullPath = $"{Application.streamingAssetsPath}/localization.template.json";
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(fullPath, json);
            Debug.LogWarning("OK");
        }

        public readonly HashSet<Type> RelavantAttributes = new HashSet<Type> {
            typeof(DependAttribute),
            typeof(ConceptAttribute),
        };

        public readonly Dictionary<Type, Dictionary<Attribute, object>> Data 
            = new Dictionary<Type, Dictionary<Attribute, object>>();

        public bool HasAttribute(Type type, Type attr) {
            return Attribute.GetCustomAttribute(type, attr) == null;
        }

        public static AttributesPreprocessor Ins { get; private set; }
        private void Awake() {
            if (Ins != null) {
                throw new Exception();
            }
            Ins = this;

            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (var type in RelavantAttributes) {
                if (type.IsAssignableFrom(typeof(Attribute))) {
                    throw new Exception(type.FullName);
                }
            }

            foreach (var type in assembly.GetTypes()) {
                bool dictCreated = false;
                Attribute[] attributes = Attribute.GetCustomAttributes(type);
                foreach (var attribute in attributes) {
                    if (RelavantAttributes.Contains(attribute.GetType())) {
                        Dictionary<Attribute, object> set;
                        if (!dictCreated) {
                            dictCreated = true;
                            set = new Dictionary<Attribute, object>();
                            Data.Add(type, set);
                        } else {
                            set = Data[type];
                        }
                        set.Add(attribute, null);
                    }
                }
            }
        }
    }
}

