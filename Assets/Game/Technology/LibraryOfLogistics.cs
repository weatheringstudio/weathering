

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfLogistics : AbstractTechnologyCenter
    {
        public const long BaseCost = 1000;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(WheelPrimitive);
        protected override long TechnologyPointIncRequired => 1;

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(RoadForSolid), 0),
            (typeof(RoadOfStone), 0),
            (typeof(RoadAsBridge), 1*BaseCost),
            (typeof(RoadAsTunnel), 1*BaseCost),

            (typeof(TransportStationSimpliest), 1*BaseCost),
            (typeof(TransportStationDestSimpliest), 1*BaseCost),

            (typeof(WareHouseOfGrass), 0),
            (typeof(WareHouseOfWood), 1*BaseCost),
            (typeof(WareHouseOfStone), 2*BaseCost),
            (typeof(WareHouseOfBrick), 2*BaseCost),

            (typeof(RoadAsCanal), 3*BaseCost),
            (typeof(RoadLoaderOfRoadAsCanal), 3*BaseCost),
        };
    }
}
