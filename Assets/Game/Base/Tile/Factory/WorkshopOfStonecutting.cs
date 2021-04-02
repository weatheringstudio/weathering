

using System;

namespace Weathering
{
    // 石砖
    [ConceptSupply(typeof(StoneBrickSupply))]
    [ConceptDescription(typeof(StoneBrickDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class StoneBrick { }

    [ConceptResource(typeof(StoneBrick))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class StoneBrickSupply { }

    [Concept]
    public class StoneBrickDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    public class WorkshopOfStonecutting : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfWoodcutting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(StoneBrickSupply), 1);

        protected override (Type, long) In_0 => (typeof(StoneSupply), 3);
    }
}
