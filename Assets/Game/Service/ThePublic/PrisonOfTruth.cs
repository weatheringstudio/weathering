

using System;

namespace Weathering
{
    // 真理
    [Depend(typeof(Discardable))]
    [Concept]
    public class Truth { }

    public class ServiceOfTruth { }

    public class PrisonOfTruth : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(PrisonOfTruth).Name;
        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) Out0_Inventory => (typeof(Truth), 100);
    }
}
