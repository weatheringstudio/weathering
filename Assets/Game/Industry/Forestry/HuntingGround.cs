
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    // 兽肉
    [Depend(typeof(AnimalFlesh))]
    public class Meat { }

    // 鹿肉
    [Depend(typeof(Meat))]
    public class DeerMeat { }

    // 兔肉
    [Depend(typeof(Meat))]
    public class RabbitMeat { }


    /// <summary>
    /// 猎场
    /// </summary>
    [ConstructionCostBase(typeof(Wood), 10)]
    [BindTerrainType(typeof(TerrainType_Forest))]
    [Concept]
    public class HuntingGround : AbstractFactoryStatic, IPassable
    {
        protected override bool PreserveLandscape => true;
        public override string SpriteKey => DecoratedSpriteKey(typeof(HuntingGround).Name);
        protected override (Type, long) Out0 => (typeof(DeerMeat), 3);

        public bool Passable => false;
        // protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override void AddBuildingDescriptionPage(List<IUIItem> items) {
            items.Add(UIItem.CreateMultilineText($"{Localization.Ins.Get<HuntingGround>()}之间不能相邻"));
        }
    }
}

