

using System;
using System.Collections.Generic;

namespace Weathering
{
    // 金币
    [Depend(typeof(DiscardableSolid))]
    public class GoldCoin { }

    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfEconomy : AbstractTechnologyCenter
    {
        public const long BaseCost = 100;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(GoldOre);
        protected override long TechnologyPointIncRequired => 1;

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            // (typeof(MarketForPlayer), BaseCost),
            (typeof(CellarForPersonalStorage), 0),
            (typeof(RecycleStation), 0),
            (typeof(MarketOfAgriculture), 1*BaseCost),
            (typeof(MarketOfMineral), 2*BaseCost),
            (typeof(MarketOfHandcraft), 2*BaseCost),
            (typeof(MarketOfMetalProduct), 3* BaseCost),
        };
    }
}
