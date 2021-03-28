

using System;

namespace Weathering
{
    // 红砖
    [ConceptSupply(typeof(BrickSupply))]
    [ConceptDescription(typeof(BrickDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Brick { }

    [ConceptResource(typeof(Brick))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class BrickSupply { }

    [Concept]
    public class BrickDescription { }

    [ConstructionCostBase(typeof(StoneBrick), 100)]
    public class WorkshopOfBrickMaking : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(BrickSupply), 1);

        protected override (Type, long) In_0 => (typeof(ClaySupply), 3);
        protected override (Type, long) In_1 => (typeof(FuelSupply), 1);
        // protected override (Type, long) In_2 => (typeof(StoneSupply), 1);
    }
}
