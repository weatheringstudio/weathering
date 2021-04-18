

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfPhysics : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(SchoolEquipment);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(SeaWaterPump), 0),
            (typeof(PowerGeneratorOfWood), 1000),
            (typeof(PowerGeneratorOfCoal), 1000),
            (typeof(PowerGeneratorOfLiquefiedPetroleumGas), 1000),
            (typeof(PowerGeneratorOfWindTurbineStation), 2000),
            (typeof(PowerGeneratorOfSolarPanelStation), 2000),
            (typeof(PowerGeneratorOfNulearFissionEnergy), 10000),
            (typeof(PowerGeneratorOfNulearFusionEnergy), 10000),
        };
    }
}
