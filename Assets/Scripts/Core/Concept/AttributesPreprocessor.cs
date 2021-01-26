
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
        public HashSet<Type> Set { get; private set; }
        public DependAttribute(params Type[] types) {
            Types = types;
            Set = new HashSet<Type>(types);
        }
    }

    /// <summary>
    /// Localization.Ins.Get<T> 中的 T 一般必须有ConceptAttribute，
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConceptAttribute : Attribute
    {
    }

    public class AttributesPreprocessor : MonoBehaviour, IComparer<Type>
    {
        [ContextMenu("生成本地化文件")]
        private void GenerateLocalizationFile() {
            // editor only script
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
            // typeof(ConceptAttribute),
        };

        //public readonly Dictionary<Type, Dictionary<Attribute, object>> Data
        //    = new Dictionary<Type, Dictionary<Attribute, object>>();
        public readonly Dictionary<Type, DependAttribute> DependAttribute = new Dictionary<Type, DependAttribute>(); // 缓存
        public readonly List<Type> DependAttributeList = new List<Type>(); // 排序表
        public readonly Dictionary<Type, int> IndexDict = new Dictionary<Type, int>(); // 排序表，逆序
        public readonly Dictionary<Type, HashSet<Type>> FinalResult = new Dictionary<Type, HashSet<Type>>(); // 
        public readonly Dictionary<Type, List<Type>> FinalResultSorted = new Dictionary<Type, List<Type>>(); // 

        /// <summary>
        /// DependAttribute 构成的偏序关系就存在这里了
        /// </summary>
        public readonly Dictionary<Type, Dictionary<Type, object>> DependAttributeClosure
            = new Dictionary<Type, Dictionary<Type, object>>();

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

            //// 记录所有自定义的 attribute
            //// 结果存在 Dictionary<Type, Dictionary<Attribute, null>>
            //foreach (var type in assembly.GetTypes()) {
            //    bool dictCreated = false;
            //    Attribute[] attributes = Attribute.GetCustomAttributes(type);
            //    foreach (var attribute in attributes) {
            //        if (RelavantAttributes.Contains(attribute.GetType())) {
            //            Dictionary<Attribute, object> set;
            //            if (!dictCreated) {
            //                dictCreated = true;
            //                set = new Dictionary<Attribute, object>();
            //                Data.Add(type, set);
            //            } else {
            //                set = Data[type];
            //            }
            //            set.Add(attribute, null);
            //        }
            //    }
            //}

            // 计算 depend attribute
            // 记录所有自定义的 attribute
            // 结果存在 Dictionary<Type, Dictionary<Attribute, null>>
            foreach (var type in assembly.GetTypes()) {
                Attribute[] attributes = Attribute.GetCustomAttributes(type);
                foreach (var attribute in attributes) {
                    if (attribute is DependAttribute) {
                        DependAttribute.Add(type, attribute as DependAttribute);
                        DependAttributeList.Add(type);
                    }
                }
            }

            // 所有被依赖的都应该也有Depend特性
            foreach (var pair in DependAttribute) {
                foreach (var dependee in DependAttribute[pair.Key].Set) {
                    if (!DependAttribute.ContainsKey(dependee)) {
                        throw new Exception($"{pair.Key} depends on {dependee}");
                    }
                }
            }

            // 计算传递闭包。不用啊，depend是偏序关系，不用图啊。排序就行
            for (int i = 0; i < DependAttributeList.Count; i++) {
                for (int j = i; j < DependAttributeList.Count; j++) {
                    // if a[i] > a[j] 即 a[i] depend a[j]
                    if (Depend(DependAttributeList[i], DependAttributeList[j])) {
                        var t = DependAttributeList[i];
                        DependAttributeList[i] = DependAttributeList[j];
                        DependAttributeList[j] = t;
                    }
                }
            }

            // 检验循环依赖
            for (int i = 0; i < DependAttributeList.Count; i++) {
                for (int j = i; j < DependAttributeList.Count; j++) {
                    // if a[i] > a[j] 即 a[i] depend a[j]
                    if (Depend(DependAttributeList[i], DependAttributeList[j])) {
                        throw new Exception($"{DependAttributeList[i]} 循环依赖 {DependAttributeList[j]}");
                    }
                }
            }

            // 记录下标，判断所有
            for (int i = 0; i < DependAttributeList.Count; i++) {
                IndexDict.Add(DependAttributeList[i], i);
            }

            // 对于每个元素
            for (int i = 0; i < DependAttributeList.Count; i++) {
                Type depender = DependAttributeList[i];
                var set = new HashSet<Type>();
                FinalResult.Add(depender, set);
                // 的依赖元素（在数组左边
                DependAttribute attribute = DependAttribute[depender];
                foreach (var dependee in attribute.Set) {
                    if (!set.Contains(dependee)) {
                        set.Add(dependee);
                        var dependees = FinalResult[dependee];
                        foreach (var dependeeOfDependee in dependees) {
                            if (!set.Contains(dependeeOfDependee)) {
                                set.Add(dependeeOfDependee);
                            }
                        }
                    }
                }
            }

            foreach (var pair in FinalResult) {
                Type type = pair.Key;
                var list = new List<Type>(pair.Value);
                list.Sort(this);
                FinalResultSorted.Add(type, list);
            }

            //foreach (var pair in FinalResult) {
            //    Debug.LogWarning(pair.Key.Name);
            //    foreach (var type in pair.Value) {
            //        Debug.Log(type.Name);
            //    }
            //}

        }

        private bool Depend(Type type1, Type type2) {
            return DependAttribute[type1].Set.Contains(type2);
        }

        public int Compare(Type x, Type y) {
            return IndexDict[x] > IndexDict[y] ? 1 : -1;
        }
    }
}

