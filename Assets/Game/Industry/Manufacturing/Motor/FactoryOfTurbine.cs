

using System;

namespace Weathering
{
    // 铜导线
    [Depend(typeof(DiscardableSolid))]
    public class Turbine { }

    public class FactoryOfTurbine : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Factory).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 5);

        protected override (Type, long) Out0 => (typeof(SteelPipe), 4);
        protected override (Type, long) In_0 => (typeof(SteelPlate), 2);
    }
}
