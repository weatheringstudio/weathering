

using System;

namespace Weathering
{
    // 牛奶
    [ConceptSupply(typeof(MilkSupply))]
    [Depend(typeof(Food))]
    [Concept]
    public class Milk { }
    [ConceptResource(typeof(Milk))]
    [Depend(typeof(FoodSupply))]
    [Concept]
    public class MilkSupply { }

    [ConstructionCostBase(typeof(Food), 100, 20)]
    public class Pasture : AbstractFactoryStatic, IPassable
    {
        public bool Passable => true;

        public override string SpriteKey => DecoratedSpriteKey(typeof(Pasture).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) In_0 => (typeof(GrainSupply), 12);
        protected override (Type, long) Out0 => (typeof(MilkSupply), 30);
    }
}
