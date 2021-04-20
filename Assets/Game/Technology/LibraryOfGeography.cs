

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfGeography : AbstractTechnologyCenter
    {
        private const long BaseCost = 3000;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override long TechnologyPointIncRequired => 1;
        protected override Type TechnologyPointType => typeof(Stone);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(MountainQuarry), 0),
            (typeof(MineOfClay), 1*BaseCost),
            (typeof(MineOfGold), 2*BaseCost),
            (typeof(MineOfCopper), 5*BaseCost),
            (typeof(MineOfIron), 5*BaseCost),
            //(typeof(MineOfSand), 1000),
            //(typeof(MineOfSalt), 1000),
        };
    }
}
