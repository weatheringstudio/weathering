
using System;
using System.Collections.Generic;

namespace Weathering
{
    // 谷物
    [ConceptSupply(typeof(GrainSupply))]
    [Depend(typeof(Food))]
    [Concept]
    public class Grain { }
    [ConceptResource(typeof(Grain))]
    [Depend(typeof(FoodSupply))]
    [Concept]
    public class GrainSupply { }


    [Concept]
    public class Farm : Factory
    {
        public override string SpriteKey => Running ? "FarmRipe" : "FarmGrowing";

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override (Type, long) Out0 => (typeof(GrainSupply), 8);
    }
}

