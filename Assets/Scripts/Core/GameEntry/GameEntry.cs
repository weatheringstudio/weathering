
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class GameEntry : MonoBehaviour
    {
        public static GameEntry Ins;
        private void Awake() {
            if (Ins != null) throw new System.Exception();
            Ins = this;
            Application.runInBackground = false;
        }

        public const string ActiveMapFilename = "last_save";
        private void Start() {
            data = DataPersistence.Ins;
            // 判断"ActiveMapFilename"这个文件是否存在，
            if (data.HasFile(ActiveMapFilename)) {
                // 如果存在，那么文件里的内容就是"活跃地图"
                string activeMapTypeFullName = data.Read(ActiveMapFilename);
                GotoMap(Type.GetType(activeMapTypeFullName));
            } else {
                // 如果不存在，那么激活"初始地图"
                GotoMap(typeof(InitialMap));
            }
            lastSaveTimeInSeconds = Utility.GetSeconds();
        }


        public void GotoMap(Type type) { // 换地图
            // 目前"活跃地图"以"Map.Ins.Map"访问
            if (MapView.Ins.Map != null) {
                if (MapView.Ins.Map.GetType() == type) {
                    Debug.LogWarning("same map");
                    return;
                }
                // 保存之前的"活跃地图"
                data.SaveMap(MapView.Ins.Map);
            }

            IMapDefinition map = Activator.CreateInstance(type) as IMapDefinition;

            // 每个IMap实例的FullName对应一个存档里有这个地图
            if (data.HasMap(type)) {
                // 创建地图，反序列化地图，设置活跃地图
                data.LoadMap(map); // LoadMap里会map.OnEnable() 初始化入口
                MapView.Ins.Map = map; // 更新入口
            } else {
                if (map == null) {
                    throw new Exception();
                }
                map.OnEnable(); // 初始化入口
                map.OnConstruct(); // 第一次创建的地图会调用OnConstruct()
            }
            data.Write(ActiveMapFilename, MapView.Ins.Map.GetType().FullName);
            MapView.Ins.Map = map; // 每帧渲染入口
            data.SaveMap(map); // 全部保存

        }

        // 每120秒自动存档
        public const int AutoSaveInSeconds = 120;
        private long lastSaveTimeInSeconds = 0;
        private IDataPersistence data;
        private void Update() {
            //try {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Save();
            }
            long now = Utility.GetSeconds();
            if (Utility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSeconds) {
                Save();
                lastSaveTimeInSeconds = now;
            }
            //}
            //catch (System.Exception e) {
            //    UI.Ins.Error(e);
            //    throw new Exception(e.StackTrace);
            //}

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
            //try {
            IMapDefinition map = MapView.Ins.Map as IMapDefinition;
            if (map == null) throw new Exception();
            map.OnDisable(); // 存档前调用OnDisable，保存相机位置
            DataPersistence.Ins.SaveMap(MapView.Ins.Map); // 保存地图
            data.Write(ActiveMapFilename, map.GetType().FullName); // 记录当前活跃地图是哪个
            Debug.Log("<color=yellow>Save OK</color>");
            //} catch (System.Exception e) {
            //    UI.Ins.Error(e);
            //    throw e;
            //}
        }

        // 删除存档
        public void DeleteSave() {
            //try {
            data.DeleteSaves();
            //#if UNITY_EDITOR
            //            UnityEditor.EditorApplication.isPlaying = false;
            //#else
            //        Application.Quit();
            //#endif
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            Debug.Log("<color=red>Deleted</color>");
            //} catch (System.Exception e) {
            //    UI.Ins.Error(e);
            //    throw e;
            //}

        }
    }
}

