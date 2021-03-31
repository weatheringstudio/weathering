

using System;

namespace Weathering
{
    [BindTerrainType(typeof(TerrainType_Sea))]
    [ConstructionCostBase(typeof(WoodPlank), 100, 20)]
    public class ResidenceCoastal : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(ResidenceOfWood).Name);
        protected override (Type, long) In_0 => (typeof(FoodSupply), 6);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 2);
    }
}
