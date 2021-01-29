
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public class Teleport : StandardTile
    {
        public override string SpriteKey => targetMap.Name;

        public static string teleport;
        public override void OnEnable() {
            base.OnEnable();
            teleport = Localization.Ins.Get<Teleport>();
            targetMap = Refs.GetOrCreate<Teleport>().Type;
        }

        public override void OnConstruct() {
            Refs = Weathering.Refs.GetOne();
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"是否传送到 {Localization.Ins.Get(targetMap)}",
                OnTap = () => {
                    GameMenu.Entry.EnterMap(targetMap);
                    UI.Ins.Active = false;
                }
            });

            UI.Ins.ShowItems(teleport, items);
        }

        private Type targetMap;
        public Type TargetMap {
            set {
                if (targetMap != null) {
                    throw new Exception();
                }
                if (value == null) throw new Exception();
                Refs.Get<Teleport>().Type = value;
                targetMap = value;
            }
        }
    }
}

