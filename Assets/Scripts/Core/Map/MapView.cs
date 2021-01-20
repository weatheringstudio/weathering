
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weathering
{

    public interface IMapView
    {
        /// <summary>
        /// 设置和获取当前显示的地图。同时只会显示一个地图。
        /// </summary>
        IMap Map { get; set; }

        Vector2 CameraPosition { get; set; }
    }

    public class MapView : MonoBehaviour, IMapView
    {
        public static IMapView Ins { get; private set; }

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
#if !UNITY_EDITOR
            Indicator.SetActive(false);
#endif
        }

        public IMap Map { get; set; }

        public Vector2 CameraPosition {
            get {
                return mainCamera.transform.position;
            }
            set {
                mainCamera.transform.position
                    = new Vector3(value.x, value.y,
                    mainCamera.transform.position.z);
            }
        }

        private int width;
        private int height;
        private void Update() {
            width = Map.Width;
            height = Map.Height;
            UpdateInput();
            if (Map != null) {
                if (!UI.Ins.Active) {
                    UpdateCameraWithTapping();
                }
                UpdateCameraWidthArrowKey();
                CorrectCameraPosition();
                UpdateMap();
            }
        }

        private float cameraSpeed = 10;
        private void UpdateCameraWidthArrowKey() {
            float ratio = cameraSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.RightArrow)) {
                target = mainCamera.transform.position + Vector3.right * ratio;
                mainCamera.transform.position = target;
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                target = mainCamera.transform.position + Vector3.left * ratio;
                mainCamera.transform.position = target;
            }
            if (Input.GetKey(KeyCode.UpArrow)) {
                target = mainCamera.transform.position + Vector3.up * ratio;
                mainCamera.transform.position = target;
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                target = mainCamera.transform.position + Vector3.down * ratio;
                mainCamera.transform.position = target;
            }
        }

        [Range(1, 5)]
        public float TappingSensitivity = 2f;
        private void UpdateCameraWithTapping() {
            if (!tapping) return;
            target = mainCamera.transform.position;
            Vector2 cameraDeltaDistance = deltaDistance * Time.deltaTime * TappingSensitivity;
            target += (Vector3)cameraDeltaDistance;
            mainCamera.transform.Translate(cameraDeltaDistance);
        }

        private Vector3 target;
        private void CorrectCameraPosition() {

            target = mainCamera.transform.position;
            if (target.x > width / 2) {
                target.x -= width; ;
            }
            if (target.x < -width / 2) {
                target.x -= -width;
            }
            if (target.y > height / 2) {
                target.y -= height;
            }
            if (target.y < -height / 2) {
                target.y -= -height;
            }
            target.z = -17;
            mainCamera.transform.position = target;
        }
        private void UpdateMap() {
            int cameraWidthHalf = 12;
            int cameraHeightHalf = 7;

            Vector3 pos = mainCamera.transform.position;
            int x = (int)pos.x;
            int y = (int)pos.y;

            for (int i = x - cameraWidthHalf; i < x + cameraWidthHalf; i++) {
                for (int j = y - cameraHeightHalf; j < y + cameraHeightHalf; j++) {
                    ITileDefinition iTile = Map.Get(i, j) as ITileDefinition;
                    Tile tile = iTile == null ? null : Res.Ins.GetTile(iTile.SpriteKey);
                    tilemap.SetTile(new Vector3Int(i, j, 0), tile);
                    tilemap.SetTile(new Vector3Int(i - width, j, 0), tile);
                    tilemap.SetTile(new Vector3Int(i, j - height, 0), tile);
                    tilemap.SetTile(new Vector3Int(i - width, j - height, 0), tile);
                }
            }
        }

        [SerializeField]
        private Tilemap tilemap;
        [SerializeField]
        private Camera mainCamera;



        private bool tapping;
        private Vector2 downRaw;
        private Vector2 down;
        private Vector2 deltaDistance;

        private readonly float deadZoneRadius = 0.2f;

        private void UpdateInput() {
            Vector3 mousePosition = Input.mousePosition;


            tapping = false;
            Vector2 now = mainCamera.ScreenToWorldPoint(mousePosition);
            if (Input.GetMouseButtonDown(0)) {
                downRaw = mousePosition;
                down = now;
            }
            if (Input.GetMouseButton(0)) {
                deltaDistance = now - (Vector2)mainCamera.ScreenToWorldPoint(downRaw);
                tapping = deltaDistance.sqrMagnitude > deadZoneRadius * deadZoneRadius;
            }

            if (mousePosition.x > Screen.width * 19 / 20f && mousePosition.y > Screen.height * 10 / 11f) {
                return;
            }

            if (Input.GetMouseButtonUp(0)) {
                Vector2Int nowInt = ToVector2Int(now);
                if (nowInt == ToVector2Int(down)) {
                    OnTap(nowInt);
                }
            }

#if UNITY_EDITOR
            if (!UI.Ins.Active) {
                UpdateIndicator(ToVector2Int(now));
            }
#endif
        }
        private void OnTap(Vector2Int pos) {
            if (UI.Ins.Active) {
                return;
            }
            Map.Get(pos.x, pos.y)?.OnTap();
        }
        private Vector2Int ToVector2Int(Vector2 vec) {
            return new Vector2Int((int)Mathf.Floor(vec.x), (int)Mathf.Floor(vec.y));
        }

        [SerializeField]
        private GameObject Indicator;
        private void UpdateIndicator(Vector2Int pos) {
            Indicator.transform.position = (Vector2)(pos) + new Vector2(0.5f, 0.5f);
        }

    }
}

