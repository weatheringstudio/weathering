
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class UrbanArea : StandardTile
    {
        public override string SpriteKey => typeof(UrbanArea).Name;

        public override void OnTap() {

            var items = new List<IUIItem>();

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"{Localization.Ins.Get<Destruct>()}{Localization.Ins.Get<UrbanArea>()}",
                OnTap = () => {
                    Map.UpdateAt<Grassland>(Pos);
                    UI.Ins.Active = false;
                }
            });

            UI.Ins.ShowItems(Localization.Ins.Get<UrbanArea>(), items);
        }
    }
}

