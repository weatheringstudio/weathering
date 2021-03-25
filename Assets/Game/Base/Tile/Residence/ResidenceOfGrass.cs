

using System;

namespace Weathering
{
    public class ResidenceOfGrass : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(ResidenceOfGrass).Name);
        protected override (Type, long) In_0 => (typeof(FoodSupply), 5);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 1);
    }
}
