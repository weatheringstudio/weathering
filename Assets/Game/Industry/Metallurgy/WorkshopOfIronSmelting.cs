

using System;

namespace Weathering
{

    // 铁锭
    [Depend(typeof(MetalIngot))]
    public class IronIngot { }



    [ConstructionCostBase(typeof(ToolPrimitive), 100)]
    public class WorkshopOfIronSmelting : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(WorkshopOfMetalSmelting).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(IronIngot), 1);

        protected override (Type, long) In_0 => (typeof(IronOre), 2);

        protected override (Type, long) In_1 => (typeof(Fuel), 2);
    }
}
