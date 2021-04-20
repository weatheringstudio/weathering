

using System;

namespace Weathering
{
    [BindTerrainType(typeof(TerrainType_Sea))]
    [ConstructionCostBase(typeof(WoodPlank), 100, 20)]
    public class ResidenceCoastal : AbstractFactoryStatic
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => typeof(ResidenceCoastal).Name;
        protected override (Type, long) In_0 => (typeof(Food), 6);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 2);
    }
}
