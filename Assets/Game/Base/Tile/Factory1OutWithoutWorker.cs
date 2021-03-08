

using System;
using System.Collections.Generic;

namespace Weathering
{
    public abstract class Factory1OutWithoutWorker : StandardTile, ILinkProvider
    {
        public override string SpriteKeyBase => TerrainDefault.CalculateTerrain(Map as StandardMap, Pos).Name;
        public abstract override string SpriteKey { get; }

        private Type resourceType;
        protected abstract Type Type { get; }

        protected abstract long BaseValue { get; }

        public sealed override void OnConstruct() {
            base.OnConstruct();
            Refs = Weathering.Refs.GetOne();

            resourceType = Type;
            Resource = Refs.Create(resourceType);
            Resource.Type = resourceType;
            Resource.BaseValue = 1;
            Resource.Value = Resource.BaseValue;
        }

        private IRef Resource { get; set; }

        public override void OnEnable() {
            base.OnEnable();

            if (resourceType == null) resourceType = Type;
            Resource = Refs.Get(resourceType);
        }

        public sealed override void OnTap() {
            var items = UI.Ins.GetItems();

            LinkUtility.AddButtons(items, this);

            items.Add(UIItem.CreateDestructButton<TerrainDefault>(this, () => Resource.Value == Resource.BaseValue));

            UI.Ins.ShowItems(Localization.Ins.Get<HuntingGround>(), items);
        }

        public void Provide(List<IRef> refs) {
            refs.Add(Resource);
        }
    }
}
