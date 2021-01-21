
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("主地图")]
    public class MainMap : StandardMap
    {
        public override int Width => 10;

        public override int Height => 10;


        public override void OnEnable() {
            base.OnEnable();
            MapView.Ins.ClearColor = Color.grey;
        }

        public override Type Generate(Vector2Int pos) {
            return pos == Vector2.zero ? typeof(Teleport) : typeof(Pyramid);
        }

        public override void OnConstruct() {
        }

        public override void AfterGeneration() {
            base.AfterGeneration();
            Teleport t = Get(Vector2Int.zero) as Teleport;
            t.TargetMap = typeof(IslandMap);
        }
    }
}

