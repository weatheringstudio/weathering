
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{

    public static class LinkUtility
    {

        public static void CreateDescription(List<IUIItem> items, IRef res) {
            if (res.Type != null) {
                items.Add(UIItem.CreateText($"运输中资源：{Localization.Ins.Val(res.Type, res.Value)}"));
            } else {
                items.Add(UIItem.CreateText($"运输中资源：无"));
            }
        }
        public static void CreateButtons(List<IUIItem> items, ITile tile, IRef r) {
            IRefs refs = tile.Refs;
            if (refs == null) throw new Exception();
            if (!refs.Has<IConsumerUp>() && !refs.Has<IProviderDown>()) {
                CreateLinkButton(tile, "获取南方", items, r, Vector2Int.up);
            }
            if (!refs.Has<IConsumerDown>() && !refs.Has<IProviderUp>()) {
                CreateLinkButton(tile, "获取北方", items, r, Vector2Int.down);
            }
            if (!refs.Has<IConsumerLeft>() && !refs.Has<IProviderRight>()) {
                CreateLinkButton(tile, "获取东方", items, r, Vector2Int.left);
            }
            if (!refs.Has<IConsumerRight>() && !refs.Has<IProviderLeft>()) {
                CreateLinkButton(tile, "获取西方", items, r, Vector2Int.right);
            }
        }

        private static void CreateLinkButton(ITile tileConsumer, string text, List<IUIItem> items, IRef r, Vector2Int direction) {
            ITile tileProvider = tileConsumer.GetMap().Get(tileConsumer.GetPos() - direction);
            IProvider provider = tileProvider as IProvider;
            if (provider != null) {
                var canProvide = provider.CanProvide;
                if (canProvide.Item2 != 0 && canProvide.Item1 != null && (r.Type == null || r.Type == canProvide.Item1)) {
                    items.Add(UIItem.CreateButton($"{text}{Localization.Ins.Val(canProvide.Item1, canProvide.Item2)}", () => {
                        provider.Provide(canProvide);
                        RegisterLink(tileProvider, tileConsumer, direction, canProvide);
                        r.Type = canProvide.Item1;
                        r.Value += canProvide.Item2;
                        tileConsumer.OnTap();
                    }));
                }
            }
        }

        private static void RegisterLink(ITile provider, ITile consumer, Vector2Int direction, (Type, long) request) {
            IRef providerRef;
            IRef consumerRef;

            if (direction == Vector2Int.left) {
                providerRef = provider.Refs.Create<IProviderLeft>();
                consumerRef = consumer.Refs.Create<IConsumerLeft>();
            } else if (direction == Vector2Int.right) {
                providerRef = provider.Refs.Create<IProviderRight>();
                consumerRef = consumer.Refs.Create<IConsumerRight>();
            } else if (direction == Vector2Int.up) {
                providerRef = provider.Refs.Create<IProviderUp>();
                consumerRef = consumer.Refs.Create<IConsumerUp>();
            } else if (direction == Vector2Int.down) {
                providerRef = provider.Refs.Create<IProviderDown>();
                consumerRef = consumer.Refs.Create<IConsumerDown>();
            } else {
                throw new Exception();
            }

            providerRef.Type = request.Item1;
            providerRef.Value = request.Item2;
            consumerRef.Type = request.Item1;
            consumerRef.Value = request.Item2;
            provider.NeedUpdateSpriteKeys = true;
            consumer.NeedUpdateSpriteKeys = true;
        }

        public static bool HasLink(ITile thisTile, Vector2Int direction) {
            if (direction == Vector2Int.up) {
                return thisTile.Refs.Has<IConsumerUp>() || thisTile.Refs.Has<IProviderDown>();
            } else if (direction == Vector2Int.down) {
                return thisTile.Refs.Has<IConsumerDown>() || thisTile.Refs.Has<IProviderUp>();
            } else if (direction == Vector2Int.left) {
                return thisTile.Refs.Has<IConsumerLeft>() || thisTile.Refs.Has<IProviderRight>();
            } else if (direction == Vector2Int.right) {
                return thisTile.Refs.Has<IConsumerRight>() || thisTile.Refs.Has<IProviderLeft>();
            }
            return true;
        }
    }

    public interface IProvider
    {
        (Type, long) CanProvide { get; }
        void Provide((Type, long) items);
    }

    public interface IConsumer
    {
        (Type, long) CanConsume { get; }
        void Consume((Type, long) items);
    }

    public interface IConsumerLeft { }
    public interface IConsumerRight { }
    public interface IConsumerDown { }
    public interface IConsumerUp { }

    public interface IProviderLeft { }
    public interface IProviderRight { }
    public interface IProviderDown { }
    public interface IProviderUp { }

}

