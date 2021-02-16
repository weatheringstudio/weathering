﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class ScreenAdaptation : MonoBehaviour
    {
        private void Start() {
            SyncUICameraOrthographicSizeWithScreenSize();
        }

        public const bool DoubleSize = true;
        public const float RefOrthographcSize = 5.625f;
        private void SyncUICameraOrthographicSizeWithScreenSize() {
            screenWidthLastTime = Screen.width;
            screenHeightLastTime = Screen.height;

            const int sizeScale = DoubleSize ? 2 : 1;

            int screenScale = UI.DefaultHeight * Math.Min(Screen.width / UI.DefaultWidth, Screen.height / UI.DefaultHeight);
            if (screenScale < 1) screenScale = 1; // tiny screen

            if (screenScale == Screen.height) {
                (UI.Ins as UI).CameraSize = RefOrthographcSize;
                MapView mapView = MapView.Ins as MapView;
                // 上下渲染8个格子
                mapView.CameraHeightHalf = 8 * sizeScale;
                mapView.CameraWidthHalf = ((int)(8f * Screen.width / Screen.height) + 1) * sizeScale;
                mapView.CameraSize = RefOrthographcSize * sizeScale;
            } else {
                float newSize = RefOrthographcSize * Screen.height / screenScale;
                (UI.Ins as UI).CameraSize = newSize;
                MapView mapView = MapView.Ins as MapView;
                // 左右渲染11个格子
                mapView.CameraWidthHalf = 11 * sizeScale;
                mapView.CameraHeightHalf = ((int)(11f * Screen.height / Screen.width) + 1) * sizeScale;
                (MapView.Ins as MapView).CameraSize = newSize * sizeScale;
            }
        }
        private int screenWidthLastTime;
        private int screenHeightLastTime;

        private void Update() {
            if (Screen.width != screenWidthLastTime || Screen.height != screenHeightLastTime) {
                SyncUICameraOrthographicSizeWithScreenSize();
            }
        }
    }
}

