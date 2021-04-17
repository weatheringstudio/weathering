

namespace Weathering
{
    [ConstructionCostBase(typeof(Grain), 10, 20)]
    [Concept]
    public class WareHouseOfGrass : AbstractWareHouse
    {
        protected override long Capacity => 1000;
    }
}
