

using System;
using UnityEngine;

namespace Weathering
{
    public interface ITerrainType { }
    [Concept]
    public class TerrainType_Plain : ITerrainType { }
    [Concept]
    public class TerrainType_Mountain : ITerrainType { }
    [Concept]
    public class TerrainType_Sea : ITerrainType { }
    [Concept]
    public class TerrainType_Forest : ITerrainType { }



    public class BindTerrainTypeAttribute : Attribute
    {
        public Type BindedType { get; private set; }
        public BindTerrainTypeAttribute(Type terrainType) {
            if (!(typeof(ITerrainType)).IsAssignableFrom(terrainType)) throw new Exception();
            BindedType = terrainType;
        }
    }

    public class CanBeBuildOnNotPassableTerrainAttribute : Attribute { }



    public class MapOfPlanet : StandardMap, ILandable, IHasDayNightRecycle
    {
        private int width = 100;
        private int height = 100;

        public override string GetSpriteKeyBedrock(Vector2Int pos) => $"{PlanetType.Name}_Bedrock_{Get(pos).GetTileHashCode() % 16}";
        public override string GetSpriteKeyWater(Vector2Int pos) {
            ITile tile = Get(pos);
            uint tileHashCode = tile.GetTileHashCode();

            Type type = GetRealTerrainType(pos);

            if (type == typeof(TerrainType_Sea)) {

                int index = TileUtility.Calculate4x4RuleTileIndex(Get(pos), (otherTile, b) => GetRealTerrainType(otherTile.GetPos()) == typeof(TerrainType_Sea));
                if ((pos.x + pos.y) % 2 == 0) {
                    if (index >= 12) {
                        index -= 12;
                    }
                } else {
                    if (index < 4) {
                        index += 12;
                    }
                }
                return $"{PlanetType.Name}_Water_{index}";
            }
            return null;
        }
        public override string GetSpriteKeyGrass(Vector2Int pos) {
            ITile tile = Get(pos);
            uint tileHashCode = tile.GetTileHashCode();

            Type type = GetRealTerrainType(pos);

            if (type == typeof(TerrainType_Forest) || type == typeof(TerrainType_Plain)) {
                int index = TileUtility.Calculate4x4RuleTileIndex(Get(pos), (otherTile, direction) => {

                    Vector2Int otherPos = otherTile.GetPos();
                    Type otherType = GetRealTerrainType(otherPos);
                    bool hasGrass = otherType == typeof(TerrainType_Forest) || otherType == typeof(TerrainType_Plain);

                    return hasGrass;

                });
                if (index == 5) { // center
                    index = 16 + (int)(tile.GetTileHashCode() % 16);
                }
                return $"{PlanetType.Name}_Grass_{index}";
            }
            return null;
        }
        public override string GetSpriteKeyTree(Vector2Int pos) {
            ITile tile = Get(pos);
            uint tileHashCode = tile.GetTileHashCode();
            Type type = GetRealTerrainType(pos);

            if (type == typeof(TerrainType_Forest)) {
                int index = TileUtility.Calculate4x4RuleTileIndex(Get(pos), (otherTile, dir) => {

                    Vector2Int otherPos = otherTile.GetPos();
                    Type otherType = GetRealTerrainType(otherPos);
                    bool isForest = otherType == typeof(TerrainType_Forest);
                    // bool smallAltitudeDifference = Math.Abs(Altitudes[otherPos.x, otherPos.y] - Altitudes[pos.x, pos.y]) <= 500 && (Altitudes[otherPos.x, otherPos.y] - Altitudes[pos.x, pos.y] < 0);

                    return isForest;
                });
                return $"PlanetLandForm_Tree";
                // return $"PlanetLandform_{index}";
            }
            return null;
        }

        public override string GetSpriteKeyHill(Vector2Int pos) {
            ITile tile = Get(pos);
            uint tileHashCode = tile.GetTileHashCode();
            Type type = GetRealTerrainType(pos);

            if (type == typeof(TerrainType_Mountain)) {
                // 显示矿物
                int index = TileUtility.Calculate6x8RuleTileIndex(Get(pos), otherTile => {

                    Vector2Int otherPos = otherTile.GetPos();
                    Type otherType = GetRealTerrainType(otherPos);
                    bool isMountain = otherType == typeof(TerrainType_Mountain);
                    // bool smallAltitudeDifference = Math.Abs(Altitudes[otherPos.x, otherPos.y] - Altitudes[pos.x, pos.y]) <= 500 && (Altitudes[otherPos.x, otherPos.y] - Altitudes[pos.x, pos.y] < 0);

                    return isMountain;
                });
                return $"{PlanetType.Name}_Hill_{index}";
            }
            return null;
        }









        public Type GetRealTerrainType(Vector2Int pos) {
            ITile tile = Get(pos);
            if (tile is MapOfPlanetDefaultTile defaultTile) {
                // tile是地图基础类型
                return defaultTile.TerraformedTerrainType;
            } else {
                var attr = Tag.GetAttribute<BindTerrainTypeAttribute>(tile.GetType());
                if (attr != null) {
                    return attr.BindedType;
                } else {
                    // 默认绑定地形
                    return typeof(TerrainType_Plain);
                }
            }
        }

        public Type GetOriginalTerrainType(Vector2Int pos) {
            pos = Validate(pos);
            // 不是海, 就是地
            if (AltitudeTypes[pos.x, pos.y] != typeof(AltitudeSea)) {
                // 不是森林, 就是平原/秃地

                if (TemporatureTypes[pos.x, pos.y] == typeof(TemporatureTemporate)) {
                    if (MoistureTypes[pos.x, pos.y] == typeof(MoistureForest)) {
                        return typeof(TerrainType_Forest);
                    } else {
                        return typeof(TerrainType_Plain);
                    }
                } else {
                    return typeof(TerrainType_Mountain);
                }

            } else if (AltitudeTypes[pos.x, pos.y] == typeof(AltitudeSea)) {
                return typeof(TerrainType_Sea);
            }
            //else if (AltitudeTypes[pos.x, pos.y] == typeof(AltitudeMountain)) {
            //    return typeof(TerrainType_Mountain);
            //} 
            else {
                throw new Exception();
            }
        }


        public override Type DefaultTileType => typeof(MapOfPlanetDefaultTile);

        public override int Width => width;
        public override int Height => height;

        protected override int RandomSeed { get => 5; }



        public override void OnConstruct() {
            CalcMap();
            base.OnConstruct();

            landed = Values.Create<CharacterLanded>();
            landed.Max = 0;

            SetCameraPos(new Vector2(0, Height / 2));
            SetCharacterPos(new Vector2Int(0, 0));
            SetClearColor(new Color(124 / 255f, 181 / 255f, 43 / 255f));

            Inventory.QuantityCapacity = GameConfig.DefaultInventoryOfResourceQuantityCapacity;
            Inventory.TypeCapacity = GameConfig.DefaultInventoryOfResourceTypeCapacity;

            InventoryOfSupply.QuantityCapacity = GameConfig.DefaultInventoryOfSupplyQuantityCapacity;
            InventoryOfSupply.TypeCapacity = GameConfig.DefaultInventoryOfSupplyTypeCapacity;
        }

        public static int CalculateMineralDensity(uint hashcode) => (int)(3 + HashUtility.AddSalt(hashcode, 2641779086) % 27);
        public static int CalculatePlanetSize(uint hashcode) => (int)(50 + hashcode % 100);
        private void CalcMap() {
            int size = CalculatePlanetSize(GameEntry.SelfMapKeyHashCode(this));
            width = size;
            height = size;
            BaseAltitudeNoiseSize = (int)(5 + (HashCode % 11));
            BaseMoistureNoiseSize = (int)(7 + (HashCode % 17));
        }

        public int MineralDensity { get; private set; } = 0;
        public Type PlanetType { get; private set; }
        public override void OnEnable() {
            CalcMap();
            base.OnEnable();


            PlanetType = (ParentTile as MapOfStarSystemDefaultTile).CelestialBodyType;

            landed = Values.Get<CharacterLanded>();

            MineralDensity = CalculateMineralDensity(GameEntry.SelfMapKeyHashCode(this));
        }





        private int BaseAltitudeNoiseSize = 1;

        protected override AltitudeConfig GetAltitudeConfig {
            get {
                var result = base.GetAltitudeConfig;
                result.CanGenerate = true;
                result.Min = -10000;
                result.Max = 9500;
                result.BaseNoiseSize = BaseAltitudeNoiseSize;
                return result;
            }
        }
        private int BaseMoistureNoiseSize = 1;
        protected override MoistureConfig GetMoistureConfig {
            get {
                var result = base.GetMoistureConfig;
                result.CanGenerate = true;
                result.BaseNoiseSize = BaseMoistureNoiseSize;
                return result;
            }
        }
        protected override TemporatureConfig GetTemporatureConfig {
            get {
                var result = base.GetTemporatureConfig;
                result.CanGenearate = true;

                result.BaseNoiseSize = 4;
                result.AltitudeInfluence = 0;
                result.Max = 40;
                result.Min = -20;
                return result;
            }
        }


        public override ITile ParentTile => GameEntry.Ins.GetParentTile(typeof(MapOfStarSystem), this);

        public override void EnterParentMap() {
            GameEntry.Ins.EnterParentMap(typeof(MapOfStarSystem), this);
        }

        public override void EnterChildMap(Vector2Int pos) {
            throw new NotImplementedException();
        }

        public override bool CanUpdateAt(Type type, int i, int j) {
            if (Get(i, j) is MapOfPlanetDefaultTile mapOfPlanetDefaultTile) {
                return mapOfPlanetDefaultTile.CanConstruct(type);
            }
            return false;
        }









        #region landing
        protected virtual bool NeedLanding { get; } = true;

        public override bool ControlCharacter => landed.Max == 1;

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
        public override void OnTapTile(ITile tile) {

            // Debug.LogWarning(MapKey);

            if (!NeedLanding || Landed) {
                tile.OnTap();
            } else {
                bool isDefaultTile = DefaultTileType.IsAssignableFrom(tile.GetType());
                bool isDontSave = !(tile is IDontSave dontSave) || dontSave.DontSave;
                var items = UI.Ins.GetItems();
                // 只能降落在这种地形上...
                if (isDefaultTile && isDontSave && tile is IPassable passable && passable.Passable) {
                    items.Add(UIItem.CreateMultilineText("飞船是否在此着陆"));
                    items.Add(UIItem.CreateButton("就在这里着陆", () => {
                        UpdateAt<PlanetLander>(tile);
                        Land(tile.GetPos());
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateButton("换个地方着陆", () => {
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateStaticButton("离开这个星球", LeavePlanet, true));
                } else {
                    items.Add(UIItem.CreateMultilineText("飞船只能在空旷的平原着陆"));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateButton("继续寻找平原", () => {
                        UI.Ins.Active = false;
                    }));
                    items.Add(UIItem.CreateSeparator());
                    items.Add(UIItem.CreateStaticButton("离开这个星球", LeavePlanet, true));
                }
                UI.Ins.ShowItems("飞船未降落", items);
            }
        }
        private void LeavePlanet() {
            GameEntry.Ins.EnterParentMap(typeof(MapOfStarSystem), this);
            UI.Ins.Active = false;
        }

    }
}
