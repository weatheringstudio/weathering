

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(StoneBrick), 100, 20)]
    public class ResidenceOfStone : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(ResidenceOfGrass).Name);
        protected override (Type, long) In_0 => (typeof(FoodSupply), 9);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 3);
    }
}
