

using System.Collections.Generic;
using UnityEngine;

namespace Weathering
{
	/// <summary>
	/// 发布时和测试时，需要改哪几个地方？
	/// GlobalGameEvents的设置
	/// </summary>
	public static class GameConfig
	{
		public static bool CheatMode = true;
		public static long DefaultInventoryOfResourceQuantityCapacity { get; } = 1000000000000000;
		public static int DefaultInventoryOfResourceTypeCapacity { get; } = 30;
		public static long DefaultInventoryOfSupplyQuantityCapacity { get; } = 10000000000;
		public static int DefaultInventoryOfSupplyTypeCapacity { get; } = 10;


		public const int VersionCode = 20210415;
		public static void OnConstruct(IGlobals globals) {

			// 全局理智
			IValue sanity = globals.Values.Create<Sanity>();
			sanity.Max = 100;
			sanity.Val = sanity.Max / 2;
			sanity.Inc = 1;
			sanity.Del = Value.Second;

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
		}

		public static void OnGameConstruct() {

		}

		public static void OnGameEnable() {

		}

		public static void OnSave() {

		}


	}






	[Depend]
	[ConceptDescription(typeof(TutorialMapTheBookDescription))]
	[Concept]
	public class Book { }

	[Depend(typeof(Book))]
	[ConceptDescription(typeof(TutorialMapTheBookDescription))]
	[Concept]
	public class TutorialMapTheBook { }
	[Concept]
	public class TutorialMapTheBookDescription { }

	[Depend(typeof(Book))]
	[ConceptDescription(typeof(TutorialMapTheDiaryDescription))]
	[Concept]
	public class TutorialMapTheDiary { }
	[Concept]
	public class TutorialMapTheDiaryDescription { }


	[Depend(typeof(Book))]
	[ConceptDescription(typeof(TutorialMapTheCurseDescription))]
	[Concept]
	public class TutorialMapTheCurse { }
	[Concept]
	public class TutorialMapTheCurseDescription { }
}

