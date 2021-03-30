

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(ConcretePowder), 100, 30)]
    public class ResidenceOfConcrete : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(ResidenceOfWood).Name);
        protected override (Type, long) In_0 => (typeof(FoodSupply), 18);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 6);
    }
}
