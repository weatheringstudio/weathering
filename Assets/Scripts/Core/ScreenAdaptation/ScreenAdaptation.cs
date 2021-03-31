
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class ScreenAdaptation : MonoBehaviour
    {
        public static ScreenAdaptation Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        private void Start() {
            SyncUICameraOrthographicSizeWithScreenSize();
        }

        public class DoubleSizeOption { }

        private bool doubleSize = false;
        public bool DoubleSize { get { return doubleSize; } set { doubleSize = value; SyncUICameraOrthographicSizeWithScreenSize(); } }
        public int DoubleSizeMultiplier { get => DoubleSize ? 2 : 1; }
        public const float RefOrthographcSize = 5.625f;
        public int SizeScale { get; private set; }
        private void SyncUICameraOrthographicSizeWithScreenSize() {
            screenWidthLastTime = Screen.width;
            screenHeightLastTime = Screen.height;

            SizeScale = DoubleSize ? 2 : 1;

            int screenScale = Math.Min(Screen.width / UI.DefaultWidth, Screen.height / UI.DefaultHeight);
            if (screenScale < 1) screenScale = 1; // tiny screen


            MapView mapView = MapView.Ins as MapView;

            float newSize = RefOrthographcSize * Screen.height / (UI.DefaultHeight * screenScale);
            mapView.CameraHeightHalf = Math.Min(12, (7 + (Screen.height - UI.DefaultHeight * screenScale) / UI.DefaultPPU)) * SizeScale;
            mapView.CameraWidthHalf = Math.Min(21, (12 + (Screen.width - UI.DefaultWidth * screenScale) / UI.DefaultPPU)) * SizeScale;

            mapView.CameraSize = newSize * SizeScale * scale;

            (UI.Ins as UI).CameraSize = newSize;
        }
        private int screenWidthLastTime;
        private int screenHeightLastTime;

        private const string mouseScrollWheel = "Mouse ScrollWheel";
        private float scale = 1f;
        private void Update() {
            if (Screen.width != screenWidthLastTime || Screen.height != screenHeightLastTime) {
                SyncUICameraOrthographicSizeWithScreenSize();
            }
            if (GameMenu.IsInStandalone) {
                if (!UI.Ins.Active) {
                    // PC上会根据鼠标滚轮进行一定缩放
                    float dv = Input.GetAxisRaw(mouseScrollWheel);
                    if (dv != 0) {
                        scale -= dv * Time.deltaTime * 100;
                        scale = Mathf.Clamp(scale, 0.5f, 1);
                        SyncUICameraOrthographicSizeWithScreenSize();
                    }
                }
            }
        }
    }
}

