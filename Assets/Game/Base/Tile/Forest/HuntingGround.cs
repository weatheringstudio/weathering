
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


    [Concept]
    public class HuntingGround : StandardTile, ILinkProvider
    {
        public override string SpriteKeyBase => TerrainDefault.CalculateTerrain(Map as StandardMap, Pos).Name;
        public override string SpriteKey {
            get {
                return meatType == typeof(RabbitMeatSupply) ? "HuntingGroundRabbit" : "HuntingGroundDeer";// typeof(HuntingGround).Name;
            }
        }

        private Type meatType;
        private Type GetMeatType() {
            // return (Map as StandardMap).TemporatureTypes[Pos.x, Pos.y] == typeof(TemporatureTemporate) ? typeof(RabbitMeatSupply) : typeof(DearMeatSupply);
            return typeof(DeerMeatSupply);
        }
        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();

            meatType = GetMeatType();
            Res = Refs.Create(meatType);
            Res.Type = meatType;
            Res.BaseValue = 1;
            Res.Value = Res.BaseValue;
        }

        public IRef Res { get; private set; }

        public override void OnEnable() {
            base.OnEnable();

            if (meatType == null) meatType = GetMeatType();
            Res = Refs.Get(meatType);
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            LinkUtility.AddButtons(items, this);

            if (Res.Value == Res.BaseValue) {
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }

            UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(), items);
        }

        public void Provide(List<IRef> refs) {
            refs.Add(Res);
        }
    }
}

