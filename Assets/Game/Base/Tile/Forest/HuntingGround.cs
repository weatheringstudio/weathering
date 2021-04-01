
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    // 兽肉
    [ConceptDescription(typeof(AnimalFleshDescription))]
    [Depend(typeof(AnimalFlesh))]
    [Concept]
    public class Meat { }

    [Depend(typeof(AnimalFleshSupply))]
    [Concept]
    public class MeatSupply { }
    [Concept]
    public class MeatDescription { }

    // 鹿肉
    [ConceptSupply(typeof(DeerMeatSupply))]
    [ConceptDescription(typeof(DeerMeatDescription))]
    [Depend(typeof(Meat))]
    [Concept]
    public class DeerMeat { }
    [ConceptResource(typeof(DeerMeat))]
    [Depend(typeof(MeatSupply))]
    [Concept]
    public class DeerMeatSupply { }
    [Concept]
    public class DeerMeatDescription { }

    // 兔肉
    [ConceptSupply(typeof(RabbitMeatSupply))]
    [ConceptDescription(typeof(RabbitMeatDescription))]
    [Depend(typeof(Meat))]
    [Concept]
    public class RabbitMeat { }
    [ConceptResource(typeof(RabbitMeat))]
    [Depend(typeof(MeatSupply))]
    [Concept]
    public class RabbitMeatSupply { }
    [Concept]
    public class RabbitMeatDescription { }


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
        protected override (Type, long) Out0 => (typeof(DeerMeatSupply), 3);

        public bool Passable => false;
        // protected override (Type, long) In_0_Inventory => (typeof(Worker), 1);

        protected override void AddBuildingDescriptionPage(List<IUIItem> items) {
            items.Add(UIItem.CreateMultilineText($"{Localization.Ins.Get<HuntingGround>()}之间不能相邻"));
        }
    }
}

