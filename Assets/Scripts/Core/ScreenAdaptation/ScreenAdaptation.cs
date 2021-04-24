
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
        public Vector2 Size { get; private set; }
        private void Start() {
            RecalcSize();
        }

        private void RecalcSize() {
            Vector2Int size = CalcRecommendedSize(out int scale);
            Scale = scale;
            Size = size;

            SyncUICameraOrthographicSizeWithScreenSize();

            MapView mapView = MapView.Ins as MapView;


            var rt = new RenderTexture(size.x, size.y, -1);
            rt.name = $"{size.x}x{size.y}";
            rt.filterMode = FilterMode.Point;


            (UI.Ins as UI).Raw = rt;
            mapView.TargetTexture = rt;


            Vector2 camScale = new Vector2(20 * ((float)size.x / UI.DefaultWidth), 11.25f * ((float)size.y / UI.DefaultHeight));
            // mapView.OnlyThingScale = scale;
            mapView.CameraSize = camScale.y / 2;

            MainMat.SetTexture("_MainTex", rt);
        }

        private Vector2Int CalcRecommendedSize(out int scale) {
            int width = Screen.width;
            int height = Screen.height;
            scale = 1;
            if (width % 320 == 0 && height % 180 == 0) { // 16:9
                scale = width / 320;
                width = 320;
                height = 180;
            } else if (width % 320 == 0 && height % 200 == 0) { // 16:10
                scale = width / 320;
                width = 320;
                height = 200;
            } else if (width % 360 == 0 && height % 180 == 0) { // 2:1
                scale = width / 360;
                width = 360;
                height = 180;
            } else {
                while (width / 2 >= UI.DefaultWidth && height / 2 >= UI.DefaultHeight) {
                    if (width / 2 >= UI.DefaultWidth && height / 2 >= UI.DefaultHeight && width % 2 == 0 && height % 2 == 0) {
                        width /= 2;
                        height /= 2;
                        scale *= 2;
                    } else if (width / 3 >= UI.DefaultWidth && height / 3 >= UI.DefaultHeight && width % 3 == 0 && height % 3 == 0) {
                        width /= 3;
                        height /= 3;
                        scale *= 3;
                    } else if (width / 5 >= UI.DefaultWidth && height / 5 >= UI.DefaultHeight && width % 5 == 0 && height % 5 == 0) {
                        width /= 5;
                        height /= 5;
                        scale *= 5;
                    } else if (width / 7 >= UI.DefaultWidth && height / 7 >= UI.DefaultHeight && width % 7 == 0 && height % 7 == 0) {
                        width /= 7;
                        height /= 7;
                        scale *= 7;
                    } else {
                        width /= 2;
                        height /= 2;
                        scale *= 2;
                    }
                }
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

            mapView.CameraHeightHalf = Math.Min(12, 3 + (9 * (int)Size.x / UI.DefaultHeight));
            mapView.CameraWidthHalf = Math.Min(21, 3 + (15 * (int)Size.y / UI.DefaultWidth));

            var ui = (UI.Ins as UI).GetComponent<UnityEngine.UI.CanvasScaler>();
            ui.scaleFactor = screenScale;
        }

        private int screenWidthLastTime;
        private int screenHeightLastTime;

        //private const string mouseScrollWheel = "Mouse ScrollWheel";
        //private float scale = 1f;

#if UNITY_EDITOR
        private void Update() {
            if (Screen.width != screenWidthLastTime || Screen.height != screenHeightLastTime) {
                RecalcSize();
                screenWidthLastTime = Screen.width;
                screenHeightLastTime = Screen.height;
            }
            //if (GameMenu.IsInStandalone) {
            //    if (!UI.Ins.Active) {
            //        // PC上会根据鼠标滚轮进行一定缩放
            //        float dv = Input.GetAxisRaw(mouseScrollWheel);
            //        if (dv != 0) {
            //            scale -= dv * Time.deltaTime * 100;
            //            scale = Mathf.Clamp(scale, 0.5f, 1);
            //            SyncUICameraOrthographicSizeWithScreenSize();
            //        }
            //    }
            //}
        }
#endif
    }
}

