

using System;

namespace Weathering
{
    // 卫星组件
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class SatelliteComponent { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfSatelliteComponent : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(FactoryOfSatelliteComponent).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 50);

        protected override (Type, long) In_0 => (typeof(LightMaterial), 1);
        protected override (Type, long) In_1 => (typeof(CircuitBoardAdvanced), 1);
    }
}
