

using System;

namespace Weathering
{
    public class MapOfStarSystem : StandardMap
    {
        public override Type DefaultTileType => typeof(MapOfStarSystemDefaultTile);

        public override int Width => 36;

        public override int Height => 36;

        public override string GetSpriteKeyBackground(uint hashcode) => $"StarSystemBackground_{hashcode % 16}";

        public const long DefaultInventoryQuantityCapacity = 1000000;
        public const int DefaultInventoryTypeCapacity = 20;
        public override void OnConstruct() {
            base.OnConstruct();

            Inventory.QuantityCapacity = DefaultInventoryQuantityCapacity;
            Inventory.TypeCapacity = DefaultInventoryTypeCapacity;
        }
    }
}
