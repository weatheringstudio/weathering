

using System;

namespace Weathering
{
    // 木板
    [ConceptSupply(typeof(WoodPlankSupply))]
    [ConceptDescription(typeof(WoodPlankDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class WoodPlank { }

    [ConceptResource(typeof(WoodPlank))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class WoodPlankSupply { }

    [Concept]
    public class WoodPlankDescription { }

    [ConstructionCostBase(typeof(Wood), 100)]
    public class WorkshopOfWoodcutting : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfWoodcutting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(WoodPlankSupply), 1);

        protected override (Type, long) In_0 => (typeof(WoodSupply), 2);
    }
}
