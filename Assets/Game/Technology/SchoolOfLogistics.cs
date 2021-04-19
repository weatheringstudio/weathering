

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfLogistics : AbstractTechnologyCenter
    {
        protected override long TechnologyPointMaxRevenue => 100;
        protected override Type TechnologyPointType => typeof(SchoolEquipment);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(RoadForSolid), 100),
            (typeof(RoadAsBridge), 100),
            (typeof(RoadAsTunnel), 100),
            (typeof(RoadOfConcrete), 100),
            (typeof(RoadAsRailRoad), 500),
            (typeof(RoadLoaderOfRoadAsRailRoad), 500),


            (typeof(TransportStationSimpliest), 500),
            (typeof(TransportStationDestSimpliest), 500),

            (typeof(TransportStationPort), 500),
            (typeof(TransportStationDestPort), 500),

            (typeof(TransportStationAirport), 500),
            (typeof(TransportStationDestAirport), 500),
        };
    }
}
