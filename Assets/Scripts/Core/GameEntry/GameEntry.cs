
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
        }

        public const string ActiveMapFilename = "last_save";
        private void Start() {
            data = DataPersistence.Ins;

            if (data.HasFile(ActiveMapFilename)) {
                string activeMapTypeFullName = data.Read(ActiveMapFilename);
                GotoMap(Type.GetType(activeMapTypeFullName));
            } else {
                GotoMap(typeof(InitialMap));
            }
            lastSaveTimeInSeconds = Utility.GetSeconds();
        }


        public void GotoMap(Type type) {
            if (MapView.Ins.Map != null) {
                if (MapView.Ins.Map.GetType() == type) {
                    Debug.LogWarning("same map");
                    return;
                }
                data.SaveMap(MapView.Ins.Map);
            }

            if (data.HasMap(type)) {
                IMapDefinition map = Activator.CreateInstance(type) as IMapDefinition;
                data.LoadMap(map);
                MapView.Ins.Map = map;
            } else {
                IMapDefinition map = Activator.CreateInstance(type) as IMapDefinition;
                if (map == null) {
                    throw new Exception();
                }
                map.Initialize();
                map.OnConstruct();
                MapView.Ins.Map = map;
            }
            data.Write(ActiveMapFilename, MapView.Ins.Map.GetType().FullName);
        }

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

        public const int AutoSaveInSecondsWhenCloseWindow = 30;
        public void TrySave() {
            long now = Utility.GetSeconds();
            if (Utility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSecondsWhenCloseWindow) {
                Save();
                lastSaveTimeInSeconds = now;
            }
        }

        public void Save() {
            //try {
            IMap map = MapView.Ins.Map;
            DataPersistence.Ins.SaveMap(MapView.Ins.Map);
            data.Write(ActiveMapFilename, map.GetType().FullName);
            Debug.Log("<color=yellow>Save OK</color>");
            //} catch (System.Exception e) {
            //    UI.Ins.Error(e);
            //    throw e;
            //}
        }

        public void DeleteSave() {
            //try {
            data.DeleteSaves();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            Debug.Log("<color=red>Deleted</color>");
            //} catch (System.Exception e) {
            //    UI.Ins.Error(e);
            //    throw e;
            //}

        }
    }
}

