

using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	/// <summary>
	/// 发布时和测试时, 需要改哪几个地方? 
	/// GlobalGameEvents的设置
	/// </summary>
	public static class GameConfig
	{
		public static bool CheatMode = false;
		public static long DefaultInventoryOfResourceQuantityCapacity { get; } = 1000000000000000;
		public static int DefaultInventoryOfResourceTypeCapacity { get; } = 30;
		public static long DefaultInventoryOfSupplyQuantityCapacity { get; } = 10000000000;
		public static int DefaultInventoryOfSupplyTypeCapacity { get; } = 10;

		public const string InitialMapKey = "Weathering.MapOfPlanet#=1,4=14,93=24,31";

		public const int VersionCode = 20210417;
		public static void OnConstruct(IGlobals globals) {

			// 全局理智
			IValue sanity = globals.Values.Create<Sanity>();
			sanity.Max = 100;
			sanity.Val = sanity.Max / 10;
			sanity.Inc = 1;
			sanity.Del = 10 * Value.Second;

			// 饱腹度
			IValue satiety = globals.Values.Create<Satiety>();
			satiety.Max = 100;
			satiety.Inc = 1;
			satiety.Val = 0;
			satiety.Del = Value.Second;

			// 行动冷却
			IValue cooldown = globals.Values.Create<CoolDown>();
			cooldown.Inc = 1;
			cooldown.Max = 1;
			cooldown.Del = Value.Second;

			IInventory inventory = globals.Inventory;
			inventory.QuantityCapacity = DefaultInventoryOfResourceQuantityCapacity;
			inventory.TypeCapacity = 10;

			inventory.Add<TutorialMapTheBook>(1);
			inventory.Add<TutorialMapTheDiary>(1);
			inventory.Add<TutorialMapTheCurse>(1);


			Globals.Ins.Values.GetOrCreate<QuestResource>().Del = Value.Second;

			Globals.Unlock<TotemOfNature>();

			Globals.Ins.Values.GetOrCreate<KnowledgeOfNature>().Max = KnowledgeOfNature.Max;
			Globals.Ins.Values.GetOrCreate<KnowledgeOfAncestors>().Max = KnowledgeOfAncestors.Max;
		}

		public static void OnGameConstruct() {

		}

		public static void OnGameEnable() {

		}

		public static void OnSave() {

		}


	}





	[Depend(typeof(Book))]
	public class TutorialMapTheBook { }

	[Depend(typeof(Book))]
	public class TutorialMapTheDiary { }

	[Depend(typeof(Book))]
	public class TutorialMapTheCurse { }
}

