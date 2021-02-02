
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    [Concept]
    public class GrasslandRoad : StandardTile, IRoadLike
    {
        public override string SpriteKey {
            get {
                int index = TileUtility.Calculate4x4RuleTileIndex(tile => tile.GetType() == typeof(GrasslandRoad), Map, Pos);
                return $"StoneRoad_{index}";
            }
        }

        public bool IsRoadLike() {
            return true;
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(UIItem.CreateMultilineText("【在目前游戏版本中，道路暂时没有作用。在以后的版本中，建筑需要贴近道路才能自动化，进行物流】"));

            //items.Add(UIItem.CreateButton(Localization.Ins.Get<Destruct>(), () => {
            //    ITile depender = Road.FindDepender(this);
            //    ITile dependee = Road.FindDependee(this);
            //    if (depender != null) {
            //        if (dependee == null) {
            //            // 自己是第一块路
            //            UIPreset.Notify(OnTap, "旁边的道路需要这个道路，不能拆除");
            //        } else {
            //            // 自己不是第一块路
            //            UIPreset.Notify(OnTap, "旁边的道路需要这个道路，不能拆除");
            //        }
            //        return;
            //    }
            //    else {

            //    }
            //}));

            //items.Add(UIItem.CreateText($"左边的道路需要这个道路吗： {Refs.Has<IRoadLeftDepender>()}"));
            //items.Add(UIItem.CreateText($"右边的道路需要这个道路吗： {Refs.Has<IRoadRightDepender>()}"));
            //items.Add(UIItem.CreateText($"上边的道路需要这个道路吗： {Refs.Has<IRoadUpDepender>()}"));
            //items.Add(UIItem.CreateText($"下边的道路需要这个道路吗： {Refs.Has<IRoadDownDepender>()}"));

            items.Add(UIItem.CreateDestructButton<Grassland>(this));

            UI.Ins.ShowItems(Localization.Ins.Get<GrasslandRoad>(), items);
        }
    }
}

