
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Weathering
//{
//    [Concept]
//    public class IslandMap2 : StandardMap
//    {
//        public override int Width => 20;

//        public override int Height => 20;

//        private Texture2D tex;
//        public override Type GenerateTileType(Vector2Int pos) {
//            int i = pos.x;
//            int j = pos.y;

//            Color pixel = tex.GetPixel(i, j);
//            return ColorTileConfig.Ins.Find(pixel);
//        }

//        public override void OnConstruct() {
//            base.OnConstruct();
//            SetCameraPos(new Vector2(Width / 2, Height / 2));
//            SetClearColor(new Color(125 / 255f, 180 / 255f, 43 / 255f));

//            tex = Res.Ins.GetSprite(typeof(IslandMap2).Name).texture;
//            if (tex.width != Width || tex.height != Height) throw new Exception();

//            Inventory.QuantityCapacity = 1000;
//            Inventory.TypeCapacity = 8;
//        }
//    }
//}

