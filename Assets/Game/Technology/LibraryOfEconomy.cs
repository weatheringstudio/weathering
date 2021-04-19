

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfEconomy : AbstractTechnologyCenter
    {
        protected override long TechnologyPointMaxRevenue => 100;
        protected override Type TechnologyPointType => typeof(GoldOre);
        protected override long TechnologyPointMaxRevenueIncRequired => 1;

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(MarketForPlayer), 100),
        };
    }
}
