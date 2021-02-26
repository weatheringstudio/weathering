
using System;
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
    public class HuntingGround : StandardTile, IProvider
    {
        public (Type, long) CanProvide => (meatType, meat.Value);
        public void Provide((Type, long) request) {
            if (request.Item1 != meatType) throw new Exception();
            if (request.Item2 > meat.Value) throw new Exception();
            meat.Value -= request.Item2;
        }

        public override string SpriteKey {
            get {
                return typeof(HuntingGround).Name;
            }
        }

        private Type meatType;
        private IRef meat;

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            meatType = typeof(RabbitMeat);
            Refs.Create<HuntingGround>().Type = meatType;

            meat = Refs.Create<Meat>();
            meat.Value = 1;
        }

        public override void OnEnable() {
            base.OnEnable();
            meatType = Refs.Get<HuntingGround>().Type;
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();
            items.Add(UIItem.CreateText($"肉产量1，剩余肉产量{meat.Value}"));
            UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(), items);
        }
    }
}

