

using System;

namespace Weathering
{
    public class BerryBush : AbstractFactoryStatic
    {
        protected override (Type, long) Out0 => (typeof(BerrySupply), 1);
    }
}
