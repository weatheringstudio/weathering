

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfAgriculture : AbstractTechnologyCenter
    {
        private const long BaseCost = 3000;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override long TechnologyPointIncRequired => 6;
        protected override Type TechnologyPointType => typeof(Grain);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(Farm), 0),
            (typeof(HuntingGround), 1*BaseCost),
            (typeof(SeaFishery), 2*BaseCost),
            (typeof(Pasture), 5*BaseCost),
            (typeof(Hennery), 5*BaseCost),
        };
    }
}
