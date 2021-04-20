

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfElectronics : AbstractTechnologyCenter
    {
        public const long BaseCost = 1000;

        protected override long TechnologyPointIncRequired => 8;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(CircuitBoardSimple);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(FactoryOfConductorOfCopperWire), 0),
            (typeof(FactoryOfCircuitBoardSimple), 0),
            (typeof(FactoryOfCircuitBoardIntegrated), 1*BaseCost),
            (typeof(FactoryOfCircuitBoardAdvanced), 2*BaseCost),
        };
    }
}
