

using System;

namespace Weathering
{
    // 木材
    [ConceptSupply(typeof(WoodPlankSupply))]
    [ConceptDescription(typeof(WoodPlankDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class WoodPlank { }

    [ConceptResource(typeof(WoodPlank))]
    [Depend(typeof(NonDiscardable))]
    [Concept]
    public class WoodPlankSupply { }

    [Concept]
    public class WoodPlankDescription { }

    public class WorkshopOfWoodcutting : Factory
    {
        public override string SpriteKey => "Workshop";

        protected override long WorkerCost => 1;

        protected override (Type, long) Out0 => (typeof(WoodPlankSupply), 1);

        protected override (Type, long) In_0 => (typeof(WoodSupply), 1);
    }
}
