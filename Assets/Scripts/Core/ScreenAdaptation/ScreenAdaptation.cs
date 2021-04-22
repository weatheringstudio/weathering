
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class ScreenAdaptation : MonoBehaviour
    {
        [SerializeField]
        private Material MainMat;

        public static ScreenAdaptation Ins { get; private set; }
        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
        }

        public int Scale { get; private set; }
        private void Start() {
            SyncUICameraOrthographicSizeWithScreenSize();

            MapView mapView = MapView.Ins as MapView;
            Vector2Int size = CalcRecommendedSize(out int scale);
            Scale = scale;

            var rt = new RenderTexture(size.x, size.y, -1);
            rt.name = $"{size.x}x{size.y}";
            rt.filterMode = FilterMode.Point;

            (UI.Ins as UI).Raw = rt;
            mapView.TargetTexture = rt;


            Vector2 camScale = new Vector2(20 * ((float)size.x / 320), 11.25f * ((float)size.y / 180));
            // mapView.OnlyThingScale = scale;
            mapView.CameraSize = camScale.y / 2;

            MainMat.SetTexture("_MainTex", rt);
        }

        private Vector2Int CalcRecommendedSize(out int scale) {
            int width = Screen.width;
            int height = Screen.height;
            scale = 1;
            while (width / 2 >= 320 && height / 2 >= 180) {
                width /= 2;
                height /= 2;
                scale *= 2;
            }
            return new Vector2Int(width, height);
        }

        //public class DoubleSizeOption { }

        //private bool doubleSize = false;
        //public bool DoubleSize { get { return doubleSize; } set { doubleSize = value; SyncUICameraOrthographicSizeWithScreenSize(); } }
        //public int DoubleSizeMultiplier { get => DoubleSize ? 2 : 1; }
        //public const float RefOrthographcSize = 5.625f;
        public int DoubleSizeMultiplier { get => 1; }

        public int SizeScale { get; private set; } = 1;
        private void SyncUICameraOrthographicSizeWithScreenSize() {
            //screenWidthLastTime = Screen.width;
            //screenHeightLastTime = Screen.height;

            // SizeScale = DoubleSize ? 2 : 1;

            int screenScale = Math.Min(Screen.width / UI.DefaultWidth, Screen.height / UI.DefaultHeight);
            if (screenScale < 1) screenScale = 1; // tiny screen

            MapView mapView = MapView.Ins as MapView;

            mapView.CameraHeightHalf = Math.Min(12, (7 + (Screen.height - UI.DefaultHeight * screenScale) / UI.DefaultPPU)) * SizeScale;
            mapView.CameraWidthHalf = Math.Min(21, (12 + (Screen.width - UI.DefaultWidth * screenScale) / UI.DefaultPPU)) * SizeScale;

            // float newSize = RefOrthographcSize * Screen.height / (UI.DefaultHeight * screenScale);
            // (UI.Ins as UI).CameraSize = newSize;

            var ui = (UI.Ins as UI).GetComponent<UnityEngine.UI.CanvasScaler>();
            ui.scaleFactor = screenScale;
        }

        //private int screenWidthLastTime;
        //private int screenHeightLastTime;

        //private const string mouseScrollWheel = "Mouse ScrollWheel";
        //private float scale = 1f;
        //private void Update() {
        //    if (Screen.width != screenWidthLastTime || Screen.height != screenHeightLastTime) {
        //        SyncUICameraOrthographicSizeWithScreenSize();
        //    }
        //    if (GameMenu.IsInStandalone) {
        //        if (!UI.Ins.Active) {
        //            // PC上会根据鼠标滚轮进行一定缩放
        //            float dv = Input.GetAxisRaw(mouseScrollWheel);
        //            if (dv != 0) {
        //                scale -= dv * Time.deltaTime * 100;
        //                scale = Mathf.Clamp(scale, 0.5f, 1);
        //                SyncUICameraOrthographicSizeWithScreenSize();
        //            }
        //        }
        //    }
        //}
    }
}

