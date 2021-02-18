
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class IslandMap : StandardMap
    {
        public override int Width => 32;

        public override int Height => 32;

        protected override int RandomSeed { get => 1; }

        //private Texture2D tex;
        public override void OnConstruct() {
            base.OnConstruct();
            SetCameraPos(new Vector2(Width / 2, Height / 2));
            SetClearColor(new Color(125 / 255f, 180 / 255f, 43 / 255f));

            //tex = Res.Ins.GetSprite(typeof(IslandMap).Name).texture;
            //if (tex.width != Width || tex.height != Height) throw new Exception("Texutre Map don't match");

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


            Types = new Type[Width, Height];

            Texture2D tex2 = new Texture2D(Width, Height);
            const int size = 4;
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    float noise = HashUtility.PerlinNoise((float)size * i / Width, (float)size * j / Height, size, size);
                    tex2.SetPixel(i, j, Color.Lerp(Color.black, Color.white, (noise + 1) / 2));
                    if (noise > 0.4f) {
                        Types[i, j] = typeof(Forest);
                    } 
                    else if (noise > 0.1f) {
                        Types[i, j] = typeof(Grassland);
                    }
                    else {
                        Types[i, j] = typeof(Sea);
                    }
                }
            }
            System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/a.png", tex2.EncodeToPNG());

            
        }

        private Type[,] Types;

        public override Type GenerateTileType(Vector2Int pos) {

            //float noise = HashUtility.PerlinNoise((float)size * pos.x / Width, (float)size * pos.y / Height, size, size);
            //if (noise > 0) {
            //    return typeof(Grassland);
            //}
            //else {
            //    return typeof(Sea);
            //}
            return Types[pos.x, pos.y];

            //int i = pos.x;
            //int j = pos.y;

            //Color pixel = tex.GetPixel(i, j);
            //return ColorTileConfig.Ins.Find(pixel);

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

