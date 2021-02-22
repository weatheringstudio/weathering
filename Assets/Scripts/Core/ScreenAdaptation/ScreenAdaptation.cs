
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
        private void SyncUICameraOrthographicSizeWithScreenSize() {
            screenWidthLastTime = Screen.width;
            screenHeightLastTime = Screen.height;

            int sizeScale = DoubleSize ? 2 : 1;

            int screenScale = UI.DefaultHeight * Math.Min(Screen.width / UI.DefaultWidth, Screen.height / UI.DefaultHeight);
            if (screenScale < 1) screenScale = 1; // tiny screen

            if (screenScale == Screen.height) {
                (UI.Ins as UI).CameraSize = RefOrthographcSize;
                MapView mapView = MapView.Ins as MapView;
                // 上下渲染8个格子
                mapView.CameraHeightHalf = 7 * sizeScale;
                mapView.CameraWidthHalf = ((int)(7f * Screen.width / Screen.height) + 1) * sizeScale;
                mapView.CameraSize = RefOrthographcSize * sizeScale * scale;
            } else {
                float newSize = RefOrthographcSize * Screen.height / screenScale;
                (UI.Ins as UI).CameraSize = newSize;
                MapView mapView = MapView.Ins as MapView;
                // 左右渲染11个格子
                mapView.CameraWidthHalf = 12 * sizeScale;
                mapView.CameraHeightHalf = ((int)((12f * Screen.height) / Screen.width) + 1) * sizeScale;
                (MapView.Ins as MapView).CameraSize = newSize * sizeScale * scale;
            }
        }
        private int screenWidthLastTime;
        private int screenHeightLastTime;

        private const string mouseScrollWheel = "Mouse ScrollWheel";
        private float scale = 1f;
        private void Update() {
            if (Screen.width != screenWidthLastTime || Screen.height != screenHeightLastTime) {
                SyncUICameraOrthographicSizeWithScreenSize();
            }
#if !UNITY_EDITOR && !UNITY_STANDALONE

#else
            if (!UI.Ins.Active) {
                // PC上会根据鼠标滚轮进行一定缩放
                float dv = Input.GetAxisRaw(mouseScrollWheel);
                if (dv != 0) {
                    scale -= dv * Time.deltaTime * 100;
                    scale = Mathf.Clamp(scale, 0.5f, 1);
                    SyncUICameraOrthographicSizeWithScreenSize();
                }
            }
#endif

        }
    }
}

