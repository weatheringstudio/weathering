

using System;

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
        public const int DefaultWidth = 36;
        public const int DefaultHeight = 36;

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

        public override string GetSpriteKeyBackground(uint hashcode) => $"StarSystemBackground_{hashcode % 16}";

        public const long DefaultInventoryQuantityCapacity = 1000000;
        public const int DefaultInventoryTypeCapacity = 20;
        public override void OnConstruct() {
            base.OnConstruct();

            Inventory.QuantityCapacity = DefaultInventoryQuantityCapacity;
            Inventory.TypeCapacity = DefaultInventoryTypeCapacity;


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
                }
                else {
                    secondStarPos = ABS(secondStarPos);
                    secondStarX = secondStarPos % DefaultWidth;
                    secondStarY = secondStarPos / DefaultHeight;
                }
            }
        }
        private int ABS(int x) => x >= 0 ? x : -x;
    }
}
