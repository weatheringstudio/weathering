

using System;
using UnityEngine;

namespace Weathering
{
    public class MapOfUniverse : StandardMap 
    {
        public override Type DefaultTileType => typeof(MapOfUniverseDefaultTile);

        public override int Width => 100;

        public override int Height => 100;

        public override string GetSpriteKeyBackground(uint hashcode) => $"UniverseBackground_{hashcode % 16}";

        public const long DefaultInventoryQuantityCapacity = 1000000;
        public const int DefaultInventoryTypeCapacity = 20;
        public override void OnConstruct() {
            base.OnConstruct();

            Inventory.QuantityCapacity = DefaultInventoryQuantityCapacity;
            Inventory.TypeCapacity = DefaultInventoryTypeCapacity;
        }
    }
}
