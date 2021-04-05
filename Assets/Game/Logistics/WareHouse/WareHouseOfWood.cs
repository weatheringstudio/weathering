

namespace Weathering
{
    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [Concept]
    public class WareHouseOfWood : AbstractWareHouse
    {
        protected override long Capacity => 2000;
    }
}
