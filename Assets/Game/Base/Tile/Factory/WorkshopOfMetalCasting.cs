

using System;

namespace Weathering
{
    // 木材
    [ConceptSupply(typeof(MetalProductSupply))]
    [ConceptDescription(typeof(MetalProductDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class MetalProduct { }

    [ConceptResource(typeof(MetalProduct))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class MetalProductSupply { }

    [Concept]
    public class MetalProductDescription { }

    [ConstructionCostBase(typeof(StoneBrick), 100)]
    public class WorkshopOfMetalCasting : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalCasting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(MetalProductSupply), 1);

        protected override (Type, long) In_0 => (typeof(MetalIngotSupply), 1);

        protected override (Type, long) In_1 => (typeof(FuelSupply), 1);
    }
}
