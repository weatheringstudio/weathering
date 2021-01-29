

namespace Weathering
{
	[Depend(typeof(Food))]
	[Concept]
	public class Creature { }

	[Depend(typeof(Creature))]
	[Concept]
	public class Kitten { }

	[Depend(typeof(Creature))]
	[Concept]
	public class Slime { }

	[Depend(typeof(Wood))]
	[Concept]
	public class SlimeLiquid { }

	[Depend(typeof(Creature))]
	[Concept]
	public class Snake { }

	[Depend(typeof(Creature))]
	[Concept]
	public class Goblin { }
}

