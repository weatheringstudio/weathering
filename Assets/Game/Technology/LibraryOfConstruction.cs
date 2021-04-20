

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfConstruction : AbstractTechnologyCenter
    {
        public const long BaseCost = 100;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(ToolPrimitive);
        protected override long TechnologyPointIncRequired => 1;

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(ResidenceOfGrass), 0),
            (typeof(ResidenceOfWood), 1*BaseCost),
            (typeof(ResidenceCoastal), 2*BaseCost),
            (typeof(ResidenceOverTree), 2*BaseCost),
            (typeof(ResidenceOfStone), 2*BaseCost),
            (typeof(ResidenceOfBrick), 3*BaseCost),
            // (typeof(ResidenceOfConcrete), 5*BaseCost),
        };
    }
}
