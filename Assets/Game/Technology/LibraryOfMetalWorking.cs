

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfMetalWorking : AbstractTechnologyCenter
    {
        public const long BaseCost = 300;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(MetalOre);
        protected override long TechnologyPointIncRequired => 1;

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(RoadAsTunnel), 1*BaseCost),
            (typeof(WorkshopOfCopperSmelting), 0),
            (typeof(WorkshopOfCopperCasting), 1*BaseCost),

            (typeof(WorkshopOfIronSmelting), 2*BaseCost),
            (typeof(WorkshopOfIronCasting), 3*BaseCost),
        };
    }
}
