

using System;

namespace Weathering
{
    // 金属矿
    [ConceptSupply(typeof(MetalOreSupply))]
    [ConceptDescription(typeof(MetalOreDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class MetalOre { }
    [ConceptResource(typeof(MetalOre))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class MetalOreSupply { }
    [Concept]
    public class MetalOreDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [Concept]
    public class MountainMine : AbstractFactoryStatic
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(MetalOreSupply), 1);
    }
}

