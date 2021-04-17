

using System;
using System.Collections.Generic;

namespace Weathering
{
    public static class ItemUsage
    {
        public static readonly Dictionary<Type, Action<IInventory, long, Action>> Usage = new Dictionary<Type, Action<IInventory, long, Action>> {

            {
                typeof(Berry), (IInventory inventory, long quantity, Action back) => {

                    const long recover = 5;

                    long sanityCanRecover = Globals.Sanity.Max - Globals.Sanity.Val;
                    long maxSanity = sanityCanRecover/recover;

                    long deltaBerry = Math.Min(quantity, maxSanity);

                    IValue satiety = Globals.Ins.Values.Get<Satiety>();
                    long maxSatiety = satiety.Max - satiety.Val;
                    deltaBerry = Math.Min(deltaBerry, maxSatiety);

                    long deltaSanity = Math.Min(deltaBerry * recover, sanityCanRecover);
                    satiety.Val += deltaBerry;

                    Globals.Sanity.Val += deltaSanity;
                    inventory.Remove<Berry>(deltaBerry);

                    // ui

                    var items = UI.Ins.GetItems();
                    items.Add(UIItem.CreateReturnButton(back));
                    items.Add(UIItem.CreateValueProgress<Sanity>(Globals.Sanity));

                    string eaten = Localization.Ins.Val<Berry>(deltaBerry);
                    string recovered = Localization.Ins.Val<Sanity>(deltaSanity);
                    if (deltaBerry == 0) {
                        if (deltaBerry == maxSanity) {
                            items.Add(UIItem.CreateMultilineText($"感觉体力充沛, 什么都不用吃"));
                        }
                        else {
                            items.Add(UIItem.CreateMultilineText($"吃了{eaten}, 相当于什么都没吃"));
                        }
                    }
                    else if (deltaBerry == quantity) {
                        items.Add(UIItem.CreateMultilineText($"吃光了所有的{eaten}, 感觉还是没吃饱"));
                    }
                    else if (deltaBerry == maxSatiety) {
                        items.Add(UIItem.CreateMultilineText($"吃撑了, 只吃下了{eaten}"));
                    }
                    else { // min == maxSanity
                        items.Add(UIItem.CreateMultilineText($"吃下了{eaten}, 感觉体力充沛"));
                    }
                    UI.Ins.ShowItems($"吃掉了{eaten}恢复了{recovered}", items);
                }
            }
        };
    }
}
