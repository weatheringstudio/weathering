

using System;

namespace Weathering
{
    // 金属锭
    [ConceptSupply(typeof(MetalIngotSupply))]
    [ConceptDescription(typeof(MetalIngotDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class MetalIngot { }

    [ConceptResource(typeof(MetalIngot))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class MetalIngotSupply { }

    [Concept]
    public class MetalIngotDescription { }

    public class WorkshopOfMetalSmelting : Factory
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(MetalIngotSupply), 1);

        protected override (Type, long) In_0 => (typeof(MetalOreSupply), 2);

        protected override (Type, long) In_1 => (typeof(FuelSupply), 1);
    }
}
