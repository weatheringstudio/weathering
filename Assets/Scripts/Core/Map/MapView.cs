
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


    public interface IHasDayNightRecycle
    {

    }


    public interface IMapView
    {
        /// <summary>
        /// 设置和获取当前显示的地图。同时只会显示一个地图。
        /// </summary>
        IMap TheOnlyActiveMap { get; set; }

        // Camera MainCamera { get; }

        Vector2 CameraPosition { get; set; }

        Vector2Int CharacterPosition { get; set; }

        Color ClearColor { get; set; }

        float TappingSensitivityFactor { get; set; }
        long AnimationIndex { get; }

        // void SetMaterialForAllTilemaps(string matKey);
    }

    // IgnoreTool的ITile会忽略选中的工具影响
    public interface IIgnoreTool
    {
        bool IgnoreTool { get; }
    }

    public interface IHasFrameAnimationOnSpriteKey
    {
        int HasFrameAnimation { get; }
    }

    public class MapView : MonoBehaviour, IMapView
    {
        public static IMapView Ins { get; private set; }

        private void Awake() {
            if (Ins != null) throw new Exception();
            Ins = this;
            mainCameraTransform = mainCamera.transform;
            characterTransform = playerCharacter.transform;

            renderer_tilemapBedrock = tilemapBedrock.GetComponent<TilemapRenderer>();
            renderer_tilemapHill = tilemapHill.GetComponent<TilemapRenderer>();
            renderer_tilemapGrass = tilemapGrass.GetComponent<TilemapRenderer>();
            renderer_tilemapTree = tilemapTree.GetComponent<TilemapRenderer>();
            renderer_tilemapRoad = tilemapRoad.GetComponent<TilemapRenderer>();
            renderer_tilemapLeft = tilemapLeft.GetComponent<TilemapRenderer>();
            renderer_tilemapRight = tilemapRight.GetComponent<TilemapRenderer>();
            renderer_tilemapUp = tilemapUp.GetComponent<TilemapRenderer>();
            renderer_tilemapDown = tilemapDown.GetComponent<TilemapRenderer>();
            renderer_tilemap = tilemap.GetComponent<TilemapRenderer>();
            renderer_tilemapOverlay = tilemapOverlay.GetComponent<TilemapRenderer>();
        }

        /// <summary>
        /// 单例, 全局唯一, 代表正在显示中的地图
        /// </summary>
        public IMap TheOnlyActiveMap { get; set; }

        // public Camera MainCamera { get => mainCamera; }

        /// <summary>
        /// 用于调整主相机大小, ScreenAdaption用到了
        /// </summary>
        public float CameraSize {
            get => mainCamera.orthographicSize;
            set {
                mainCamera.orthographicSize = value;
            }
        }

        /// <summary>
        /// 用于进入地图时, 初始化相机位置。StandardMap用到了
        /// </summary>
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
        /// <summary>
        /// 玩家角色的真实坐标, 整数值。用于进入地图时, 初始化玩家位置。也用于获取
        /// </summary>
        public Vector2Int CharacterPosition {
            get => CharacterPositionInternal; set {
                CharacterPositionInternal = value;
            }
        }

        /// <summary>
        /// 主相机颜色, StandardMap设置保存, 基本用不到, 因为有贴图
        /// </summary>
        public Color ClearColor {
            get {
                return mainCamera.backgroundColor;
            }
            set {
                mainCamera.backgroundColor = value;
            }
        }


        private void Start() {
            characterView.SetCharacterSprite(Vector2Int.down, false);
            SyncCharacterPosition();
        }

        private int width;
        private int height;
        private bool mapControlCharacterLastFrame = false;
        private void Update() {
            // 按下ESC键打开关闭菜单, Standalone专享
            if (GameMenu.IsInStandalone) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    if (UI.Ins.Active) {
                        UI.Ins.Active = false;
                    } else {
                        GameMenu.Ins.OnTapSettings();
                    }
                }
            }

            IMapDefinition map = TheOnlyActiveMap as IMapDefinition; if (map == null) throw new Exception();

            // 缓存一下宽高, MapView本帧常用
            width = map.Width;
            height = map.Height;

            // 输入获取、处理、检测, tap, move
            UpdateInput();

            // 控制玩家时
            bool mapControlCharacter = map.ControlCharacter;
            if (mapControlCharacter) {
                if (!UI.Ins.Active && (tapping || Input.anyKey)) {
                    // 玩家移动
                    UpdateCharacterPositionWithTapping();
                    UpdateCharacterPositionWIthArrowKey();
                    TryTriggerOnStepEvent();
                    // GlobalLight.Ins.SyncCharacterLightPosition(MaterialWithShadow, characterTransform.position);
                }
                // 切换时, 瞬移玩家位置, 灯光位置
                if (!mapControlCharacterLastFrame) {
                    SyncCharacterPosition();
                }

                // 校验玩家位置, 保证玩家在地图边缘间移动时会瞬移传送
                CorrectCharacterPositionAndLight();

                // 移动玩家(受移动速度影响), 移动主相机, 移动玩家灯光, 玩家动画
                CameraFollowsCharacter();
                CharacterLightFollowCharacter();

            } else {
                if (!UI.Ins.Active) {
                    if (tapping) {
                        UpdateCameraWithTapping();
                    }
                    UpdateCameraWidthArrowKey();
                }
                CorrectCameraPosition();
            }
            playerCharacter.SetActive(mapControlCharacter);
            mapControlCharacterLastFrame = mapControlCharacter;

            // 渲染地图
            UpdateMap();
            // UpdateDayNightCycleLightingAndShadow();
            // 地图动画, 会用着色器代替
            UpdateMapAnimation();
        }

        private void SyncCharacterPosition() {
            // 同步位置
            Vector3 displayPositionOfCharacter = GetRealPositionOfCharacter();
            characterTransform.position = displayPositionOfCharacter;
            mainCameraTransform.position = new Vector3(displayPositionOfCharacter.x, displayPositionOfCharacter.y, cameraZ);

            // 同步灯光位置
            GlobalLight.Ins.CharacterLightPosition = displayPositionOfCharacter;
        }

        private const float cameraSpeed = 5;
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
            Vector2 cameraDeltaDistance = deltaDistance * Time.deltaTime * TappingSensitivityFactor * cameraSpeed * ScreenAdaptation.Ins.DoubleSizeMultiplier;
            mainCameraTransform.position += (Vector3)cameraDeltaDistance;
        }

        Vector2Int characterMovement = Vector2Int.zero;
        private float lastTimeUpdated = 0;

        // 人物走过1格需要的时间
        private const float WalkingTimeForUnitTileBase = 0.3f;
        private float WalkingTimeForUnitTile = WalkingTimeForUnitTileBase;
        private void UpdateCharacterPositionWithTapping() {
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
            }
        }
        private void UpdateCharacterPositionWIthArrowKey() {
            if (!tapping) {
                if (Input.GetKey(KeyCode.LeftArrow)) characterMovement = Vector2Int.left;
                if (Input.GetKey(KeyCode.RightArrow)) characterMovement = Vector2Int.right;
                if (Input.GetKey(KeyCode.UpArrow)) characterMovement = Vector2Int.up;
                if (Input.GetKey(KeyCode.DownArrow)) characterMovement = Vector2Int.down;
            }
        }
        private void TryTriggerOnStepEvent() {
            // 触发走路移动事件
            if (characterMovement != Vector2Int.zero) {
                Vector2Int newPosition = CharacterPositionInternal + characterMovement;
                // IPassable用于判断能否此地块能否通过
                ITile oldTile = TheOnlyActiveMap.Get(CharacterPositionInternal);
                IPassable oldPassable = oldTile as IPassable;
                bool oldIsPassable = oldPassable != null && oldPassable.Passable;
                ITile newTile = TheOnlyActiveMap.Get(newPosition);
                IPassable newPassable = newTile as IPassable;
                bool newIsPassable = newPassable != null && newPassable.Passable;


                if (newIsPassable || !oldIsPassable) {
                    if (Time.time > lastTimeUpdated + WalkingTimeForUnitTile) {
                        lastTimeUpdated = Time.time;
                        CharacterPositionInternal = newPosition;
                        if (newTile is IStepOn step) {
                            try {
                                step.OnStepOn();
                            } catch (Exception e) {
                                UI.Ins.ShowItems("踩到一个错误! ! ! ", UIItem.CreateText(e.GetType().Name), UIItem.CreateMultilineText(e.Message), UIItem.CreateMultilineText(e.StackTrace));
                                throw e;
                            }
                        }
                    }
                }
            }
        }

        private bool corrected = false;
        private Vector2Int correctingVector = Vector2Int.zero;
        private void CorrectCharacterPositionAndLight() {
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
                characterTransform.position += offset;
                SyncCharacterLightPosition();
            }
        }

        private const int cameraZ = -17;
        private Vector3 GetRealPositionOfCharacter() {
            return new Vector3(CharacterPositionInternal.x + 0.5f, CharacterPositionInternal.y + 0.5f, 1);
        }
        private const float moreAnimationTimeInSecond = 0.05f; // 动画
        private float movingLastTime = 0;
        private void CameraFollowsCharacter() {
            // 调整走路速度
            ITile tile = TheOnlyActiveMap.Get(CharacterPositionInternal.x, CharacterPositionInternal.y);
            if (tile is IWalkingTimeModifier walkingTimeModifier) {
                float modifier = walkingTimeModifier.WalkingTimeModifier;
                modifier = Mathf.Clamp(modifier, 0.2f, 3f);
                WalkingTimeForUnitTile = modifier * WalkingTimeForUnitTileBase;
            } else {
                WalkingTimeForUnitTile = WalkingTimeForUnitTileBase;
            }

            // 移动人物和相机
            Vector3 displayPositionOfCharacter = GetRealPositionOfCharacter();
            Vector3 deltaPosition = displayPositionOfCharacter - characterTransform.position;
            Vector3 newPosition = characterTransform.position + deltaPosition.normalized * Time.deltaTime / WalkingTimeForUnitTile;
            float deltaPositionSqrMagnitude = deltaPosition.sqrMagnitude;
            bool moving = deltaPosition.sqrMagnitude > 0.001f;
            if (!moving || deltaPositionSqrMagnitude < (newPosition - displayPositionOfCharacter).sqrMagnitude) {
                characterTransform.position = displayPositionOfCharacter;
            } else {
                characterTransform.position = newPosition;
            }
            if (moving) {
                movingLastTime = Time.time;
            }
            mainCameraTransform.position = new Vector3(characterTransform.position.x, characterTransform.position.y, cameraZ);

            // 移动动画
            characterView.SetCharacterSprite(characterMovement, moving || (Time.time - movingLastTime) < moreAnimationTimeInSecond); // 不会短暂停止动画

        }
        public const float CharacterLightOffset = 1f;
        private void CharacterLightFollowCharacter() {
            // 移动手电光
            Vector3 target = characterTransform.position + (Vector3)(Vector2)(characterMovement) * CharacterLightOffset;
            GlobalLight.Ins.CharacterLightPosition = Vector3.SmoothDamp(GlobalLight.Ins.CharacterLightPosition, target, ref lightVelocity, 0.15f);

        }
        private void SyncCharacterLightPosition() {
            GlobalLight.Ins.CharacterLightPosition = characterTransform.position + (Vector3)(Vector2)(characterMovement) * CharacterLightOffset;
        }

        private Vector3 lightVelocity;

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

        public long AnimationIndex { get; private set; } = 0;
        private const long animationUpdateRate = 100;
        private long animationFrameLastTime = 0;
        private long animationFrame = 0;
        private int animationScanerIndexOffsetY = 0;

        public int CameraWidthHalf { get; set; } = 5;
        public int CameraHeightHalf { get; set; } = 5;

        private static HashSet<string> SpriteNamesNotFound = null;
        private void ThrowSpriteNotFoundException(string spriteName, ITile tile, string info) {
            if (false) {
                throw new Exception($"Tile {spriteName} not found for ITile {tile.GetType().Name}, in {info}");
            } else {
                const int maxDisplay = 20;

                if (SpriteNamesNotFound == null) {
                    SpriteNamesNotFound = new HashSet<string>();
                }
                if (SpriteNamesNotFound.Count >= maxDisplay) return;

                if (!SpriteNamesNotFound.Contains(spriteName)) {
                    SpriteNamesNotFound.Add(spriteName);

                    var items = UI.Ins.GetItems();

                    items.Add(UIItem.CreateText($"已经检测到{SpriteNamesNotFound.Count}个丢失的贴图, 列表如下："));

                    int i = 0;
                    foreach (string spriteNameNotFount in SpriteNamesNotFound) {
                        items.Add(UIItem.CreateText(spriteNameNotFount));
                        i++;
                        if (i >= maxDisplay) {
                            break;
                        }
                    }
                    if (i == maxDisplay) {
                        items.Add(UIItem.CreateText("还有更多找不到的贴图..."));
                    }

                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateMultilineText(System.Environment.StackTrace));

                    UI.Ins.ShowItems($"忘记加贴图了! ! ! {Localization.Ins.Get(tile.GetType())}", items);
                    if (GameMenu.IsInEditor) {
                        throw new Exception($"Tile {spriteName} not found for ITile {tile.GetType().Name}, in {info}");
                    }
                }
            }
        }
        private void UpdateMap() {
            if (TheOnlyActiveMap as StandardMap == null) throw new Exception(); // 现在地图只能继承StandardMap, 已经强耦合了。实现一个其他的IMapDefinition挺难的
            Vector3 pos = mainCameraTransform.position;
            int x = (int)pos.x;
            int y = (int)pos.y;


            int startY = y - CameraHeightHalf;
            int endY = y + CameraHeightHalf;

            // 动画更新tile会从下而上扫过横排, 把部分SetTile开销分配到不同的帧。如果渲染压力过大, 还会停止一些帧。其实SetTile消耗很小的, 过度考虑了。有垂直同步问题
            if (animationScanerIndexOffsetY <= endY - startY) {
                animationScanerIndexOffsetY++;
            }

            // 每100毫秒, 刷新一下动画
            animationFrame = TimeUtility.GetMiniSeconds();
            if (animationFrame - animationFrameLastTime > animationUpdateRate) {
                animationFrameLastTime = animationFrame;
                AnimationIndex++;

                if (animationScanerIndexOffsetY > endY - startY) {
                    animationScanerIndexOffsetY = 0;
                }
            }


            IRes res = Res.Ins;
            for (int i = x - CameraWidthHalf; i <= x + CameraWidthHalf; i++) {
                for (int j = startY; j <= endY; j++) {
                    ITileDefinition iTile = TheOnlyActiveMap.Get(i, j) as ITileDefinition;

                    // Tile缓存优化, 使用了NeedUpdateSpriteKey TileSpriteKeyBuffer
                    Tile tileBedrock = null;
                    Tile tileWater = null;
                    Tile tileGrass = null;
                    Tile tileTree = null;
                    Tile tileHill = null;
                    Tile tileRoad = null;
                    Tile tile = null;
                    Tile tileOverlay = null;

                    bool needUpdateFrameAnimationForThisTile = (animationScanerIndexOffsetY + startY == j) &&
                        iTile is IHasFrameAnimationOnSpriteKey hasFrameAnimationOnSpriteKey &&
                        hasFrameAnimationOnSpriteKey.HasFrameAnimation > 0 &&
                        AnimationIndex % hasFrameAnimationOnSpriteKey.HasFrameAnimation == 0;
                    bool needUpdateSpriteKey = iTile.NeedUpdateSpriteKeys || needUpdateFrameAnimationForThisTile;

                    if (needUpdateSpriteKey) {

                        string spriteKeyBackground = iTile.SpriteKeyBedrock;
                        if (spriteKeyBackground != null && !res.TryGetTile(spriteKeyBackground, out tileBedrock)) {
                            ThrowSpriteNotFoundException(spriteKeyBackground, iTile, nameof(spriteKeyBackground));
                        }
                        iTile.TileSpriteKeyBedrockBuffer = tileBedrock;

                        string spriteKeyWater = iTile.SpriteKeyWater;
                        if (spriteKeyWater != null && !res.TryGetTile(spriteKeyWater, out tileWater)) {
                            ThrowSpriteNotFoundException(spriteKeyWater, iTile, nameof(spriteKeyWater));
                        }
                        iTile.TileSpriteKeyWaterBuffer = tileWater;

                        string spriteKeyBase = iTile.SpriteKeyGrass;
                        if (spriteKeyBase != null && !res.TryGetTile(spriteKeyBase, out tileGrass)) {
                            ThrowSpriteNotFoundException(spriteKeyBase, iTile, nameof(spriteKeyBase));
                        }
                        iTile.TileSpriteKeyGrassBuffer = tileGrass;

                        string spriteKeyBaseBorderline = iTile.SpriteKeyTree;
                        if (spriteKeyBaseBorderline != null && !res.TryGetTile(spriteKeyBaseBorderline, out tileTree)) {
                            ThrowSpriteNotFoundException(spriteKeyBaseBorderline, iTile, nameof(spriteKeyBaseBorderline));
                        }
                        iTile.TileSpriteKeyTreeBuffer = tileTree;

                        string spriteKeyHill = iTile.SpriteKeyHill;
                        if (spriteKeyHill != null && !res.TryGetTile(spriteKeyHill, out tileHill)) {
                            ThrowSpriteNotFoundException(spriteKeyHill, iTile, nameof(spriteKeyHill));
                        }
                        iTile.TileSpriteKeyHillBuffer = tileHill;

                        string spriteKeyRoad = iTile.SpriteKeyRoad;
                        if (spriteKeyRoad != null && !res.TryGetTile(spriteKeyRoad, out tileRoad)) {
                            ThrowSpriteNotFoundException(spriteKeyRoad, iTile, nameof(spriteKeyRoad));
                        }
                        iTile.TileSpriteKeyRoadBuffer = tileRoad;

                        string spriteKey = iTile.SpriteKey;
                        if (spriteKey != null && !res.TryGetTile(spriteKey, out tile)) {
                            ThrowSpriteNotFoundException(spriteKey, iTile, nameof(spriteKey));
                        }
                        iTile.TileSpriteKeyBuffer = tile;

                        string spriteKeyOverlay = iTile.SpriteKeyOverlay;
                        if (spriteKeyOverlay != null && !res.TryGetTile(spriteKeyOverlay, out tileOverlay)) {
                            ThrowSpriteNotFoundException(spriteKeyOverlay, iTile, nameof(spriteKeyOverlay));
                        }
                        iTile.TileSpriteKeyOverlayBuffer = tileOverlay;
                    } else {
                        tileBedrock = iTile.TileSpriteKeyBedrockBuffer;
                        tileWater = iTile.TileSpriteKeyWaterBuffer;
                        tileGrass = iTile.TileSpriteKeyGrassBuffer;
                        tileTree = iTile.TileSpriteKeyTreeBuffer;
                        tileHill = iTile.TileSpriteKeyHillBuffer;
                        tileRoad = iTile.TileSpriteKeyRoadBuffer;
                        tile = iTile.TileSpriteKeyBuffer;
                        tileOverlay = iTile.TileSpriteKeyOverlayBuffer;
                    }

                    Tile tileLeft = null;
                    Tile tileRight = null;
                    Tile tileUp = null;
                    Tile tileDown = null;
                    if (needUpdateSpriteKey) {
                        string spriteLeft = iTile.SpriteLeft;
                        if (spriteLeft != null && !res.TryGetTile(spriteLeft, out tileLeft)) {
                            ThrowSpriteNotFoundException(spriteLeft, iTile, nameof(spriteLeft));
                        }
                        iTile.TileSpriteKeyLeftBuffer = tileLeft;

                        string spriteRight = iTile.SpriteRight;
                        if (spriteRight != null && !res.TryGetTile(spriteRight, out tileRight)) {
                            ThrowSpriteNotFoundException(spriteRight, iTile, nameof(spriteRight));
                        }
                        iTile.TileSpriteKeyRightBuffer = tileRight;

                        string spriteUp = iTile.SpriteUp;
                        if (spriteUp != null && !res.TryGetTile(spriteUp, out tileUp)) {
                            ThrowSpriteNotFoundException(spriteUp, iTile, nameof(spriteUp));
                        }
                        iTile.TileSpriteKeyUpBuffer = tileUp;

                        string spriteDown = iTile.SpriteDown;
                        if (spriteDown != null && !res.TryGetTile(spriteDown, out tileDown)) {
                            ThrowSpriteNotFoundException(spriteDown, iTile, nameof(spriteDown));
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
                        tilemapBedrock.SetTile(pos3d, tileBedrock);
                        tilemapWater.SetTile(pos3d, tileWater);
                        tilemapGrass.SetTile(pos3d, tileGrass);
                        tilemapTree.SetTile(pos3d, tileTree);
                        tilemapHill.SetTile(pos3d, tileHill);
                        tilemapRoad.SetTile(pos3d, tileRoad);

                        tilemapLeft.SetTile(pos3d, tileLeft);
                        tilemapRight.SetTile(pos3d, tileRight);
                        tilemapUp.SetTile(pos3d, tileUp);
                        tilemapDown.SetTile(pos3d, tileDown);

                        tilemap.SetTile(pos3d, tile);
                        tilemapOverlay.SetTile(pos3d, tileOverlay);

                        //tilemap.SetTileFlags(pos3d, TileFlags.None);
                        //tilemap.SetColor(pos3d, (i + j) % 2 == 0 ? Color.red : Color.blue);

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

            long size = 1000;
            long remider = longTime % size;

            float t = (float)remider / size;
            float fraction;
            if (GameMenu.IsLinear) {
                fraction = Mathf.Lerp(0, 1, EaseFuncUtility.Linear(t)); // (float)(time - longTime);
            } else {
                fraction = Mathf.Lerp(0, 1, EaseFuncUtility.EaseInOutCubic(EaseFuncUtility.ShrinkOnHalf(t, 0.2f))); // (float)(time - longTime);
            }

            tilemapLeft.transform.position = Vector3.left * fraction + Vector3.right;
            tilemapRight.transform.position = Vector3.right * fraction + Vector3.left;
            tilemapUp.transform.position = Vector3.up * fraction + Vector3.down;
            tilemapDown.transform.position = Vector3.down * fraction + Vector3.up;
        }



        //[Space]
        //[Header("Materials")]

        //[SerializeField]
        //private Material DefaultMaterial;
        //[SerializeField]
        //private Material[] Materials;
        //private Dictionary<string, Material> MaterialDict;

        //public void SetMaterialForAllTilemaps(string matKey) {
        //    if (MaterialDict == null) {
        //        MaterialDict = new Dictionary<string, Material>();
        //        foreach (Material material in Materials) {
        //            if (material == null) throw new Exception();
        //            MaterialDict.Add(material.name, material);
        //        }
        //    }

        //    Material mat = null;
        //    if (matKey != null) {
        //        if (!MaterialDict.TryGetValue(matKey, out mat)) {
        //            Debug.LogWarning($"not found {matKey}");
        //        }
        //    }
        //    if (mat == null) {
        //        mat = DefaultMaterial;
        //    }

        //    renderer_tilemapBackground.material = mat;
        //    renderer_tilemapBase.material = mat;
        //    renderer_tilemapBaseBorderLine.material = mat;
        //    renderer_tilemapRoad.material = mat;
        //    renderer_tilemapLeft.material = mat;
        //    renderer_tilemapRight.material = mat;
        //    renderer_tilemapUp.material = mat;
        //    renderer_tilemapDown.material = mat;
        //    renderer_tilemap.material = mat;
        //    renderer_tilemapOverlay.material = mat;
        //}

        [SerializeField]
        private Material MaterialWithShadow;
        [SerializeField]
        private Material MaterialCharacterWithShadow;


        public Gradient StarLightColorOverTime;
        private void UpdateDayNightCycleLightingAndShadow() {

            GlobalLight.Ins.UseCharacterLight = false; // TheOnlyActiveMap.ControlCharacter;

            bool hasCycle = TheOnlyActiveMap is IHasDayNightRecycle;
            GlobalLight.Ins.UseDayNightCycle = hasCycle;

            if (hasCycle) {
                const float day_duration_in_second = 24;
                float day_count = Time.time / day_duration_in_second;
                float progress_of_day = day_count - (int)day_count;
                float t = progress_of_day * (2 * Mathf.PI);

                float star_light_pos_x = Mathf.Cos(t);
                float star_light_pos_y = -Mathf.Sin(t);

                MaterialWithShadow.SetFloat("_StarLightPosX", star_light_pos_x);
                MaterialWithShadow.SetFloat("_StarLightPosY", star_light_pos_y);
                MaterialCharacterWithShadow.SetFloat("_StarLightPosX", star_light_pos_x);
                MaterialCharacterWithShadow.SetFloat("_StarLightPosY", star_light_pos_y);

                float t_day;
                float t_night;


                const float twilightTime = 0.075f; // 0.01f - 0.25f
                if (progress_of_day < twilightTime) {
                    t_day = progress_of_day / twilightTime;
                } else if (progress_of_day < 0.5f - twilightTime) {
                    t_day = 1;
                } else if (progress_of_day < 0.5f + twilightTime) {
                    t_day = 1 - (progress_of_day - (0.5f - twilightTime)) / (2 * twilightTime);
                } else if (progress_of_day < 1 - twilightTime) {
                    t_day = 0;
                } else {
                    t_day = -1 + (progress_of_day - twilightTime);
                }
                t_night = 1 - t_day;



                float day_shadow = t_day * t_day;
                float night_shadow = t_night * t_night * t_night;


                MaterialCharacterWithShadow.SetFloat("_PlayerLightAlpha", t_night);
                MaterialWithShadow.SetFloat("_PlayerLightAlpha", t_night);

                MaterialCharacterWithShadow.SetFloat("_StarLightAlpha", day_shadow);
                MaterialWithShadow.SetFloat("_StarLightAlpha", day_shadow);

                const float playerLightContribution = 0.7f;


                GlobalLight.Ins.CharacterLightIntensity = Mathf.Lerp(0, playerLightContribution, night_shadow);
                GlobalLight.Ins.StarLightIntensity = Mathf.Lerp(1 - playerLightContribution, 1.05f, t_day);
                GlobalLight.Ins.StarLightColor = StarLightColorOverTime.Evaluate(progress_of_day);

                Vector3 starLightPosition = mainCameraTransform.position + 20 * new Vector3(star_light_pos_x, star_light_pos_y, 0);
                starLightPosition.z = 1;
                GlobalLight.Ins.StarLightPosition = starLightPosition;

                GlobalLight.Ins.UseDayNightCycle = true;
                if (TheOnlyActiveMap.ControlCharacter) {

                }
            }
        }



































        [Space]
        [Header("Tilemaps")]

        [SerializeField]
        private Tilemap tilemapBedrock;
        [SerializeField]
        private Tilemap tilemapWater;
        [SerializeField]
        private Tilemap tilemapGrass;
        [SerializeField]
        private Tilemap tilemapTree;
        [SerializeField]
        private Tilemap tilemapHill;
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
        private Tilemap tilemap;
        [SerializeField]
        private Tilemap tilemapOverlay;


        private TilemapRenderer renderer_tilemapBedrock;
        private TilemapRenderer renderer_tilemapGrass;
        private TilemapRenderer renderer_tilemapTree;
        private TilemapRenderer renderer_tilemapHill;
        private TilemapRenderer renderer_tilemapRoad;

        private TilemapRenderer renderer_tilemapLeft;
        private TilemapRenderer renderer_tilemapRight;
        private TilemapRenderer renderer_tilemapUp;
        private TilemapRenderer renderer_tilemapDown;

        private TilemapRenderer renderer_tilemap;
        private TilemapRenderer renderer_tilemapOverlay;


        [Space]
        [Header("Other")]

        [SerializeField]
        private Camera mainCamera;
        private Transform mainCameraTransform;
        [SerializeField]
        private GameObject playerCharacter;
        private Transform characterTransform;

        [SerializeField]
        private CharacterView characterView;




        [SerializeField]
        private Transform Head;
        [SerializeField]
        private Transform Tail;

        private bool tapping = false;
        private const float tappingSensitivity = 0.05f;

        private Vector2 tailMousePosition;
        //private Vector2 originalMousePosition;
        private Vector3 originalDownMousePosition;
        private Vector2 deltaDistance;

        private Vector2 tail;
        private bool hasBeenOutOfTheSameTile;

        [SerializeField]
        private GameObject Indicator;
        private void UpdateIndicator(Vector2Int pos) {
            Indicator.transform.position = (Vector2)(pos) + new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// 获取输入, 计算moving和tapping
        /// </summary>
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

            bool onSameTile = false; // 检验是否在同一个按下
            Vector2Int nowInt = MathVector2Floor(head);
            if (nowInt == MathVector2Floor(originalDownMousePosition)) {
                onSameTile = true;
            } else {
                hasBeenOutOfTheSameTile = true;
            }

            bool showHeadAndTail = false;

            if (Input.GetMouseButton(0)) {
                // 移动检测
                float radius = Math.Min(Screen.width, Screen.height) / 10;
                Vector2 deltaMousePosition = mousePosition - tailMousePosition;
                if (deltaMousePosition.sqrMagnitude > radius * radius) {
                    tailMousePosition += deltaMousePosition * Mathf.Min(0.64f, Time.deltaTime * 10);
                }

                tail = mainCamera.ScreenToWorldPoint(tailMousePosition);

                deltaDistance = head - tail;
                if (GameMenu.UseInversedMovement) {
                    deltaDistance = -deltaDistance;
                }
                if (deltaDistance.sqrMagnitude > 1f) {
                    deltaDistance.Normalize();
                }
                tapping = deltaDistance.sqrMagnitude > tappingSensitivity * tappingSensitivity;

                showHeadAndTail = !UI.Ins.Active && tapping && (!onSameTile || hasBeenOutOfTheSameTile);

                // 拖拽按钮显示条件：主UI不显示, 正在拖拽, 在相同格子上放下
                // bool showHeadAndTail = !UI.Ins.Active && tapping && (!onSameTile || hasBeenOutOfTheSameTile);
                Head.gameObject.SetActive(showHeadAndTail);
                Tail.gameObject.SetActive(showHeadAndTail);


                Head.localPosition = head;
                Tail.localPosition = tail;

                // 移动端金色边框显示条件：主UI不显示, 拖拽UI不显示
                if (GameMenu.IsInMobile) {
                    Indicator.SetActive(!showHeadAndTail && !UI.Ins.Active);

                }
                if (!showHeadAndTail) {
                    UpdateIndicator(MathVector2Floor(mainCamera.ScreenToWorldPoint(mousePosition)));
                }
            }

            // Standalone专享功能, 显示鼠标指针位置的TileDescription
            if (GameMenu.IsInStandalone) {
                bool showIndicator = !UI.Ins.Active && !showHeadAndTail;
                Indicator.SetActive(showIndicator);
                if (showIndicator) {
                    UpdateIndicator(MathVector2Floor(mainCamera.ScreenToWorldPoint(mousePosition)));
                }

                ITile tile = TheOnlyActiveMap.Get(nowInt.x, nowInt.y);
                if (tile != theTileToBeTapped) {
                    if (tile is ITileDescription tileDescription) {
                        GameMenu.Ins.SetTileDescriptionForStandalong(tileDescription.TileDescription);
                    } else {
                        GameMenu.Ins.SetTileDescriptionForStandalong(Localization.Ins.Get(tile.GetType()));
                    }

                    theTileToBeTapped = tile;
                }
            }

            // 检测是否按下地块
            if (Input.GetMouseButtonUp(0)) {
                if (onSameTile) {

                    // 在非编辑器模式下, 捕捉报错, 并且
                    if (GameMenu.IsInEditor) {
                        OnTap(nowInt);
                    } else {
                        try {
                            OnTap(nowInt);
                        } catch (Exception e) {
                            UI.Ins.ShowItems("地块出现错误! ! ! ", UIItem.CreateText(e.GetType().Name), UIItem.CreateMultilineText(e.Message), UIItem.CreateMultilineText(e.StackTrace));
                            throw e;
                        }
                    }
                }
                if (GameMenu.IsInMobile) {
                    Indicator.SetActive(false);
                }
                Head.gameObject.SetActive(false);
                Tail.gameObject.SetActive(false);
            }
        }
        private ITile theTileToBeTapped;

        private Vector2Int MathVector2Floor(Vector2 vec) {
            return new Vector2Int((int)Mathf.Floor(vec.x), (int)Mathf.Floor(vec.y));
        }




        // 按到gameMenu按钮时, 临时禁用onTap。也许有执行顺序的bug
        public static bool InterceptInteractionOnce = false;
        //private AudioClip soundOfMagnet;
        //private AudioClip soundOfHammer;

        /// <summary>
        /// 地块被按下时的通用逻辑。锤子和磁铁工具的功能也配置在这里了
        /// </summary>
        private void OnTap(Vector2Int pos) {
            // UI 打开时, 禁用OnTap
            if (UI.Ins.Active) {
                return;
            }
            // GameMenu 点击时, 禁用OnTap, 一次
            if (InterceptInteractionOnce) {
                InterceptInteractionOnce = false;
                return;
            }

            // 被Tap的地图
            IMapDefinition map = TheOnlyActiveMap as IMapDefinition;
            if (map == null) throw new Exception();
            // 被Tap的地块
            ITile tile = map.Get(pos.x, pos.y);
            if (tile == null) throw new Exception();
            Type tileType = tile.GetType();
            // 被Tap的地块, 若可运行
            IRunnable runable = tile as IRunnable;

            // 快捷方式
            GameMenu.ShortcutMode CurrentMode = GameMenu.Ins.CurrentShortcutMode;
            // 快捷方式参数
            Type shortcutType = UIItem.ShortcutType;

            // 地图默认类型
            Type defaultTileType = map.DefaultTileType;
            // 地块是否属于地图默认类型
            bool tileIsDefaultTileType = defaultTileType.IsAssignableFrom(tileType);


            // 大部分简单工具已经弃用了, 一般使用多功能工具
            if (CurrentMode != GameMenu.ShortcutMode.None) {
                // 无视工具的条件。目前询问tile
                IIgnoreTool ignoreTool = tile as IIgnoreTool;
                bool dontIgnoreToolForTile = ignoreTool == null || !ignoreTool.IgnoreTool;

                if (CurrentMode == GameMenu.ShortcutMode.LinkUnlink) {
                    if (Globals.IsCool) {
                        if (tile is IMagnetAttraction magnetAttraction) {
                            (Type, long) attracted = magnetAttraction.Attracted;
                            Type attractedType = attracted.Item1;
                            long attractedQuantity = attracted.Item2; ;
                            if (attractedType != null && attractedQuantity > 0) {
                                if (map.Inventory.CanAdd(attractedType) >= attractedQuantity) {
                                    if (Globals.Sanity.Val >= attractedQuantity) {
                                        map.Inventory.Add(attractedType, attractedQuantity);
                                        Globals.Sanity.Val -= attractedQuantity;
                                        GameMenu.Ins.PushNotification($"磁铁吸引到{Localization.Ins.Val(attractedType, attractedQuantity)}");
                                        Globals.SetCooldown = 1;
                                        tile.OnTapPlaySound();
                                    } else {
                                        GameMenu.Ins.PushNotification($"体力不足, 无法使用磁铁");
                                    }
                                }
                                else {
                                    GameMenu.Ins.PushNotification($"背包已满");
                                }
                            }
                        }
                    }
                }

                if (dontIgnoreToolForTile) {
                    switch (CurrentMode) {
                        // 建造和拆除工具
                        case GameMenu.ShortcutMode.ConstructDestruct:
                            // 如果是TerrainDefault, 并且有快捷方式
                            if (tileIsDefaultTileType && shortcutType != null) {
                                // 如果能造, 则造
                                if (map.CanUpdateAt(shortcutType, pos)) {
                                    map.UpdateAt(shortcutType, tile);
                                    tile.OnTapPlaySound();
                                }
                            }
                            // 如果是建筑
                            else {
                                // 如果可以停止, 则停止
                                if (runable != null && !LinkUtility.HasAnyLink(tile)) {
                                    if (runable.CanStop()) runable.Stop();
                                }
                                // 如果可以拆除, 则拆除
                                if (tile.CanDestruct()) {
                                    map.UpdateAt(defaultTileType, tile);
                                    tile.OnTapPlaySound();
                                }
                                else {
                                    GameMenu.Ins.PushNotification($"复制建筑{Localization.Ins.Get(tileType)}");
                                }

                                UIItem.ShortcutType = tileType; // 复制
                            }
                            break;

                        // 物流工具, 也常用于运行
                        case GameMenu.ShortcutMode.LinkUnlink:
                            // 采集资源
                            if (!LinkUtility.HasAnyLink(tile)) {
                                // 如果没连接

                                // 尝试建立输入连接, 有上下左右的优先顺序
                                LinkUtility.AutoConsume(tile);

                                // 如果能够运行, 则运行。如果能停止, 则停止
                                if (runable != null) {
                                    if (runable.Running) {
                                        if (runable.CanStop()) {
                                            runable.Stop();
                                            tile.OnTapPlaySound();
                                            GameMenu.Ins.PushNotification($"{Localization.Ins.Get(tile.GetType())}停止运行");
                                        }
                                    } else {
                                        if (runable.CanRun()) {
                                            runable.Run();
                                            tile.OnTapPlaySound();
                                            GameMenu.Ins.PushNotification($"{Localization.Ins.Get(tile.GetType())}开始运行");
                                        }
                                    }
                                }
                            } else {
                                // 如果有连接

                                // 如果能停止, 则停止
                                if (runable != null && runable.CanStop()) {
                                    runable.Stop();
                                    tile.OnTapPlaySound();
                                }

                                // 如果能取消输出, 先取消
                                LinkUtility.AutoProvide_Undo(tile);
                                // 如果能取消输入, 则取消
                                LinkUtility.AutoConsume_Undo(tile);

                                // 如果上述操作过后, 还有连接, 说明? 
                                if (LinkUtility.HasAnyLink(tile)) {
                                    LinkUtility.AutoConsume(tile);
                                }
                            }
                            break;
                    }
                }
            } else if (TheOnlyActiveMap.ControlCharacter && CharacterPositionInternal == pos) {
                GameMenu.Ins.OnTapPlayerInventory();
                tile.OnTapPlaySound();
            } else {
                map.OnTapTile(tile);
                tile.OnTapPlaySound();
            }
        }
    }
}

