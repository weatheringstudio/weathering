

namespace Weathering
{
    public abstract class AbstractDecoration : StandardTile
    {
        public abstract override void OnTap();

        public sealed override bool CanDestruct() => true;
    }
}
