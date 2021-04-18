

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfChemistry : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(SchoolEquipment);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(FactoryOfPetroleumRefining), 1000),
            (typeof(FactoryOfHeavyOilCracking), 1000),
            (typeof(FactoryOfLightOilCracking), 1000),
            (typeof(FactoryOfPlastic), 1000),

            (typeof(AirSeparator), 1000),
            (typeof(FactoryOfElectrolysisOfSaltedWater), 1000),
            (typeof(FactoryOfDesalination), 1000),
            (typeof(FactoryOfElectrolysisOfWater), 1000),

            (typeof(FactoryOfJetFuel), 2000),
            (typeof(FactoryOfFuelPack_Oxygen_Hydrogen), 2000),
            (typeof(FactoryOfFuelPack_Oxygen_JetFuel), 2000),
        };
    }
}
