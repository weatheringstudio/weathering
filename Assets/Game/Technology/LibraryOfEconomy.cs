

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfEconomy : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(Book);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(MarketForPlayer), 100),
        };
    }
}
