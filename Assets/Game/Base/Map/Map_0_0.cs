
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class Map_0_0 : StandardMap
    {
        public override int Width => 32;
        public override int Height => 32;

        private Type[,] Types;
        public override Type GenerateTileType(Vector2Int pos) {
            return Types[pos.x, pos.y];
        }

        public override void OnConstruct() {
            base.OnConstruct();
            SetCameraPos(new Vector2(Width / 2, Height / 2));
            SetClearColor(new Color(125 / 255f, 180 / 255f, 43 / 255f));

            //tex = Res.Ins.GetSprite(typeof(IslandMap).Name).texture;
            //if (tex.width != Width || tex.height != Height) throw new Exception("Texutre Map don't match");

            Inventory.QuantityCapacity = 1000;
            Inventory.TypeCapacity = 10;

            //// 测试
            IValue sanityValue = Globals.Ins.Values.Get<Sanity>();
            sanityValue.Val = sanityValue.Max;
            //Inventory.Add<Food>(100);
            //Inventory.Add<Wood>(100);
            //Inventory.Add<Worker>(10);
            //Inventory.Add<FoodSupply>(6);
            //Inventory.Add<WoodSupply>(6);
            //Inventory.Add<Worker>(6);
            //Inventory.Add<Berry>(1);


            Types = new Type[Width, Height];

            // Texture2D tex2 = new Texture2D(Width, Height);
            const int size = 4;
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    float noise2 = HashUtility.PerlinNoise((float)size * i / Width, (float)size * j / Height, size, size, 2);
                    float noise3 = HashUtility.PerlinNoise((float)size * i / Width, (float)size * j / Height, size, size, 3);
                    Type result;
                    // tex2.SetPixel(i, j, Color.Lerp(Color.black, Color.white, (noise + 1) / 2));
                    if (noise2 > 0.5f) {
                        result = typeof(Mountain);
                    } else if (noise2 > 0.1f) {
                        if (noise3 > 0.5f) {
                            result = typeof(Forest);
                        } else if (noise3 > -0.2f) {
                            result = typeof(Grassland);
                        } else {
                            result = typeof(Desert);
                        }
                    } else {
                        result = typeof(Sea);
                    }
                    Types[i, j] = result;
                }
            }
            // System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/a.png", tex2.EncodeToPNG());
        }
    }
}

