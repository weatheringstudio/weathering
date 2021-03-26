

using System;

namespace Weathering
{
    // 浆果
    [ConceptSupply(typeof(BerrySupply))]
    [ConceptDescription(typeof(BerryDescription))]
    [Depend(typeof(Fruit))]
    [Concept]
    public class Berry { }
    [ConceptResource(typeof(Berry))]
    [Depend(typeof(FruitSupply))]
    [Concept]
    public class BerrySupply { }
    [Concept]
    public class BerryDescription { }


    [ConstructionCostBase(typeof(Berry), 10)]
    public class BerryBush : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(BerryBush).Name);
        protected override (Type, long) Out0 => (typeof(BerrySupply), 1);
    }
}
