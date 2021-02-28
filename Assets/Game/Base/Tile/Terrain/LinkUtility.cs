
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    public interface ILinkable
    {
        IRef Res { get; }
        void OnLink(Type direction);
    }

    public interface ILinkableConsumer : ILinkable
    {
        (Type, long) CanConsume { get; } // -1 for infinity
    }

    public interface ILinkableProvider : ILinkable
    {
        (Type, long) CanProvide { get; }
    }

    public interface ILeft { }
    public interface IRight { }
    public interface IUp { }
    public interface IDown { }

    public static class LinkUtility
    {
        public static void CreateButtons(List<IUIItem> items, ITile tile, IRef res) {
            CreateDescription(items, res);
            CreateLinkButtons(items, tile, res);
            CreateUnlinkButtons(items, tile, res);
        }
        public static void CreateDescription(List<IUIItem> items, IRef res) {
            if (res.Type != null && res.Value != 0) {
                items.Add(UIItem.CreateText($"【此处】资源：{Localization.Ins.Val(res.Type, res.Value)}"));
            } else {
                items.Add(UIItem.CreateText($"【此处】资源：无"));
            }
        }
        public static void CreateLinkButtons(List<IUIItem> items, ITile tile, IRef consumerRes) {
            IRefs refs = tile.Refs;
            if (refs == null) throw new Exception();
            if (!refs.Has<IDown>()) {
                TryCreateLinkButton(items, tile, consumerRes, "获取南方", Vector2Int.up, typeof(IUp), typeof(IDown));
            }
            if (!refs.Has<IUp>()) {
                TryCreateLinkButton(items, tile, consumerRes, "获取北方", Vector2Int.down, typeof(IDown), typeof(IUp));
            }
            if (!refs.Has<IRight>()) {
                TryCreateLinkButton(items, tile, consumerRes, "获取东方", Vector2Int.left, typeof(ILeft), typeof(IRight));
            }
            if (!refs.Has<ILeft>()) {
                TryCreateLinkButton(items, tile, consumerRes, "获取西方", Vector2Int.right, typeof(IRight), typeof(ILeft));
            }
        }

        private static void TryCreateLinkButton(List<IUIItem> items, ITile tileConsumer, IRef consumerRes, string text,
            Vector2Int direction, Type providerLinkType, Type consumerLinkType) {

            ITile tileProvider = tileConsumer.GetMap().Get(tileConsumer.GetPos() - direction);
            ILinkable linkableProvider = (tileProvider as ILinkable);
            if (linkableProvider == null) return;
            IRef providerRes = linkableProvider.Res;
            if (providerRes == null) return;
            if (providerRes.Type == null) return;
            if (providerRes.Value == 0) return;
            if (consumerRes.Type != null) {
                if (!Tag.HasTag(providerRes.Type, consumerRes.Type)) {
                    // Debug.LogWarning($"{providerRes.Type} 没有 {consumerRes.Type}");
                    return;
                }
            }
            items.Add(UIItem.CreateButton($"{text}{Localization.Ins.Val(providerRes.Type, providerRes.Value)}", () => {

                BuildLink(tileProvider, tileConsumer, providerRes, consumerRes, providerLinkType, consumerLinkType);

                tileConsumer.OnTap();
            }));
        }
        public static void BuildLink(ITile tileProvider, ITile tileConsumer, IRef providerRes, IRef consumerRes, Type providerLinkType, Type consumerLinkType) {
            BuildLinkTransformed(tileProvider, tileConsumer, providerRes, consumerRes, providerLinkType, consumerLinkType, 
                providerRes.Type, 
                providerRes.Value);
        }

        public static void BuildLinkTransformed(ITile tileProvider, ITile tileConsumer, IRef providerRes, IRef consumerRes, 
            Type providerLinkType, Type consumerLinkType, Type consumerAcceptedType, long consumerAccptedQuantity) {
            if (!Tag.HasTag(providerRes.Type, consumerAcceptedType)) throw new Exception($"不能建立这两种类型的连接 {providerRes.Type} {consumerAcceptedType}");

            IRef providerLink = tileProvider.Refs.Create(providerLinkType);
            IRef consumerLink = tileConsumer.Refs.Create(consumerLinkType);

            // 没错，下面四行没写错
            providerLink.Type = providerRes.Type;
            providerLink.Value = -consumerAccptedQuantity;
            consumerLink.Type = consumerAcceptedType;
            consumerLink.Value = consumerAccptedQuantity;

            if (consumerRes.Type == null) consumerRes.Type = consumerAcceptedType;
            consumerRes.Value += providerRes.Value; // 合并
            providerRes.Value -= consumerAccptedQuantity;
            if (providerRes.Value < 0) throw new Exception();

            tileConsumer.NeedUpdateSpriteKeys = true;
            tileProvider.NeedUpdateSpriteKeys = true;

            (tileProvider as ILinkable)?.OnLink(providerLinkType);
            (tileConsumer as ILinkable)?.OnLink(consumerLinkType);
        }

        public static void CreateUnlinkButtons(List<IUIItem> items, ITile tile, IRef consumerRes) {
            IRefs refs = tile.Refs;
            if (refs == null) throw new Exception();

            if (refs.Has<IUp>() && refs.Get<IUp>().Value > 0) {
                TryCreateUnlinkButton(items, tile, consumerRes, "还给北方", Vector2Int.down, typeof(IDown), typeof(IUp));
            }
            if (refs.Has<IDown>() && refs.Get<IDown>().Value > 0) {
                TryCreateUnlinkButton(items, tile, consumerRes, "还给南方", Vector2Int.up, typeof(IUp), typeof(IDown));
            }
            if (refs.Has<ILeft>() && refs.Get<ILeft>().Value > 0) {
                TryCreateUnlinkButton(items, tile, consumerRes, "还给西方", Vector2Int.right, typeof(IRight), typeof(ILeft));
            }
            if (refs.Has<IRight>() && refs.Get<IRight>().Value > 0) {
                TryCreateUnlinkButton(items, tile, consumerRes, "还给东方", Vector2Int.left, typeof(ILeft), typeof(IRight));
            }
        }

        private static void TryCreateUnlinkButton(List<IUIItem> items, ITile tileConsumer, IRef consumerRes, string text, Vector2Int direction, Type providerLinkType, Type consumerLinkType) {
            ITile tileProvider = tileConsumer.GetMap().Get(tileConsumer.GetPos() - direction);
            ILinkable linkableProvider = (tileProvider as ILinkable);
            if (linkableProvider == null) throw new Exception();
            IRef providerRes = linkableProvider.Res;
            if (providerRes == null) throw new Exception();
            IRef linkConsumer = tileConsumer.Refs.Get(consumerLinkType);
            IRef linkProvider = tileProvider.Refs.Get(providerLinkType);
            if (consumerRes.Value < linkConsumer.Value) { return; }
            items.Add(UIItem.CreateButton($"{text}{Localization.Ins.Val(linkConsumer.Type, linkConsumer.Value)}", () => {
                consumerRes.Value -= linkConsumer.Value;
                if (consumerRes.Value == 0) consumerRes.Type = null;
                providerRes.Value += linkConsumer.Value;

                tileConsumer.Refs.Remove(consumerLinkType);
                tileProvider.Refs.Remove(providerLinkType);

                tileConsumer.NeedUpdateSpriteKeys = true;
                tileProvider.NeedUpdateSpriteKeys = true;
                (tileConsumer as ILinkable)?.OnLink(consumerLinkType);
                linkableProvider.OnLink(providerLinkType);
                tileConsumer.OnTap();
            }));
        }

        public static bool HasLink(ITile thisTile, Vector2Int direction) {
            if (direction == Vector2Int.up) {
                return thisTile.Refs.Has<IUp>();
            } else if (direction == Vector2Int.down) {
                return thisTile.Refs.Has<IDown>();
            } else if (direction == Vector2Int.left) {
                return thisTile.Refs.Has<ILeft>();
            } else if (direction == Vector2Int.right) {
                return thisTile.Refs.Has<IRight>();
            } else {
                throw new Exception($"{direction}");
            }
        }

        public static bool HasAnyLink(ITile thisTile) {
            return thisTile.Refs.Has<IUp>()
                || thisTile.Refs.Has<IDown>()
                || thisTile.Refs.Has<ILeft>()
                || thisTile.Refs.Has<IRight>();
        }

        public static bool HasAnyOutputLink(ITile thisTile) {
            return (thisTile.Refs.Has<IUp>() && thisTile.Refs.Get<IUp>().Value > 0)
                || thisTile.Refs.Has<IDown>() && thisTile.Refs.Get<IDown>().Value > 0
                || thisTile.Refs.Has<ILeft>() && thisTile.Refs.Get<ILeft>().Value > 0
                || thisTile.Refs.Has<IRight>() && thisTile.Refs.Get<IRight>().Value > 0;
        }

        public static bool HasAnyInputLink(ITile thisTile) {
            return (thisTile.Refs.Has<IUp>() && thisTile.Refs.Get<IUp>().Value < 0)
                || thisTile.Refs.Has<IDown>() && thisTile.Refs.Get<IDown>().Value < 0
                || thisTile.Refs.Has<ILeft>() && thisTile.Refs.Get<ILeft>().Value < 0
                || thisTile.Refs.Has<IRight>() && thisTile.Refs.Get<IRight>().Value < 0;
        }
        // -----------

        public static UIItem CreateDestructionButton(ITile thisTile, IRef res) {
            return (res.Value == res.BaseValue && !HasAnyLink(thisTile)) ? UIItem.CreateDestructButton<TerrainDefault>(thisTile) : null;
        }
    }
}

