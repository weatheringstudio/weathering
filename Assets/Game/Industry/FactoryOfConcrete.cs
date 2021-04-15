﻿

using System;

namespace Weathering
{

    // 水泥
    [Depend(typeof(DiscardableSolid))]
    public class ConcretePowder { }


    [ConstructionCostBase(typeof(MachinePrimitive), 100)]
    public class FactoryOfConcrete : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0 => (typeof(ConcretePowder), 1);
        protected override (Type, long) In_0 => (typeof(IronOre), 2);
        protected override (Type, long) In_1 => (typeof(Stone), 3);
    }
}
