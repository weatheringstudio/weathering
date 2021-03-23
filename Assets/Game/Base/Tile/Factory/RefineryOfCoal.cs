

using System;

namespace Weathering
{
    public class RefineryOfCoal : Factory
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(WorkshopOfMetalSmelting).Name);

        protected override (Type, long) Out0 => (typeof(FuelSupply), 5);

        protected override (Type, long) In_0 => (typeof(CoalSupply), 1);

    }
}
