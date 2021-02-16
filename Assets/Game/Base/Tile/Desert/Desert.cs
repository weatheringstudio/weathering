
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Desert : StandardTile
    {
        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate6x8RuleTileIndex(tile => tile is Desert, Map, Pos);
                return "Desert_" + index.ToString();
            }
        }

        public override void OnTap() {
        }
    }
}

