
using System;
using UnityEngine;

namespace Weathering
{
    [Concept("1 - 岛屿地图")]
    public class IslandMap : StandardMap
    {
        public override int Width => 16;

        public override int Height => 16;

        private Texture2D tex;
        public override void OnConstruct() {
            MapView.Ins.CameraPosition = new Vector2(Width / 2, Height / 2);
            MapView.Ins.ClearColor = new Color(125 / 255f, 180 / 255f, 43 / 255f);

            tex  = Res.Ins.GetSprite(typeof(IslandMap).Name).texture;
            if (tex.width != Width || tex.height != Height) throw new Exception();

            Inventory.QuantityCapacity = 100;
            Inventory.TypeCapacity = 6;

            // 测试
            Globals.Ins.Values.Get<Sanity>().Val += 50;
            Inventory.Add<Food>(10);
            Inventory.Add<Wood>(10);
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

