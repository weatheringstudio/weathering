

using System;
using UnityEngine;

namespace Weathering
{
    public class MapOfPlanet : StandardMap
    {

        private int width = 100;
        private int height = 100;
        public override Type DefaultTileType => typeof(MapOfPlanetDefaultTile);

        public override int Width => width;
        public override int Height => height;

        protected override int RandomSeed { get => 5; }

        protected override bool NeedLanding => true;

        public override string GetSpriteKeyBackground(uint hashcode) => $"GrasslandBackground_{hashcode % 16}";

        public override void OnConstruct() {
            CalcMap();
            base.OnConstruct();

            SetCameraPos(new Vector2(0, Height / 2));
            SetCharacterPos(new Vector2Int(0, 0));
            SetClearColor(new Color(124 / 255f, 181 / 255f, 43 / 255f));

            Inventory.QuantityCapacity = GameConfig.DefaultInventoryQuantityCapacity;
            Inventory.TypeCapacity = GameConfig.DefaultInventoryTypeCapacity;

        }

        private void CalcMap() {
            int size = (int)(30 + (HashCode % 100));
            width = size;
            height = size;
            BaseAltitudeNoiseSize = (int)(2 + (HashCode % 11));
            BaseMoistureNoiseSize = (int)(5 + (HashCode % 17));
        }

        public override void OnEnable() {
            CalcMap();
            base.OnEnable();
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
                return result;
            }
        }

    }
}
