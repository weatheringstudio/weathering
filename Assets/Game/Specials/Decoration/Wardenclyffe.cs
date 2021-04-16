

using UnityEngine;

namespace Weathering
{

    public class WardenclyffeBeacon {}


    [ConstructionCostBase(typeof(MachinePrimitive), 1000, 0)]
    public class Wardenclyffe : AbstractDecoration, IHasFrameAnimationOnSpriteKey
    {
        public override void OnTap() {

            var items = UI.Ins.GetItems();


            string thisMapKey = Map.GetMapKey;
            for (int i = 0; i < 5; i++) {
                string planetMapKeyKey = $"{typeof(WardenclyffeBeacon).Name}{i}";

                string planetKey = Globals.Ins.String(planetMapKeyKey);
                if (planetKey == null) {

                }
                else {
                    items.Add(UIItem.CreateButton($"前往【信标{i}】{planetKey}", () => {
                        PlanetLander.Ins.LeavePlanet();
                        GameEntry.Ins.EnterMap(planetKey);
                    }));
                }
            }

            items.Add(UIItem.CreateSeparator());

            for (int i = 0; i < 5; i++) {
                string planetMapKeyKey = $"{typeof(WardenclyffeBeacon).Name}_{i}";

                string planetKey = Globals.Ins.String(planetMapKeyKey);

                items.Add(UIItem.CreateButton($"设置【信标{i}】为当前星球", () => {
                    Globals.Ins.String(planetMapKeyKey, thisMapKey);
                    OnTap();
                }));
            }

            items.Add(UIItem.CreateStaticDestructButton(this));

            UI.Ins.ShowItems(Localization.Ins.Get<Wardenclyffe>(), items);
        }



        public override string SpriteKey => $"{typeof(Wardenclyffe).Name}{MapView.Ins.AnimationIndex % 6}";

        public int HasFrameAnimation => 1;
    }
}
