

using System;

namespace Weathering
{
    // 铜矿
    [ConceptSupply(typeof(CopperOreSupply))]
    [ConceptDescription(typeof(OreOfCopperDescription))]
    [Depend(typeof(MetalOre))]
    [Concept]
    public class OreOfCopper { }
    [ConceptResource(typeof(OreOfCopper))]
    [Depend(typeof(MetalOreSupply))]
    [Concept]
    public class CopperOreSupply { }
    [Concept]
    public class OreOfCopperDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [BindTerrainType(TerrainType.Mountain)]
    [Concept]
    public class MineOfCopper : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(CopperOreSupply), 1);

        public bool Passable => false;
    }
}
