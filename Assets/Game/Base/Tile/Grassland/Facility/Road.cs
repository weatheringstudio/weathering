
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Road : StandardTile
    {
        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(typeof(Road), Map, Pos);
                return $"Table_{index}"; //$"Road_{index}";
            }
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            Map.UpdateAt<Pyramid>(Pos);
        }
    }
}

