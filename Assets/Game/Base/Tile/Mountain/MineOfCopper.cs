

using System;

namespace Weathering
{
    // 铜矿
    [ConceptSupply(typeof(OreOfCopperSupply))]
    [ConceptDescription(typeof(OreOfCopperDescription))]
    [Depend(typeof(MetalOre))]
    [Concept]
    public class OreOfCopper { }
    [ConceptResource(typeof(OreOfCopper))]
    [Depend(typeof(MetalOreSupply))]
    [Concept]
    public class OreOfCopperSupply { }
    [Concept]
    public class OreOfCopperDescription { }

    [Concept]
    public class MineOfCopper : Factory
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);
        protected override long WorkerCost => 1;
        protected override (Type, long) Out0 => (typeof(OreOfCopperSupply), 1);
    }
}
