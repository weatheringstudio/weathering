

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfGeology : AbstractTechnologyCenter
    {
        protected override long TechnologyPointMaxRevenue => 100;
        protected override Type TechnologyPointType => typeof(SchoolEquipment);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {
            //(typeof(MineOfSand), 1000),
            //(typeof(MineOfSalt), 1000),

            (typeof(SeaWaterPump), 0),
            (typeof(MineOfCoal), 1000),
            (typeof(MineOfAluminum), 3000),
            (typeof(OilDriller), 5000),
        };
    }
}
