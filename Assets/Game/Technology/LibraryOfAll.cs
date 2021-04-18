

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfAll : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(Book);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(LibraryOfAll), 0),

            (typeof(LibraryOfAgriculture), 100),
            (typeof(LibraryOfGeography), 100),
            (typeof(LibraryOfHandcraft), 100),
            (typeof(LibraryOfLogistics), 100),
            (typeof(LibraryOfEconomy), 100),
            (typeof(LibraryOfConstruction), 200),
            (typeof(LibraryOfMetalWorking), 500),

            (typeof(SchoolOfAll), 1000)
        };
    }
}
