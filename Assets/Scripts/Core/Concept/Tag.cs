
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public static class Tag
    {
        public static T GetAttribute<T>(Type type) where T : Attribute {
            return Attribute.GetCustomAttribute(type, typeof(T)) as T;
        }

        public static bool HasTag(Type type, Type tag) {
            if (type == tag) return true;
            if (AttributesPreprocessor.Ins.FinalResult.TryGetValue(type, out HashSet<Type> result)) {
                return result.Contains(tag);
            }
            return false;
        }

        public static List<Type> AllTagOf(Type type) {
            if (AttributesPreprocessor.Ins.FinalResult.ContainsKey(type)) {
                return AttributesPreprocessor.Ins.FinalResultSorted[type];
            }
            throw new Exception(type.FullName);
        }

        public static List<Type> AllSubTagOf(Type type) {
            if (AttributesPreprocessor.Ins.FinalResult.ContainsKey(type)) {
                return AttributesPreprocessor.Ins.FinalResultInversedSorted[type];
            }
            throw new Exception(type.FullName);
        }
    }
}

