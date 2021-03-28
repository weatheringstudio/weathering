

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(Brick), 100)]
    public class ResidenceOfBrick : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(ResidenceOfGrass).Name);
        protected override (Type, long) In_0 => (typeof(FoodSupply), 12);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 4);
    }
}
