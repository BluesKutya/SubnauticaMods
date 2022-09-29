using System;
using FMODUnity;
using HarmonyLib;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace MedicalCabinetFix
{
	class MedicalCabinetFix
	{

		private static bool addToKnownTech(TechType teckType)
        {
			if (!KnownTech.Contains(teckType))
				return KnownTech.Add(teckType, false);

			return true;
		}

		[HarmonyPatch(typeof(MainGameController))]
		[HarmonyPatch(nameof(MainGameController.StartGame))]
		internal class MainGameController_StartGame_Patch
		{
			[HarmonyPostfix]
			public static void Postfix()
			{
				addToKnownTech(TechType.MedicalCabinetBlueprint);
				addToKnownTech(TechType.MedicalCabinet);
				CraftDataHandler.AddToGroup(TechGroup.InteriorModules, TechCategory.InteriorModule, TechType.MedicalCabinet);
			}
		}

	}

}
