
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public static class LinkUtility
    {
        public static void CreateButtons(List<IUIItem> items, ITile tile, IRef res) {
            CreateDescription(items, res);
            CreateLinkButtons(items, tile, res);
            CreateUnlinkButtons(items, tile, res);
        }
        public static void CreateDescription(List<IUIItem> items, IRef res) {
            if (res.Type != null) {
                items.Add(UIItem.CreateText($"拥有资源：{Localization.Ins.Val(res.Type, res.Value)}"));
            } else {
                items.Add(UIItem.CreateText($"拥有资源：无"));
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
            if (consumerRes.Type != null && consumerRes.Type != providerRes.Type) return;

            items.Add(UIItem.CreateButton($"{text}{Localization.Ins.Val(providerRes.Type, providerRes.Value)}", () => {
                RegisterLink(tileProvider, tileConsumer, providerRes, providerLinkType, consumerLinkType);
                consumerRes.Type = providerRes.Type;
                consumerRes.Value += providerRes.Value; // 合并
                providerRes.Value = 0; // 怎么能全吃了
                tileConsumer.NeedUpdateSpriteKeys = true;
                tileProvider.NeedUpdateSpriteKeys = true;
                (tileConsumer as ILinkable)?.OnLink(consumerLinkType);
                linkableProvider.OnLink(providerLinkType);
                tileConsumer.OnTap();
            }));
        }

        private static void RegisterLink(ITile provider, ITile consumer, IRef providerRes, Type providerLinkType, Type consumerLinkType) {
            IRef providerLink = provider.Refs.Create(providerLinkType);
            IRef consumerLink = consumer.Refs.Create(consumerLinkType);

            providerLink.Type = providerRes.Type;
            providerLink.Value = -providerRes.Value;
            consumerLink.Type = providerRes.Type;
            consumerLink.Value = providerRes.Value;
        }

        public static void CreateUnlinkButtons(List<IUIItem> items, ITile tile, IRef consumerRes) {
            IRefs refs = tile.Refs;
            if (refs == null) throw new Exception();

            if (refs.Has<IDown>() && refs.Get<IDown>().Value > 0) {
                TryCreateUnlinkButton(items, tile, consumerRes, "还给南方", Vector2Int.up);
            }
            if (refs.Has<IUp>() && refs.Get<IUp>().Value > 0) {
                TryCreateUnlinkButton(items, tile, consumerRes, "还给北方", Vector2Int.down);
            }
            if (refs.Has<ILeft>() && refs.Get<ILeft>().Value > 0) {
                TryCreateUnlinkButton(items, tile, consumerRes, "还给东方", Vector2Int.right);
            }
            if (refs.Has<IRight>() && refs.Get<IRight>().Value > 0) {
                TryCreateUnlinkButton(items, tile, consumerRes, "还给西方", Vector2Int.left);
            }
        }

        private static void TryCreateUnlinkButton(List<IUIItem> items, ITile tileConsumer, IRef consumerRes, string text, Vector2Int direction) {
            ITile tileProvider = tileConsumer.GetMap().Get(tileConsumer.GetPos() - direction);
            ILinkable linkable = (tileProvider as ILinkable);
            if (linkable == null) throw new Exception();
            IRef providerRes = linkable.Res;
            if (providerRes == null) throw new Exception();
            IRef linkConsumer = null;
            IRef linkProvider = null;
            if (direction == Vector2Int.up) {
                linkConsumer = tileConsumer.Refs.Get<IDown>();
                linkProvider = tileProvider.Refs.Get<IUp>();
            } else if (direction == Vector2Int.down) {
                linkConsumer = tileConsumer.Refs.Get<IUp>();
                linkProvider = tileProvider.Refs.Get<IDown>();
            } else if (direction == Vector2Int.right) {
                linkConsumer = tileConsumer.Refs.Get<ILeft>();
                linkProvider = tileProvider.Refs.Get<IRight>();
            } else if (direction == Vector2Int.left) {
                linkConsumer = tileConsumer.Refs.Get<IRight>();
                linkProvider = tileProvider.Refs.Get<ILeft>();
            } else {
                throw new Exception();
            }

            if (consumerRes.Type != linkConsumer.Type) { return; }
            if (consumerRes.Value < linkConsumer.Value) { return; }
            items.Add(UIItem.CreateButton($"{text}{Localization.Ins.Val(linkConsumer.Type, linkConsumer.Value)}", () => {
                consumerRes.Value -= linkConsumer.Value;
                if (consumerRes.Value == 0) consumerRes.Type = null;
                providerRes.Value += linkConsumer.Value;
                if (direction == Vector2Int.up) {
                    tileConsumer.Refs.Remove<IDown>();
                    tileProvider.Refs.Remove<IUp>();
                } else if (direction == Vector2Int.down) {
                    tileConsumer.Refs.Remove<IUp>();
                    tileProvider.Refs.Remove<IDown>();
                } else if (direction == Vector2Int.right) {
                    tileConsumer.Refs.Remove<ILeft>();
                    tileProvider.Refs.Remove<IRight>();
                } else if (direction == Vector2Int.left) {
                    tileConsumer.Refs.Remove<IRight>();
                    tileProvider.Refs.Remove<ILeft>();
                } else {
                    throw new Exception();
                }
                tileConsumer.NeedUpdateSpriteKeys = true;
                tileProvider.NeedUpdateSpriteKeys = true;
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
                throw new Exception();
            }
        }
    }

    public interface ILinkable
    {
        IRef Res { get; }
        void OnLink(Type direction);
    }

    public interface ILeft { }
    public interface IRight { }
    public interface IUp { }
    public interface IDown { }

}

