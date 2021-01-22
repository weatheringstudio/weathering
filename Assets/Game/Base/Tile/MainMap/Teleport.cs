
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("传送点")]
    public class Teleport : StandardTile
    {
        public override string SpriteKey => targetMap.Name;

        public static string teleport;
        public override void OnEnable() {
            base.OnEnable();
            teleport = Concept.Ins.ColoredNameOf<Teleport>();
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
                Content = $"是否传送到 {Concept.Ins.ColoredNameOf(targetMap)}",
                OnTap = () => {
                    GameEntry.Ins.EnterMap(targetMap);
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

