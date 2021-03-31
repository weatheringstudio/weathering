

namespace Weathering
{
    [ConstructionCostBase(typeof(ConcretePowder), 100)]
    [Concept]
    public class WareHouseOfConcrete : AbstractWareHouse
    {
        protected override long Capacity => 30000;
    }
}
