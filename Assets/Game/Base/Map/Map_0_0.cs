
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Map_0_0 : StandardMap {
        public override int Width => 32;
        public override int Height => 32;

        public override bool ControlCharacter => true;

        // private Type[,] Types;
        public override Type GenerateTileType(Vector2Int pos) {
            return typeof(TerrainDefault); // Types[pos.x, pos.y];
        }

        public override void OnEnable() {
            base.OnEnable();
        }

        public override void OnConstruct() {
            base.OnConstruct();
            SetCharacterPos(Vector2Int.zero);
            // SetCameraPos(new Vector2(Width / 2, Height / 2));
            // SetClearColor(new Color(125 / 255f, 180 / 255f, 43 / 255f));
            SetClearColor(Color.magenta);

            Inventory.QuantityCapacity = 1000;
            Inventory.TypeCapacity = 10;

            //// 测试
            IValue sanityValue = Globals.Ins.Values.Get<Sanity>();
            sanityValue.Val = sanityValue.Max;

        }

        protected override AltitudeConfig GetAltitudeConfig {
            get {
                var result = base.GetAltitudeConfig;
                result.CanGenerate = true;
                result.Min = -10000;
                result.Max = 9500;
                return result;
            }
        }
        protected override MoistureConfig GetMoistureConfig {
            get {
                var result = base.GetMoistureConfig;
                result.CanGenerate = true;
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

