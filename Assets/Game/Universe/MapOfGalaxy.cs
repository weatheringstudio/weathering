

using System;

namespace Weathering
{
    public class MapOfGalaxy : StandardMap
    {
        public override Type DefaultTileType => typeof(MapOfGalaxyDefaultTile);

        public override int Width => 100;

        public override int Height => 100;

        public override string GetSpriteKeyBackground(uint hashcode) => $"GalaxyBackground_{(hashcode % 16) + (16 * ((HashCode) % 6))}";

        public const long DefaultInventoryQuantityCapacity = 1000000;
        public const int DefaultInventoryTypeCapacity = 20;
        public override void OnConstruct() {
            base.OnConstruct();

            Inventory.QuantityCapacity = DefaultInventoryQuantityCapacity;
            Inventory.TypeCapacity = DefaultInventoryTypeCapacity;
        }
    }
}
