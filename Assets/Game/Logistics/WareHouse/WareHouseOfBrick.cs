

namespace Weathering
{
    [ConstructionCostBase(typeof(Brick), 100)]
    [Concept]
    public class WareHouseOfBrick : AbstractWareHouse
    {
        protected override long Capacity => 10000;
    }
}
