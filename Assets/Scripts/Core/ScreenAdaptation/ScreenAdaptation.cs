
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

        public const int RefHeight = 360;
        public const int RefWidth = 640;
        public const float RefOrthographcSize = 5.625f;
        private void SyncUICameraOrthographicSizeWithScreenSize() {
            screenWidthLastTime = Screen.width;
            screenHeightLastTime = Screen.height;

            int screenScale = RefHeight * Math.Min(Screen.width / RefWidth, Screen.height / RefHeight);
            if (screenScale < 1) screenScale = 1; // tiny screen

            if (screenScale == Screen.height) {
                (UI.Ins as UI).CameraSize = RefOrthographcSize;
                (MapView.Ins as MapView).CameraSize = RefOrthographcSize;
            } else {
                float newSize = RefOrthographcSize * Screen.height / screenScale;
                (UI.Ins as UI).CameraSize = newSize;
                (MapView.Ins as MapView).CameraSize = newSize;
            }
        }
        private int screenWidthLastTime;
        private int screenHeightLastTime;
        private void TrySyncUICameraOrthographicSizeWithScreenSize() {
            if (Screen.width != screenWidthLastTime || Screen.height != screenHeightLastTime) {
                SyncUICameraOrthographicSizeWithScreenSize();
            }
        }

        private void Update() {
            TrySyncUICameraOrthographicSizeWithScreenSize();
        }
    }
}

