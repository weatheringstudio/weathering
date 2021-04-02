
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Map_0_4 : StandardMap
    {
        public override int Width => 100;
        public override int Height => 100;

        protected override int RandomSeed { get => 5; }

        public override void Update() {
            base.Update();
        }

        // private Type[,] Types;
        public override Type GenerateTileType(Vector2Int pos) {
            return typeof(TerrainDefault); // Types[pos.x, pos.y];
        }

        public override void OnEnable() {
            base.OnEnable();
            if (Inventory.TypeCapacity < 20) {
                Inventory.TypeCapacity = 20;
            }
        }

        public override void OnConstruct() {
            base.OnConstruct();
            SetCameraPos(new Vector2(0, Height / 2));
            SetCharacterPos(new Vector2Int(0, 0));
            SetClearColor(new Color(124 / 255f, 181 / 255f, 43 / 255f));

            Inventory.QuantityCapacity = GameConfig.DefaultInventoryQuantityCapacity;
            Inventory.TypeCapacity = 20;
        }

        protected override AltitudeConfig GetAltitudeConfig {
            get {
                var result = base.GetAltitudeConfig;
                result.CanGenerate = true;
                result.Min = -10000;
                result.Max = 9500;
                result.BaseNoiseSize = 7;
                return result;
            }
        }
        protected override MoistureConfig GetMoistureConfig {
            get {
                var result = base.GetMoistureConfig;
                result.CanGenerate = true;
                result.BaseNoiseSize = 13;
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

