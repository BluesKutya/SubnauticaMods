using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SeaTruckNuclearRod
{

	class SeaTruckNuclearModule: Equipable
    {

        public const String SeaTruckNuclearModuleBatteryName = "SeaTruckNuclearBattery";

        public static TechType thisTechType;

        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        public SeaTruckNuclearModule() : base(
            "SeaTruckNuclearRod",
            "Seatruck Nuclear Rod",
            "Craftable nuclear reactor rod for seatruck.")
        {
            OnFinishedPatching += () =>
            {
                SeaTruckNuclearModule.thisTechType = this.TechType;
                SMLHelper.V2.Handlers.CraftTreeHandler.Main.AddCraftingNode(CraftTree.Type.SeamothUpgrades, this.TechType, new string[] { "SeaTruckUpgrade" });
                SMLHelper.V2.Handlers.CraftTreeHandler.Main.AddCraftingNode(CraftTree.Type.Fabricator, this.TechType, new string[] { "Upgrades", "SeatruckUpgrades" });
            };
        }

        public override EquipmentType EquipmentType => EquipmentType.SeaTruckModule;

        public override TechType RequiredForUnlock => TechType.Workbench;
        public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
        public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Workbench;

        public override float CraftingTime => 1f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "SeaTruckNuclearRod.png"));
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            var task = CraftData.GetPrefabForTechTypeAsync(TechType.SeamothReinforcementModule, false);
            yield return task;
            var prefab = GameObject.Instantiate(task.GetResult());
            gameObject.Set(prefab);

            Battery battery = prefab.AddComponent<Battery>();
            battery.name = "SeaTruckNuclearBattery";
            battery._capacity = QMod.Config.StoredNuclearEnergy;
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(TechType.ReactorRod, 1),
                    new Ingredient(TechType.Copper, 1)
                }
            };
        }

    }

}
