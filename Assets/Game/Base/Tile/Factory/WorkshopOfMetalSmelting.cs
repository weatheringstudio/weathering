﻿

using System;

namespace Weathering
{
    // 木材
    [ConceptSupply(typeof(MetalIngotSupply))]
    [ConceptDescription(typeof(MetalIngotDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class MetalIngot { }

    [ConceptResource(typeof(MetalIngot))]
    [Depend(typeof(NonDiscardableSupply))]
    [Concept]
    public class MetalIngotSupply { }

    [Concept]
    public class MetalIngotDescription { }

    public class WorkshopOfMetalSmelting : Factory
    {
        public override string SpriteKey => "Workshop";

        protected override long WorkerCost => 1;

        protected override (Type, long) Out0 => (typeof(MetalIngotSupply), 1);

        protected override (Type, long) In_0 => (typeof(MetalOreSupply), 1);
    }
}