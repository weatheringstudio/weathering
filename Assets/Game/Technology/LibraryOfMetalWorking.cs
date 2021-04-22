

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfMetalWorking : AbstractTechnologyCenter
    {
        public const long BaseCost = 600;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(CopperOre);
        protected override long TechnologyPointIncRequired => 1;

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(RoadAsTunnel), 0*BaseCost),
            (typeof(WorkshopOfCopperSmelting), 1*BaseCost),
            (typeof(WorkshopOfCopperCasting), 2*BaseCost),

            (typeof(WorkshopOfIronSmelting), 2*BaseCost),
            (typeof(WorkshopOfIronCasting), 3*BaseCost),
        };
    }
}
