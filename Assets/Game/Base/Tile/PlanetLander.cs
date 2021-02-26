
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CharacterLanded { }

    public interface ILandable
    {
        bool Landable { get; }
        void Land(Vector2Int pos);
        void Leave();
    }

    public class GlobalProgress { }

    public class PlanetLander : StandardTile, IStepOn
    {
        public override string SpriteKeyOverlay => typeof(PlanetLander).Name;
        public override bool HasSpriteDirection => true;
        public override string SpriteLeft => Refs.Has<IConsumerLeft>() ? typeof(Food).Name : null;
        public override string SpriteRight => Refs.Has<IConsumerRight>() ? typeof(Food).Name : null;
        public override string SpriteUp => Refs.Has<IConsumerUp>() ? typeof(Food).Name : null;
        public override string SpriteDown => Refs.Has<IConsumerDown>() ? typeof(Food).Name : null;

        public void OnStepOn() {
            ILandable landable = Map as ILandable;
            UI.Ins.ShowItems("是否乘坐火箭进入行星轨道",
                UIItem.CreateButton("开启火箭", () => {
                    Map.UpdateAt<TerrainDefault>(Pos);
                    UI.Ins.Active = false;
                    landable.Leave();
                }),
                UIItem.CreateButton("暂不开启", () => {
                    UI.Ins.Active = false;
                })
            );
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            Refs.Create<PlanetLander>();
        }

        private IRef res;
        public override void OnEnable() {
            base.OnEnable();
            res = Refs.Get<PlanetLander>();
        }

        public override void OnTap() {
            // MainQuest.Ins.OnTap();
            var items = UI.Ins.GetItems();

            LinkUtility.CreateDescription(items, res);
            LinkUtility.CreateButtons(items, this, res);

            UI.Ins.ShowItems("火箭", items);
        }
    }
}

