
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

        void SaveGlobals();
        void LoadGlobals();
        bool HasGlobals();

        bool HasMap(Type type);

        void DeleteSaves();
    }

    public class DataPersistence : MonoBehaviour, IDataPersistence
    {
        // 存档根目录
        public string Base { get; private set; }
        public const string SavesBase = "Saves/";
        public string Saves { get; private set; }

        private Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings {
            DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore,
        };

        public static IDataPersistence Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
            Base = Application.persistentDataPath + "/";
            Saves = Base + SavesBase;
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

        private string globalValuesFilename = "_Global.Values";
        private string globalRefsFilename = "_Globals.Refs";
        public void SaveGlobals() {
            Dictionary<string, ValueData> values = Weathering.Values.ToData(Globals.Ins.Values);
            Dictionary<string, RefData> refs = Weathering.Refs.ToData(Globals.Ins.Refs);

            Write(globalValuesFilename + JSON_SUFFIX, Newtonsoft.Json.JsonConvert.SerializeObject(
                values, Newtonsoft.Json.Formatting.Indented, setting));
            Write(globalRefsFilename + JSON_SUFFIX, Newtonsoft.Json.JsonConvert.SerializeObject(
                refs, Newtonsoft.Json.Formatting.Indented, setting));
        }

        public void LoadGlobals() {
            Dictionary<string, ValueData> values = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ValueData>>(
                Read(globalValuesFilename + JSON_SUFFIX), setting);
            Dictionary<string, RefData> refs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, RefData>>(
                Read(globalRefsFilename + JSON_SUFFIX), setting);

            IGlobalsDefinition globals = Globals.Ins as IGlobalsDefinition;
            if (globals == null) throw new Exception();
            globals.ValuesInternal = Values.FromData(values);
            globals.RefsInternal = Refs.FromData(refs);
        }
        public bool HasGlobals() {
            return HasFile(globalValuesFilename + JSON_SUFFIX) && HasFile(globalRefsFilename + JSON_SUFFIX);
        }


        public struct MapData
        {
            public string type;
            public Dictionary<string, ValueData> values;
            public Dictionary<string, RefData> references;
            public long inventory_quantity;
            public long inventory_capacity;
            public int inventory_type_capacity;
            public Dictionary<string, long> inventory;
        }

        public struct TileData
        {
            public string type;
            public Dictionary<string, ValueData> values;
            public Dictionary<string, RefData> references;
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
            MapData mapHeadData = new MapData {
                type = map.GetType().FullName,
                values = Values.ToData(map.Values),
                references = Refs.ToData(map.Refs),
                inventory_quantity = map.Inventory.Quantity,
                inventory_capacity = map.Inventory.QuantityCapacity,
                inventory_type_capacity = map.Inventory.TypeCapacity,
                inventory = Inventory.ToData(map.Inventory),
            };

            // data => json
            string mapHeadJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                mapHeadData, Newtonsoft.Json.Formatting.Indented, setting
            );
            // json => file
            Write(map.GetType().FullName + HeadSuffix, mapHeadJson);

            // obj => data
            int width = map.Width;
            int height = map.Height;
            Dictionary<string, TileData> mapBodyData = new Dictionary<string, TileData>();
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    ITileDefinition tile = map.Get(i, j) as ITileDefinition;
                    if (tile == null) throw new Exception();

                    TileData tileData = new TileData {
                        values = Values.ToData(tile.Values),
                        references = Refs.ToData(tile.Refs),
                        type = tile.GetType().FullName,
                    };

                    mapBodyData.Add(SerializeVector2(new Vector2Int(i, j)), tileData);
                }
            }
            // data => json
            string mapBodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                mapBodyData
                 , Newtonsoft.Json.Formatting.Indented, setting
                );
            // json => file
            Write(map.GetType().FullName, mapBodyJson);
        }

        public bool HasMap(Type type) {
            string mapName = type.FullName;
            return HasFile(mapName + HeadSuffix);
        }


        public IMap LoadMap(IMapDefinition map) {
            if (map == null) throw new Exception();
            string mapName = map.GetType().FullName;

            // 1. 读取对应位置json存档
            // file => json
            string mapHeadJson = Read(mapName + HeadSuffix);

            // 2. 将json反序列化为数据 Dictionary<string, ValueData>, string为数值类型
            // json => data
            MapData mapData = Newtonsoft.Json.JsonConvert.DeserializeObject<MapData>(
                mapHeadJson, setting
            );

            // 3. 从数据中同步到地图对象中
            // data => obj
            if (!mapData.type.Equals(mapName)) {
                throw new Exception("地图存档类型与读取方式不一致");
            }
            IValues mapValues = Values.FromData(mapData.values);
            if (mapValues == null) throw new Exception();
            map.SetValues(mapValues);
            IRefs mapRefs = Refs.FromData(mapData.references);
            if (mapRefs == null) throw new Exception();
            map.SetRefs(mapRefs);
            IInventory mapInventory = Inventory.FromData(
                mapData.inventory, 
                mapData.inventory_quantity,
                mapData.inventory_capacity, 
                mapData.inventory_type_capacity);
            if (mapInventory == null) throw new Exception();
            map.SetInventory(mapInventory);

            // 4. 休息一下
            // 5. 再休息一下

            // 6. 读取对应位置地块json存档
            // file => json
            string mapBodyJson = Read(mapName);
            // 7. 将json反序列化为数据 Dictionary<string, TileData>, string为位置, TileData包含类型和值
            // json => data
            Dictionary<string, TileData> mapBodyData = Newtonsoft.Json.JsonConvert.DeserializeObject<
                Dictionary<string, TileData>>(mapBodyJson, setting
                );
            // 8. 对于每一个地块，通过SetTile塞到地图里
            // map => obj
            List<ITileDefinition> tiles = new List<ITileDefinition>(mapBodyData.Count);
            foreach (var pair in mapBodyData) {
                Vector2Int pos = DeserializeVector2(pair.Key);
                TileData tileData = pair.Value;
                Type tileType = Type.GetType(tileData.type);
                ITileDefinition tile = Activator.CreateInstance(tileType) as ITileDefinition;
                if (tile == null) throw new Exception();

                tile.Pos = pos;
                tile.Map = map;

                IValues tileValues = Values.FromData(tileData.values);
                // if (tileValues == null) throw new Exception();
                tile.SetValues(tileValues);
                IRefs tileRefs = Refs.FromData(tileData.references);
                // if (tileRefs == null) throw new Exception();
                map.SetRefs(tileRefs);

                map.SetTile(pos, tile);
                tiles.Add(tile);
            }
            map.OnEnable();
            foreach (var tile in tiles) {
                tile.OnEnable();
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

