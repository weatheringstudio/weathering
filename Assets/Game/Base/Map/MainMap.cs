
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

        public override Type Generate(Vector2Int pos) {
            return Teleports.ContainsKey(pos) ? typeof(Teleport) : typeof(EmptyTile);
        }

        public override void OnConstruct() {
            base.OnConstruct();
            MapView.Ins.ClearColor = Color.grey;
        }

        public override void AfterGeneration() {
            base.AfterGeneration();
            foreach (var pair in Teleports) {
                (Get(pair.Key) as Teleport).TargetMap = pair.Value;
            }
        }

        private Dictionary<Vector2Int, Type> Teleports = new Dictionary<Vector2Int, Type> {
            { Vector2Int.zero, typeof(IslandMap) },
            { Vector2Int.right*2, typeof(IslandMap2) },
        };
    }
}

