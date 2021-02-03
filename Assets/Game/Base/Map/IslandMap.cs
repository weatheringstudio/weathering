
using System;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class IslandMap : StandardMap
    {
        public override int Width => 32;

        public override int Height => 32;

        private Texture2D tex;
        public override void OnConstruct() {
            base.OnConstruct();
            SetCameraPos(new Vector2(Width / 2, Height / 2));
            SetClearColor(new Color(125 / 255f, 180 / 255f, 43 / 255f));

            tex  = Res.Ins.GetSprite(typeof(IslandMap).Name).texture;
            if (tex.width != Width || tex.height != Height) throw new Exception("Texutre Map don't match");

            Inventory.QuantityCapacity = 1000;
            Inventory.TypeCapacity = 10;

            //// 测试
            Globals.Ins.Values.Get<Sanity>().Val += 50;
            Inventory.Add<Food>(100);
            Inventory.Add<Wood>(100);
            Inventory.Add<Worker>(10);
            Inventory.Add<FoodSupply>(6);
            Inventory.Add<WoodSupply>(6);
            Inventory.Add<Worker>(6);
            Inventory.Add<Berry>(1);
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

