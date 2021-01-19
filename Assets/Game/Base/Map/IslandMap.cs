
using System;
using UnityEngine;

namespace Weathering
{
    public class IslandMap : StandardMap
    {
        public override int Width => 10;

        public override int Height => 10;


        private Texture2D tex;
        public override void OnConstruct() {
            tex  = Res.Ins.GetSprite(typeof(IslandMap).Name).texture;

            Inventory.QuantityCapacity = 100;
            Inventory.TypeCapacity = 6;
        }

        public override Type Generate(Vector2Int pos) {
            int i = pos.x;
            int j = pos.y;

            Color pixel = tex.GetPixel(i, j);
            return ColorTileConfig.Ins.Find(pixel);

            //if (pixel == Color.white) {
            //    return typeof(Grassland);
            //}
            //else if (pixel == Color.black) {
            //    return typeof(Sea);
            //}
            //else {
            //    return typeof(Mountain);
            //}
        }
    }
}

