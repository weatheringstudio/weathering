﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Map_0_4 : StandardMap, ILandable
    {
        public override int Width => 32;
        public override int Height => 32;

        protected override int RandomSeed { get => 1; }

        public override bool ControlCharacter => landed.Max == 1;

        public bool Landable {
            get => ControlCharacter;
        }
        public void Land(Vector2Int pos) {
            landed.Max = 1;
            SetCharacterPos(pos);
        }
        public void Leave() {
            landed.Max = 0;
        }


        public override void Update() {
            base.Update();
        }

        // private Type[,] Types;
        public override Type GenerateTileType(Vector2Int pos) {
            return typeof(TerrainDefault); // Types[pos.x, pos.y];
        }

        public override void OnEnable() {
            base.OnEnable();
            landed = Values.Get<CharacterLanded>();
        }

        private IValue landed;
        public override void OnConstruct() {
            base.OnConstruct();
            SetCharacterPos(new Vector2Int(0, Height / 2));
            SetClearColor(new Color(124 / 255f, 181 / 255f, 43 / 255f));

            Inventory.QuantityCapacity = 1000;
            Inventory.TypeCapacity = 10;

            landed = Values.Create<CharacterLanded>();
            landed.Max = 0;
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
