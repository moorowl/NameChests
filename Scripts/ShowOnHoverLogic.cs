using HarmonyLib;
using Interaction;
using Pug.UnityExtensions;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics;

// ReSharper disable InconsistentNaming

namespace NameChests {
	[HarmonyPatch]
	public static class ShowOnHoverLogic {
		private const float HoveredDetectionRadius = 3f;
		private static readonly float2 HoveredDetectionOffset = new(0f, -0.2f);

		private static EntityMonoBehaviour HoveredEntityMono;

		[HarmonyPatch(typeof(PlayerController), "ManagedUpdate")]
		[HarmonyPostfix]
		public static void PlayerController_ManagedUpdate(PlayerController __instance) {
			if (__instance.isLocal)
				UpdateHoveredEntityMono(__instance);
		}
		
		[HarmonyPatch(typeof(WorldLabel), "UpdateWorldText")]
		[HarmonyPrefix]
		public static void WorldLabel_UpdateWorldText_Prefix(WorldLabel __instance, string text, ref string __state) {
			__state = text;
		}
		
		[HarmonyPatch(typeof(WorldLabel), "UpdateWorldText")]
		[HarmonyPostfix]
		public static void WorldLabel_UpdateWorldText_Postfix(WorldLabel __instance, int ____visibilityState, string __state, string text) {
			if (____visibilityState == 1 && __instance.worldLabel != null) {
				var showOnFacing = Options.Instance.ShowOnHover != ShowOnHoverMode.ShowOverrideFacing;
				var showOnHover = Options.Instance.ShowOnHover != ShowOnHoverMode.None;

				var isBeingInteractedWith = Manager.main.player.GetCurrentInteractableObject() == __instance.GetComponentInChildren<InteractableObject>();
				var showLabel = (showOnHover && HoveredEntityMono == __instance && !string.IsNullOrWhiteSpace(__state)) || (showOnFacing && isBeingInteractedWith);
				
				if (EntityUtility.HasComponentData<IsClosestLocalInteractableCD>(__instance.entity, __instance.world))
					EntityUtility.SetComponentEnabled<IsClosestLocalInteractableCD>(__instance.entity, __instance.world, showLabel || isBeingInteractedWith);

				__instance.worldLabel.Render(showLabel ? __state : "", false, false, false);
			}
		}
		
		private static void UpdateHoveredEntityMono(PlayerController player) {
			HoveredEntityMono = null;

			if (Manager.ui.isAnyInventoryShowing || Manager.prefs.hideInGameUI || !player.inputModule.PrefersKeyboardAndMouse())
				return;
			
			var cursorTilePosition = (EntityMonoBehaviour.ToWorldFromRender(Manager.ui.mouse.GetMouseGameViewPosition()).ToFloat2() + HoveredDetectionOffset).RoundToInt2();
			var collisionWorld = PhysicsManager.GetCollisionWorld();

			var outHits = new NativeList<ColliderCastHit>(Allocator.Temp);
			collisionWorld.SphereCastAll(cursorTilePosition.ToFloat3(), HoveredDetectionRadius, float3.zero, HoveredDetectionRadius, ref outHits, new CollisionFilter {
				BelongsTo = PhysicsLayerID.Everything,
				CollidesWith = PhysicsLayerID.Everything
			});

			foreach (var hit in outHits) {
				if (EntityUtility.HasComponentData<ObjectDataCD>(hit.Entity, player.world) && Manager.memory.TryGetEntityMono(hit.Entity, out var entityMono)) {
					if (entityMono.objectInfo.objectType == ObjectType.PlaceablePrefab && IsOccupiedByObject(cursorTilePosition, entityMono)) {
						HoveredEntityMono = entityMono;
						break;
					}
				}
			}

			outHits.Dispose();
		}

		private static bool IsOccupiedByObject(int2 position, EntityMonoBehaviour entityMono) {
			var tilePosition = entityMono.WorldPosition.RoundToInt2();
			GetOffsetAndTileSize(entityMono, out var entitySize, out var entityOffset);

			for (var y = entityOffset.y; y < entityOffset.y + entitySize.y; y++) {
				for (var x = entityOffset.x; x < entityOffset.x + entitySize.x; x++) {
					var offsetTilePosition = tilePosition + new int2(x, y);

					if (offsetTilePosition.x == position.x && offsetTilePosition.y == position.y)
						return true;
				}
			}

			return false;
		}
		
		private static void GetOffsetAndTileSize(EntityMonoBehaviour entityMono, out int2 entitySize, out int2 entityOffset) {
			var objectInfo = entityMono.objectInfo;
			entitySize = objectInfo.prefabTileSize.ToInt2();
			entityOffset = objectInfo.prefabCornerOffset.ToInt2();

			if (EntityUtility.TryGetComponentData<DirectionCD>(entityMono.entity, entityMono.world, out var directionCD))
				directionCD.GetPrefabOffsetAndTileSize(entityOffset, entitySize, out entityOffset, out entitySize);
		}
	}
}