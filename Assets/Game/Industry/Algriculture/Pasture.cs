

using System;

namespace Weathering
{
    // 牛奶
    [Depend(typeof(Food))]
    public class Milk { }

    [ConstructionCostBase(typeof(Food), 100, 20)]
    public class Pasture : AbstractFactoryStatic, IPassable
    {
        public bool Passable => true;

        public override string SpriteKey => DecoratedSpriteKey(typeof(Pasture).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) In_0 => (typeof(Grain), 12);
        protected override (Type, long) Out0 => (typeof(Milk), 30);
    }
}
