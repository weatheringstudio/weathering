
using UnityEngine;

namespace Weathering
{
	public static class Test
	{
		public static void OnUpdate() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				GameEntry.Ins.SaveGame();
			}
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (UI.Ins.Active) {
					UI.Ins.Active = false;
                }
				else {
					GameMenu.Ins.OnTap();
                }
            }
			if (Input.GetKeyDown(KeyCode.Z)) {
            }
		}
	}
}

