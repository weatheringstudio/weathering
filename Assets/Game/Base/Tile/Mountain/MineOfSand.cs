

using System;

namespace Weathering
{
    // 粘土
    [ConceptSupply(typeof(SandSupply))]
    [ConceptDescription(typeof(SandDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Sand { }
    [ConceptResource(typeof(Clay))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class SandSupply { }
    [Concept]
    public class SandDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [Concept]
    public class MineOfSand : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(SandSupply), 3);

        public bool Passable => false;
    }
}
