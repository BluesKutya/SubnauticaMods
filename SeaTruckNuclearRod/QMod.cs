using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Handlers;
using SeaTruckNuclearRod.Configuration;


namespace SeaTruckNuclearRod
{

    [QModCore]
    public static class QMod
    {

        internal static Config Config { get; private set; }

        [QModPatch]
        public static void Patch()
        {

            Config = OptionsPanelHandler.Main.RegisterModOptions<Config>();

            var assembly = Assembly.GetExecutingAssembly();
            var modName = ($"blueskutya_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Debug, $"Patching {modName}");

            new SeaTruckNuclearModule().Patch();

            Harmony harmony = new Harmony(modName);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }

    }

}
