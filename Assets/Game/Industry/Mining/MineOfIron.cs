

using System;

namespace Weathering
{
    // 铁矿
    [Depend(typeof(MetalOre))]
    [Concept]
    public class IronOre : IMineralType { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [CanBeBuildOnNotPassableTerrain]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [MineOfMineralType(typeof(IronOre))]
    [Concept]
    public class MineOfIron : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(IronOre), 1);

        public bool Passable => false;
    }
}
