using HarmonyLib;

namespace SeaTruckNuclearRod
{

    [HarmonyPatch]
    class Prefixes
    {
        [HarmonyPatch(typeof(TechTypeExtensions), nameof(TechTypeExtensions.IsObsolete))]
        [HarmonyPrefix]
        public static bool ObseleteStopper(ref bool __result)
        {
            __result = false;
            return false;
        }
    }


    [HarmonyPatch]
    class Postfixes
    {
        [HarmonyPatch(typeof(SeaTruckUpgrades), nameof(SeaTruckUpgrades.OnUpgradeModuleChange))]
        [HarmonyPostfix]
        public static void PostUpgradeModuleChange(SeaTruckUpgrades __instance, int slotID, TechType techType, bool added)
        {
            SeaTruckNuclearControl nuclearControl = null;
            var noneEquipped = false;
            var foundBehaviour = false;

            // We only care about the SeaTruckNuclearModule, return otherwise
            if (techType != SeaTruckNuclearModule.thisTechType)
                return;

            noneEquipped = __instance.modules.GetCount(techType) < 1;
            foundBehaviour = __instance.gameObject.TryGetComponent(out nuclearControl);

            // If equipped, proceed
            if (added && !foundBehaviour)
            {
                nuclearControl = __instance.gameObject.AddComponent<SeaTruckNuclearControl>();
                nuclearControl.seatruckUpgr = __instance;
                nuclearControl.slotID = slotID;
            }

            if (!added && noneEquipped && foundBehaviour)
            {
                nuclearControl.isActive = false;
                UnityEngine.Object.Destroy(nuclearControl);
            }

        }

    }

}
