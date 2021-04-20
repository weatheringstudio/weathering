
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    // 水产
    [Depend(typeof(AnimalFlesh))]
    public class AquaticProduct { }

    // 鱼肉
    [Depend(typeof(AquaticProduct))]
    public class FishFlesh { }

    [ConstructionCostBase(typeof(Wood), 10)]
    [BindTerrainType(typeof(TerrainType_Sea))]
    public class SeaFishery : AbstractFactoryStatic
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => typeof(SeaFishery).Name;
        protected override (Type, long) Out0 => (typeof(FishFlesh), 3);

        protected override void AddBuildingDescriptionPage(List<IUIItem> items) {
            items.Add(UIItem.CreateMultilineText($"{Localization.Ins.Get<SeaFishery>()}之间不能相邻"));
        }
    }
}

