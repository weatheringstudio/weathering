

using System;

namespace Weathering
{
    // 红砖
    [Depend(typeof(DiscardableSolid))]
    public class Brick { }


    [ConstructionCostBase(typeof(ToolPrimitive), 100)]
    public class WorkshopOfBrickMaking : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(Brick), 1);

        protected override (Type, long) In_0 => (typeof(Clay), 3);
        protected override (Type, long) In_1 => (typeof(Fuel), 2);
        // protected override (Type, long) In_2 => (typeof(StoneSupply), 1);
    }
}
