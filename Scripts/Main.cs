using System.Linq;
using Interaction;
using Pug.UnityExtensions;
using PugMod;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming

namespace NameChests {
	public class Main : IMod {
		public const string Version = "2.1";
		public const string InternalName = "NameChests";
		public const string DisplayName = "More Labels";

		internal static GameObject WorldTextPrefab { get; private set; }
		
		internal static AssetBundle AssetBundle { get; private set; }

		public void EarlyInit() {
			Debug.Log($"[{DisplayName}]: Mod version: {Version}");
			
			var modInfo = API.ModLoader.LoadedMods.FirstOrDefault(modInfo => modInfo.Handlers.Contains(this));
			AssetBundle = modInfo!.AssetBundles[0];

			WorldTextPrefab = AssetBundle.LoadAsset<GameObject>($"Assets/{InternalName}/Prefabs/WorldText.prefab");
			
			Options.Instance.Init();
		}

		public void Init() { }
		
		public void ModObjectLoaded(Object obj) { }
		
		public void Shutdown() { }

		public void Update() {
			Options.Instance.Update();
		}
	}
}