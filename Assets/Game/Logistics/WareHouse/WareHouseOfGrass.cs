

namespace Weathering
{
    [ConstructionCostBase(typeof(Food), 10)]
    [Concept]
    public class WareHouseOfGrass : AbstractWareHouse
    {
        protected override long Capacity => 1000;
    }
}
