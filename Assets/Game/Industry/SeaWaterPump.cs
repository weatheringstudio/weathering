

using System;

namespace Weathering
{
    // 海水
    [ConceptSupply(typeof(SeaWaterSupply))]
    [ConceptDescription(typeof(SeaWaterDescription))]
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class SeaWater { }
    [ConceptResource(typeof(SeaWater))]
    [Depend(typeof(TransportableFluid))]
    [Concept]
    public class SeaWaterSupply { }
    [Concept]
    public class SeaWaterDescription { }

    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    [BindTerrainType(typeof(TerrainType_Sea))]
    [Concept]
    public class SeaWaterPump : AbstractFactoryStatic, IPassable
    {
        public bool Passable => false;

        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey("FactoryOfAirSeparator");

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 3);
        protected override (Type, long) Out0 => (typeof(SeaWaterSupply), 3);
    }
}
