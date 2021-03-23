

using System;

namespace Weathering
{
    public class BerryBush : Factory
    {
        protected override (Type, long) Out0 => (typeof(BerrySupply), 1);
    }
}
