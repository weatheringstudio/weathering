
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public class CharacterLanded { }

    public interface ILandable
    {
        bool Landed { get; }
        void Land(Vector2Int pos);
        void Leave(Vector2Int pos);
    }

    public class PlanetLanderRes { }

    public class PlanetLander : StandardTile, IStepOn, IIgnoreTool, IPassable
    {
        public bool Passable => true;

        public override string SpriteKey => typeof(PlanetLander).Name;
        public override string SpriteKeyHighLight => GlobalLight.Decorated(SpriteKey);
        //public override bool HasDynamicSpriteAnimation => true;
        //public override string SpriteLeft => Refs.Has<IRight>() && Refs.Get<IRight>().Value > 0 ? ConceptResource.Get(TypeOfResource.Type).Name : null;
        //public override string SpriteRight => Refs.Has<ILeft>() && Refs.Get<ILeft>().Value > 0 ? ConceptResource.Get(TypeOfResource.Type).Name : null;
        //public override string SpriteUp => Refs.Has<IDown>() && Refs.Get<IDown>().Value > 0 ? ConceptResource.Get(TypeOfResource.Type).Name : null;
        //public override string SpriteDown => Refs.Has<IUp>() && Refs.Get<IUp>().Value > 0 ? ConceptResource.Get(TypeOfResource.Type).Name : null;

        public IRef Res { get; private set; }

        public bool IgnoreTool => true;

        public void OnStepOn() {
            // Res是以前火箭接入物流时用的
            if (Res.Value == 0 && Globals.Unlocked<KnowledgeOfPlanetLander>()) {
                LeavePlanet();
            }
        }

        public void LeavePlanet() {
            ILandable landable = Map as ILandable;
            if (landable == null) throw new Exception();

            UI.Ins.Active = false;
            landable.Leave(Pos);
        }

        public static PlanetLander Ins { get; private set; }
        public override void OnConstruct(ITile tile) {

            base.OnConstruct(tile);

            if (Refs == null) {
                Refs = Weathering.Refs.GetOne();
            }
            Res = Refs.Create<PlanetLanderRes>();
        }

        public override void OnDestruct(ITile newTile) {
            base.OnDestruct(newTile);
            Ins = null;
        }

        //private IValue ValueOfResource;
        //private IRef TypeOfResource;

        //private IValue ValueOfRequirement;
        //private IRef TypeOfRequirement;

        public override void OnEnable() {
            base.OnEnable();

            if (Ins != null) throw new Exception();
            Ins = this;

            Res = Refs.Get<PlanetLanderRes>();
            //ValueOfResource = Globals.Ins.Values.GetOrCreate<QuestResource>();
            //TypeOfResource = Globals.Ins.Refs.GetOrCreate<QuestResource>();
            //ValueOfRequirement = Globals.Ins.Values.GetOrCreate<QuestRequirement>();
            //TypeOfRequirement = Globals.Ins.Refs.GetOrCreate<QuestRequirement>();
        }

        public override void OnTap() {
            var items = UI.Ins.GetItems();

            ILandable landable = Map as ILandable;
            if (landable == null) throw new Exception();

            if (Globals.Unlocked<KnowledgeOfPlanetLander>()) {
                if (Res.Value == 0) {
                    items.Add(UIItem.CreateButton("开启飞船", () => {
                        LeavePlanet();
                    }));
                    items.Add(UIItem.CreateButton("暂不开启", () => {
                        UI.Ins.Active = false;
                    }));
                }
            }
            else {
                items.Add(UIItem.CreateMultilineText($"{Localization.Ins.Get<PlanetLander>()}已经坏了，需要研究{Localization.Ins.Get<KnowledgeOfPlanetLander>()}"));
            }

            //items.Add(UIItem.CreateText($"当前任务：{Localization.Ins.Get(MainQuest.Ins.CurrentQuest)}"));

            //if (TypeOfResource.Type != null) {
            //    items.Add(UIItem.CreateButton("提交任务", () => {
            //        MainQuest.Ins.CompleteQuest(MainQuest.Ins.CurrentQuest);
            //    }, () => ValueOfResource.Maxed)); // 资源任务的提交条件：ValueOfResource.Maxed

            //    items.Add(UIItem.CreateButton($"提交任务物品{Localization.Ins.ValUnit(TypeOfResource.Type)}", () => {
            //        long quantity = Math.Min(ValueOfResource.Max - ValueOfResource.Val, Map.Inventory.GetWithTag(TypeOfResource.Type));
            //        Map.Inventory.RemoveWithTag(TypeOfResource.Type, quantity);
            //        ValueOfResource.Val += quantity;
            //    }, () => !ValueOfResource.Maxed));

            //    items.Add(UIItem.CreateValueProgress(TypeOfResource.Type, ValueOfResource));

            //    items.Add(UIItem.CreateText("背包里的相关物品"));
            //    UIItem.AddEntireInventoryContentWithTag(TypeOfResource.Type, Map.Inventory, items, OnTap);
            //}

            //if (TypeOfRequirement.Type != null) {

            //    long quantity = Map.Inventory.GetWithTag(TypeOfRequirement.Type);
            //    ValueOfRequirement.Val = quantity;

            //    items.Add(UIItem.CreateButton("提交任务", () => {
            //        MainQuest.Ins.CompleteQuest(MainQuest.Ins.CurrentQuest);
            //    }, () => quantity == ValueOfRequirement.Max));

            //    items.Add(UIItem.CreateValueProgress(TypeOfRequirement.Type, ValueOfResource));

            //    items.Add(UIItem.CreateText("背包里的相关物品"));
            //    UIItem.AddEntireInventoryContentWithTag(TypeOfRequirement.Type, Map.Inventory, items, OnTap);
            //}

            UI.Ins.ShowItems(Localization.Ins.Get<PlanetLander>(), items);
        }

        //public void Consume(List<IRef> refs) {
        //   refs.Add(questProgressRef);
        //}

        //public void OnLink(Type direction, long quantity) {
        //    questProgress.Inc += quantity;
        //    if (questProgress.Inc < 0) throw new Exception();
        //}
    }
}

