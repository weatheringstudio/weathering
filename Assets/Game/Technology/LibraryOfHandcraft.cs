

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfHandcraft : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(Book);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(WorkshopOfWoodcutting), 100),
            (typeof(WorkshopOfStonecutting), 100),
            (typeof(WorkshopOfBrickMaking), 200),
            (typeof(WorkshopOfToolPrimitive), 500),
            (typeof(WorkshopOfWheelPrimitive), 1000),

            (typeof(WorkshopOfMachinePrimitive), 2000),
        };
    }
}
