using HarmonyLib;
using NameChests.Utilities;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace NameChests.UserInterface {
	[HarmonyPatch]
	public static class LabelRootPositioning {
		private const float BackgroundWidth = 6f;
		private static Vector3? OriginalLabelRootPosition;
		private static Vector3? OriginalSignStateTogglePosition;
		
		[HarmonyPatch(typeof(ChestInventoryUI), "UpdateContainerSize")]
		[HarmonyPostfix]
		public static void ChestInventoryUI_UpdateContainerSize(ChestInventoryUI __instance) {
			if (__instance.labelRoot.gameObject.activeSelf && LabelUtils.HasLabel(__instance.GetInventoryHandler().entityMonoBehaviour, out _)) {
				OriginalLabelRootPosition  ??= __instance.labelRoot.transform.localPosition;
				OriginalSignStateTogglePosition ??= __instance.signStateToggle.transform.localPosition;
				
				__instance.labelRoot.localPosition = new Vector3(
					__instance.labelRoot.localPosition.x,
					(__instance.visibleRows - 3) * 0.6875f,
					__instance.labelRoot.localPosition.z
				);
				__instance.signStateToggle.transform.localPosition = new Vector3(
					(BackgroundWidth / 2f) + 0.9375f,
					__instance.inputField.gameObject.transform.localPosition.y,
					__instance.signStateToggle.transform.localPosition.z
				);
			} else {
				if (OriginalLabelRootPosition != null)
					__instance.labelRoot.transform.localPosition = OriginalLabelRootPosition.Value;
				
				if (OriginalSignStateTogglePosition != null)
					__instance.signStateToggle.transform.localPosition = new Vector3(__instance.signStateToggle.transform.localPosition.x, OriginalSignStateTogglePosition.Value.y, __instance.signStateToggle.transform.localPosition.z);
			}
		}
	}
}