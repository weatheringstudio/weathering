

using System;

namespace Weathering
{
    [Depend(typeof(DiscardableSolid))]
    public class SolarPanelComponent { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfSolarPanelComponent : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Factory).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 2);
        protected override (Type, long) Out0 => (typeof(SolarPanelComponent), 2);
        protected override (Type, long) In_0 => (typeof(CircuitBoardSimple), 1);
    }
}
