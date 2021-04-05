
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public interface IGameEntry
    {
        void EnterParentMap(Type parentType, IMap map);
        void EnterChildMap(Type childType, IMap map, Vector2Int pos);
        ISavable GetParentMap(Type parentType, IMap map);

        void SaveGame();
        void TrySaveGame();
        void DeleteGameSave();

        void ExitGame();
        void ExitGameUnsaved();
    }

    public class GameAutoSaveInterval { }

    public class GameEntry : MonoBehaviour, IGameEntry
    {


        public static IGameEntry Ins { get; private set; }

        private void Awake() {
            // 单例
            if (Ins != null) throw new Exception();
            Ins = this;

            // 游戏在后台不会占用资源
            Application.runInBackground = false;

            // 依赖注入到GameMenu
            if (GameMenu.Entry != null) throw new Exception();
            GameMenu.Entry = this;

            // Awake加载顺序
            // 1. AttributePreprocessor
            // 2. DataPersistence
            // 3. Globals
            // 4. GameEntry
            data = DataPersistence.Ins;
            globals = (Globals.Ins as IGlobalsDefinition);
            if (globals == null) throw new Exception();

            // 读取或创建Globals.Ins
            if (data.HasGlobals()) {
                data.LoadGlobals();
            } else {
                globals.ValuesInternal = Values.GetOne();
                globals.RefsInternal = Refs.GetOne();
                globals.PlayerPreferencesInternal = new Dictionary<string, string>();
                globals.InventoryInternal = Inventory.GetOne();

                constructGameThisTime = true;
            }
        }

        private const string gameEntryMapKey = "__GAME_ENTRY_INDEX__";
        private bool constructGameThisTime = false;
        private IGlobalsDefinition globals;
        private IValue autoSaveInterval;
        private void Start() {
            // 读取或创建自动存档间隔
            if (constructGameThisTime) {

                GameConfig.OnConstruct(globals); // 全局游戏逻辑相关
                GameMenu.OnConstruct(); // 全局游戏设置相关

                // 自动存档功能也在GameEntry里负责
                autoSaveInterval = Globals.Ins.Values.Create<GameAutoSaveInterval>();
                autoSaveInterval.Max = 60;
                if (autoSaveInterval.Max < 10) throw new Exception(); // 太短了吧
            }
            autoSaveInterval = Globals.Ins.Values.Get<GameAutoSaveInterval>();


            if (globals.PlayerPreferences.TryGetValue(gameEntryMapKey, out string mapKey)) {
                // 如果Globals记录了了之前的地图，则直接进入
                EnterMap(mapKey);
            } else {
                // 如果Globals没有进入之前的地图，进入指定地图
                globals.PlayerPreferences.Add(gameEntryMapKey, null);
                EnterMap(typeof(MapOfUniverse), $"");
            }
            lastSaveTimeInSeconds = TimeUtility.GetSeconds(); // 自动存档间隔
            GameConfig.OnGameEnable();
        }

        public const string MAGIC = "#";
        public const char MAGIC_CHAR = '#';
        public const char MAGIC_CHAR2 = '=';
        private static string ParentMapKeyIndex(string mapKey) {
            int startIndex = mapKey.IndexOf(MAGIC);
            int endIndex = mapKey.LastIndexOf(MAGIC_CHAR2);
            if (endIndex < 0) return MAGIC;
            return mapKey.Substring(startIndex, endIndex - startIndex);
        }
        private static string ParentMapKey(Type parentType, string selfIndex) {
            return $"{parentType.FullName}{ParentMapKeyIndex(selfIndex)}";
        }
        public void EnterParentMap(Type parentType, IMap map) {
            IMapDefinition mapDefinition = map as IMapDefinition;
            if (mapDefinition == null) throw new Exception();
            string mapKey = mapDefinition.MapKey;

            string parentMapKey = ParentMapKey(parentType, mapKey);
            if (parentMapKey == null) throw new Exception(mapKey);
            EnterMap(parentMapKey);
        }
        private static string SelfMapKeyIndex(string mapKey) {
            int startIndex = mapKey.IndexOf(MAGIC_CHAR);
            if (startIndex < 0) return MAGIC;
            return mapKey.Substring(startIndex, mapKey.Length - startIndex);
        }
        public void EnterChildMap(Type childType, IMap map, Vector2Int pos) {
            IMapDefinition mapDefinition = map as IMapDefinition;
            if (mapDefinition == null) throw new Exception();
            string mapKey = mapDefinition.MapKey;

            string selfMapKeyIndex = SelfMapKeyIndex(mapKey);
            string childMapKey = $"{childType.FullName}{selfMapKeyIndex}{MAGIC_CHAR2}{pos.x},{pos.y}";
            EnterMap(childMapKey);

            UI.Ins.Active = false;
        }
        // char '#' 是用魔法强耦合的
        private void EnterMap(string mapKey) {
            string[] args = mapKey.Split(MAGIC_CHAR);
            if (args.Length != 2) throw new Exception(mapKey);

            Type selfType = Type.GetType(args[0]);
            if (selfType == null) throw new Exception(mapKey);
            string selfIndex = args[1];

            EnterMap(selfType, selfIndex, mapKey);

            UI.Ins.Active = false;
        }

        private void EnterMap(Type selfType, string selfIndex, string selfKeyVertify = null) {
            if (selfType == null) throw new Exception();
            if (selfIndex == null) throw new Exception();

            // mapKey由四部分组成
            string mapKey = $"{selfType.FullName}{MAGIC_CHAR}{selfIndex}";
            if (selfKeyVertify != null && !selfKeyVertify.Equals(mapKey)) throw new Exception();

            // 目前"活跃地图"以"MapView.Ins.Map"访问
            IMap oldMap = MapView.Ins.TheOnlyActiveMap;
            if (oldMap != null) {
                SaveGame(); // 读新图前，保存
            }


            IMapDefinition map = Activator.CreateInstance(selfType) as IMapDefinition;
            if (map == null) throw new Exception(mapKey);
            map.MapKey = mapKey;
            map.HashCode = HashUtility.Hash(mapKey);

            MapView.Ins.TheOnlyActiveMap = map;

            // 每个IMap实例的MapKey对应一个存档里有这个地图
            if (data.HasMap(mapKey)) {
                data.LoadMapHead(map, mapKey);
                map.OnEnable();
                data.LoadMapBody(map, mapKey);
            } else {
                map.OnConstruct();
                map.OnEnable();
                ConstructMapBody(map);
                // newMap.AfterGeneration();
            }

            // 记录当前地图
            globals.PlayerPreferences[gameEntryMapKey] = mapKey;

#if UNITY_EDITOR
            // Debug.LogWarning($"Enter Map {selfKey}");
#endif
        }

        private Dictionary<string, IMapDefinition> otherMaps = new Dictionary<string, IMapDefinition>();
        public ISavable GetParentMap(Type parentType, IMap map) {
            if (map != MapView.Ins.TheOnlyActiveMap) throw new Exception();
            IMapDefinition mapDefinition = map as IMapDefinition;
            if (mapDefinition == null) throw new Exception();

            string parentMapKey = ParentMapKey(parentType, mapDefinition.MapKey);

            IMapDefinition parentMap;
            if (otherMaps.TryGetValue(parentMapKey, out parentMap)) {

            }
            else {
                parentMap = Activator.CreateInstance(parentType) as IMapDefinition;
                if (parentMap == null) throw new Exception(parentType.Name);
                parentMap.MapKey = parentMapKey;
                parentMap.HashCode = HashUtility.Hash(parentMapKey);

                // 每个IMap实例的MapKey对应一个存档里有这个地图
                if (data.HasMap(parentMapKey)) {
                    data.LoadMapHead(parentMap, parentMapKey);
                    parentMap.OnEnable();
                } else {
                    parentMap.OnConstruct();
                    parentMap.OnEnable();
                }
                otherMaps.Add(parentMapKey, parentMap);
            }
            if (otherMaps.Count > 1) throw new Exception(); // 目前只支持影响parentmap，所以otherMaps.Count必须小于等于1。可能以后会更新其他功能
            return parentMap;
        }


        private void ConstructMapBody(IMapDefinition map) {
            Type tileType = map.DefaultTileType;
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    // Type tileType = map.GenerateTileType(new Vector2Int(i, j)); // 每个地图自己决定在ij生成什么地块
                    if (tileType == null) throw new Exception();
                    ITileDefinition tile = Activator.CreateInstance(tileType) as ITileDefinition;
                    if (tile == null) throw new Exception(tileType.Name);
                    map.SetTile(new Vector2Int(i, j), tile, true);
                    tile.Map = map;
                    tile.Pos = new Vector2Int(i, j);
                    tile.HashCode = HashUtility.Hash(i, j, map.Width, map.Height, (int)map.HashCode); //HashUtility.Hash((uint)(i + j * map.Width));
                    tile.OnConstruct();
                }
            }
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    ITileDefinition tile = map.Get(i, j) as ITileDefinition;
                    if (tile == null) throw new Exception();
                    tile.NeedUpdateSpriteKeys = true;
                    tile.OnEnable();
                }
            }
        }

        private long lastSaveTimeInSeconds = 0;
        private IDataPersistence data;
        private void Update() {
            long now = TimeUtility.GetSeconds();
            long delta = now - lastSaveTimeInSeconds;
            if (delta > 5 * autoSaveInterval.Max) {
                SaveGame();
                lastSaveTimeInSeconds = now;
            }
        }

        public void TrySaveGame() {
            long now = TimeUtility.GetSeconds();
            long delta = now - lastSaveTimeInSeconds;
            if (delta > autoSaveInterval.Max) {
                SaveGame();
                lastSaveTimeInSeconds = now;
            }
        }

        // 存档
        public void SaveGame() {
            IMapDefinition map = MapView.Ins.TheOnlyActiveMap as IMapDefinition;
            if (map == null) throw new Exception();
            map.OnDisable();



            // 开始存档
            const string save_complete = "__save_complete__";
            const string incomplete = "incomplete";

            // 损坏校验
            if (!DataPersistence.Ins.HasSave(save_complete)) {
                DataPersistence.Ins.WriteSave(save_complete, TimeUtility.GetTicks().ToString());
            }
            if (DataPersistence.Ins.ReadSave(save_complete).StartsWith(incomplete)) {
                throw new Exception("存档损坏");
            }
            DataPersistence.Ins.WriteSave(save_complete, $"{incomplete} {TimeUtility.GetTicks()}");

            // 存档
            data.SaveGlobals();
            data.SaveMapHead(map); // 保存地图
            data.SaveMapBody(map);
            foreach (var pair in otherMaps) {
                data.SaveMapHead(pair.Value);
            }

            lastSaveTimeInSeconds = TimeUtility.GetSeconds();

            // 结束存档
            DataPersistence.Ins.WriteSave(save_complete, TimeUtility.GetTicks().ToString());
#if UNITY_EDITOR
            Debug.LogWarning("Save OK");
#endif
        }

        // 删除存档
        public void DeleteGameSave() {
            data.DeleteSaves();
            // Debug.Log("<color=red>Save Deleted</color>");
            ExitGameUnsaved();
        }

        public void ExitGame() {
            SaveGame();
            ExitGameInternal();
        }

        public void ExitGameUnsaved() {
            ExitGameInternal();
        }

        private void ExitGameInternal(bool save = true) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                                Application.Quit();
#endif
        }
    }
}

