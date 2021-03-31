﻿

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CameraX { }
    public class CameraY { }

    public class CharacterX { }
    public class CharacterY { }

    public class ClearColorR { }
    public class ClearColorG { }
    public class ClearColorB { }

    public struct CostInfo
    {
        public Type CostType;
        public long BaseCostQuantity;
        public long RealCostQuantity;
        public long CostMultiplier;
        public long CountForDoubleCost;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ConstructionCostBaseAttribute : Attribute
    {
        public readonly Type CostType;
        public readonly long CostQuantity;
        public readonly long CountForDoubleCost;
        public ConstructionCostBaseAttribute(Type costType, long costQuantity, long costForDoubleCost = 10) {
            if (costType == null) throw new Exception(ToString());
            if (costQuantity < 0) throw new Exception(ToString());
            if (costQuantity == 0 && costForDoubleCost != 0) throw new Exception(ToString());
            if (costForDoubleCost < 0 || CountForDoubleCost >= 10000) throw new Exception(ToString());
            CostType = costType;
            CostQuantity = costQuantity;
            CountForDoubleCost = costForDoubleCost;
        }
        public static ValueTuple<Type, long> GetCostBase(Type type) {
            ConstructionCostBaseAttribute attr = Tag.GetAttribute<ConstructionCostBaseAttribute>(type);
            if (attr == null) {
                return (null, 0);
            }
            return (attr.CostType, attr.CostQuantity);
        }
        public static CostInfo GetCost(Type type, IMap map, bool forConstruction) {
            CostInfo result = new CostInfo();
            ConstructionCostBaseAttribute attr = Tag.GetAttribute<ConstructionCostBaseAttribute>(type);
            if (attr == null) {
                return result;
            }
            result.CostType = attr.CostType;
            result.BaseCostQuantity = attr.CostQuantity;
            result.CostMultiplier = GetCostMultiplier(type, map, forConstruction, attr.CountForDoubleCost);
            result.RealCostQuantity = attr.CostQuantity * result.CostMultiplier;
            result.CountForDoubleCost = attr.CountForDoubleCost;
            return result;
        }
        public static long GetCostMultiplier(Type type, IMap map, bool forConstruction, long countForDoubleCost) {
            long count = map.Refs.GetOrCreate(type).Value; // Map.Ref.Get<建筑>.Value，为建筑数量。Map.Ref.Get<资源>.Value，为资源产量
            if (!forConstruction) {
                // 计算拆除返还费用，与建筑费用有1count的差距。如count为10时，建筑费用增加，拆除费用不变
                count--;
            }
            if (count < 0) throw new Exception($"建筑数量为负 {type.Name} {count}");

            //// 10个以上建筑时，才开始增加费用
            //count = Math.Max(count - 10, 0);

            const long maximun = long.MaxValue / 100000;

            long multiplier = 1;

            if (countForDoubleCost != 0) {
                long magic = countForDoubleCost;
                long magic10 = magic * 10;

                while (count / magic10 > 0) {
                    count -= magic10;
                    multiplier *= 1000;

                    if (multiplier > maximun) break;
                }
                while (count / magic > 0) {
                    count -= magic;
                    multiplier *= 2;

                    if (multiplier > maximun) break;
                }
            }

            return multiplier;
        }
    }

    public abstract class StandardMap : IMapDefinition, ILandable
    {

        public virtual Type DefaultTileType { get; } = typeof(TerrainDefault);

        public int HashCode { get; private set; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        public virtual string GetSpriteKeyBackground(uint hashcode) => $"GrasslandBackground_{hashcode % 16}";

        public virtual void Update() { }



        #region landing
        public bool ControlCharacter => landed.Max == 1;

        public bool Landed {
            get => ControlCharacter;
        }

        public void Land(Vector2Int pos) {
            landed.Max = 1;
            SetCharacterPos(pos);
        }
        public void Leave() {
            landed.Max = 0;
        }

        private IValue landed;

        #endregion

        public virtual void OnConstruct() {
            if (Values == null) {
                Tiles = new ITileDefinition[Width, Height];
                Values = Weathering.Values.GetOne();
            } else {
                throw new Exception();
            }
            if (Refs == null) {
                Refs = Weathering.Refs.GetOne();
            } else {
                throw new Exception();
            }
            if (Inventory == null) {
                Inventory = Weathering.Inventory.GetOne();
            } else {
                throw new Exception();
            }

            Values.Create<CharacterX>();
            Values.Create<CharacterY>();
            Values.Create<CameraX>();
            Values.Create<CameraY>();
            Values.Create<ClearColorR>();
            Values.Create<ClearColorG>();
            Values.Create<ClearColorB>();

            landed = Values.Create<CharacterLanded>();
            landed.Max = 0;
        }

        protected void SetCharacterPos(Vector2Int characterPosition) {
            Values.Get<CharacterX>().Max = characterPosition.x;
            Values.Get<CharacterY>().Max = characterPosition.y;
            MapView.Ins.CharacterPosition = characterPosition;
        }

        protected void SetCameraPos(Vector2 cameraPos) {
            Values.Get<CameraX>().Max = (long)(cameraPos.x * factor);
            Values.Get<CameraY>().Max = (long)(cameraPos.y * factor);
            MapView.Ins.CameraPosition = cameraPos;
        }

        protected void SetClearColor(Color clearColor) {
            Values.Get<ClearColorR>().Max = (long)(clearColor.r * factor);
            Values.Get<ClearColorG>().Max = (long)(clearColor.g * factor);
            Values.Get<ClearColorB>().Max = (long)(clearColor.b * factor);
            MapView.Ins.ClearColor = clearColor;
        }

        public virtual void OnEnable() {
            HashCode = GetType().Name.GetHashCode();
            autoInc = RandomSeed;
            altitudeConfig = GetAltitudeConfig;
            moistureConfig = GetMoistureConfig;
            temporatureConfig = GetTemporatureConfig;
            GenerateNoise();

            Vector2 cameraPos = Vector2.zero;
            cameraPos.x = Values.Get<CameraX>().Max / factor;
            cameraPos.y = Values.Get<CameraY>().Max / factor;
            MapView.Ins.CameraPosition = cameraPos;

            Color color = Color.black;
            color.r = Values.Get<ClearColorR>().Max / factor;
            color.g = Values.Get<ClearColorG>().Max / factor;
            color.b = Values.Get<ClearColorB>().Max / factor;
            MapView.Ins.ClearColor = color;
            MapView.Ins.CharacterPosition = new Vector2Int((int)Values.Get<CharacterX>().Max, (int)Values.Get<CharacterY>().Max);

            landed = Values.Get<CharacterLanded>();
        }
        private const float factor = 1024f;
        public void OnDisable() {
            SetCharacterPos(MapView.Ins.CharacterPosition);
            SetCameraPos(MapView.Ins.CameraPosition);
            SetClearColor(MapView.Ins.ClearColor);
        }

        public IValues Values { get; protected set; }
        public void SetValues(IValues values) => Values = values;
        public IRefs Refs { get; protected set; }

        public void SetRefs(IRefs refs) => Refs = refs;
        public IInventory Inventory { get; protected set; }
        public void SetInventory(IInventory inventory) => Inventory = inventory;

        // ------------------------------------------------------------

        protected ITileDefinition[,] Tiles;

        public ITile Get(int i, int j) {
            Validate(ref i, ref j);
            ITile result = Tiles[i, j];
            return result;
        }

        public ITile Get(Vector2Int pos) {
            return Get(pos.x, pos.y);
        }

        public T UpdateAt<T>(Vector2Int pos) where T : class, ITile {
            return UpdateAt(typeof(T), pos.x, pos.y) as T;
        }
        public ITile UpdateAt(Type type, Vector2Int pos) {
            return UpdateAt(type, pos.x, pos.y);
        }

        public T UpdateAt<T>(int i, int j) where T : class, ITile {
            return UpdateAt(typeof(T), i, j) as T;
        }
        public ITile UpdateAt(Type type, int i, int j) {
            Validate(ref i, ref j);

            ITileDefinition oldTile = Tiles[i, j];
            if (!GameConfig.CheatMode) {
                // 居然在这里消耗资源，架构不好

                // 拆除时返还资源
                CostInfo desctructOldCost = ConstructionCostBaseAttribute.GetCost(oldTile.GetType(), this, false);

                // 建筑时消耗资源
                CostInfo constructNewCost = ConstructionCostBaseAttribute.GetCost(type, this, true);

                if (desctructOldCost.CostType != null) {
                    if (!Inventory.CanAdd((desctructOldCost.CostType, desctructOldCost.RealCostQuantity))) {
                        UI.Ins.ShowItems("背包空间不足", UIItem.CreateMultilineText($"{Localization.Ins.Val(desctructOldCost.CostType, desctructOldCost.RealCostQuantity)} 被拆建筑资源无法返还"));
                        return null;
                    }
                }
                if (constructNewCost.CostType != null) {
                    if (!Inventory.CanRemoveWithTag((constructNewCost.CostType, constructNewCost.RealCostQuantity))) {
                        var items = UI.Ins.GetItems();
                        items.Add(UIItem.CreateMultilineText($"无法建造{Localization.Ins.Get(type)}"));
                        items.Add(UIItem.CreateButton("关闭", () => UI.Ins.Active = false));

                        items.Add(UIItem.CreateMultilineText($"需要{Localization.Ins.Val(constructNewCost.CostType, constructNewCost.RealCostQuantity)}"));

                        items.Add(UIItem.CreateSeparator());

                        UIItem.AddItemDescription(items, constructNewCost.CostType);

                        UI.Ins.ShowItems($"建筑资源不足", items);
                        return null;
                    }
                }
                if (constructNewCost.CostType != null) {
                    Inventory.RemoveWithTag(constructNewCost.CostType, constructNewCost.RealCostQuantity);
                }
                if (desctructOldCost.CostType != null) {
                    Inventory.Add(desctructOldCost.CostType, desctructOldCost.RealCostQuantity);
                }
            }


            // 通过建造验证
            oldTile.OnDestruct();
            ITileDefinition tile = (Activator.CreateInstance(type) as ITileDefinition);
            if (tile == null) throw new Exception();

            Vector2Int pos = new Vector2Int(i, j);
            tile.Map = this;
            tile.Pos = pos;
            tile.HashCode = HashUtility.Hash(i, j, Width, Height);

            // Tiles[i, j] = tile;
            SetTile(pos, tile, true);

            LinkUtility.NeedUpdateNeighbors(tile);

            tile.OnConstruct();
            tile.OnEnable();
            return tile;
        }

        public Vector2Int Validate(Vector2Int pos) {
            pos.x %= Width;
            if (pos.x < 0) pos.x += Width;
            pos.y %= Height;
            if (pos.y < 0) pos.y += Height;
            return pos;
        }
        private void Validate(ref int i, ref int j) {
            i %= Width;
            if (i < 0) i += Width;
            j %= Height;
            if (j < 0) j += Height;
        }

        // modify
        public void SetTile(Vector2Int pos, ITileDefinition tile, bool inConstruction = false) {
            if (Tiles == null) {
                Tiles = new ITileDefinition[Width, Height];
            }

            // 建筑计数
            if (inConstruction) {
                ITile oldTile = Tiles[pos.x, pos.y];
                if (oldTile != null) {
                    Refs.Get(oldTile.GetType()).Value--;
                    // Debug.LogWarning($"{oldTile.GetType().Name}--");
                }
                Refs.GetOrCreate(tile.GetType()).Value++;
                // Debug.LogWarning($"{tile.GetType().Name}++");
            }


            Tiles[pos.x, pos.y] = tile;
        }

        public abstract Type GenerateTileType(Vector2Int pos);
        public virtual void AfterGeneration() { }

        // ------------------------------------------------------------

        protected virtual AltitudeConfig GetAltitudeConfig { get => AltitudeConfig.Create(); }
        protected virtual MoistureConfig GetMoistureConfig { get => MoistureConfig.Create(); }
        protected virtual TemporatureConfig GetTemporatureConfig { get => TemporatureConfig.Create(); }

        public struct TemporatureConfig
        {
            public static TemporatureConfig Create() {
                var result = new TemporatureConfig();
                result.CanGenearate = false;
                result.BaseNoiseSize = 4;
                result.AltitudeInfluence = 0.8f;
                result.Max = 40;
                result.Min = -30;
                return result;
            }
            public bool CanGenearate;
            public int BaseNoiseSize;
            public float AltitudeInfluence;
            public int Min;
            public int Max;
        }

        public struct MoistureConfig
        {
            public static MoistureConfig Create() {
                var result = new MoistureConfig();
                result.CanGenerate = false;
                result.BaseNoiseSize = 4;
                result.Max = 100;
                result.Min = 0;
                return result;
            }
            public int BaseNoiseSize;
            public bool CanGenerate;
            public int Max;
            public int Min;
        }

        public struct AltitudeConfig
        {
            public static AltitudeConfig Create() {
                var result = new AltitudeConfig();
                result.CanGenerate = false;
                result.BaseNoiseSize = 3;
                result.Min = -10000;
                result.Max = 9800;
                result.EaseFunction = null;
                return result;
            }
            public int BaseNoiseSize;
            public bool CanGenerate;
            public int Min;
            public int Max;
            public Func<float, float> EaseFunction;
        }

        private AltitudeConfig altitudeConfig;
        private MoistureConfig moistureConfig;
        private TemporatureConfig temporatureConfig;


        public int[,] Altitudes { get; private set; }
        public Type[,] AltitudeTypes { get; private set; }
        public int[,] Moistures { get; private set; }
        public Type[,] MoistureTypes { get; private set; }
        public int[,] Temporatures { get; private set; }
        public Type[,] TemporatureTypes { get; private set; }
        public Type[,] ResourceTypes { get; private set; }


        protected virtual int RandomSeed { get; } = 0;
        private int autoInc = 0;
        private int AutoInc { get => autoInc++; }
        private void GenerateNoise() {
            GenerateAltitude();
            GenerateMoisture();
            GenerateTemporature();
            GenerateResources();
        }
        private void GenerateResources() {
            if (altitudeConfig.CanGenerate && moistureConfig.CanGenerate) {
                // TODO
            }
        }

        private void GenerateAltitude() {
            bool debugAltitude = false;
            Texture2D texAltitude = null;
            if (debugAltitude) texAltitude = new Texture2D(Width, Height);
            if (altitudeConfig.CanGenerate) {
                int noise0Size = altitudeConfig.BaseNoiseSize;
                int noise1Size = noise0Size * 2;
                int noise2Size = noise1Size * 2;
                Altitudes = new int[Width, Height];
                AltitudeTypes = new Type[Width, Height];
                int offset0 = AutoInc;
                int offset1 = AutoInc;
                int offset2 = AutoInc;
                for (int i = 0; i < Width; i++) {
                    for (int j = 0; j < Height; j++) {
                        //float noise0 = HashUtility.PerlinNoise((float)noise0Size * i / Width, (float)noise0Size * j / Height, noise0Size, noise0Size, offset0 + HashCode);
                        //float floatResult = (noise0+1)/2;
                        float noise0 = HashUtility.PerlinNoise((float)noise0Size * i / Width, (float)noise0Size * j / Height, noise0Size, noise0Size, offset0 + HashCode);
                        float noise1 = HashUtility.PerlinNoise((float)noise1Size * i / Width, (float)noise1Size * j / Height, noise1Size, noise1Size, offset1 + HashCode);
                        float noise2 = HashUtility.PerlinNoise((float)noise2Size * i / Width, (float)noise2Size * j / Height, noise2Size, noise2Size, offset2 + HashCode);
                        float floatResult = (noise0 * 4 + noise1 * 2 + noise2 * 1 + 7) / 14;
                        if (altitudeConfig.EaseFunction != null) floatResult = altitudeConfig.EaseFunction(floatResult);

                        int altitude = (int)Mathf.Lerp(altitudeConfig.Min, altitudeConfig.Max, floatResult);
                        Altitudes[i, j] = altitude;
                        AltitudeTypes[i, j] = GeographyUtility.GetAltitudeType(altitude);

                        if (debugAltitude) texAltitude.SetPixel(i, j, Color.Lerp(Color.black, Color.white, floatResult));
                    }
                }
            }
            if (debugAltitude) System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/altitude.png", texAltitude.EncodeToPNG());
        }
        private void GenerateMoisture() {
            bool debugMoisture = false;
            Texture2D texMoisture = null;
            if (debugMoisture) texMoisture = new Texture2D(Width, Height);
            if (moistureConfig.CanGenerate) {
                int size = moistureConfig.BaseNoiseSize;
                Moistures = new int[Width, Height];
                MoistureTypes = new Type[Width, Height];
                int offset = AutoInc;
                for (int i = 0; i < Width; i++) {
                    for (int j = 0; j < Height; j++) {
                        float noise = HashUtility.PerlinNoise((float)size * i / Width, (float)size * j / Height, size, size, offset + HashCode);
                        float floatResult = (noise + 1) / 2;

                        int moisture = (int)Mathf.Lerp(moistureConfig.Min, moistureConfig.Max, floatResult); ;
                        Moistures[i, j] = moisture;
                        MoistureTypes[i, j] = GeographyUtility.GetMoistureType(moisture);

                        if (debugMoisture) texMoisture.SetPixel(i, j, Color.Lerp(Color.black, Color.white, floatResult));
                    }
                }
            }
            if (debugMoisture) System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/moisture.png", texMoisture.EncodeToPNG());
        }
        private void GenerateTemporature() {
            bool debugTemporature = false;
            Texture2D texTemporature = null;
            if (debugTemporature) texTemporature = new Texture2D(Width, Height);
            if (temporatureConfig.CanGenearate) {
                if (!altitudeConfig.CanGenerate) throw new Exception();
                int size = temporatureConfig.BaseNoiseSize;
                Temporatures = new int[Width, Height];
                TemporatureTypes = new Type[Width, Height];
                int offset = AutoInc;
                for (int i = 0; i < Width; i++) {
                    for (int j = 0; j < Height; j++) {
                        float noise = HashUtility.PerlinNoise((float)size * i / Width, (float)size * j / Height, size, size, offset + HashCode);
                        noise = (noise + 1) / 2;
                        float latitude = Mathf.Sin(Mathf.PI * j / Width);
                        float floatResult = Mathf.Lerp(noise, latitude, temporatureConfig.AltitudeInfluence);

                        // 海拔升高，温度降低
                        int altitude = Altitudes[i, j];
                        if (altitude > 0 && altitudeConfig.Max > 0) {
                            float t = 0.02f * altitude / altitudeConfig.Max;
                            floatResult = Mathf.Lerp(floatResult, temporatureConfig.Min, t);
                        }

                        int temporature = temporatureConfig.Min + (int)(floatResult * (temporatureConfig.Max - temporatureConfig.Min));
                        Temporatures[i, j] = temporature;
                        TemporatureTypes[i, j] = GeographyUtility.GetTemporatureType(temporature);

                        if (debugTemporature) texTemporature.SetPixel(i, j, Color.Lerp(Color.black, Color.white, floatResult));
                    }
                }
            }
            if (debugTemporature) System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/temporature.png", texTemporature.EncodeToPNG());
        }

        // OnTapTile 生效前，是否需要降落
        protected virtual bool NeedLanding { get; } = true;

        public void OnTapTile(ITile tile) {
            if (!NeedLanding || Landed) {
                tile.OnTap();
            }
            else {
                var items = UI.Ins.GetItems();
                bool allQuestsCompleted = MainQuest.Ins.IsUnlocked<Quest_CongratulationsQuestAllCompleted>();
                // 只能降落在这种地形上...
                if (tile is TerrainDefault && tile is IPassable passable && passable.Passable) {
                    items.Add(UIItem.CreateMultilineText("飞船是否在此着陆"));
                    items.Add(UIItem.CreateButton("就在这里着陆", () => {
                        MainQuest.Ins.CompleteQuest(typeof(Quest_LandRocket));
                        Vector2Int pos = tile.GetPos();
                        this.UpdateAt<PlanetLander>(pos);
                        Land(pos);
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateButton("换个地方着陆", () => {
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateStaticButton(allQuestsCompleted ? "离开这个星球" : "离开这个星球 (主线任务通关后解锁)", () => {
                        GameEntry.Ins.EnterMap(typeof(MainMap));
                        UI.Ins.Active = false;
                    }, allQuestsCompleted));
                } else {
                    items.Add(UIItem.CreateMultilineText("飞船只能在空旷的平原着陆"));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateButton("继续寻找平原", () => {
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateStaticButton(allQuestsCompleted ? "离开这个星球" : "离开这个星球 (主线任务通关后解锁)", () => {
                        GameEntry.Ins.EnterMap(typeof(MainMap));
                        UI.Ins.Active = false;
                    }, allQuestsCompleted));
                }
                UI.Ins.ShowItems("飞船未降落", items);
            }
        }
    }
}

