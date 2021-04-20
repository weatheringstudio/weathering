

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfGeology : AbstractTechnologyCenter
    {
        public const long BaseCost = 1000;
        protected override long TechnologyPointMaxRevenue => BaseCost;
        protected override Type TechnologyPointType => typeof(Coal);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(MineOfCoal), 0),
            (typeof(SeaWaterPump), 1*BaseCost),
            (typeof(MineOfAluminum), 2*BaseCost),
            (typeof(OilDriller), 5*BaseCost),
        };
    }
}
