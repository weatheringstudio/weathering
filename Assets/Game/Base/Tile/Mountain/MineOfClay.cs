

using System;

namespace Weathering
{
    // 粘土
    [ConceptSupply(typeof(ClaySupply))]
    [ConceptDescription(typeof(ClayDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Clay { }
    [ConceptResource(typeof(Clay))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class ClaySupply { }
    [Concept]
    public class ClayDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [BindTerrainType(TerrainType.Mountain)]
    [Concept]
    public class MineOfClay : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(ClaySupply), 1);

        public bool Passable => false;
    }
}
