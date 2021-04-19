

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfMetalWorking : AbstractTechnologyCenter
    {
        protected override long TechnologyPointMaxRevenue => 100;
        protected override Type TechnologyPointType => typeof(MetalOre);
        protected override long TechnologyPointMaxRevenueIncRequired => 1;

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(RoadAsTunnel), 300),
            (typeof(WorkshopOfCopperSmelting), 500),
            (typeof(WorkshopOfIronSmelting), 500),
            (typeof(WorkshopOfCopperCasting), 1000),
            (typeof(WorkshopOfIronCasting), 1000),
        };
    }
}
