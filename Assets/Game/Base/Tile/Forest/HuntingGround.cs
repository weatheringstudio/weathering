
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    // 鹿肉
    [ConceptSupply(typeof(DearMeatSupply))]
    [ConceptDescription(typeof(DearMeatDescription))]
    [Depend(typeof(Meat))]
    [Concept]
    public class DearMeat { }
    [ConceptResource(typeof(DearMeat))]
    [Depend(typeof(MeatSupply))]
    [Concept]
    public class DearMeatSupply { }
    [Concept]
    public class DearMeatDescription { }

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
                return typeof(HuntingGround).Name;
            }
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();

            Res = Refs.Create<HuntingGround>();
            Res.BaseType = (Map as StandardMap).TemporatureTypes[Pos.x, Pos.y] == typeof(TemporatureTemporate) ? typeof(RabbitMeatSupply) : typeof(DearMeatSupply);
            Res.BaseValue = 1;

            Res.Type = Res.BaseType;
            Res.Value = Res.BaseValue;
        }

        public IRef Res { get; private set; }


        public override void OnEnable() {
            base.OnEnable();
            Res = Refs.Get<HuntingGround>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();
            items.Add(LinkUtility.CreateTextForPair(OnlyPair()));
            UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(), items);
        }

        public void Provide((Type, long) pair) {
            if (pair.Item1 != Res.Type) throw new Exception();
            if (pair.Item2 > Res.Value) throw new Exception();
            Res.Value -= pair.Item2;
        }

        public void CanProvide(List<(Type, long)> items) {
            items.Add(OnlyPair());
        }
        private (Type, long) OnlyPair() {
            return (Res.Type, Res.Value);
        }
    }
}

