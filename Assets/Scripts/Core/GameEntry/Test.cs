
using UnityEngine;

namespace Weathering
{
	public static class Test
	{
		public static void OnUpdate() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				GameEntry.Ins.SaveGame();
			}
		}
	}
}

