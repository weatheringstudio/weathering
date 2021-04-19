

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfElectronics : AbstractTechnologyCenter
    {
        protected override long TechnologyPointMaxRevenue => 100;
        protected override Type TechnologyPointType => typeof(SchoolEquipment);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(FactoryOfConductorOfCopperWire), 1000),
            (typeof(FactoryOfCircuitBoardSimple), 3000),
            (typeof(FactoryOfCircuitBoardIntegrated), 5000),
            (typeof(FactoryOfCircuitBoardAdvanced), 8000),
        };
    }
}
