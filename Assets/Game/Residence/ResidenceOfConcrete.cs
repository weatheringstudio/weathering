

using System;

namespace Weathering
{
    [ConstructionCostBase(typeof(ConcretePowder), 100, 30)]
    public class ResidenceOfConcrete : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(ResidenceOfConcrete).Name;
        protected override (Type, long) In_0 => (typeof(Food), 18);
        protected override (Type, long) Out0_Inventory => (typeof(Worker), 6);
    }
}
