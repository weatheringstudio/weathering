
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
            dataPersistence = DataPersistence.Ins;

            if (dataPersistence.HasFile(ActiveMapFilename)) {
                string activeMapType = dataPersistence.Read(ActiveMapFilename);
                IMap map = dataPersistence.LoadMap(activeMapType);
                MapView.Ins.Map = map;
            } else {
                var map = new InitialMap();
                map.Initialize();
                map.OnConstruct();
                MapView.Ins.Map = map;
            }
            lastSaveTimeInSeconds = Utility.GetSeconds();
        }

        public const int AutoSaveInSeconds = 120;
        private long lastSaveTimeInSeconds = 0;
        private IDataPersistence dataPersistence;
        private void Update() {
            try {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Save();
                }
                long now = Utility.GetSeconds();
                if (Utility.GetSeconds() - lastSaveTimeInSeconds > AutoSaveInSeconds) {
                    Save();
                    lastSaveTimeInSeconds = now;
                }
            }
            catch (System.Exception e) {
                UI.Ins.Error(e);
            }

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
            try {
                IMap map = MapView.Ins.Map;
                DataPersistence.Ins.SaveMap(MapView.Ins.Map);
                dataPersistence.Write(ActiveMapFilename, map.GetType().FullName);
                Debug.Log("<color=yellow>Save OK</color>");
            } catch (System.Exception e) {
                UI.Ins.Error(e);
            }
        }

        public void DeleteSave() {
            try {
                dataPersistence.DeleteSaves();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                Debug.Log("<color=red>Deleted</color>");
            } catch (System.Exception e) {
                UI.Ins.Error(e);
            }

        }
    }
}

