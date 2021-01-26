
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Tag : MonoBehaviour
    {
        public static Tag Ins { get; private set; }
        private void Awake() {
            if (Ins != null) {
                throw new Exception();
            }
            Ins = this;
        }

        public bool HasTag(Type type, Type tag) {
            if (type == tag) return true;
            if (AttributesPreprocessor.Ins.FinalResult.TryGetValue(type, out HashSet<Type> result)) {
                return result.Contains(tag);
            }
            return false;
        }

        public List<Type> AllTagOf(Type type) {
            if (AttributesPreprocessor.Ins.FinalResult.ContainsKey(type)) {
                return AttributesPreprocessor.Ins.FinalResultSorted[type];
            }
            throw new Exception(type.FullName);
        }
    }
}

