
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    //public interface IGameEntry
    //{
    //    void EnterMap(string mapKey, bool enterChildMap = false);
    //    void EnterParentMap(Type parentType, IMapDefinition map);
    //    void EnterChildMap(Type childType, IMapDefinition map, Vector2Int pos);
    //    ITile GetParentTile(Type parentType, IMapDefinition map);
    //    void DeleteMap(IMapDefinition map, bool enterSelf = false);

    //    void SaveGame();
    //    void TrySaveGame();
    //    void DeleteGameSave();

    //    void ExitGame();
    //    void ExitGameUnsaved();
    //}

    public class GameAutoSaveInterval { }

    public class GameEntry : MonoBehaviour //, IGameEntry
    {

        public static GameEntry Ins { get; private set; }

        private void Awake() {
            // 单例
            if (Ins != null) throw new Exception();
            Ins = this;

////#if !UNITY_EDITOR
//            Application.logMessageReceived += (string condition, string stackTrace, LogType type) => {
//                UI.Ins.ShowItems($"奇怪的错误出现了! ! !{type} ",
//                    UIItem.CreateText("快把错误截图糊在程序员脸上"),
//                    UIItem.CreateText(stackTrace),
//                    UIItem.CreateSeparator(),
//                    UIItem.CreateMultilineText(System.Environment.StackTrace)
//                    );
//            };
////#endif

            // 游戏在后台不会占用资源
            Application.runInBackground = false;

            // 依赖注入到GameMenu
            if (GameMenu.Entry != null) throw new Exception();
            GameMenu.Entry = this;

            // Awake加载顺序
            // 1. AttributePreprocessor
            // 2. DataPersistence
            // 3. Globals
            // 4. GameEntry
            data = DataPersistence.Ins;
            globals = (Globals.Ins as IGlobalsDefinition);
            if (globals == null) throw new Exception();

            // 读取或创建Globals.Ins
            if (data.HasGlobals()) {
                data.LoadGlobals();
            } else {
                globals.ValuesInternal = Values.GetOne();
                globals.RefsInternal = Refs.GetOne();
                globals.PlayerPreferencesInternal = new Dictionary<string, string>();
                globals.InventoryInternal = Inventory.GetOne();

                constructGameThisTime = true;
            }
        }

        private const string gameEntryMapKey = "__GAME_ENTRY_INDEX__";
        private bool constructGameThisTime = false;
        private IGlobalsDefinition globals;
        private IValue autoSaveInterval;

        private void Start() {
            // 读取或创建自动存档间隔
            if (constructGameThisTime) {

                GameConfig.OnConstruct(globals); // 全局游戏逻辑相关
                GameMenu.OnConstruct(); // 全局游戏设置相关

                // 自动存档功能也在GameEntry里负责
                autoSaveInterval = Globals.Ins.Values.Create<GameAutoSaveInterval>();
                autoSaveInterval.Max = 60;
                if (autoSaveInterval.Max < 10) throw new Exception(); // 太短了吧
            }
            autoSaveInterval = Globals.Ins.Values.Get<GameAutoSaveInterval>();


            if (globals.PlayerPreferences.TryGetValue(gameEntryMapKey, out string mapKey)) {
                // 如果Globals记录了了之前的地图, 则直接进入
                EnterMap(mapKey);
            } else {
                // 如果Globals没有进入之前的地图, 进入指定地图
                globals.PlayerPreferences.Add(gameEntryMapKey, null);
                // EnterMap(typeof(MapOfUniverse), $"");
                EnterMap(GameConfig.InitialMapKey);
            }
            lastSaveTimeInSeconds = TimeUtility.GetSeconds(); // 自动存档间隔
            GameConfig.OnGameEnable();
        }


        /// <summary>
        /// MapKey满足以下格式：
        /// <类型.FullName>#=x,y=x,y=x,y
        /// 其子类
        /// <类型.FullName>#=x,y=x,y=x,y=x,y
        /// 其父类
        /// <类型.FullName>#=x,y=x,y
        /// </summary>

        public const string MAGIC = "#";
        public const char MAGIC_CHAR = '#';
        public const char MAGIC_CHAR2 = '=';
        public const char MAGIC_CHAR3 = ',';


        private Vector2Int GetPosInParentMap(string mapKey) {
            int startIndex = mapKey.LastIndexOf(MAGIC_CHAR2);
            if (startIndex < 0) throw new Exception("没有上级地图");
            string subString = mapKey.Substring(startIndex + 1);
            string[] splitedStrings = subString.Split(MAGIC_CHAR3);
            if (splitedStrings.Length != 2) throw new Exception();

            if (!int.TryParse(splitedStrings[0], out int x)) throw new Exception($"{subString} --- {mapKey}");
            if (!int.TryParse(splitedStrings[1], out int y)) throw new Exception($"{subString} --- {mapKey}");
            return new Vector2Int(x, y);
        }
        /// <summary>
        /// 输入 <类型.FullName>#=x,y=x,y=x,y
        /// 输出 #=x,y=x,y
        /// 
        /// 输入 <类型.FullName>#=x,y
        /// 输出 #
        /// </summary>
        private static string SliceParentMapKeyIndex(string mapKey) {
            int startIndex = mapKey.IndexOf(MAGIC);
            int endIndex = mapKey.LastIndexOf(MAGIC_CHAR2);
            if (endIndex < 0) return MAGIC;
            return mapKey.Substring(startIndex, endIndex - startIndex);
        }
        private static string ConstructParentMapKey(Type parentType, string selfIndex) {
            return $"{parentType.FullName}{SliceParentMapKeyIndex(selfIndex)}";
        }
        public void EnterParentMap(Type parentType, IMapDefinition mapDefinition) {
            string mapKey = mapDefinition.MapKey;

            string parentMapKey = ConstructParentMapKey(parentType, mapKey);
            if (parentMapKey == null) throw new Exception(mapKey);
            EnterMap(parentMapKey);
        }
        /// <summary>
        /// 输入 <类型.FullName>#=x,y=x,y=x,y
        /// 输出 #=x,y=x,y=x,y
        /// </summary>
        private static string SliceSelfMapKeyIndex(string mapKey) {
            int startIndex = mapKey.IndexOf(MAGIC_CHAR);
            if (startIndex < 0) return MAGIC;
            return mapKey.Substring(startIndex, mapKey.Length - startIndex);
        }
        public void EnterChildMap(Type childType, IMapDefinition mapDefinition, Vector2Int pos) {
            string mapKey = mapDefinition.MapKey;

            string childMapKey = $"{childType.FullName}{ConstructMapKeyIndexWithPosition(mapKey, pos)}";
            EnterMap(childMapKey, true);

            UI.Ins.Active = false;
        }
        private static string ConstructMapKeyIndexWithPosition(string mapKey, Vector2Int pos) => $"{SliceSelfMapKeyIndex(mapKey)}{MAGIC_CHAR2}{pos.x}{MAGIC_CHAR3}{pos.y}";
        /// <summary>
        /// 例如, 在银河系, 求本tile对应恒星hashcode
        /// </summary>
        public static uint ChildMapKeyHashCode(IMap map, Vector2Int pos) => HashUtility.Hash(ConstructMapKeyIndexWithPosition((map as IMapDefinition).MapKey, pos));
        /// <summary>
        /// 例如, 在恒星系, 求此本恒星系恒星hashcode
        /// </summary>
        public static uint SelfMapKeyHashCode(IMap map) => HashUtility.Hash(SliceSelfMapKeyIndex((map as IMapDefinition).MapKey));


        // char '#' 是用魔法强耦合的
        public void EnterMap(string mapKey, bool enterChildMap = false) {
            string[] args = mapKey.Split(MAGIC_CHAR);
            if (args.Length != 2) throw new Exception(mapKey);

            Type selfType = Type.GetType(args[0]);
            if (selfType == null) throw new Exception(mapKey);
            string selfIndex = args[1];

            EnterMap(selfType, selfIndex, mapKey, enterChildMap);

            if (!UI.DontCloseOnIntroduction) {
                UI.Ins.Active = false;
            }
        }

        public UnityEngine.Rendering.Volume volume;
        private void TEMP(Type selfType) {
            volume.enabled = selfType != typeof(MapOfPlanet);
        }

        private void EnterMap(Type selfType, string selfIndex, string selfKeyVertify = null, bool enterChildMap = false) {

            TEMP(selfType); // 临时用于录视频

            if (selfType == null) throw new Exception();
            if (selfIndex == null) throw new Exception();

            // mapKey由四部分组成
            string mapKey = $"{selfType.FullName}{MAGIC_CHAR}{selfIndex}";
            if (selfKeyVertify != null && !selfKeyVertify.Equals(mapKey)) throw new Exception();

            globals.PlayerPreferences[gameEntryMapKey] = mapKey;

            // 目前"活跃地图"以"MapView.Ins.Map"访问
            IMap oldMap = MapView.Ins.TheOnlyActiveMap;
            IMapDefinition oldMapDefinition = MapView.Ins.TheOnlyActiveMap as IMapDefinition;
            if (oldMap != null) {
                SaveGame(); // 读新图前, 保存
            }

            IMapDefinition map = null;
            // 正好要进入parentMap
            if (parentMap != null && parentMap.MapKey.Equals(mapKey)) {
                map = parentMap;
            } else {
                map = Activator.CreateInstance(selfType) as IMapDefinition;
                if (map == null) throw new Exception(mapKey);
                map.MapKey = mapKey;
                map.HashCode = HashUtility.Hash(mapKey);
            }
            // 优化, 如果进入子地图, parentMap设置为oldMap
            parentMap = enterChildMap ? oldMapDefinition : null;


            MapView.Ins.TheOnlyActiveMap = map;

            // 每个IMap实例的MapKey对应一个存档里有这个地图
            if (data.HasMap(mapKey)) {
                data.LoadMapHead(map, mapKey);
                map.OnEnable();
                data.LoadMapBody(map, mapKey);
            } else {
                map.OnConstruct();
                map.OnEnable();
                ConstructMapBody(map);
                map.AfterConstructMapBody();
            }

            // 记录当前地图

#if UNITY_EDITOR
            // Debug.LogWarning($"Enter Map {selfKey}");
#endif
        }

        public void DeleteMap(IMapDefinition map, bool enterSelf=false) {
            string mapKey = map.MapKey;
            if (map.CanDelete) {
                map.EnterParentMap();
                data.DeleteMap(map);
                if (enterSelf) {
                    EnterMap(mapKey);
                }
            }
        }


        private IMapDefinition parentMap = null;

        public ITile GetParentTile(Type parentType, IMapDefinition mapDefinition) {
            if (mapDefinition != MapView.Ins.TheOnlyActiveMap) throw new Exception();

            string mapKey = mapDefinition.MapKey;
            Vector2Int positionInParentMap = GetPosInParentMap(mapKey);

            if (parentMap == null) {
                string parentMapKey = ConstructParentMapKey(parentType, mapKey);

                parentMap = Activator.CreateInstance(parentType) as IMapDefinition;
                if (parentMap == null) throw new Exception(parentType.Name);
                parentMap.MapKey = parentMapKey;
                parentMap.HashCode = HashUtility.Hash(parentMapKey);

                // 每个IMap实例的MapKey对应一个存档里有这个地图
                if (data.HasMap(parentMapKey)) {
                    data.LoadMapHead(parentMap, parentMapKey);
                    parentMap.OnEnable();
                    data.LoadMapBody(parentMap, parentMapKey);
                } else {
                    parentMap.OnConstruct();
                    parentMap.OnEnable();
                    ConstructMapBody(parentMap);
                }
            } else {
                if (parentType != parentMap.GetType()) {
                    throw new ArgumentException(parentType.Name);
                }
            }
            ITile tile = parentMap.Get(positionInParentMap); // 可能有异常
            if (tile == null) throw new Exception();
            return tile;
        }


        private void ConstructMapBody(IMapDefinition map) {
            Type tileType = map.DefaultTileType;
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    // Type tileType = map.GenerateTileType(new Vector2Int(i, j)); // 每个地图自己决定在ij生成什么地块
                    if (tileType == null) throw new Exception();
                    ITileDefinition tile = Activator.CreateInstance(tileType) as ITileDefinition;
                    if (tile == null) throw new Exception(tileType.Name);
                    map.SetTile(new Vector2Int(i, j), tile, true);
                    tile.Map = map;
                    tile.Pos = new Vector2Int(i, j);
                    tile.TileHashCode = HashUtility.Hash(i, j, map.Width, map.Height, (int)map.HashCode); //HashUtility.Hash((uint)(i + j * map.Width));
                    tile.OnConstruct(null);
                }
            }
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    ITileDefinition tile = map.Get(i, j) as ITileDefinition;
                    if (tile == null) throw new Exception();
                    tile.NeedUpdateSpriteKeys = true;
                    tile.OnEnable();
                }
            }
        }

        private long lastSaveTimeInSeconds = 0;
        private IDataPersistence data;
        private void Update() {
            long now = TimeUtility.GetSeconds();
            long delta = now - lastSaveTimeInSeconds;
            if (delta > 5 * autoSaveInterval.Max) {
                SaveGame();
                lastSaveTimeInSeconds = now;
            }
        }

        public void TrySaveGame() {
            long now = TimeUtility.GetSeconds();
            long delta = now - lastSaveTimeInSeconds;
            if (delta > autoSaveInterval.Max) {
                SaveGame();
                lastSaveTimeInSeconds = now;
            }
        }

        // 存档
        public void SaveGame() {
            IMapDefinition map = MapView.Ins.TheOnlyActiveMap as IMapDefinition;
            if (map == null) throw new Exception();
            map.OnDisable();


            // 开始存档
            const string save_complete = "__save_complete__";
            const string incomplete = "incomplete";

            // 损坏校验
            if (!DataPersistence.Ins.HasSave(save_complete)) {
                DataPersistence.Ins.WriteSave(save_complete, TimeUtility.GetTicks().ToString());
            }
            if (DataPersistence.Ins.ReadSave(save_complete).StartsWith(incomplete)) {
                throw new Exception("存档损坏");
            }
            DataPersistence.Ins.WriteSave(save_complete, $"{incomplete} {TimeUtility.GetTicks()}");

            // 存档
            data.SaveGlobals();
            data.SaveMapHead(map); // 保存地图
            data.SaveMapBody(map);

            if (parentMap != null) {
                data.SaveMapHead(parentMap);
                data.SaveMapBody(parentMap);
            }

            lastSaveTimeInSeconds = TimeUtility.GetSeconds();

            // 结束存档
            DataPersistence.Ins.WriteSave(save_complete, TimeUtility.GetTicks().ToString());
#if UNITY_EDITOR
            Debug.LogWarning("Save OK");
#endif
        }

        // 删除存档
        public void DeleteGameSave() {
            data.DeleteSaves();
            // Debug.Log("<color=red>Save Deleted</color>");
            ExitGameUnsaved();
            // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        public void ExitGame() {
            SaveGame();
            ExitGameInternal();
        }

        public void ExitGameUnsaved() {
            ExitGameInternal();
        }

        private void ExitGameInternal(bool save = true) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                                Application.Quit();
#endif
        }
    }
}

