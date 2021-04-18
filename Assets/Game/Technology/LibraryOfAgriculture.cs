

using System;
using System.Collections.Generic;

namespace Weathering
{
    [ConstructionCostBase(typeof(Book), 100, 10)]
    public class LibraryOfAgriculture : AbstractTechnologyCenter
    {
        protected override long TechnologyPointCapacity => 100;
        protected override Type TechnologyPointType => typeof(Book);

        protected override List<(Type, long)> TechList => new List<(Type, long)> {

            (typeof(HuntingGround), 100), // 农场
            (typeof(SeaFishery), 100), // 仓库
            (typeof(Pasture), 300), // 道路
            (typeof(Hennery), 300), // 伐木场
        };
    }
}
