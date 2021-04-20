

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfChemistry : AbstractTechnologyCenter
    {
        public const long BaseCost = 3000;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(LiquefiedPetroleumGas);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(FactoryOfDesalination), 0*BaseCost),
            (typeof(FactoryOfPetroleumRefining), 0),
            (typeof(FactoryOfLightOilCracking), BaseCost),
            (typeof(FactoryOfHeavyOilCracking), 2*BaseCost),
            (typeof(FactoryOfPlastic), 3*BaseCost),

            (typeof(FactoryOfJetFuel), 5*BaseCost),
            (typeof(FactoryOfFuelPack_Oxygen_Hydrogen), 5*BaseCost),
            (typeof(FactoryOfFuelPack_Oxygen_JetFuel), 5*BaseCost),

            (typeof(AirSeparator), 5*BaseCost),
            (typeof(FactoryOfElectrolysisOfSaltedWater), 5*BaseCost),
            (typeof(FactoryOfElectrolysisOfWater), 5*BaseCost),
        };
    }
}
