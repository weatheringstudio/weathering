
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Road : StandardTile
    {
        public override string SpriteKey {
            get {
                Func<ITile, bool> predicate = tile => {
                    return tile as Road != null;
                    //if (tile as IRoadlike != null) {
                    //    return true;
                    //}
                    //if (tile as ILookLikeRoad != null && (tile as ILookLikeRoad).LookLikeRoad) {
                    //    return true;
                    //}
                    //return false;
                };
                int index = TileUtility.Calculate4x4RuleTileIndex(predicate, Map, Pos);
                return $"StoneRoad_{index}";
            }
        }
        public override void OnTap() {
        }
    }
}

