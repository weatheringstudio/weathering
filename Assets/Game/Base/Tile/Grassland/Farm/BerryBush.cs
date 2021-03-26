

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(Berry), 100)]
    public class BerryBush : AbstractFactoryStatic
    {
        protected override (Type, long) Out0 => (typeof(BerrySupply), 1);
    }
}
