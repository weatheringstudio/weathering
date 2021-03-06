﻿

using System;

namespace Weathering
{
    // 轻质材料
    [Depend(typeof(DiscardableSolid))]
    [Concept]
    public class LightMaterial { }

    [ConstructionCostBase(typeof(BuildingPrefabrication), 100)]
    public class FactoryOfLightMaterial : AbstractFactoryStatic
    {
        public override string SpriteKey => typeof(Factory).Name;

        protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);
        protected override (Type, long) In_1_Inventory => (typeof(Electricity), 5);

        protected override (Type, long) Out0 => (typeof(LightMaterial), 3);

        protected override (Type, long) In_0 => (typeof(Plastic), 2);
        protected override (Type, long) In_1 => (typeof(AluminiumIngot), 2);
    }
}
