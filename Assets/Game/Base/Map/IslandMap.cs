
using System;
using UnityEngine;

namespace Weathering
{
    public class IslandMap : StandardMap
    {
        public override int Width => 16;

        public override int Height => 16;

        public override void OnEnable() {
            base.OnEnable();
            MapView.Ins.CameraPosition = new Vector2(Width / 2, Height / 2);
        }

        private Texture2D tex;
        public override void OnConstruct() {
            tex  = Res.Ins.GetSprite(typeof(IslandMap).Name).texture;
            if (tex.width != Width || tex.height != Height) throw new Exception();

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

