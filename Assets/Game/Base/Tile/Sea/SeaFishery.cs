
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    // 水产
    [ConceptDescription(typeof(AquaticProductDescription))]
    [Depend(typeof(AnimalFlesh))]
    [Concept]
    public class AquaticProduct { }

    [Depend(typeof(AnimalFleshSupply))]
    [Concept]
    public class AquaticProductSupply { }
    [Concept]
    public class AquaticProductDescription { }

    // 鱼肉
    [ConceptSupply(typeof(FishFleshSupply))]
    [ConceptDescription(typeof(FishFleshDescription))]
    [Depend(typeof(AquaticProduct))]
    [Concept]
    public class FishFlesh { }

    [ConceptResource(typeof(FishFlesh))]
    [Depend(typeof(AquaticProductSupply))]
    [Concept]
    public class FishFleshSupply { }
    [Concept]
    public class FishFleshDescription { }

    public class SeaFishery : StandardTile, ISealike, ILinkProvider
    {
        public bool IsLikeSea => true;

        public override string SpriteKeyBase { 
            get {
                int index = TileUtility.Calculate6x8RuleTileIndex(tile => (tile as ISealike) != null && (tile as ISealike).IsLikeSea, Map, Pos);
                return "Sea_" + index.ToString();
            }
        }
        public override string SpriteKey => typeof(SeaFishery).Name;


        private Type fishType;
        private Type GetMeatType() {
            return typeof(FishFleshSupply);
            // return (Map as StandardMap).TemporatureTypes[Pos.x, Pos.y] == typeof(TemporatureTemporate) ? typeof(RabbitMeatSupply) : typeof(DearMeatSupply);
        }
        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();

            fishType = GetMeatType();
            Res = Refs.Create(fishType);
            Res.Type = fishType;
            Res.BaseValue = 1;
            Res.Value = Res.BaseValue;
        }

        public IRef Res { get; private set; }

        public override void OnEnable() {
            base.OnEnable();

            if (fishType == null) fishType = GetMeatType();
            Res = Refs.Get(fishType);
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            LinkUtility.AddButtons(items, this);

            UI.Ins.ShowItems(Localization.Ins.Get<SeaFishery>(), items);

            if (Res.Value == Res.BaseValue) {
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }
        }

        public void Provide(List<IRef> refs) {
            refs.Add(Res);
        }
    }
}

