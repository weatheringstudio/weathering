
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CameraX { }
    public class CameraY { }

    public interface IGameEntry
    {
        void GotoMap(Type type);
        void Save();
        void TrySave();
        void DeleteSave();
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

        private IGlobalsDefinition globalsDefinition;
        private void Start() {
            data = DataPersistence.Ins;
            globalsDefinition = (Globals.Ins as IGlobalsDefinition);
            if (globalsDefinition == null) throw new Exception();

            // 读取或创建Globals.Ins
            if (data.HasGlobals()) {
                data.LoadGlobals();
            } else {
                globalsDefinition.ValuesInternal = Values.Create();
                globalsDefinition.RefsInternal = Refs.Create();
            }

            // 读取或创建Globals.Ins.Refs.Get<GameEntry>().Type。实例在MapView.Ins.Map
            Type activeMapType = globalsDefinition.Refs.Get<GameEntry>().Type;
            if (activeMapType != null) {
                GotoMap(activeMapType);
            } else {
                GotoMap(initialMap);
            }
        }

        private float cameraRatio = 1000f;
        public void GotoMap(Type type) { // 换地图
            // 目前"活跃地图"以"Map.Ins.Map"访问
            if (MapView.Ins.Map != null) {
                if (MapView.Ins.Map.GetType() == type) {
                    Debug.LogWarning("same map");
                    return;
                }
                // 读新图前，保存
                Save();
            }

            IMapDefinition map = Activator.CreateInstance(type) as IMapDefinition;
            if (map == null) throw new Exception();

            // 每个IMap实例的FullName对应一个存档里有这个地图
            if (data.HasMap(type)) {
                data.LoadMap(map);
            } else {
                map.OnEnable();
                map.OnConstruct();
            }

            // 读取相机位置
            long cameraX = globalsDefinition.Values.Get<CameraX>().Max;
            long cameraY = globalsDefinition.Values.Get<CameraY>().Max;
            MapView.Ins.CameraPosition = new Vector2(cameraX / cameraRatio, cameraY / cameraRatio);

            MapView.Ins.Map = map; // 每帧渲染入口
            globalsDefinition.Refs.Get<GameEntry>().Type = map.GetType();
        }

        // 每120秒自动存档
        public const int AutoSaveInSeconds = 120;
        private long lastSaveTimeInSeconds = 0;
        private IDataPersistence data;
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Save();
            }
            long now = Utility.GetSeconds();
            if (Utility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSeconds) {
                Save();
                lastSaveTimeInSeconds = now;
            }
        }

        // 关闭窗口时，每10秒自动存档
        public const int AutoSaveInSecondsWhenCloseWindow = 30;
        public void TrySave() {
            long now = Utility.GetSeconds();
            if (Utility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSecondsWhenCloseWindow) {
                Save();
                lastSaveTimeInSeconds = now;
            }
        }

        // 存档
        public void Save() {
            IMapDefinition map = MapView.Ins.Map as IMapDefinition;
            if (map == null) throw new Exception();
            // map.OnDisable(); // 存档前调用OnDisable，保存相机位置
            globalsDefinition.Values.Get<CameraX>().Max = (long)(MapView.Ins.CameraPosition.x * cameraRatio);
            globalsDefinition.Values.Get<CameraY>().Max = (long)(MapView.Ins.CameraPosition.y * cameraRatio);
            data.SaveGlobals();
            data.SaveMap(MapView.Ins.Map); // 保存地图
            lastSaveTimeInSeconds = Utility.GetSeconds();
            Debug.Log("<color=yellow>Save OK</color>");
        }

        // 删除存档
        public void DeleteSave() {
            data.DeleteSaves();
            Debug.Log("<color=red>Deleted</color>");
            ExitGame();
        }

        public void ExitGame() {
            Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                                Application.Quit();
#endif
        }
    }
}

