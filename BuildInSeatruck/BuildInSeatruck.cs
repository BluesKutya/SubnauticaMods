using System;
using FMODUnity;
using HarmonyLib;
using UnityEngine;

namespace BuildInSeatruck
{
	class BuildInSeatruck
	{

		[HarmonyPatch(typeof(Builder))]
		[HarmonyPatch(nameof(Builder.CheckTag))]
		internal class Builder_CheckTag_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(Collider c, ref bool __result)
			{
				//Set return value
				__result = !(c == null) && c.gameObject != null;

				//Tell Harmony to not run the original method
				return false;

			}
		}

		[HarmonyPatch(typeof(Builder))]
		[HarmonyPatch(nameof(Builder.TryPlace))]
		internal class Builder_TryPlace_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(ref bool __result)
			{
				if (Builder.prefab == null || !Builder.canPlace)
				{
					RuntimeManager.PlayOneShot("event:/bz/ui/item_error", default(Vector3));
					__result = false;
					return false; //Tell Harmony to not run the original method

				}
				RuntimeManager.PlayOneShot("event:/tools/builder/place", Builder.ghostModel.transform.position);
				ConstructableBase componentInParent = Builder.ghostModel.GetComponentInParent<ConstructableBase>();
				if (componentInParent != null)
				{
					BaseGhost component = Builder.ghostModel.GetComponent<BaseGhost>();
					component.Place();
					if (component.TargetBase != null)
					{
						componentInParent.transform.SetParent(component.TargetBase.transform, true);
					}
					componentInParent.SetState(false, true);
				}
				else
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Builder.prefab);
					bool flag = false;
					bool flag2 = false;
					SubRoot currentSub = Player.main.GetCurrentSub();

					////PATCHED_PART_BEGIN/////
					if (currentSub == null)
						currentSub = SeaTruckSegmentHelper.getCurrentSeaTruckSegmentSubRoot();
					////PATCHED_PART_END/////

					if (currentSub != null)
					{
						flag = currentSub.isBase;
						flag2 = currentSub.isCyclops;
						gameObject.transform.parent = currentSub.GetModulesRoot();
					}
					else if (Builder.placementTarget != null && Builder.allowedOutside)
					{
						SubRoot componentInParent2 = Builder.placementTarget.GetComponentInParent<SubRoot>();
						if (componentInParent2 != null)
						{
							gameObject.transform.parent = componentInParent2.GetModulesRoot();
						}
					}
					Transform transform = gameObject.transform;
					transform.position = Builder.placePosition;
					transform.rotation = Builder.placeRotation;
					Constructable componentInParent3 = gameObject.GetComponentInParent<Constructable>();
					componentInParent3.SetState(false, true);
					if (Builder.ghostModel != null)
					{
						UnityEngine.Object.Destroy(Builder.ghostModel);
					}
					componentInParent3.SetIsInside(flag || flag2);
					SkyEnvironmentChanged.Send(gameObject, currentSub);
				}
				Builder.ghostModel = null;
				Builder.prefab = null;
				Builder.canPlace = false;
				__result = true;
				return false; //Tell Harmony to not run the original method
			}
		}

		[HarmonyPatch(typeof(Builder))]
		[HarmonyPatch(nameof(Builder.ValidateOutdoor))]
		internal class Builder_ValidateOutdoor_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(GameObject hitObject, ref bool __result)
			{
				Rigidbody component = hitObject.GetComponent<Rigidbody>();
				if (component && !component.isKinematic)
				{
					__result = false;
					return false; //Tell Harmony to not run the original method
				}
				UnityEngine.Object component4 = hitObject.GetComponent<SubRoot>();
				Base component2 = hitObject.GetComponent<Base>();
				if (component4 != null && component2 == null)
				{
					__result = SeaTruckSegmentHelper.isPlayerInSeaTruckSegment();
					return false; //Tell Harmony to not run the original method
				}
				if (hitObject.GetComponent<Pickupable>() != null)
				{
					__result = false;
					return false; //Tell Harmony to not run the original method
				}
				LiveMixin component3 = hitObject.GetComponent<LiveMixin>();
				__result = !(component3 != null) || !component3.destroyOnDeath;
				return false; //Tell Harmony to not run the original method
			}
		}


		[HarmonyPatch(typeof(Constructable))]
		[HarmonyPatch(nameof(Constructable.CheckFlags))]
		internal class Constructable_CheckFlags_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(ref bool __result)
			{
				SeaTruckSegment sts;
				SubRoot subRoot;

				sts = SeaTruckSegmentHelper.getCurrentSeaTruckSegment();
				if (sts == null || sts.isFrontConnected || sts.isRearConnected)
				{
					return true; //run the orignal code
				}

				subRoot = SeaTruckSegmentHelper.getCurrentSeaTruckSegmentSubRoot();
				if (subRoot == null)
				{
					subRoot = sts.gameObject.AddComponent<SubRoot>();
				}

				if (subRoot == null)
					return true; //run the orignal code

				subRoot.isCyclops = true;
				subRoot.modulesRoot = sts.transform;

				__result = true;
				return false; //Tell Harmony to not run the original method
			}


		}


		[HarmonyPatch(typeof(PowerConsumer))]
		[HarmonyPatch(nameof(PowerConsumer.IsPowered))]
		internal class PowerConsumer_IsPowered_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(PowerConsumer __instance, ref bool __result)
			{
				SeaTruckSegment sts = SeaTruckSegmentHelper.getParentSeaTruckSegment(__instance.gameObject);
				if (sts == null)
					return true; //run the orignal code

				__result = ((IInteriorSpace)sts).CanBreathe();
				return false; //Tell Harmony to not run the original method
			}

		}


		[HarmonyPatch(typeof(SeaTruckSegment))]
		[HarmonyPatch(nameof(SeaTruckSegment.CanEnter))]
		internal class SeaTruckSegment_CanEnter_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(ref bool __result)
			{
				__result = true;
				return false; //Tell Harmony to not run the original method
			}
		}

	}
}
