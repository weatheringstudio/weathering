

namespace Weathering
{
    [ConstructionCostBase(typeof(WoodPlank), 20)]
    [Concept]
    public class WareHouseOfWood : AbstractWareHouse
    {
        protected override long Capacity => 2000;
    }
}
