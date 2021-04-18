

using System;

namespace Weathering
{
    /// <summary>
    /// 内燃机
    /// </summary>
    [Depend(typeof(DiscardableSolid))]
    public class CombustionMotor { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfCombustionMotor : AbstractFactoryStatic
    {
        public override string SpriteKey => DecoratedSpriteKey(typeof(FactoryOfMetalSmelting).Name);

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 2);

        protected override (Type, long) Out0 => (typeof(CombustionMotor), 1);
        protected override (Type, long) In_0 => (typeof(SteelPipe), 4);
        protected override (Type, long) In_1 => (typeof(SteelRod), 8);
    }
}
