

using System;

namespace Weathering
{
    // 浆果
    [Depend(typeof(Fruit))]
    public class Berry { }



    [ConstructionCostBase(typeof(Berry), 10)]
    public class BerryBush : AbstractFactoryStatic, IPassable
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(BerryBush).Name);

        public bool Passable => true;

        protected override (Type, long) Out0 => (typeof(Berry), 1);
    }
}
