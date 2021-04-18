

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfGeography : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(Book);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(ForestLoggingCamp), 100),
            (typeof(MountainQuarry), 100),
            (typeof(MineOfClay), 100),
            (typeof(MineOfCopper), 500),
            (typeof(MineOfIron), 500),
            //(typeof(MineOfSand), 1000),
            //(typeof(MineOfSalt), 1000),
        };
    }
}
