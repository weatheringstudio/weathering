

using System;

namespace Weathering
{
    // 金矿石
    [ConceptSupply(typeof(GoldOreSupply))]
    [ConceptDescription(typeof(GoldOreDescription))]
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class GoldOre { }
    [ConceptResource(typeof(GoldOre))]
    [Depend(typeof(TransportableSolid))]
    [Concept]
    public class GoldOreSupply { }
    [Concept]
    public class GoldOreDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [Concept]
    public class MineOfGold : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(GoldOreSupply), 1);

        public bool Passable => false;
    }
}
