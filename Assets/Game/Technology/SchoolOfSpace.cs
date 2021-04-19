

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfSpace : AbstractTechnologyCenter
    {
        protected override long TechnologyPointMaxRevenue => 100;
        protected override Type TechnologyPointType => typeof(SchoolEquipment);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(LaunchSite), 10000),
            (typeof(SpaceElevator), 20000),
        };
    }
}
