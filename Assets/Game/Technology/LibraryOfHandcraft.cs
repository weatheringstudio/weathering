

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfHandcraft : AbstractTechnologyCenter
    {
        private const long BaseCost = 1000;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override long TechnologyPointIncRequired => 1;
        protected override Type TechnologyPointType => typeof(WoodPlank);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(WorkshopOfWoodcutting), 0),
            (typeof(WorkshopOfStonecutting), 1*BaseCost),
            (typeof(WorkshopOfBrickMaking), 1*BaseCost),
            (typeof(WorkshopOfToolPrimitive), 3*BaseCost),
            (typeof(WorkshopOfWheelPrimitive), 3*BaseCost),

            (typeof(WorkshopOfMachinePrimitive), 5*BaseCost),
        };
    }
}
