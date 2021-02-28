
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
    public class HuntingGround : StandardTile, ILinkable, ILinkableProvider
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

        public void OnLink(Type direction) { }
        public IRef Res { get; private set; }

        public (Type, long) CanProvide => (Res.Type, Res.Value);

        public override void OnEnable() {
            base.OnEnable();
            Res = Refs.Get<HuntingGround>();
        }


        public override void OnTap() {
            var items = UI.Ins.GetItems();
            LinkUtility.CreateDescription(items, this);

            // todo 变换类型
            TransformTypes(items);

            items.Add(LinkUtility.CreateDestructionButton(this, Res));

            UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(), items);
        }
        private void TransformTypes(List<IUIItem> items) {
            var tags = Tag.AllTagOf(Res.BaseType);
            foreach (var tag in tags) {
                TransformType(items, tag);
            }
            TransformType(items, Res.BaseType);
        }
        private void TransformType(List<IUIItem> items, Type type) {
            if (Attribute.GetCustomAttribute(type, typeof(ConceptTheAbstract)) == null) {
                items.Add(UIItem.CreateButton($"转换为{Localization.Ins.ValUnit(type)}", () => {
                    TransformTo(type);
                }));
            }
        }
        private void TransformTo(Type type) {
            Res.Type = type;
            OnTap();
        }
    }
}

