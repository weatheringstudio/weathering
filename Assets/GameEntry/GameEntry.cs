
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
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
            // 1. DataPersistence
            // 1. Globals
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
                GameConfig.OnGameConstruct(globals);
            }
        }

        private IGlobalsDefinition globals;

        private void Start() {
            // 读取或创建Globals.Ins.Refs.Get<GameEntry>().Type。实例在MapView.Ins.Map
            Type activeMapType = globals.Refs.GetOrCreate<GameEntry>().Type;
            if (activeMapType != null) {
                EnterMap(activeMapType);
            } else {
                GameConfig.OnGameConstruct();

                // 初始地图
                Type initialMap = GameConfig.InitialMap;
                if (initialMap == null) throw new Exception("需要配置GameConfig.InitialMap，确定初始地图");
                if (!typeof(IMapDefinition).IsAssignableFrom(initialMap)) throw new Exception($"初始地图{initialMap.Name}未实现接口${typeof(IMapDefinition).Name}");
                EnterMap(initialMap);
            }
            lastSaveTimeInSeconds = TimeUtility.GetTicks(); // 自动存档间隔
            GameConfig.OnGameEnable();
        }

        public void EnterMap(Type type) { // 换地图
            if (type == null) throw new Exception();

            // 目前"活跃地图"以"MapView.Ins.Map"访问
            IMap oldMap = MapView.Ins.TheOnlyActiveMap;
            if (oldMap != null) {
                if (oldMap.GetType() == type) {
                    throw new Exception("不应前往相同地图");
                }

                globals.Refs.GetOrCreate<GameEntry>().Type = type;
                SaveGame(); // 读新图前，保存
            }

            IMapDefinition newMap = Activator.CreateInstance(type) as IMapDefinition;
            if (newMap == null) throw new Exception();
            MapView.Ins.TheOnlyActiveMap = newMap;

            // 每个IMap实例的FullName对应一个存档里有这个地图
            if (data.HasMap(type)) {
                data.LoadMap(newMap);
            } else {
                newMap.OnConstruct();
                newMap.OnEnable();
                GenerateMap(newMap);
                newMap.AfterGeneration();
            }
        }
        private void GenerateMap(IMapDefinition map) {
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    Type tileType = map.GenerateTileType(new Vector2Int(i, j)); // 每个地图自己决定在ij生成什么地块
                    if (tileType == null) throw new Exception();
                    ITileDefinition tile = Activator.CreateInstance(tileType) as ITileDefinition;
                    if (tile == null) throw new Exception(tileType.Name);
                    map.SetTile(new Vector2Int(i, j), tile);
                    tile.Map = map;
                    tile.Pos = new Vector2Int(i, j);
                    tile.HashCode = HashUtility.Hash(i, j, map.Width, map.Height); //HashUtility.Hash((uint)(i + j * map.Width));
                    tile.OnConstruct();
                }
            }
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    ITileDefinition tile = map.Get(i, j) as ITileDefinition;
                    if (tile == null) throw new Exception();
                    tile.OnEnable();
                }
            }
        }

        // 每60秒自动存档
        public const int AutoSaveInSeconds = 60;
        private long lastSaveTimeInSeconds = 0;
        private IDataPersistence data;
        private void Update() {
            GameConfig.OnGameUpdate();
            long now = TimeUtility.GetSeconds();
            if (TimeUtility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSeconds) {
                SaveGame();
                lastSaveTimeInSeconds = now;
            }
        }

        // 关闭窗口时，每30秒自动存档
        public const int AutoSaveInSecondsWhenCloseWindow = 30;
        public void TrySaveGame() {
            long now = TimeUtility.GetSeconds();
            if (now - lastSaveTimeInSeconds > AutoSaveInSecondsWhenCloseWindow) {
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
            if (!DataPersistence.Ins.HasSave(save_complete)) {
                DataPersistence.Ins.WriteSave(save_complete, TimeUtility.GetTicks().ToString());
            }
            if (DataPersistence.Ins.ReadSave(save_complete).StartsWith(incomplete)) {
                throw new Exception("存档损坏");
            }
            DataPersistence.Ins.WriteSave(save_complete, $"{incomplete} {TimeUtility.GetTicks()}");
            data.SaveGlobals();
            data.SaveMap(map); // 保存地图
            lastSaveTimeInSeconds = TimeUtility.GetSeconds();
            // 结束存档
            DataPersistence.Ins.WriteSave(save_complete, TimeUtility.GetTicks().ToString());
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

