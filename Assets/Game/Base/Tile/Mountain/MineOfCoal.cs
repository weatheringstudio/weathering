

using System;

namespace Weathering
{
    // 燃料
    [ConceptSupply(typeof(FuelSupply))]
    [ConceptDescription(typeof(FuelDescription))]
    [Depend(typeof(Discardable))]
    [Concept]
    public class Fuel { }
    [ConceptResource(typeof(Fuel))]
    [Depend(typeof(Transportable))]
    [Concept]
    public class FuelSupply { }
    [Concept]
    public class FuelDescription { }


    // 煤矿
    [ConceptSupply(typeof(CoalSupply))]
    [ConceptDescription(typeof(CoalDescription))]
    [Depend(typeof(Fuel))]
    [Concept]
    public class Coal { }
    [ConceptResource(typeof(Coal))]
    [Depend(typeof(FuelSupply))]
    [Concept]
    public class CoalSupply { }
    [Concept]
    public class CoalDescription { }

    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    [BindTerrainType(typeof(TerrainType_Mountain))]
    [Concept]
    public class MineOfCoal : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(MountainMine).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 2);
        protected override (Type, long) Out0 => (typeof(CoalSupply), 3);

        public bool Passable => false;
    }
}
