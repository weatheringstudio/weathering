
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept("传送点")]
    public class Teleport : StandardTile
    {
        public override string SpriteKey => "MeetingStone";

        public static string teleport;
        public override void OnEnable() {
            base.OnEnable();
            teleport = Concept.Ins.ColoredNameOf<Teleport>();
            if (Refs == null) {
                Refs = Weathering.Refs.GetOne();
            }
        }

        public override void OnConstruct() {
        }

        public override void OnDestruct() {
        }

        public override void OnTap() {
            var items = new List<IUIItem>();

            Type targetMap = Refs.Get<Teleport>().Type;

            items.Add(new UIItem {
                Type = IUIItemType.Button,
                Content = $"是否传送到 {Concept.Ins.ColoredNameOf<IslandMap>()}",
                OnTap = () => {
                    GameEntry.Ins.EnterMap(targetMap);
                    UI.Ins.Active = false;
                }
            });

            UI.Ins.ShowItems(teleport, items);
        }

        public Type TargetMap {
            set {
                if (Refs.Has<Teleport>()) {
                    throw new Exception();
                }
                Refs.Create<Teleport>().Type = value;
            }
        }
    }
}

