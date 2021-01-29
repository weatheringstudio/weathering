
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class GameSaveComplete { }

    public interface IGameEntry
    {
        void EnterMap(Type type);
        void SaveGame();
        void TrySaveGame();
        void DeleteGameSave();

        void ExitGame();
        void ExitGameUnsaved();
    }

    public class GameEntry : MonoBehaviour, IGameEntry
    {
        private Type initialMap = typeof(IslandMap);

        public static IGameEntry Ins;
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
            Application.runInBackground = false;

            data = DataPersistence.Ins;
            globals = (Globals.Ins as IGlobalsDefinition);
            if (globals == null) throw new Exception();

            BeforeGameStart?.Invoke();

            // 读取或创建Globals.Ins
            if (data.HasGlobals()) {
                data.LoadGlobals();
            } else {
                globals.ValuesInternal = Values.GetOne();
                globals.RefsInternal = Refs.GetOne();
                globals.PlayerPreferencesInternal = new Dictionary<string, string>();
                GlobalGameEvents.OnGameConstruct(globals);
            }
        }

        public static event Action BeforeGameStart;

        private IGlobalsDefinition globals;

        private void Start() {
            // 读取或创建Globals.Ins.Refs.Get<GameEntry>().Type。实例在MapView.Ins.Map
            Type activeMapType = globals.Refs.GetOrCreate<GameEntry>().Type;
            if (activeMapType != null) {
                EnterMap(activeMapType);
            } else {
                EnterMap(initialMap);
            }
            lastSaveTimeInSeconds = TimeUtility.GetTicks();
        }

        public void EnterMap(Type type) { // 换地图
            // 目前"活跃地图"以"Map.Ins.Map"访问
            if (MapView.Ins.Map != null) {
                IMapDefinition oldMap = MapView.Ins.Map as IMapDefinition;
                if (oldMap == null) throw new Exception();
                if (oldMap.GetType() == type) {
                    // Debug.LogWarning("same map");
                    return;
                }

                SaveGame(); // 读新图前，保存
            }

            IMapDefinition map = Activator.CreateInstance(type) as IMapDefinition;
            if (map == null) throw new Exception();

            // 每个IMap实例的FullName对应一个存档里有这个地图
            if (data.HasMap(type)) {
                data.LoadMap(map);
            } else {
                map.OnConstruct();
                map.OnEnable();
                GenerateMap(map);
                map.AfterGeneration();
            }

            MapView.Ins.Map = map; // 每帧渲染入口
            globals.Refs.GetOrCreate<GameEntry>().Type = map.GetType(); // 记录活跃地图
        }
        private void GenerateMap(IMapDefinition map) {
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    Type tileType = map.Generate(new Vector2Int(i, j)); // 每个地图自己决定在ij生成什么地块
                    if (tileType == null) throw new Exception();
                    ITileDefinition tile = Activator.CreateInstance(tileType) as ITileDefinition;
                    if (tile == null) throw new Exception(tileType.Name);
                    map.SetTile(new Vector2Int(i, j), tile);
                    tile.Map = map;
                    tile.Pos = new Vector2Int(i, j);
                    tile.HashCode = HashUtility.Hash((uint)(i + j * map.Width));
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
            Test.OnUpdate();
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
            if (TimeUtility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSecondsWhenCloseWindow) {
                SaveGame();
                lastSaveTimeInSeconds = now;
            }
        }

        // 存档
        public void SaveGame() {
            IMapDefinition map = MapView.Ins.Map as IMapDefinition;
            if (map == null) throw new Exception();
            map.OnDisable();

            // 开始存档
            const string save_complete = "save_complete";
            const string incomplete = "incomplete";
            if (!DataPersistence.Ins.HasSave(save_complete)) {
                DataPersistence.Ins.WriteSave(save_complete, TimeUtility.GetTicks().ToString());
            }
            if (DataPersistence.Ins.ReadSave(save_complete).StartsWith(incomplete)) {
                throw new Exception("存档损坏");
            }
            DataPersistence.Ins.WriteSave(save_complete, $"{incomplete} {TimeUtility.GetTicks().ToString()}");
            data.SaveGlobals();
            data.SaveMap(MapView.Ins.Map); // 保存地图
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

