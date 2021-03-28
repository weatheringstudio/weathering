

namespace Weathering
{
    [ConstructionCostBase(typeof(ConcretePowder), 1000)]
    [Concept]
    public class WareHouseOfConcrete : AbstractWareHouse
    {
        protected override long Capacity => 30000;
    }
}
