

namespace Weathering
{
    [ConstructionCostBase(typeof(StoneBrick), 100)]
    [Concept]
    public class WareHouseOfStone : AbstractWareHouse
    {
        protected override long Capacity => 5000;
    }
}
