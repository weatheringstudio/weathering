

using System;

namespace Weathering
{
    // 海水
    [Depend(typeof(DiscardableFluid))]
    [Concept]
    public class SeaWater { }

    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    [BindTerrainType(typeof(TerrainType_Sea))]
    [Concept]
    public class SeaWaterPump : AbstractFactoryStatic, IPassable
    {
        public bool Passable => false;

        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey("FactoryOfAirSeparator");

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 3);
        protected override (Type, long) Out0 => (typeof(SeaWater), 3);
    }
}
