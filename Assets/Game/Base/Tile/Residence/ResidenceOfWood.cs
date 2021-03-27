

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(WoodPlank), 100)]
    public class ResidenceOfWood : AbstractFactoryStatic, IPassable
    {
        public bool Passable => false;
        public override string SpriteKey => DecoratedSpriteKey(typeof(ResidenceOfWood).Name);
        protected override (Type, long) In_0 => (typeof(FoodSupply), 10);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 3);
    }
}
