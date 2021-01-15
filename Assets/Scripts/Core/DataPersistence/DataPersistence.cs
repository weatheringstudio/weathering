﻿
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
        IMap LoadMap(IMapDefinition map);

        bool HasMap(Type type);

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
            File.WriteAllText(tempPath, content);
            if (File.Exists(targetPath)) {
                File.Delete(targetPath);
            }
            File.Move(tempPath, targetPath);
        }
        public string Read(string filename) {
            return File.ReadAllText(Saves + filename + JSON_SUFFIX);
        }

        public bool HasFile(string filename) {
            return File.Exists(Saves + filename + JSON_SUFFIX);
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

        public bool HasMap(Type type) {
            string mapName = type.FullName;
            return HasFile(mapName + HeadSuffix);
        }


        public IMap LoadMap(IMapDefinition map)  {
            if (map == null) throw new Exception();
            string mapName = map.GetType().FullName;

            // 1. 读取对应位置json存档
            // file => json
            string mapHeadJson = Read(mapName + HeadSuffix);

            // 2. 将json反序列化为数据 Dictionary<string, ValueData>, string为数值类型
            // json => data
            Dictionary<string, ValueData> mapHeadData = Newtonsoft.Json.JsonConvert.DeserializeObject<
                Dictionary<string, ValueData>>(mapHeadJson);
            if (mapHeadData == null) throw new Exception();

            // 3. 从数据中同步到地图对象中
            // data => obj
            IValues mapValues = Values.FromData(mapHeadData);
            if (mapValues == null) throw new Exception();
            map.SetValues(mapValues);

            // 4. 休息一下
            // 5. 再休息一下

            // 6. 读取对应位置地块json存档
            // file => json
            string mapBodyJson = Read(mapName);
            // 7. 将json反序列化为数据 Dictionary<string, TileData>, string为位置, TileData包含类型和值
            // json => data
            Dictionary<string, TileData> mapBodyData = Newtonsoft.Json.JsonConvert.DeserializeObject<
                Dictionary<string, TileData>>(mapBodyJson);
            // 8. 对于每一个地块，通过SetTile塞到地图里
            // map => obj
            List<ITileDefinition> tiles = new List<ITileDefinition>(mapBodyData.Count);
            foreach (var pair in mapBodyData) {
                Vector2Int pos = DeserializeVector2(pair.Key);
                TileData tileData = pair.Value;
                Type tileType = Type.GetType(tileData.Type);
                ITileDefinition tile = Activator.CreateInstance(tileType) as ITileDefinition;
                if (tile == null) throw new Exception();

                tile.Pos = pos;
                tile.Map = map;

                IValues tileValues = Values.FromData(tileData.Values);
                
                tile.SetValues(tileValues);
                map.SetTile(pos, tile);
                tiles.Add(tile);
            }
            map.Initialize();
            foreach (var tile in tiles) {
                tile.Initialize();
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

