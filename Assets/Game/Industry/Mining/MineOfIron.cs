

using System;

namespace Weathering
{
    // 铁矿
    [ConceptSupply(typeof(IronOreSupply))]
    [ConceptDescription(typeof(IronOreDescription))]
    [Depend(typeof(MetalOre))]
    [Concept]
    public class IronOre : IMineralType { }
    [ConceptResource(typeof(IronOre))]
    [Depend(typeof(MetalOreSupply))]
    [Concept]
    public class IronOreSupply { }
    [Concept]
    public class IronOreDescription { }

    [ConstructionCostBase(typeof(WoodPlank), 100)]
    [CanBeBuildOnNotPassableTerrain]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [MineOfMineralType(typeof(IronOre))]
    [Concept]
    public class MineOfIron : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(IronOreSupply), 2);

        public bool Passable => false;
    }
}
