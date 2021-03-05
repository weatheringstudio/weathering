
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weathering
{
    public interface IPassable
    {
        bool Passable { get; }
    }

    public interface IStepOn
    {
        void OnStepOn();
    }

    public interface IMapView
    {
        /// <summary>
        /// 设置和获取当前显示的地图。同时只会显示一个地图。
        /// </summary>
        IMap TheOnlyActiveMap { get; set; }

        Camera MainCamera { get; }

        Vector2 CameraPosition { get; set; }

        Vector2Int CharacterPosition { get; set; }

        Color ClearColor { get; set; }

        float TappingSensitivityFactor { get; set; }
    }

    public class MapView : MonoBehaviour, IMapView
    {
        public static IMapView Ins { get; private set; }

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
            //#if !UNITY_EDITOR && !UNITY_STANDALONE
            //            Indicator.SetActive(false);
            //#endif
            mainCameraTransform = mainCamera.transform;
            playerCharacterTransform = playerCharacter.transform;
        }

        public IMap TheOnlyActiveMap { get; set; }

        public Camera MainCamera { get => mainCamera; }

        public float CameraSize {
            set {
                mainCamera.orthographicSize = value;
            }
        }

        public Vector2 CameraPosition {
            get {
                return mainCameraTransform.position;
            }
            set {
                mainCameraTransform.position
                    = new Vector3(value.x, value.y,
                    mainCameraTransform.position.z);
            }
        }

        private Vector2Int CharacterPositionInternal;
        public Vector2Int CharacterPosition {
            get => CharacterPositionInternal; set {
                CharacterPositionInternal = value;
            }
        }

        public Color ClearColor {
            get {
                return mainCamera.backgroundColor;
            }
            set {
                mainCamera.backgroundColor = value;
            }
        }


        private void Start() {
            characterView.SetCharacterSprite(lastTimeMovement, false);
        }
        private int width;
        private int height;
        private bool mapControlPlayerLastTime = false;
        private void Update() {
            // 按下ESC键打开关闭菜单
#if UNITY_EDITOR || UNITY_STANDALONE
            CheckESCKey();
#endif
            IMapDefinition map = TheOnlyActiveMap as IMapDefinition;
            if (map == null) throw new Exception();
            width = map.Width;
            height = map.Height;
            UpdateInput();
            if (map != null) {
                if (map.ControlCharacter) {
                    playerCharacter.SetActive(true);
                    if (!UI.Ins.Active) {
                        UpdateCharacterWithTappingAndArrowKey();
                    }
                    CorrectCharacterPosition();
                    CameraFollowsCharacter();
                    if (!mapControlPlayerLastTime) {
                        SyncCharacterPosition();
                    }
                    mapControlPlayerLastTime = true;
                } else {
                    playerCharacter.SetActive(false);
                    if (!UI.Ins.Active) {
                        UpdateCameraWithTapping();
                        UpdateCameraWidthArrowKey();
                    }
                    CorrectCameraPosition();
                    mapControlPlayerLastTime = false;
                }
                UpdateMap();
                UpdateMapAnimation();
                map.Update();
            } else {
                throw new Exception();
            }
        }

        private void SyncCharacterPosition() {
            Vector3 displayPositionOfCharacter = GetRealPositionOfCharacter();
            playerCharacterTransform.position = displayPositionOfCharacter;
            mainCameraTransform.position = new Vector3(displayPositionOfCharacter.x, displayPositionOfCharacter.y, cameraZ);
        }

        private void CheckESCKey() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (UI.Ins.Active) {
                    UI.Ins.Active = false;
                } else {
                    GameMenu.Ins.OnTapSettings();
                }
            }
        }

        private const float cameraSpeed = 10;
        private void UpdateCameraWidthArrowKey() {
            float ratio = cameraSpeed * Time.deltaTime * TappingSensitivityFactor * ScreenAdaptation.Ins.DoubleSizeMultiplier;
            if (Input.GetKey(KeyCode.RightArrow)) {
                target = mainCameraTransform.position + Vector3.right * ratio;
                mainCameraTransform.position = target;
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                target = mainCameraTransform.position + Vector3.left * ratio;
                mainCameraTransform.position = target;
            }
            if (Input.GetKey(KeyCode.UpArrow)) {
                target = mainCameraTransform.position + Vector3.up * ratio;
                mainCameraTransform.position = target;
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                target = mainCameraTransform.position + Vector3.down * ratio;
                mainCameraTransform.position = target;
            }
        }

        public class TappingSensitivity { }

        public const float DefaultTappingSensitivity = 2f;
        public float TappingSensitivityFactor { get; set; } = 2f;
        private void UpdateCameraWithTapping() {
            if (!tapping) return;
            Vector2 cameraDeltaDistance = deltaDistance * Time.deltaTime * TappingSensitivityFactor * cameraSpeed * ScreenAdaptation.Ins.DoubleSizeMultiplier;
            mainCameraTransform.position += (Vector3)cameraDeltaDistance;
        }

        Vector2Int characterMovement = Vector2Int.zero;
        private float lastTimeUpdated = 0;
        private Vector2Int lastTimeMovement = Vector2Int.down;
        private bool passable = false; // passable为false，等价于旁边的tile不可通过，且自己站着的tile可通过
        [NonSerialized]
        public float UpdateInterval = 0.3f;
        private void UpdateCharacterWithTappingAndArrowKey() {
            passable = false;
            if (tapping || Input.anyKey) {
                characterMovement = Vector2Int.zero;
                float absX = Mathf.Abs(deltaDistance.x);
                float absY = Mathf.Abs(deltaDistance.y);
                const float deadZoneRadius = 0.5f;
                if (tapping) {
                    if (absX > absY) {
                        if (absX > deadZoneRadius) {
                            if (deltaDistance.x > 0) {
                                characterMovement = Vector2Int.right;
                            } else {
                                characterMovement = Vector2Int.left;
                            }
                        }
                    } else {
                        if (absY > deadZoneRadius) {
                            if (deltaDistance.y > 0) {
                                characterMovement = Vector2Int.up;
                            } else {
                                characterMovement = Vector2Int.down;
                            }
                        }
                    }
                } else {
                    if (Input.GetKey(KeyCode.LeftArrow)) characterMovement = Vector2Int.left;
                    if (Input.GetKey(KeyCode.RightArrow)) characterMovement = Vector2Int.right;
                    if (Input.GetKey(KeyCode.UpArrow)) characterMovement = Vector2Int.up;
                    if (Input.GetKey(KeyCode.DownArrow)) characterMovement = Vector2Int.down;
                }

                if (characterMovement != Vector2Int.zero) {
                    Vector2Int newPosition = CharacterPositionInternal + characterMovement;
                    // IPassable用于判断能否此地块能否通过
                    ITile tileStepOn = TheOnlyActiveMap.Get(newPosition);
                    IPassable passableTile = tileStepOn as IPassable;
                    if (passableTile == null) {
                        passable = true;
                    }
                    if (passableTile != null) {
                        passable = passableTile.Passable;
                        if (!passable) {
                            IPassable passableTileSelf = TheOnlyActiveMap.Get(CharacterPositionInternal) as IPassable;
                            if (passableTileSelf == null) {
                                passable = false;
                            } else {
                                passable = !passableTileSelf.Passable;
                            }
                        }
                    }
                    if (passable) {
                        if (Time.time > lastTimeUpdated + UpdateInterval) {
                            lastTimeUpdated = Time.time;
                            CharacterPosition = newPosition; // CharacterPositionInternal += characterMovement;
                            (tileStepOn as IStepOn)?.OnStepOn();
                        }
                    }
                    lastTimeMovement = characterMovement;
                }
            }
        }

        private bool corrected = false;
        private Vector2Int correctingVector = Vector2Int.zero;
        private void CorrectCharacterPosition() {
            Vector2Int characterPosition = CharacterPositionInternal;
            corrected = false;
            correctingVector = Vector2Int.zero;
            if (characterPosition.x >= width) {
                characterPosition.x -= width;
                corrected = true;
                correctingVector = new Vector2Int(-width, 0);
            } else if (characterPosition.x < 0) {
                characterPosition.x += width;
                corrected = true;
                correctingVector = new Vector2Int(width, 0);
            }
            if (characterPosition.y >= height) {
                characterPosition.y -= height;
                corrected = true;
                correctingVector = new Vector2Int(0, -height);
            } else if (characterPosition.y < 0) {
                characterPosition.y += height;
                corrected = true;
                correctingVector = new Vector2Int(0, height);
            }
            CharacterPositionInternal = characterPosition;
            if (corrected) {
                Vector3 offset = (Vector3Int)correctingVector;
                mainCameraTransform.position += offset;
                playerCharacterTransform.position += offset;
            }
        }


        private const int cameraZ = -17;
        private Vector3 GetRealPositionOfCharacter() {
            return new Vector3(CharacterPositionInternal.x + 0.5f, CharacterPositionInternal.y + 0.5f, 0);
        }
        private void CameraFollowsCharacter() {
            Vector3 displayPositionOfCharacter = GetRealPositionOfCharacter();
            Vector3 deltaPosition = displayPositionOfCharacter - playerCharacterTransform.position;
            Vector3 newPosition = playerCharacterTransform.position + deltaPosition.normalized * Time.deltaTime / UpdateInterval;
            float deltaPositionSqrMagnitude = deltaPosition.sqrMagnitude;
            bool moving = deltaPosition.sqrMagnitude > 0.001f;
            if (!moving || deltaPositionSqrMagnitude < (newPosition - displayPositionOfCharacter).sqrMagnitude) {
                playerCharacterTransform.position = displayPositionOfCharacter;
            } else {
                playerCharacterTransform.position = newPosition;
            }
            characterView.SetCharacterSprite(lastTimeMovement, moving);
            mainCameraTransform.position = new Vector3(playerCharacterTransform.position.x, playerCharacterTransform.position.y, cameraZ);
        }


        private Vector3 target;
        private void CorrectCameraPosition() {
            target = mainCameraTransform.position;
            if (target.x > width) {
                target.x -= width; ;
            } else if (target.x < 0) {
                target.x += width;
            }
            if (target.y > height) {
                target.y -= height;
            } else if (target.y < 0) {
                target.y += height;
            }
            target.z = cameraZ;
            mainCameraTransform.position = target;
        }

        public int CameraWidthHalf { private get; set; } = 5;
        public int CameraHeightHalf { private get; set; } = 5;
        private void UpdateMap() {
            if (TheOnlyActiveMap as StandardMap == null) throw new Exception(); // 现在地图只能继承StandardMap，已经强耦合了。实现一个其他的IMapDefinition挺难的
            Vector3 pos = mainCameraTransform.position;
            int x = (int)pos.x;
            int y = (int)pos.y;

            IRes res = Res.Ins;
            for (int i = x - CameraWidthHalf; i < x + CameraWidthHalf; i++) {
                for (int j = y - CameraHeightHalf; j < y + CameraHeightHalf; j++) {
                    ITileDefinition iTile = TheOnlyActiveMap.Get(i, j) as ITileDefinition;
                    bool needUpdateSpriteKey = iTile.NeedUpdateSpriteKeys;

                    // Tile缓存优化，使用了NeedUpdateSpriteKey TileSpriteKeyBuffer
                    Tile tileBase = null;
                    Tile tileRoad = null;
                    Tile tile = null;
                    Tile tileOverlay = null;

                    if (needUpdateSpriteKey) {
                        string spriteKeyBase = iTile.SpriteKeyBase;
                        if (spriteKeyBase != null && !res.TryGetTile(spriteKeyBase, out tileBase)) {
                            throw new Exception($"Tile {spriteKeyBase} not found for Tile {iTile.GetType().Name}");
                        }
                        iTile.TileSpriteKeyBaseBuffer = tileBase;

                        string spriteKeyRoad = iTile.SpriteKeyRoad;
                        if (spriteKeyRoad != null && !res.TryGetTile(spriteKeyRoad, out tileRoad)) {
                            throw new Exception($"Tile {spriteKeyRoad} not found for Tile {iTile.GetType().Name}");
                        }
                        iTile.TileSpriteKeyRoadBuffer = tileRoad;

                        string spriteKey = iTile.SpriteKey;
                        if (spriteKey != null && !res.TryGetTile(spriteKey, out tile)) {
                            throw new Exception($"Tile {spriteKey} not found for Tile {iTile.GetType().Name}");
                        }
                        iTile.TileSpriteKeyBuffer = tile;

                        string spriteKeyOverlay = iTile.SpriteKeyOverlay;
                        if (spriteKeyOverlay != null && !res.TryGetTile(spriteKeyOverlay, out tileOverlay)) {
                            throw new Exception($"Tile {spriteKeyOverlay} not found for Tile {iTile.GetType().Name}");
                        }
                        iTile.TileSpriteKeyOverlayBuffer = tileOverlay;
                    } else {
                        tileBase = iTile.TileSpriteKeyBaseBuffer;
                        tileRoad = iTile.TileSpriteKeyRoadBuffer;
                        tile = iTile.TileSpriteKeyBuffer;
                        tileOverlay = iTile.TileSpriteKeyOverlayBuffer;
                    }

                    Tile tileLeft = null;
                    Tile tileRight = null;
                    Tile tileUp = null;
                    Tile tileDown = null;
                    bool hasAnimation = iTile.HasDynamicSpriteAnimation;
                    if (hasAnimation && needUpdateSpriteKey) {
                        string spriteLeft = iTile.SpriteLeft;
                        if (spriteLeft != null && !res.TryGetTile(spriteLeft, out tileLeft)) {
                            throw new Exception($"Tile {spriteLeft} not found for Tile {iTile.GetType().Name}, in sprite left");
                        }
                        iTile.TileSpriteKeyLeftBuffer = tileLeft;

                        string spriteRight = iTile.SpriteRight;
                        if (spriteRight != null && !res.TryGetTile(spriteRight, out tileRight)) {
                            throw new Exception($"Tile {spriteRight} not found for Tile {iTile.GetType().Name}, in sprite right");
                        }
                        iTile.TileSpriteKeyRightBuffer = tileRight;

                        string spriteUp = iTile.SpriteUp;
                        if (spriteUp != null && !res.TryGetTile(spriteUp, out tileUp)) {
                            throw new Exception($"Tile {spriteUp} not found for Tile {iTile.GetType().Name}, in sprite up");
                        }
                        iTile.TileSpriteKeyUpBuffer = tileUp;

                        string spriteDown = iTile.SpriteDown;
                        if (spriteDown != null && !res.TryGetTile(spriteDown, out tileDown)) {
                            throw new Exception($"Tile {spriteDown} not found for Tile {iTile.GetType().Name}, in sprite down");
                        }
                        iTile.TileSpriteKeyDownBuffer = tileDown;

                    } else {
                        tileLeft = iTile.TileSpriteKeyLeftBuffer;
                        tileRight = iTile.TileSpriteKeyRightBuffer;
                        tileUp = iTile.TileSpriteKeyUpBuffer;
                        tileDown = iTile.TileSpriteKeyDownBuffer;
                    }

                    if (needUpdateSpriteKey || iTile.NeedUpdateSpriteKeysPositionX != i || iTile.NeedUpdateSpriteKeysPositionY != j) {
                        Vector3Int pos3d = new Vector3Int(i, j, 0);
                        tilemapBase.SetTile(pos3d, tileBase);
                        tilemapRoad.SetTile(pos3d, tileRoad);
                        if (hasAnimation) {
                            tilemapLeft.SetTile(pos3d, tileLeft);
                            tilemapRight.SetTile(pos3d, tileRight);
                            tilemapUp.SetTile(pos3d, tileUp);
                            tilemapDown.SetTile(pos3d, tileDown);
                        }
                        tilemap.SetTile(pos3d, tile);
                        tilemapOverlay.SetTile(pos3d, tileOverlay);

                        iTile.NeedUpdateSpriteKeys = false;
                        iTile.NeedUpdateSpriteKeysPositionX = i;
                        iTile.NeedUpdateSpriteKeysPositionY = j;
                    }
                }
            }
        }

        private void UpdateMapAnimation() {
            double time = TimeUtility.GetMiniSecondsInDouble();
            long longTime = (long)time;
            float fraction;
            long size = 1000;
            const long wait = 100;
            long remider = longTime % size;
            if (remider < wait) {
                fraction = 0;
            } else if (remider > size - wait) {
                fraction = 1;
            } else {
                float t = (float)remider / size;
                fraction = Mathf.Lerp(0, 1, EaseFuncUtility.EaseInOutCubic(t)); // (float)(time - longTime);
            }
            tilemapLeft.transform.position = Vector3.left * fraction + Vector3.right;
            tilemapRight.transform.position = Vector3.right * fraction + Vector3.left;
            tilemapUp.transform.position = Vector3.up * fraction + Vector3.down;
            tilemapDown.transform.position = Vector3.down * fraction + Vector3.up;
        }

        [SerializeField]
        private Tilemap tilemap;
        [SerializeField]
        private Tilemap tilemapBase;
        [SerializeField]
        private Tilemap tilemapRoad;
        [SerializeField]
        private Tilemap tilemapLeft;
        [SerializeField]
        private Tilemap tilemapRight;
        [SerializeField]
        private Tilemap tilemapUp;
        [SerializeField]
        private Tilemap tilemapDown;
        [SerializeField]
        private Tilemap tilemapOverlay;

        [SerializeField]
        private Camera mainCamera;
        private Transform mainCameraTransform;
        [SerializeField]
        private GameObject playerCharacter;
        private Transform playerCharacterTransform;

        [SerializeField]
        private CharacterView characterView;

        [SerializeField]
        private Transform Head;
        [SerializeField]
        private Transform Tail;

        private bool tapping = false;

        private Vector2 tailMousePosition;
        //private Vector2 originalMousePosition;
        private Vector3 originalDownMousePosition;
        private Vector2 deltaDistance;

        private Vector2 tail;
        private bool hasBeenOutOfTheSameTile;
        private void UpdateInput() {
            Vector2 mousePosition = Input.mousePosition;

            tapping = false;
            Vector2 head = mainCamera.ScreenToWorldPoint(mousePosition);
            if (Input.GetMouseButtonDown(0)) {
                //originalMousePosition = mousePosition;
                originalDownMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
                tailMousePosition = mousePosition;
                hasBeenOutOfTheSameTile = false;
            }

            bool onSameTile = false;
            Vector2Int nowInt = MathVector2Floor(head);
            if (nowInt == MathVector2Floor(originalDownMousePosition)) {
                onSameTile = true;
            } else {
                hasBeenOutOfTheSameTile = true;
            }

            if (Input.GetMouseButton(0)) {
                float radius = Math.Min(Screen.width, Screen.height) / 10;
                Vector2 deltaMousePosition = mousePosition - tailMousePosition;
                if (deltaMousePosition.sqrMagnitude > radius * radius) {
                    tailMousePosition += deltaMousePosition * Mathf.Min(0.64f, Time.deltaTime * 10);
                }

                tail = mainCamera.ScreenToWorldPoint(tailMousePosition);

                deltaDistance = head - tail;
                if (deltaDistance.sqrMagnitude > 1f) {
                    deltaDistance.Normalize();
                }
                tapping = deltaDistance.sqrMagnitude > 0.0001f;

                bool showHeadAndTail = !UI.Ins.Active && tapping && (!onSameTile || hasBeenOutOfTheSameTile);
                Head.gameObject.SetActive(showHeadAndTail);
                Tail.gameObject.SetActive(showHeadAndTail);

                Indicator.SetActive(!showHeadAndTail && !UI.Ins.Active);
                Head.localPosition = head;
                Tail.localPosition = tail;

                if (!showHeadAndTail) {
                    UpdateIndicator(MathVector2Floor(mainCamera.ScreenToWorldPoint(mousePosition)));
                }
            }

            // 这里与GameMenu的那个按钮产生了强耦合，当点击位置在屏幕右上角时，不会考虑UpdateInput点击地块
            if (mousePosition.x > (Screen.width - 36 * 3) && mousePosition.y > (Screen.height - 36)) {
                return;
            }

            if (Input.GetMouseButtonUp(0)) {
                if (onSameTile) {
                    OnTap(nowInt);
                }
                Indicator.SetActive(false);
                Head.gameObject.SetActive(false);
                Tail.gameObject.SetActive(false);
            }
        }

        private Vector2Int MathVector2Floor(Vector2 vec) {
            return new Vector2Int((int)Mathf.Floor(vec.x), (int)Mathf.Floor(vec.y));
        }

        [SerializeField]
        private GameObject Indicator;
        private void UpdateIndicator(Vector2Int pos) {
            Indicator.transform.position = (Vector2)(pos) + new Vector2(0.5f, 0.5f);
        }


        private void OnTap(Vector2Int pos) {
            if (UI.Ins.Active) {
                return;
            }
            // 点地图时
            // Sound.Ins.PlayDefaultSound();
            if (CharacterPositionInternal != pos || !TheOnlyActiveMap.ControlCharacter) {
                ITile tile = TheOnlyActiveMap.Get(pos.x, pos.y);
                tile?.OnTap();
                tile?.OnTapPlaySound();
            } else {
                GameMenu.Ins.OnTapPlayerInventory();
            }
        }
    }
}

