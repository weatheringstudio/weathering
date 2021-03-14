

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
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class MetalProductSupply { }

    [Concept]
    public class MetalProductDescription { }

    public class WorkshopOfMetalCasting : Factory
    {
        public override string SpriteKey => "Workshop";

        protected override long WorkerCost => 1;

        protected override (Type, long) Out0 => (typeof(MetalProductSupply), 1);

        protected override (Type, long) In_0 => (typeof(MetalIngotSupply), 1);
    }
}
