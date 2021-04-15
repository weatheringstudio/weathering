

using System;
using UnityEngine;

namespace Weathering
{
    public interface IStarPosition
    {
        int X { get; }
        int Y { get; }
        bool HasSecondStar { get; }
        int SecondStarX { get; }
        int SecondStarY { get; }
    }

    public class MapOfStarSystem : StandardMap, IStarPosition
    {
        public const int DefaultWidth = 32;
        public const int DefaultHeight = 32;

        public override Type DefaultTileType => typeof(MapOfStarSystemDefaultTile);

        public override int Width => DefaultWidth;

        public override int Height => DefaultHeight;

        private int x = 0;
        public int X => x;
        private int y = 0;
        public int Y => y;

        private const int hasSecondStarChance = 1;
        private bool hasSecondStar = false;
        public bool HasSecondStar => hasSecondStar;

        private int secondStarX = 0;
        public int SecondStarX => secondStarX;
        private int secondStarY = 0;
        public int SecondStarY => secondStarY;


        public override string GetSpriteKeyBedrock(Vector2Int pos) => $"GalaxyBackground_{(Get(pos).GetTileHashCode() % 16) + (16 * ((HashCode) % 6))}";

        public const long DefaultInventoryQuantityCapacity = 1000000;
        public const int DefaultInventoryTypeCapacity = 20;
        public override void OnConstruct() {
            base.OnConstruct();

            Inventory.QuantityCapacity = DefaultInventoryQuantityCapacity;
            Inventory.TypeCapacity = DefaultInventoryTypeCapacity;

            InventoryOfSupply.QuantityCapacity = GameConfig.DefaultInventoryOfSupplyQuantityCapacity;
            InventoryOfSupply.TypeCapacity = GameConfig.DefaultInventoryOfSupplyTypeCapacity;


            int starPos = (int)HashCode % (DefaultHeight * DefaultHeight);
            starPos = ABS(starPos);
            x = starPos % DefaultWidth;
            y = starPos / DefaultHeight;

            hasSecondStar = HashCode % hasSecondStarChance == 0;
            if (hasSecondStar) {
                int secondStarPos = (int)HashUtility.Hash(HashCode);
                secondStarPos = ABS(secondStarPos);
                if (secondStarPos == starPos) {
                    hasSecondStar = false; // coincidence
                } else {
                    secondStarPos = ABS(secondStarPos);
                    secondStarX = secondStarPos % DefaultWidth;
                    secondStarY = secondStarPos / DefaultHeight;
                }
            }
        }
        private int ABS(int x) => x >= 0 ? x : -x;


        public override ITile ParentTile => GameEntry.Ins.GetParentTile(typeof(MapOfGalaxy), this);

        public override void EnterParentMap() {
            GameEntry.Ins.EnterParentMap(typeof(MapOfGalaxy), this);
        }



        public override void EnterChildMap(Vector2Int pos) {
            if (pos.x < 0 || pos.x > Width) throw new Exception();
            if (pos.y < 0 || pos.y > Height) throw new Exception();
            GameEntry.Ins.EnterChildMap(typeof(MapOfPlanet), this, pos);
        }

        public override bool CanUpdateAt(Type type, int i, int j) {
            throw new NotImplementedException();
        }
    }
}
