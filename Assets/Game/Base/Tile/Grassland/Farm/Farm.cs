
using System;
using System.Collections.Generic;

namespace Weathering
{
    [Concept]
    public class FarmTech { }


    [Concept]
    public class Farm : StandardTile, ILinkProvider
    {
        public override string SpriteKey {
            get {
                return popValue.Max == 0 ? "FarmGrowing" : "FarmRipe";
            }
        }

        private IValue popValue;
        private IRef foodRef;

        public void Provide(List<IRef> refs) {
            refs.Add(foodRef);
        }

        public override void OnConstruct() {
            Values = Weathering.Values.GetOne();
            popValue = Values.Create<Farm>();

            Refs = Weathering.Refs.GetOne();
            foodRef = Refs.Create<Farm>();
            foodRef.Type = typeof(FoodSupply);
        }

        public override void OnEnable() {
            base.OnEnable();
            popValue = Values.Get<Farm>();
            foodRef = Refs.Get<Farm>();
        }

        public const long FarmCost = 1;
        public const long FarmRevenue = 8;
        public override void OnTap() {

            var items = new List<IUIItem>() { };

            items.Add(UIItem.CreateText($"工作人员{Localization.Ins.Val<Worker>(popValue.Max)}"));

            items.Add(UIItem.CreateButton("派遣居民种田", () => {
                if (Map.Inventory.CanRemove<Worker>() >= FarmCost) {
                    Map.Inventory.Remove<Worker>(FarmCost);
                    foodRef.Value += FarmRevenue;
                    popValue.Max += FarmCost;
                    NeedUpdateSpriteKeys = true;
                }
                OnTap();
            }, () => popValue.Max == 0 && Map.Inventory.Get<Worker>() >= FarmCost));

            items.Add(UIItem.CreateButton("取消居民种田", () => {
                if (Map.Inventory.CanAdd<Worker>() >= FarmCost) {
                    Map.Inventory.Add<Worker>(FarmCost);
                    foodRef.Value -= FarmRevenue;
                    popValue.Max -= FarmCost;
                    NeedUpdateSpriteKeys = true;
                }
                OnTap();
            }, () => popValue.Max == FarmCost && foodRef.Value == FarmRevenue));

            items.Add(UIItem.CreateSeparator());
            LinkUtility.AddButtons(items, this);

            if (popValue.Max == 0) {
                items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));
            }

            UI.Ins.ShowItems(Localization.Ins.Get<Farm>(), items);
        }


    }
}

