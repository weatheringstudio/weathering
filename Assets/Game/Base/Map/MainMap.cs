
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class MainMap : StandardMap
    {

        public override int Width => 10;
        public override int Height => 10;

        protected override bool NeedLanding => false;

        public override Type DefaultTileType => typeof(EmptyTile);

        public override void OnConstruct() {
            base.OnConstruct();
            SetCharacterPos(Vector2Int.zero);
            SetCameraPos(Vector2Int.zero);
            SetClearColor(Color.grey);
        }

        public override void AfterGeneration() {
            base.AfterGeneration();
            foreach (var pair in Teleports) {
                (Get(pair.Key) as Teleport).TargetMap = pair.Value;
            }
        }

        private Dictionary<Vector2Int, Type> Teleports = new Dictionary<Vector2Int, Type> {
            { new Vector2Int(0, 0), typeof(Map_0_0) },
            //{ new Vector2Int(7, 3), typeof(Map_0_1) },
            //{ new Vector2Int(1, 5), typeof(Map_0_2) },
            //{ new Vector2Int(6, 6), typeof(Map_0_3) },
            //{ new Vector2Int(0, 7), typeof(Map_0_4) },
            //{ new Vector2Int(9, 7), typeof(Map_0_5) },
        };
    }
}

