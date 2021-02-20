
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class MainMap : StandardMap
    {

        public override int Width => 20;
        public override int Height => 20;
        public override bool ControlCharacter => false;


        public override Type GenerateTileType(Vector2Int pos) {
            return Teleports.ContainsKey(pos) ? typeof(Teleport) : typeof(EmptyTile);
        }

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
            { Vector2Int.zero, typeof(Map_0_0) },
            { Vector2Int.right*2+Vector2Int.up*3, typeof(Map_0_0) },
        };
    }
}

