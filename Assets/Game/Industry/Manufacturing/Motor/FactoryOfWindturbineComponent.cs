

using System;

namespace Weathering
{
    [Depend(typeof(DiscardableSolid))]
    public class WindTurbineComponent { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfWindTurbineComponent : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 2);
        protected override (Type, long) Out0 => (typeof(WindTurbineComponent), 2);
        protected override (Type, long) In_0 => (typeof(AluminiumIngot), 1);
        protected override (Type, long) In_1 => (typeof(ElectroMotor), 1);
    }
}
