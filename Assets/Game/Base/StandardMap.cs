

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


    // 应该用class做，
    public enum MineralType {
        Iron, Copper, Rutile_Titanium,
        Silver, Gold,
        Coal, CrudeOil, NaturalGas,
        Bauxite_Aluminum, Barite,
        RareEarth,
        Tungsten, Tin, Zinc, Surfur, Thorium,
        Gems, Galena, Nickel, Quartz, Rutile, Lithia
    }



    public abstract class StandardMap : IMapDefinition
    {
        public int HashCode { get; private set; }

        public virtual bool ControlCharacter { get => true; }

        public abstract int Width { get; }

        public abstract int Height { get; }

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

        public bool UpdateAt<T>(Vector2Int pos) where T : ITile {
            return UpdateAt(typeof(T), pos.x, pos.y);
        }
        public bool UpdateAt(Type type, Vector2Int pos) {
            return UpdateAt(type, pos.x, pos.y);
        }

        /// <summary>
        /// 标准地图抽象类。功能：
        /// 1. 自动将width和height写入map.Values，便于保存
        /// 2. 获取地块位置时，矫正i和j，循环
        /// 3. 建立和替换地块时，自动检查调用旧地块CanDestruct和OnDestruct，调用新地块CanConstruct，OnEnable和OnConstruct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public bool UpdateAt<T>(int i, int j) where T : ITile {
            return UpdateAt(typeof(T), i, j);
        }
        public bool UpdateAt(Type type, int i, int j) {
            ITileDefinition tile = (Activator.CreateInstance(type) as ITileDefinition);
            if (tile == null) throw new Exception();

            Validate(ref i, ref j);
            tile.Map = this;
            tile.Pos = new Vector2Int(i, j);
            tile.HashCode = HashUtility.Hash(i, j, Width, Height); // HashUtility.Hash((uint)(i + j * Width));
            if (tile.CanConstruct()) {
                ITileDefinition formerTile = Get(i, j) as ITileDefinition;
                if (formerTile == null || formerTile.CanDestruct()) {
                    if (formerTile != null) {
                        formerTile.OnDestruct();
                    }
                    Tiles[i, j] = tile;

                    tile.OnConstruct();
                    tile.OnEnable();
                    return true;
                }
            }
            // tile is garbage now
            return false;
        }

        private void Validate(ref int i, ref int j) {
            i %= Width;
            while (i < 0) i += Width;
            j %= Height;
            while (j < 0) j += Height;
        }

        // modify
        public void SetTile(Vector2Int pos, ITileDefinition tile) {
            if (Tiles == null) {
                Tiles = new ITileDefinition[Width, Height];
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
                result.Min = -35;
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
                result.Max = 100;
                result.Min = 0;
                return result;
            }
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

        public int[,] Altitudes { get; private set; }
        public Type[,] AltitudeTypes { get; private set; }
        public int[,] Moistures { get; private set; }
        public Type[,] MoistureTypes { get; private set; }
        public int[,] Temporatures { get; private set; }
        public Type[,] TemporatureTypes { get; private set; }

        protected virtual int RandomSeed { get; } = 0;
        private int autoInc = 0;
        private int AutoInc { get => autoInc++; }
        private void GenerateNoise() {
            bool debugAltitude = false;
            Texture2D texAltitude = null;
            if (debugAltitude) texAltitude = new Texture2D(Width, Height);
            AltitudeConfig altitudeConfig = GetAltitudeConfig;
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


            bool debugMoisture = false;
            Texture2D texMoisture = null;
            if (debugMoisture) texMoisture = new Texture2D(Width, Height);
            MoistureConfig moistureConfig = GetMoistureConfig;
            if (moistureConfig.CanGenerate) {
                const int size = 4;
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

            bool debugTemporature = false;
            Texture2D texTemporature = null;
            if (debugTemporature) texTemporature = new Texture2D(Width, Height);
            TemporatureConfig temporatureConfig = GetTemporatureConfig;
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
    }
}

