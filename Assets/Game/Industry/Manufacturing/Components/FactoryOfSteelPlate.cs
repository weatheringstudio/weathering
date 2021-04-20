

using System;

namespace Weathering
{
    [Depend(typeof(DiscardableSolid))]
    public class SteelPlate { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfSteelPlate : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Factory).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 2);
        protected override (Type, long) Out0 => (typeof(SteelPlate), 1);
        protected override (Type, long) In_0 => (typeof(SteelIngot), 1);
    }
}
