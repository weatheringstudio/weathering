

using System;

namespace Weathering
{
    // 铝土 Bauxite
    [ConceptSupply(typeof(AluminumOreSupply))]
    [ConceptDescription(typeof(AluminumOreDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class AluminumOre { }
    [ConceptResource(typeof(AluminumOre))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class AluminumOreSupply { }
    [Concept]
    public class AluminumOreDescription { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [Concept]
    public class MineOfAluminum : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(AluminumOreSupply), 2);

        public bool Passable => false;
    }
}
