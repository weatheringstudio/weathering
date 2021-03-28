

namespace Weathering
{
    [ConstructionCostBase(typeof(StoneBrick), 300)]
    [Concept]
    public class WareHouseOfBrick : AbstractWareHouse
    {
        protected override long Capacity => 10000;
    }
}
