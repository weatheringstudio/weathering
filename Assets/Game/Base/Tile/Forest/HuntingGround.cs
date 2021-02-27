
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
    public class HuntingGround : StandardTile, ILinkable
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
            res = Refs.Create<HuntingGround>();

            res.Type = (Map as StandardMap).TemporatureTypes[Pos.x, Pos.y] == typeof(TemporatureTemporate) ? typeof(RabbitMeatSupply) : typeof(DearMeatSupply);
            res.Value = 1;
        }

        public void OnLink(Type direction) { }
        public IRef Res => res;
        private IRef res;
        public override void OnEnable() {
            base.OnEnable();
            res = Refs.Get<HuntingGround>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();
            LinkUtility.CreateDescription(items, res);


            UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(), items);
        }


    }
}

