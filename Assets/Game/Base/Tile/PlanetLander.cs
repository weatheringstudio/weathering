
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

    public class PlanetLander : StandardTile, IStepOn, ILinkable
    {
        public override string SpriteKeyOverlay => typeof(PlanetLander).Name;
        public override bool HasDynamicSpriteAnimation => true;
        public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? typeof(Food).Name : null;
        public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? typeof(Food).Name : null;

        public void OnLink(Type direction) {
            questProgress.Inc = res.Value;
        }
        public IRef Res => null; // 无法作为输入

        public void OnStepOn() {
            ILandable landable = Map as ILandable;
            if (landable == null) throw new Exception();
            if (res.Value == 0) {
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
        }

        public override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();
            res = Refs.Create<PlanetLander>();
        }

        private IValue questProgress;
        private IRef questProgressRef;
        private IRef res;
        public override void OnEnable() {
            base.OnEnable();
            res = Refs.Get<PlanetLander>();
            questProgress = Globals.Ins.Values.GetOrCreate<QuestProgress>();
            questProgressRef = Globals.Ins.Refs.GetOrCreate<QuestProgress>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateText($"当前任务 {MainQuest.Ins.CurrentQuest}"));

            if (questProgressRef.Type != null) {
                items.Add(UIItem.CreateValueProgress(questProgressRef.Type, questProgress));
                items.Add(UIItem.CreateTimeProgress(questProgressRef.Type, questProgress));

                ConceptSupply conceptSupply = Attribute.GetCustomAttribute(questProgressRef.Type, typeof(ConceptSupply)) as ConceptSupply;
                if (conceptSupply == null) throw new Exception($"{questProgressRef.Type} 没有定义 ConceptSupply");
                res.Type = conceptSupply.TheType;
                LinkUtility.CreateButtons(items, this, res);
            }

            UI.Ins.ShowItems("火箭", items);
        }
    }
}

