
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Weathering
{


	public class AttributesPreprocessor : MonoBehaviour
	{

        public readonly HashSet<Type> RelavantAttributes = new HashSet<Type> {
            typeof(ResourceSupplyAttribute)
        };

        public readonly Dictionary<Type, Dictionary<Type, object>> Data = new Dictionary<Type, Dictionary<Type, object>>();

        public static AttributesPreprocessor Ins { get; private set; }
        private void Awake() {
            if (Ins != null) {
                throw new Exception();
            }
            Ins = this;

            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes()) {
                bool dictCreated = false;
                Attribute[] attributes = Attribute.GetCustomAttributes(type);
                foreach (var attribute in attributes) {
                    if (RelavantAttributes.Contains(attribute.GetType())) {
                        Dictionary<Type, object> set;
                        if (!dictCreated) {
                            set = new Dictionary<Type, object>();
                            Data.Add(type, set);
                        }
                        else {
                            set = Data[type];
                        }
                        set.Add(type, attribute);
                    }
                }
            }
            foreach (var pair in Data) {
                foreach (var pair2 in pair.Value) {
                    UnityEngine.Debug.LogWarning(pair.Key.Name + " " + pair2.Key.Name);
                }
            }
        }

    }
}

