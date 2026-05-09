using UnityEngine;

namespace NameChests.Utilities {
	public static class LabelUtils {
		public static bool HasLabel(EntityMonoBehaviour entityMono, out Vector3 offset) {
			switch (entityMono) {
				case Mannequin:
					offset = new Vector3(0f, 0.25f, 0f);
					return true;
				case Aquarium or Terrarium:
					offset = new Vector3(0.5f, 0.7f, 0f);
					return true;
				case Pedestal and not AncientGiant:
					offset = new Vector3(0f, 0.25f, 0f);
					return true;
				default:
					offset = default;
					return false;
			}
		}
	}
}