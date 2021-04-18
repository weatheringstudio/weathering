

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfLogistics : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(Book);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(RoadForSolid), 100),
            (typeof(RoadAsBridge), 100),
            (typeof(RoadAsTunnel), 100),

            (typeof(WareHouseOfGrass), 0),
            (typeof(WareHouseOfWood), 200),
            (typeof(WareHouseOfStone), 300),
            (typeof(WareHouseOfBrick), 500),

            (typeof(TransportStationSimpliest), 500),
            (typeof(TransportStationDestSimpliest), 500),
        };
    }
}
