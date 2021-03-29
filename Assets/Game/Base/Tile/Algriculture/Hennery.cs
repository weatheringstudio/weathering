

using System;

namespace Weathering
{
    // 鸡蛋
    [ConceptSupply(typeof(EggSupply))]
    [Depend(typeof(Food))]
    [Concept]
    public class Egg { }
    [ConceptResource(typeof(Egg))]
    [Depend(typeof(FoodSupply))]
    [Concept]
    public class EggSupply { }

    [ConstructionCostBase(typeof(Food), 100)]
    public class Hennery : AbstractFactoryStatic, IPassable
    {
        public bool Passable => true;

        public override string SpriteKey => DecoratedSpriteKey(typeof(Hennery).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) In_0 => (typeof(GrainSupply), 6);
        protected override (Type, long) Out0 => (typeof(EggSupply), 15);
    }
}
