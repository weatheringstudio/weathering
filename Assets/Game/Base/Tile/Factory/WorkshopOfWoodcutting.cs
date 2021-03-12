

using System;

namespace Weathering
{
    public class WorkshopOfWoodcutting : Factory
    {
        public override string SpriteKey => "Workshop";

        protected override long WorkerCost => 1;

        protected override (Type, long) Out0 => (typeof(GrainSupply), 1);

        protected override (Type, long) In_0 => (typeof(WoodSupply), 1);
    }
}
