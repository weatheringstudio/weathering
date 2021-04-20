

using System;

namespace Weathering
{
    // 铜矿
    [BindMine(typeof(MineOfCopper))]
    [Depend(typeof(MetalOre))]
    [Concept]
    public class CopperOre : IMineralType { }


    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [CanBeBuildOnNotPassableTerrain]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [BindMineral(typeof(CopperOre))]
    [Concept]
    public class MineOfCopper : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => typeof(MountainMine).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(CopperOre), 1);

        public bool Passable => false;
    }
}
