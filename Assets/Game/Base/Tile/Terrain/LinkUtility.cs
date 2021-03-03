
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public interface ILeft { }
    public interface IRight { }
    public interface IUp { }
    public interface IDown { }

    public interface ILinkConsumer
    {
        void Consume((Type, long) pair);
        void CanConsume(List<(Type, long)> pairs);
    }

    public interface ILinkProvider
    {
        void Provide((Type, long) pair);
        void CanProvide(List<(Type, long)> pairs);
    }

    public static class LinkUtility
    {
        public static UIItem CreateTextForPair((Type, long) pair) {
            return UIItem.CreateText($"{Localization.Ins.Val(pair.Item1, pair.Item2)}");
        }
        private readonly static List<(Type, long)> consumerPairsBuffer = new List<(Type, long)>();
        public static void CreateConsumerButtons(List<IUIItem> items, ITile tile) {
            consumerPairsBuffer.Clear();
            ILinkConsumer consumer = tile as ILinkConsumer;
            if (consumer == null) throw new Exception();
            consumer.CanConsume(consumerPairsBuffer);
            if (consumerPairsBuffer.Count == 0) return;
            if (consumerPairsBuffer.Count > 8) throw new Exception();

            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            ILinkProvider up = map.Get(pos + Vector2Int.up) as ILinkProvider;
            TryCreateConsumerButtonsForTile(items, up);
            ILinkProvider down = map.Get(pos + Vector2Int.up) as ILinkProvider;
            TryCreateConsumerButtonsForTile(items, down);
            ILinkProvider left = map.Get(pos + Vector2Int.up) as ILinkProvider;
            TryCreateConsumerButtonsForTile(items, left);
            ILinkProvider right = map.Get(pos + Vector2Int.up) as ILinkProvider;
            TryCreateConsumerButtonsForTile(items, right);

            consumerPairsBuffer.Clear();
        }
        private readonly static List<(Type, long)> providerPairsBuffer = new List<(Type, long)>();
        private static void TryCreateConsumerButtonsForTile(List<IUIItem> items, ILinkProvider provider) {
            if (provider == null) return;
            if (providerPairsBuffer.Count != 0) throw new Exception();
            provider.CanProvide(providerPairsBuffer);
            if (providerPairsBuffer.Count > 8) throw new Exception();

            foreach (var consumerItem in consumerPairsBuffer) {
                foreach (var providerItem in providerPairsBuffer) {
                    if (Tag.HasTag(providerItem.Item1, consumerItem.Item1)) {
                        // 如果提供的东西是需求的东西的子类，那么创建按钮。按下按钮则建立连接
                        items.Add(UIItem.CreateButton("获取来自xx方向的xx", () => {
                            // TODO
                        }));
                    }
                }
            }
        }
    }

    //public interface ILinkable
    //{
    //    IRef Res { get; }
    //    void OnLink(Type direction);
    //}

    //public interface ILinkableConsumer : ILinkable
    //{
    //    (Type, long) CanConsume { get; } // long.MaxValue for infinity
    //}

    //public interface ILinkableProvider : ILinkable
    //{
    //    (Type, long) CanProvide { get; }
    //}


    //public static class LinkUtility
    //{
    //    public static void CreateButtons(List<IUIItem> items, ITile tile) {
    //        CreateDescription(items, tile);
    //        CreateLinkButtons(items, tile);
    //        CreateUnlinkButtons(items, tile);
    //    }
    //    public static void CreateDescription(List<IUIItem> items, ITile tile) {
    //        IRef res = (tile as ILinkable)?.Res;
    //        if (res == null) throw new Exception();
    //        if (res.Type != null && res.Value != 0) {
    //            items.Add(UIItem.CreateText($"【此处】资源：{Localization.Ins.Val(res.Type, res.Value)}"));
    //        } else {
    //            items.Add(UIItem.CreateText($"【此处】资源：无"));
    //        }
    //    }
    //    public static void CreateLinkButtons(List<IUIItem> items, ITile tile) {
    //        IRefs refs = tile.Refs;
    //        if (refs == null) throw new Exception();
    //        IMap map = tile.GetMap();
    //        Vector2Int pos = tile.GetPos();
    //        if (!refs.Has<IUp>()) {
    //            TryCreateLinkButton(items, "获取北方", tile, map.Get(pos + Vector2Int.up), typeof(IDown), typeof(IUp));
    //        }
    //        if (!refs.Has<IDown>()) {
    //            TryCreateLinkButton(items, "获取南方", tile, map.Get(pos + Vector2Int.down), typeof(IUp), typeof(IDown));
    //        }
    //        if (!refs.Has<ILeft>()) {
    //            TryCreateLinkButton(items, "获取西方", tile, map.Get(pos + Vector2Int.left), typeof(IRight), typeof(ILeft));
    //        }
    //        if (!refs.Has<IRight>()) {
    //            TryCreateLinkButton(items, "获取东方", tile, map.Get(pos + Vector2Int.right), typeof(ILeft), typeof(IRight));
    //        }
    //    }

    //    private static void TryCreateLinkButton(List<IUIItem> items, string text, ITile tileOfConsumer, ITile tileOfProvider, Type linkTypeOfProvider, Type linkTypeOfConsumer) {

    //        ILinkable linkableProvider = (tileOfProvider as ILinkable);
    //        if (linkableProvider == null) return;
    //        IRef resOfProvider = linkableProvider.Res; // 供给方资源
    //        if (resOfProvider == null) return;

    //        ILinkable linkableConsumer = (tileOfConsumer as ILinkable);
    //        if (linkableConsumer == null) return;
    //        IRef resOfConsumer = linkableConsumer.Res; // 消费方资源
    //        if (resOfConsumer == null) return;

    //        if (resOfProvider.Type == null) return; // 供给方不能提供资源
    //        if (resOfProvider.Value == 0) return;  // 供给方不能提供资源

    //        if (resOfConsumer.Type == null  // 消费方接受任意类型资源
    //            || Tag.HasTag(resOfProvider.Type, resOfConsumer.Type) // 供给方提供的资源是消费方指定类型资源的子集
    //            ) {

    //            items.Add(UIItem.CreateButton($"{text}{Localization.Ins.Val(resOfProvider.Type, resOfProvider.Value)}", () => {

    //                Link(tileOfProvider, tileOfConsumer, resOfProvider, resOfConsumer, linkTypeOfProvider, linkTypeOfConsumer);

    //                tileOfConsumer.OnTap();
    //            }));
    //        }
    //    }
    //    public static void Link(ITile tileOfProvider, ITile tileOfConsumer, IRef providerRes, IRef consumerRes, Type providerLinkType, Type consumerLinkType) {
    //        // 默认连接：供给方资源不需要转换，能给多少给多少
    //        if (providerRes.Type == null) throw new Exception();
    //        Link(tileOfProvider, tileOfConsumer, providerRes, consumerRes, providerLinkType, consumerLinkType,
    //            providerRes.Type,
    //            providerRes.Value);
    //    }

    //    public static void Link(ITile tileOfProvider, ITile tileOfConsumer, IRef resOfProvider, IRef resOfConsumer, Type linkTypeOfProvider, Type linkTypeOfConsumer,
    //        Type typeOfLink, long quantityOfLink) {

    //        if (typeOfLink == null) throw new Exception($"?{resOfProvider.Type}?");

    //        if (!Tag.HasTag(resOfProvider.Type, typeOfLink)) throw new Exception($"不能建立这两种类型的连接： {resOfProvider.Type} {typeOfLink}");

    //        // link的操作
    //        IRef linkOfProvider = tileOfProvider.Refs.GetOrCreate(linkTypeOfProvider);
    //        IRef linkOfConsumer = tileOfConsumer.Refs.GetOrCreate(linkTypeOfConsumer);
    //        linkOfProvider.Type = typeOfLink;
    //        linkOfConsumer.Type = typeOfLink;
    //        linkOfProvider.Value = -quantityOfLink;
    //        linkOfConsumer.Value = quantityOfLink;

    //        // res的操作
    //        if (resOfConsumer.Type == null) resOfConsumer.Type = typeOfLink;
    //        resOfConsumer.Value += quantityOfLink; // 输入为正
    //        resOfProvider.Value -= quantityOfLink; // 输出为负
    //        if (resOfProvider.Value < 0) throw new Exception(); // 非负校验

    //        // 图像更新
    //        tileOfConsumer.NeedUpdateSpriteKeys = true;
    //        tileOfProvider.NeedUpdateSpriteKeys = true;

    //        // 连接事件
    //        (tileOfProvider as ILinkable)?.OnLink(linkTypeOfProvider);
    //        (tileOfConsumer as ILinkable)?.OnLink(linkTypeOfConsumer);
    //    }

    //    public static void CreateUnlinkButtons(List<IUIItem> items, ITile tile) {
    //        IRefs refs = tile.Refs;
    //        if (refs == null) throw new Exception();
    //        IMap map = tile.GetMap();
    //        Vector2Int pos = tile.GetPos();
    //        if (refs.Has<IUp>() && refs.Get<IUp>().Value > 0) {
    //            TryCreateUnlinkButton(items, "还给北方", tile, map.Get(pos + Vector2Int.up), typeof(IDown), typeof(IUp));
    //        }
    //        if (refs.Has<IDown>() && refs.Get<IDown>().Value > 0) {
    //            TryCreateUnlinkButton(items, "还给南方", tile, map.Get(pos + Vector2Int.down), typeof(IUp), typeof(IDown));
    //        }
    //        if (refs.Has<ILeft>() && refs.Get<ILeft>().Value > 0) {
    //            TryCreateUnlinkButton(items, "还给西方", tile, map.Get(pos + Vector2Int.left), typeof(IRight), typeof(ILeft));
    //        }
    //        if (refs.Has<IRight>() && refs.Get<IRight>().Value > 0) {
    //            TryCreateUnlinkButton(items, "还给东方", tile, map.Get(pos + Vector2Int.right), typeof(ILeft), typeof(IRight));
    //        }
    //    }

    //    private static void TryCreateUnlinkButton(List<IUIItem> items, string text, ITile tileOfConsumer, ITile tileOfProvider, Type linkTypeOfProvider, Type linkTypeOfConsumer) {

    //        ILinkable linkableProvider = (tileOfProvider as ILinkable);
    //        if (linkableProvider == null) throw new Exception();
    //        IRef resOfProvider = linkableProvider.Res; // 供给方资源
    //        if (resOfProvider == null) throw new Exception();

    //        ILinkable linkableConsumer = (tileOfConsumer as ILinkable);
    //        if (linkableConsumer == null) throw new Exception();
    //        IRef resOfConsumer = linkableConsumer.Res; // 消费方资源
    //        if (resOfConsumer == null) throw new Exception();

    //        IRef linkOfProvider = tileOfProvider.Refs.Get(linkTypeOfProvider);
    //        IRef linkOfConsumer = tileOfConsumer.Refs.Get(linkTypeOfConsumer);

    //        if (resOfConsumer.Value < linkOfConsumer.Value) { return; } // 自身的资源不够解除连接 

    //        items.Add(UIItem.CreateButton($"{text}{Localization.Ins.Val(linkOfConsumer.Type, linkOfConsumer.Value)}", () => {
    //            Unlink(tileOfProvider, tileOfConsumer, resOfProvider, resOfConsumer, linkTypeOfProvider, linkTypeOfConsumer,
    //                linkOfProvider.Type, linkOfConsumer.Value);
    //            tileOfConsumer.OnTap();
    //        }));
    //    }

    //    public static void Unlink(ITile tileProvider, ITile tileConsumer, IRef resOfProvider, IRef resOfConsumer, Type linkTypeOfProvider, Type linkTypeOfConsumer,
    //        Type typeOfLink, long quantityOfLink) {

    //        if (!Tag.HasTag(resOfProvider.Type, typeOfLink)) throw new Exception($"不能建立这两种类型的连接： {resOfProvider.Type} {typeOfLink}");

    //        // link的操作
    //        IRef providerLink = tileProvider.Refs.Get(linkTypeOfProvider);
    //        IRef consumerLink = tileConsumer.Refs.Get(linkTypeOfConsumer);
    //        providerLink.Value += quantityOfLink;
    //        consumerLink.Value -= quantityOfLink;
    //        if (consumerLink.Value < 0) throw new Exception(); // 非负校验
    //        if (consumerLink.Value == 0) {
    //            tileConsumer.Refs.Remove(linkTypeOfConsumer);
    //            tileProvider.Refs.Remove(linkTypeOfProvider);
    //        }

    //        resOfProvider.Value += quantityOfLink;
    //        resOfConsumer.Value -= quantityOfLink;
    //        if (resOfConsumer.Value < 0) throw new Exception(); // 非负校验

    //        // 图像更新
    //        tileConsumer.NeedUpdateSpriteKeys = true;
    //        tileProvider.NeedUpdateSpriteKeys = true;

    //        // 连接事件
    //        (tileProvider as ILinkable)?.OnLink(linkTypeOfProvider);
    //        (tileConsumer as ILinkable)?.OnLink(linkTypeOfConsumer);
    //    }

    //    public static bool HasLink(ITile thisTile, Vector2Int direction) {
    //        if (direction == Vector2Int.up) {
    //            return thisTile.Refs.Has<IUp>();
    //        } else if (direction == Vector2Int.down) {
    //            return thisTile.Refs.Has<IDown>();
    //        } else if (direction == Vector2Int.left) {
    //            return thisTile.Refs.Has<ILeft>();
    //        } else if (direction == Vector2Int.right) {
    //            return thisTile.Refs.Has<IRight>();
    //        } else {
    //            throw new Exception($"{direction}");
    //        }
    //    }

    //    public static bool HasAnyLink(ITile thisTile) {
    //        return thisTile.Refs.Has<IUp>()
    //            || thisTile.Refs.Has<IDown>()
    //            || thisTile.Refs.Has<ILeft>()
    //            || thisTile.Refs.Has<IRight>();
    //    }

    //    public static bool HasAnyOutputLink(ITile thisTile) {
    //        return (thisTile.Refs.Has<IUp>() && thisTile.Refs.Get<IUp>().Value > 0)
    //            || thisTile.Refs.Has<IDown>() && thisTile.Refs.Get<IDown>().Value > 0
    //            || thisTile.Refs.Has<ILeft>() && thisTile.Refs.Get<ILeft>().Value > 0
    //            || thisTile.Refs.Has<IRight>() && thisTile.Refs.Get<IRight>().Value > 0;
    //    }

    //    public static bool HasAnyInputLink(ITile thisTile) {
    //        return (thisTile.Refs.Has<IUp>() && thisTile.Refs.Get<IUp>().Value < 0)
    //            || thisTile.Refs.Has<IDown>() && thisTile.Refs.Get<IDown>().Value < 0
    //            || thisTile.Refs.Has<ILeft>() && thisTile.Refs.Get<ILeft>().Value < 0
    //            || thisTile.Refs.Has<IRight>() && thisTile.Refs.Get<IRight>().Value < 0;
    //    }
    //    // -----------

    //    public static UIItem CreateDestructionButton(ITile thisTile, IRef res) {
    //        return (res.Value == res.BaseValue && !HasAnyLink(thisTile)) ? UIItem.CreateDestructButton<TerrainDefault>(thisTile) : null;
    //    }


    //    private const string INPUT = "输入";
    //    private const string OUTPUT = "输出";
    //    private const string NORTH = "【北方】";
    //    private const string SOUTH = "【南方】";
    //    private const string WEST = "【西方】";
    //    private const string EAST = "【东方】";
    //    public static void CreateLinkInfo(List<IUIItem> items, IRefs refs) {
    //        CreateOnTapRoadInfo(items, typeof(IUp), NORTH, refs);
    //        CreateOnTapRoadInfo(items, typeof(IDown), SOUTH, refs);
    //        CreateOnTapRoadInfo(items, typeof(ILeft), WEST, refs);
    //        CreateOnTapRoadInfo(items, typeof(IRight), EAST, refs);
    //    }
    //    private static void CreateOnTapRoadInfo(List<IUIItem> items, Type type, string directionText, IRefs refs) {
    //        if (refs.Has(type)) {
    //            IRef link = refs.Get(type);
    //            items.Add(UIItem.CreateText($"{directionText}{(link.Value > 0 ? INPUT : OUTPUT)}{Localization.Ins.Val(link.Type, Math.Abs(link.Value))}"));
    //        }
    //    }
    //}
}

