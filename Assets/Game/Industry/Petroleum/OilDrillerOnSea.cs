

using System;

namespace Weathering
{
    [BindTerrainType(typeof(TerrainType_Sea))]
    [ConstructionCostBase(typeof(LightMaterial), 100)]
    public class OilDrillerOnSea : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey("FactoryOfAirSeparator");

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(ElectricitySupply), 30);

        protected override (Type, long) Out0 => (typeof(CrudeOilSupply), 3);
    }
}
