
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    using ComponentData = Dictionary<string, string>;
    using EntityData = Dictionary<string, Dictionary<string, string>>;
    using MapData = Dictionary<string, Dictionary<string, Dictionary<string, string>>>;

    public interface ISerializable
    {
        void Serialize(ComponentData data);
        void Deserialize(ComponentData data);
    }

    public static class Utility
    {
        public static long GetTicks() {
            // will be replaced by in-game ticks
            return DateTime.Now.Ticks;
        }
        public static long GetSeconds() {
            return GetTicks() / Value.Second;
        }

        public static int GetFrame(float framerate, int spriteCount) {
            return (int)(GetTicks() / (long)(Value.Second * framerate) % spriteCount);
        }

        public static void SerializeEntity(GameObject go, out EntityData data) {
            data = new EntityData();
            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components) {
                ISerializable serializable = component as ISerializable;
                if (serializable != null) {
                    ComponentData componentData = new ComponentData();
                    data.Add(serializable.GetType().FullName, componentData);
                    serializable.Serialize(componentData);
                }
            }
            throw new NotImplementedException();
        }

        public static void DeserializeEntity(GameObject go, ref EntityData data) {
            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components) {
                ISerializable serializable = component as ISerializable;
                if (serializable != null) {
                    serializable.Deserialize(data[serializable.GetType().FullName]);
                }
            }
            throw new NotImplementedException();
        }

        public static void SaveEntity(string path, EntityData content) {
            string json = JsonUtility.ToJson(content); // JsonConvert.SerializeObject
            System.IO.File.WriteAllText(System.IO.Path.Combine(Application.streamingAssetsPath, path), json);
        }

        public static void LoadEntity(string path, EntityData content) {
            string json = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.streamingAssetsPath, path));
            JsonUtility.FromJsonOverwrite(json, content); // JsonCOnver.DeseraizlieObject
        }


    }
}

