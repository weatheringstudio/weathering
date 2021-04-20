

using System;

namespace Weathering
{
    [BindTerrainType(typeof(TerrainType_Forest))]
    [ConstructionCostBase(typeof(WoodPlank), 50, 10)]
    public class ResidenceOverTree : AbstractFactoryStatic
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => typeof(ResidenceOverTree).Name;
        protected override (Type, long) In_0 => (typeof(Food), 6);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 2);
    }
}
