

using System;

namespace Weathering
{

    // 钢锭
    [Depend(typeof(MetalIngot))]
    public class SteelIngot { }


    // 钢厂
    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class WorkshopOfSteelWorking : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 2);
        protected override (Type, long) Out0 => (typeof(SteelIngot), 1);
        protected override (Type, long) In_0 => (typeof(IronIngot), 3);
        protected override (Type, long) In_1 => (typeof(Fuel), 4);

    }
}
