

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfAgriculture : AbstractTechnologyCenter
    {
        private const long BaseCost = 1000;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override long TechnologyPointMaxRevenueIncRequired => 1;
        protected override Type TechnologyPointType => typeof(Grain);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(Farm), 0),
            (typeof(HuntingGround), 1*BaseCost),
            (typeof(SeaFishery), 1*BaseCost),
            (typeof(Pasture), 3*BaseCost),
            (typeof(Hennery), 3*BaseCost),
        };
    }
}
