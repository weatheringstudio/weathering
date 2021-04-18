

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(SchoolEquipment), 100, 10)]
    public class SchoolOfAll : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(SchoolEquipment);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(LibraryOfAll), 0),
            (typeof(SchoolOfAll), 0),

            (typeof(SchoolOfGeology), 100),
            (typeof(SchoolOfEngineering), 100),
            (typeof(SchoolOfLogistics), 100),
            (typeof(SchoolOfChemistry), 100),
            (typeof(SchoolOfPhysics), 100),
            (typeof(SchoolOfElectronics), 100),
            (typeof(SchoolOfSpace), 100),
        };
    }
}
