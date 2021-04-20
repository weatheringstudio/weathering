

using System;

namespace Weathering
{
    // 木材
    [Depend(typeof(DiscardableSolid))]
    public class MetalProduct { }


    [ConstructionCostBase(typeof(StoneBrick), 100)]
    public class WorkshopOfMetalCasting : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(WorkshopOfMetalSmelting).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(MetalProduct), 1);

        protected override (Type, long) In_0 => (typeof(MetalIngot), 1);

        protected override (Type, long) In_1 => (typeof(Fuel), 1);
    }
}
