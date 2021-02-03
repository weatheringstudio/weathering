
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class SeaFishery : StandardTile, ISealike
    {
        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate6x8RuleTileIndex(tile => typeof(ISealike).IsAssignableFrom(tile.GetType()), Map, Pos);
                return "Sea_" + index.ToString();
            }
        }
        public override string SpriteOverlayKey => typeof(SeaFishery).Name;

        public override void OnTap() {
            UI.Ins.Active = false;
        }
    }
}

