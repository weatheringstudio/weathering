

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfEngineering : AbstractTechnologyCenter
    {
        protected override long TechnologyPointMaxRevenue => 100;
        protected override Type TechnologyPointType => typeof(SchoolEquipment);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(WorkshopOfSteelWorking), 1000),
            (typeof(FactoryOfConcrete), 1000),
            (typeof(FactoryOfBuildingPrefabrication), 1000),
            (typeof(FactoryOfAluminiumWorking), 1000),

            // 钢
            (typeof(FactoryOfSteelWorking), 1000),
            (typeof(FactoryOfSteelPlate), 1000),
            (typeof(FactoryOfSteelPipe), 1000),
            (typeof(FactoryOfSteelRod), 1000),
            (typeof(FactoryOfSteelWire), 1000),
            (typeof(FactoryOfSteelGear), 1000),

            // 内燃机
            (typeof(FactoryOfCombustionMotor), 1000),

            // 电动机
            (typeof(FactoryOfElectroMotor), 1000),
            (typeof(FactoryOfWindTurbineComponent), 1000),
            (typeof(FactoryOfSolarPanelComponent), 1000),

            // 涡轮机
            (typeof(FactoryOfTurbine), 1000),
        };
    }
}
