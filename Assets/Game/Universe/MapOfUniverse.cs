

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

        public override ITile ParentTile => null;

        public override void EnterParentMap() {
            throw new NotImplementedException();
        }

        public override void EnterChildMap(Vector2Int pos) {
            if (pos.x < 0 || pos.x > Width) throw new Exception();
            if (pos.y < 0 || pos.y > Height) throw new Exception();
            GameEntry.Ins.EnterChildMap(typeof(MapOfGalaxy), this, pos);
        }

        public override bool CanUpdateAt(Type type, int i, int j) {
            throw new NotImplementedException();
        }
    }
}
