

using System;

namespace Weathering
{
    [ConceptDescription(typeof(PublicGiftPackRewardItemDescription))]
    [Concept]
    [Depend(typeof(Book))]
    public class PublicGiftPackRewardItem { }
    [Concept]
    public class PublicGiftPackRewardItemDescription { }

    [ConceptDescription(typeof(PrivateGiftPackRewardItemDescription))]
    [Concept]
    [Depend(typeof(Book))]
    public class PrivateGiftPackRewardItem { }
    [Concept]
    public class PrivateGiftPackRewardItemDescription { }

    public class AESReward : StandardTile
    {
        public override string SpriteKey => "AESReward";
        public override void OnTap() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateText("这里可以使用作弊码兑换资源"));

            if (!Globals.Ins.Bool<PublicGiftPack>()) {
                items.Add(UIItem.CreateButton("使用公开作弊码", TryUsePublicGiftPack));
            } else {
                items.Add(UIItem.CreateStaticButton("使用公开作弊码 (已使用)", TryUsePublicGiftPack, false));
            }

            if (!Globals.Ins.Bool<PrivateGiftPack>()) {
                items.Add(UIItem.CreateButton("使用秘密作弊码", TryUsePrivateGiftPack));
            } else {
                items.Add(UIItem.CreateStaticButton("使用秘密作弊码 (已使用)", TryUsePrivateGiftPack, false));
            }

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this));

            UI.Ins.ShowItems(Localization.Ins.Get<AESReward>(), items);
        }


        private void ErrorPageOfEmptyContent(Action action) {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateButton("重新输入", action));

            UI.Ins.ShowItems("作弊码不能为空", items);
        }

        private void ErrorPageOfWrongAnswer(Action action) {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateButton("重新输入", action));

            UI.Ins.ShowItems("回答不正确", items);
        }



        private class PublicGiftPack { }

        private void TryUsePublicGiftPack() {
            UI ui = UI.Ins as UI;
            if (ui == null) throw new Exception();

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateReturnButton(OnTap));
            items.Add(UIItem.CreateText("在屏幕下方黑色区域输入答案"));
            items.Add(UIItem.CreateText("问题: 玩家QQ群号是多少?"));
            items.Add(UIItem.CreateButton("做出回答", () => {
                string answer = ui.InputFieldContent;
                if (answer == null || answer.Length == 0) {
                    ErrorPageOfEmptyContent(TryUsePublicGiftPack);
                } else {
                    // string decryptedAnswer = AESUtility.Decrypt()
                    // 暂时不用AES了
                    if (answer.Contains("884032539")) {
                        UsePublicGiftPack();
                    } else {
                        ErrorPageOfWrongAnswer(TryUsePublicGiftPack);
                    }
                }
            }));

            ui.ShowInputFieldNextTime = true;
            UI.Ins.ShowItems(Localization.Ins.Get<AESReward>(), items);
        }
        private void UsePublicGiftPack() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateText("回答正确"));
            items.Add(UIItem.CreateText($"获得{Localization.Ins.Get<PublicGiftPackRewardItem>()}"));
            items.Add(UIItem.CreateText($"点击人物打开背包查看刚获得的物品"));
            Globals.Ins.Inventory.Add<PublicGiftPackRewardItem>(1);

            UI.Ins.ShowItems(Localization.Ins.Get<AESReward>(), items);
        }


        public class PrivateGiftPack { }

        private void TryUsePrivateGiftPack() {
            UI ui = UI.Ins as UI;
            if (ui == null) throw new Exception();

            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateReturnButton(OnTap));
            items.Add(UIItem.CreateText("在屏幕下方黑色区域输入答案"));
            items.Add(UIItem.CreateMultilineText("充值一个亿可以获得答案，仅有一次机会，先到先得"));
            items.Add(UIItem.CreateMultilineText("( 放心充值，即使把游戏解包也找不到答案 )"));
            items.Add(UIItem.CreateButton("做出回答", () => {
                string answer = ui.InputFieldContent;
                if (answer == null || answer.Length == 0) {
                    ErrorPageOfEmptyContent(TryUsePublicGiftPack);
                } else {
                    // string decryptedAnswer = AESUtility.Decrypt()
                    // 暂时不用AES了
                    if (AESUtility.Decrypt(encryptedAnswer, answer).Equals(AESPack.CorrectAnswer)) {
                        UsePrivateGiftPack();
                    } else {
                        ErrorPageOfWrongAnswer(TryUsePrivateGiftPack);
                    }
                }
            }));

            ui.ShowInputFieldNextTime = true;
            UI.Ins.ShowItems(Localization.Ins.Get<AESReward>(), items);
        }

        private const string encryptedAnswer = "w9dtH4DgIEhO16kLoRCmew==";
        private void UsePrivateGiftPack() {
            var items = UI.Ins.GetItems();

            items.Add(UIItem.CreateText("回答正确!!"));
            items.Add(UIItem.CreateText($"获得{Localization.Ins.Get<PrivateGiftPackRewardItem>()}"));
            items.Add(UIItem.CreateText($"点击人物打开背包查看刚获得的物品"));
            Globals.Ins.Inventory.Add<PrivateGiftPackRewardItem>(1);

            UI.Ins.ShowItems(Localization.Ins.Get<AESReward>(), items);
        }
    }
}
