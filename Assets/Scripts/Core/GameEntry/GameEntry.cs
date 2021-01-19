
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CameraX { }
    public class CameraY { }

    public interface IGameEntry
    {
        void EnterMap(Type type);
        void SaveGame();
        void TrySaveGame();
        void DeleteGameSave();
        void ExitGame();
    }

    public class GameEntry : MonoBehaviour, IGameEntry
    {
        private Type initialMap = typeof(IslandMap);

        public static IGameEntry Ins;
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
            Application.runInBackground = false;
        }

        private IGlobalsDefinition globals;
        private void Start() {
            data = DataPersistence.Ins;
            globals = (Globals.Ins as IGlobalsDefinition);
            if (globals == null) throw new Exception();

            // 读取或创建Globals.Ins
            if (data.HasGlobals()) {
                data.LoadGlobals();
            } else {
                globals.ValuesInternal = Values.Create();
                globals.RefsInternal = Refs.Create();
                GlobalGameEvents.OnGameConstruct(globals);
            }

            // 读取或创建Globals.Ins.Refs.Get<GameEntry>().Type。实例在MapView.Ins.Map
            Type activeMapType = globals.Refs.Get<GameEntry>().Type;
            if (activeMapType != null) {
                EnterMap(activeMapType);
            } else {
                EnterMap(initialMap);
            }
            lastSaveTimeInSeconds = Utility.GetTicks();
        }

        public void EnterMap(Type type) { // 换地图
            // 目前"活跃地图"以"Map.Ins.Map"访问
            if (MapView.Ins.Map != null) {
                IMapDefinition oldMap = MapView.Ins.Map as IMapDefinition;
                if (oldMap == null) throw new Exception();
                if (oldMap.GetType() == type) {
                    Debug.LogWarning("same map");
                    return;
                }
                
                oldMap.OnDisable(); // 让地图被关闭时记录数据（如相机位置）
                SaveGame(); // 读新图前，保存
            }

            IMapDefinition map = Activator.CreateInstance(type) as IMapDefinition;
            if (map == null) throw new Exception();

            // 每个IMap实例的FullName对应一个存档里有这个地图
            if (data.HasMap(type)) {
                data.LoadMap(map);
            } else {
                map.OnEnable();
                map.OnConstruct();
                GenerateMap(map);
            }

            MapView.Ins.Map = map; // 每帧渲染入口
            globals.Refs.Get<GameEntry>().Type = map.GetType(); // 记录活跃地图
        }
        private void GenerateMap(IMapDefinition map) {
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    Type tileType = map.Generate(new Vector2Int(i, j)); // 每个地图自己决定在ij生成什么地块
                    ITileDefinition tile = Activator.CreateInstance(tileType) as ITileDefinition;
                    if (tile == null) throw new Exception();
                    map.SetTile(new Vector2Int(i, j), tile);
                    tile.Map = map;
                    tile.Pos = new Vector2Int(i, j);
                    tile.OnEnable();
                }
            }
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    ITileDefinition tileDefinition = map.Get(i, j) as ITileDefinition;
                    if (tileDefinition == null) throw new Exception();
                    tileDefinition.OnConstruct();
                }
            }
        }



        // 每120秒自动存档
        public const int AutoSaveInSeconds = 120;
        private long lastSaveTimeInSeconds = 0;
        private IDataPersistence data;
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SaveGame();
            }
            long now = Utility.GetSeconds();
            if (Utility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSeconds) {
                SaveGame();
                lastSaveTimeInSeconds = now;
            }
        }

        // 关闭窗口时，每10秒自动存档
        public const int AutoSaveInSecondsWhenCloseWindow = 30;
        public void TrySaveGame() {
            long now = Utility.GetSeconds();
            if (Utility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSecondsWhenCloseWindow) {
                SaveGame();
                lastSaveTimeInSeconds = now;
            }
        }

        // 存档
        public void SaveGame() {
            IMapDefinition map = MapView.Ins.Map as IMapDefinition;
            if (map == null) throw new Exception();

            data.SaveGlobals();
            data.SaveMap(MapView.Ins.Map); // 保存地图
            lastSaveTimeInSeconds = Utility.GetSeconds();
            Debug.Log("<color=yellow>Game Saved</color>");
        }

        // 删除存档
        public void DeleteGameSave() {
            data.DeleteSaves();
            Debug.Log("<color=red>Save Deleted</color>");
            ExitGame();
        }

        public void ExitGame() {
            SaveGame();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                                Application.Quit();
#endif
        }
    }
}

