
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Weathering
{
    public interface IDataPersistence
    {
        void Write(string filename, string content);
        string Read(string filename);
        bool HasFile(string filename);

        void SaveMap(IMap map);
        IMap LoadMap(string mapName);

        void DeleteSaves();
    }

    public class DataPersistence : MonoBehaviour, IDataPersistence
    {
        public const string SavesBase = "Saves";
        public string Saves { get; private set; }

        public static IDataPersistence Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
            Saves = Application.persistentDataPath + "/" + SavesBase + "/";
            if (!Directory.Exists(Saves)) {
                Directory.CreateDirectory(Saves);
            }
        }

        public const string JSON_SUFFIX = ".json";
        public const string TEMP_FILENAME = "temp" + JSON_SUFFIX;
        public void Write(string filename, string content) {
            string tempPath = Saves + TEMP_FILENAME;
            string targetPath = Saves + filename + JSON_SUFFIX;
            System.IO.File.WriteAllText(tempPath, content);
            if (System.IO.File.Exists(targetPath)) {
                System.IO.File.Delete(targetPath);
            }
            System.IO.File.Move(tempPath, targetPath);
            // System.IO.File.WriteAllText(Saves + "/" + filename + JSON_SUFFIX, content);
        }
        public string Read(string filename) {
            return System.IO.File.ReadAllText(Saves + filename + JSON_SUFFIX);
        }

        public bool HasFile(string filename) {
            return System.IO.File.Exists(Saves + filename + JSON_SUFFIX);
        }


        public struct TileData
        {
            public string Type;
            public Dictionary<string, ValueData> Values;
        }

        private string SerializeVector2(Vector2Int vec) => vec.x + "," + vec.y;
        private Vector2Int DeserializeVector2(string s) {
            string[] ss = s.Split(',');
            int x = int.Parse(ss[0]);
            int y = int.Parse(ss[1]);
            return new Vector2Int(x, y);
        }

        public const string HeadSuffix = ".head";
        public void SaveMap(IMap map) {
            // obj => data
            Dictionary<string, ValueData> mapHeadData = Values.ToData(map.Values);
            // data => json
            string mapHeadJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                mapHeadData, Newtonsoft.Json.Formatting.Indented
            );
            // json => file
            Write(map.GetType().FullName + HeadSuffix, mapHeadJson);

            // obj => data
            int width = (int)map.Values.Get<Width>().Max;
            int height = (int)map.Values.Get<Height>().Max;
            Dictionary<string, TileData> mapBodyData = new Dictionary<string, TileData>();
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    ITileDefinition tile = map.Get(i, j) as ITileDefinition;
                    if (tile == null) throw new Exception();

                    TileData tileData = new TileData {
                        Values = Values.ToData(tile.Values),
                        Type = tile.GetType().FullName
                    };

                    mapBodyData.Add(SerializeVector2(new Vector2Int(i, j)), tileData);
                }
            }
            // data => json
            string mapBodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                mapBodyData
                 , Newtonsoft.Json.Formatting.Indented
                );
            // json => file
            Write(map.GetType().FullName, mapBodyJson);
        }

        public IMap LoadMap(string mapName) {
            IMapDefinition map = Activator.CreateInstance(Type.GetType(mapName)) as IMapDefinition;
            if (map == null) throw new Exception();

            // file => json
            string mapHeadJson = Read(mapName + HeadSuffix);
            // json => data
            Dictionary<string, ValueData> mapHeadData = Newtonsoft.Json.JsonConvert.DeserializeObject<
                Dictionary<string, ValueData>>(mapHeadJson);
            if (mapHeadData == null) throw new Exception();
            // data => obj
            IValues mapValues = Values.FromData(mapHeadData);
            if (mapValues == null) throw new Exception();

            map.SetValues(mapValues);


            map.Initialize(); // init

            // file => json
            string mapBodyJson = Read(mapName);
            // json => data
            Dictionary<string, TileData> mapBodyData = Newtonsoft.Json.JsonConvert.DeserializeObject<
                Dictionary<string, TileData>>(mapBodyJson);
            // map => obj
            foreach (var pair in mapBodyData) {
                Vector2Int pos = DeserializeVector2(pair.Key);
                TileData tileData = pair.Value;
                Type type = Type.GetType(tileData.Type);
                ITileDefinition tile = Activator.CreateInstance(type) as ITileDefinition;
                if (tile == null) throw new Exception();

                tile.Pos = pos;
                tile.Map = map;

                IValues tileValues = Values.FromData(tileData.Values);
                tile.SetValues(tileValues);
                tile.Initialize();
                map.SetTile(pos, tile);
            }

            return map;
        }

        public void DeleteSaves() {
            DeleteFolder(Saves);
        }
        public void DeleteFolder(string dir) {
            foreach (string d in Directory.GetFileSystemEntries(dir)) {
                if (File.Exists(d)) {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);
                } else {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0) {
                        DeleteFolder(d1.FullName);
                    }
                    Directory.Delete(d);
                }
            }
        }


    }
}

