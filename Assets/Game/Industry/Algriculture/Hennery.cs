

using System;

namespace Weathering
{
    // 鸡蛋
    [Depend(typeof(Food))]
    public class Egg { }

    [ConstructionCostBase(typeof(Food), 100, 20)]
    public class Hennery : AbstractFactoryStatic, IPassable
    {
        public bool Passable => false;

        public override string SpriteKey => DecoratedSpriteKey(typeof(Hennery).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) In_0 => (typeof(Grain), 6);
        protected override (Type, long) Out0 => (typeof(Egg), 18);
    }
}
