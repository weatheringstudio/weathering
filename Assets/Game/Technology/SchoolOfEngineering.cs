

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfEngineering : AbstractTechnologyCenter
    {
        public const long BaseCost = 1000;

        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(SteelIngot);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(WorkshopOfSteelWorking), 0),
            (typeof(WorkshopOfConcrete), 1*BaseCost),
            (typeof(WorkshopOfBuildingPrefabrication),  2*BaseCost),
            (typeof(FactoryOfSteelWorking), 2*BaseCost),

            (typeof(FactoryOfAluminiumWorking),  3*BaseCost),

            (typeof(FactoryOfLightMaterial), 3*BaseCost),

            // 钢

            (typeof(FactoryOfSteelPlate), 3*BaseCost),
            (typeof(FactoryOfSteelPipe), 3*BaseCost),
            (typeof(FactoryOfSteelRod), 3*BaseCost),
            (typeof(FactoryOfSteelWire), 3*BaseCost),
            (typeof(FactoryOfSteelGear), 3*BaseCost),

            // 内燃机
            (typeof(FactoryOfCombustionMotor), 5*BaseCost),

            // 电动机
            (typeof(FactoryOfElectroMotor), 5*BaseCost),
            (typeof(FactoryOfWindTurbineComponent), 5*BaseCost),
            (typeof(FactoryOfSolarPanelComponent), 5*BaseCost),

            // 涡轮机
            (typeof(FactoryOfTurbine), 5*BaseCost),
        };
    }
}
