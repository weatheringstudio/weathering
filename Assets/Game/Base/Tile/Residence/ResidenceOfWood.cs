

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(WoodPlank), 50, 20)]
    public class ResidenceOfWood : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(ResidenceOfWood).Name);
        protected override (Type, long) In_0 => (typeof(FoodSupply), 6);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 2);
    }
}
