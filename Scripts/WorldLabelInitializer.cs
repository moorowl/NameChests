using UnityEngine;

namespace NameChests {
	public class WorldLabelInitializer : MonoBehaviour {
		public Vector3 offset;
		
		private void Start() {
			if (TryGetComponent<EntityMonoBehaviour>(out var entityMonoBehaviour) && entityMonoBehaviour is WorldLabel hasLabel) {
				var worldLabel = Instantiate(Main.WorldTextPrefab, transform);

				hasLabel.worldLabel = worldLabel.GetComponent<ObjectNameTag>().text;
				hasLabel.worldLabel.transform.localPosition += offset;
			}

			Destroy(this);
		}
	}
}