
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
    [Concept]
    public interface ILeft { }
    [Concept]
    public interface IRight { }
    [Concept]
    public interface IUp { }
    [Concept]
    public interface IDown { }

    public interface ILinkConsumer : ITile
    {
        void Consume(List<IRef> refs);
    }

    public interface ILinkProvider : ITile
    {
        void Provide(List<IRef> refs);
    }

    public interface ILinkEvent : ITile
    {
        void OnLink();
    }

    public static class LinkUtility
    {
        public static UIItem CreateRefText(IRef pair) {
            if (pair.Type == null) return UIItem.CreateText($"本地内容【无】");
            return UIItem.CreateText($"本地内容{Localization.Ins.Val(pair.Type, pair.Value)}");
        }
        private readonly static List<Type> directions = new List<Type>() {
            typeof(IUp), typeof(IDown),typeof(ILeft), typeof(IRight),
        };
        public static void CreateLinkTexts(List<IUIItem> items, ITile tile) {
            bool created = false;
            foreach (var direction in directions) {
                if (tile.Refs.Has(direction)) {
                    IRef r = tile.Refs.Get(direction);
                    if (r.Value > 0) {
                        items.Add(UIItem.CreateText($"{Localization.Ins.Get(direction)}输入{Localization.Ins.Val(r.Type, r.Value)}"));

                    } else {
                        items.Add(UIItem.CreateText($"{Localization.Ins.Get(direction)}输出{Localization.Ins.Val(r.Type, -r.Value)}"));
                    }
                    created = true;
                }
            }
            if (!created) {
                items.Add(UIItem.CreateText("没有建立任何连接"));
            }
        }
        public static bool HasAnyLink(ITile tile) {
            return tile.Refs.Has<IUp>() || tile.Refs.Has<IDown>() || tile.Refs.Has<ILeft>() || tile.Refs.Has<IRight>();
        }

        private readonly static List<IRef> consumerPairsBuffer = new List<IRef>();
        private readonly static List<IRef> providerPairsBuffer = new List<IRef>();
        public static void CreateConsumerButtons(List<IUIItem> items, ITile tile) {
            // start
            if (consumerPairsBuffer.Count != 0) throw new Exception(); // assert 缓存已经清空
            ILinkConsumer consumer = tile as ILinkConsumer; // assert ILinkConsumer才能作为CreateConsumerButtons参数
            if (consumer == null) throw new Exception();
            consumer.Consume(consumerPairsBuffer); // 读取
            if (consumerPairsBuffer.Count == 0) return;
            if (consumerPairsBuffer.Count > 4) throw new Exception(); // 不能太多
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            // end
            TryCreateConsumerButton(items, tile, map.Get(pos + Vector2Int.up), typeof(IUp), typeof(IDown));
            TryCreateConsumerButton(items, tile, map.Get(pos + Vector2Int.down), typeof(IDown), typeof(IUp));
            TryCreateConsumerButton(items, tile, map.Get(pos + Vector2Int.left), typeof(ILeft), typeof(IRight));
            TryCreateConsumerButton(items, tile, map.Get(pos + Vector2Int.right), typeof(IRight), typeof(ILeft));

            consumerPairsBuffer.Clear();
        }
        public static void CreateConsumerButtons_Undo(List<IUIItem> items, ITile tile) {
            // start
            if (consumerPairsBuffer.Count != 0) throw new Exception(); // assert 缓存已经清空
            ILinkConsumer consumer = tile as ILinkConsumer; // assert ILinkConsumer才能作为CreateConsumerButtons_Undo参数
            if (consumer == null) throw new Exception();
            consumer.Consume(consumerPairsBuffer); // 读取
            if (consumerPairsBuffer.Count == 0) return;
            if (consumerPairsBuffer.Count > 4) throw new Exception(); // 不能太多
            IMap map = tile.GetMap();
            Vector2Int pos = tile.GetPos();
            // end

            TryCreateConsumerButton_Undo(items, tile, map.Get(pos + Vector2Int.up), typeof(IUp), typeof(IDown));
            TryCreateConsumerButton_Undo(items, tile, map.Get(pos + Vector2Int.down), typeof(IDown), typeof(IUp));
            TryCreateConsumerButton_Undo(items, tile, map.Get(pos + Vector2Int.left), typeof(ILeft), typeof(IRight));
            TryCreateConsumerButton_Undo(items, tile, map.Get(pos + Vector2Int.right), typeof(IRight), typeof(ILeft));

            consumerPairsBuffer.Clear();
        }
        private static void TryCreateConsumerButton_Undo(List<IUIItem> items, ITile consumerTile, ITile providerTile, Type consumerDir, Type providerDir) {
            // start
            ILinkConsumer consumer = consumerTile as ILinkConsumer;
            bool hasLink = consumer.Refs.Has(consumerDir); // 是否已经存在连接
            if (!hasLink) return; // 没有连接，则不存在解除连接的问题

            ILinkProvider provider = providerTile as ILinkProvider;
            if (provider == null) return;
            if (providerPairsBuffer.Count != 0) throw new Exception();
            provider.Provide(providerPairsBuffer);

            IRef consumerLink = consumer.Refs.Get(consumerDir); // 若存在连接则获取连接
            IRef providerLink = provider.Refs.Get(providerDir);  // 若存在连接则获取连接
            // end

            foreach (var consumerRef in consumerPairsBuffer) {
                if (consumerRef.Type == consumerLink.Type) { // 找到consumerRef
                    if (consumerRef.Value >= consumerLink.Value) {  // consumerRef有足够资源
                        foreach (var providerRef in providerPairsBuffer) {
                            if (providerRef.Type == providerLink.Type) {
                                long quantity = consumerLink.Value;
                                if (providerLink.Value != -quantity) throw new Exception($"{providerLink.Value} {quantity}");

                                items.Add(UIItem.CreateButton($"返还{Localization.Ins.Get(consumerDir)}{Localization.Ins.Val(consumerRef.Type, -quantity)}", () => {
                                    // 可以取消连接
                                    providerLink.Type = providerRef.Type;
                                    consumerLink.Type = consumerRef.Type;
                                    providerLink.Value += quantity; // providerLink以负值表示输出
                                    consumerLink.Value -= quantity; // consumerLink以正值表示输入
                                    providerRef.Value += quantity;
                                    consumerRef.Value -= quantity;

                                    if (consumerLink.Value == 0) {
                                        if (providerLink.Value != 0) {
                                            throw new Exception();
                                        }
                                        provider.Refs.Remove(providerDir);
                                        consumer.Refs.Remove(consumerDir);
                                    }

                                    providerTile.NeedUpdateSpriteKeys = true;
                                    consumerTile.NeedUpdateSpriteKeys = true;

                                    (providerTile as ILinkEvent)?.OnLink();
                                    (consumerTile as ILinkEvent)?.OnLink();

                                    consumerTile.OnTap();
                                }));
                            }
                            break; // 不用再找了
                        }
                    }
                    break; // 不用再找了
                }
            }

            providerPairsBuffer.Clear();
        }

        private static void TryCreateConsumerButton(List<IUIItem> items, ITile consumerTile, ITile providerTile, Type consumerDir, Type providerDir) {
            // start
            ILinkProvider provider = providerTile as ILinkProvider;
            if (provider == null) return;
            if (providerPairsBuffer.Count != 0) throw new Exception();
            provider.Provide(providerPairsBuffer);
            ILinkConsumer consumer = consumerTile as ILinkConsumer;
            bool hasLink = consumer.Refs.Has(consumerDir); // 是否已经存在连接
            IRef consumerLink = hasLink ? consumer.Refs.Get(consumerDir) : null; // 若存在连接则获取连接
            IRef providerLink = hasLink ? provider.Refs.Get(providerDir) : null;  // 若存在连接则获取连接
            if (hasLink != provider.Refs.Has(providerDir)) throw new Exception(); // assert !连接不成一对
            // end

            foreach (var consumerRef in consumerPairsBuffer) {
                if (hasLink && consumerRef.Type != consumerLink.Type) {
                    // 已经在此方向建立不兼容的连接
                    continue;
                }
                foreach (var providerRef in providerPairsBuffer) {
                    if (hasLink && providerRef.Type != providerLink.Type) {
                        // 已经在此方向建立不兼容的连接
                        continue;
                    }
                    // providerItem.Value 是供应方能提供的最大值
                    // consumerItem.BaseValue - consumerItem.Value 是需求方能消耗的最大值
                    long quantity = Math.Min(providerRef.Value, consumerRef.BaseValue - consumerRef.Value);
                    // 如果供需其中一个为0，则无法建立此对资源的连接
                    if (quantity == 0) continue;
                    if (quantity < 0) throw new Exception();
                    // 供给方类型为需求方类型子类，才能成功供给。需求方类型为null视为需求任意资源
                    if (consumerRef.Type == null || Tag.HasTag(providerRef.Type, consumerRef.Type)) {
                        items.Add(UIItem.CreateButton($"获取{Localization.Ins.Get(consumerDir)}{Localization.Ins.Val(consumerRef.Type ?? providerRef.Type, quantity)}", () => {

                            // 可以建立连接
                            if (!hasLink) {
                                consumerLink = consumer.Refs.Create(consumerDir);
                                providerLink = provider.Refs.Create(providerDir);
                            }
                            // 至此，consumerLink和providerLink肯定非空
                            providerLink.Type = providerRef.Type;
                            consumerLink.Type = consumerRef.Type ?? providerRef.Type;
                            providerLink.Value -= quantity; // providerLink以负值表示输出
                            consumerLink.Value += quantity; // consumerLink以正值表示输入
                            providerRef.Value -= quantity;
                            consumerRef.Value += quantity;

                            providerTile.NeedUpdateSpriteKeys = true;
                            consumerTile.NeedUpdateSpriteKeys = true;

                            consumerTile.OnTap();
                        }));
                    }
                }
            }
            providerPairsBuffer.Clear();
        }
    }
}

