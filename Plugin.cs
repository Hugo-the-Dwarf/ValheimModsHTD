using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BepInEx.Configuration;

namespace ValheimMoreTwoHanders
{
    [BepInPlugin(Plugin.GUID, Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "2.0";
        public const string ModName = "More Two Handed Weapons";
        public const string GUID = "htd.moretwohanders";
        //public static readonly string MyDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Harmony _Harmony;
        public static ManualLogSource Log;

        public readonly Harmony harmony = new Harmony(GUID);

        //config stuff
        public static readonly ConfigEntry<bool>[] EnabledWeaponRecipes = new ConfigEntry<bool>[7];

        public static ConfigEntry<string> SwordSilverGreatName;
        public static ConfigEntry<string> SwordSilverGreatDescription;
        public static ConfigEntry<string> SwordSilverGreatCraftingStation;
        public static readonly ConfigEntry<int>[] SwordSilverGreatCraftingLevels = new ConfigEntry<int>[2];
        public static readonly ConfigEntry<float>[] SwordSilverGreatDamages = new ConfigEntry<float>[10];
        public static readonly ConfigEntry<float>[] SwordSilverGreatDamagesPerUpgrade = new ConfigEntry<float>[10];

        public static ConfigEntry<string> GreatCoreMaceName;
        public static ConfigEntry<string> GreatCoreMaceDescription;
        public static ConfigEntry<string> GreatCoreMaceCraftingStation;
        public static readonly ConfigEntry<int>[] GreatCoreMaceCraftingLevels = new ConfigEntry<int>[2];
        public static readonly ConfigEntry<float>[] GreatCoreMaceDamages = new ConfigEntry<float>[10];
        public static readonly ConfigEntry<float>[] GreatCoreMaceDamagesPerUpgrade = new ConfigEntry<float>[10];

        public static ConfigEntry<string> GreatToxicMaceName;
        public static ConfigEntry<string> GreatToxicMaceDescription;
        public static ConfigEntry<string> GreatToxicMaceCraftingStation;
        public static readonly ConfigEntry<int>[] GreatToxicMaceCraftingLevels = new ConfigEntry<int>[2];
        public static readonly ConfigEntry<float>[] GreatToxicMaceDamages = new ConfigEntry<float>[10];
        public static readonly ConfigEntry<float>[] GreatToxicMaceDamagesPerUpgrade = new ConfigEntry<float>[10];

        public static ConfigEntry<string> GreatFrostMaceName;
        public static ConfigEntry<string> GreatFrostMaceDescription;
        public static ConfigEntry<string> GreatFrostMaceCraftingStation;
        public static readonly ConfigEntry<int>[] GreatFrostMaceCraftingLevels = new ConfigEntry<int>[2];
        public static readonly ConfigEntry<float>[] GreatFrostMaceDamages = new ConfigEntry<float>[10];
        public static readonly ConfigEntry<float>[] GreatFrostMaceDamagesPerUpgrade = new ConfigEntry<float>[10];

        public static ConfigEntry<string> SwordBlackMetalGreatName;
        public static ConfigEntry<string> SwordBlackMetalGreatDescription;
        public static ConfigEntry<string> SwordBlackMetalGreatCraftingStation;
        public static readonly ConfigEntry<int>[] SwordBlackMetalGreatCraftingLevels = new ConfigEntry<int>[2];
        public static readonly ConfigEntry<float>[] SwordBlackMetalGreatDamages = new ConfigEntry<float>[10];
        public static readonly ConfigEntry<float>[] SwordBlackMetalGreatDamagesPerUpgrade = new ConfigEntry<float>[10];

        public static ConfigEntry<string> SwordObsidianGreatName;
        public static ConfigEntry<string> SwordObsidianGreatDescription;
        public static ConfigEntry<string> SwordObsidianGreatCraftingStation;
        public static readonly ConfigEntry<int>[] SwordObsidianGreatCraftingLevels = new ConfigEntry<int>[2];
        public static readonly ConfigEntry<float>[] SwordObsidianGreatDamages = new ConfigEntry<float>[10];
        public static readonly ConfigEntry<float>[] SwordObsidianGreatDamagesPerUpgrade = new ConfigEntry<float>[10];

        public static ConfigEntry<string> SwordFlametalGreatName;
        public static ConfigEntry<string> SwordFlametalGreatDescription;
        public static ConfigEntry<string> SwordFlametalGreatCraftingStation;
        public static readonly ConfigEntry<int>[] SwordFlametalGreatCraftingLevels = new ConfigEntry<int>[2];
        public static readonly ConfigEntry<float>[] SwordFlametalGreatDamages = new ConfigEntry<float>[10];
        public static readonly ConfigEntry<float>[] SwordFlametalGreatDamagesPerUpgrade = new ConfigEntry<float>[10];

        //If both of these are true, set customItems to null as it's no longer needed.
        private static bool customRecipesAssembled = false;
        private static bool customItemsAssembled = false;

        //public static Dictionary<string, string> locDictionary = new Dictionary<string, string>();

        private void Awake()
        {
            _Harmony = new Harmony(GUID);
#if DEBUG
            Log = Logger;
#else
            Log = new ManualLogSource(null);
#endif
            EnabledWeaponRecipes[0] = Config.Bind("0Enabled Weapons", "b_SilverGreatSwordEnabled", true, "Enable Recipe for Silver Great Sword");
            EnabledWeaponRecipes[1] = Config.Bind("0Enabled Weapons", "b_GreatCoreMaceEnabled", true, "Enable Recipe for Great Core Mace");
            EnabledWeaponRecipes[2] = Config.Bind("0Enabled Weapons", "b_GreatToxicMaceEnabled", true, "Enable Recipe for Great Toxic Mace");
            EnabledWeaponRecipes[3] = Config.Bind("0Enabled Weapons", "b_GreatFrostMaceEnabled", true, "Enable Recipe for Great Frost Mace");
            EnabledWeaponRecipes[4] = Config.Bind("0Enabled Weapons", "b_BlackMetalGreatSwordEnabled", true, "Enable Recipe for Black Metal Great Sword");
            EnabledWeaponRecipes[5] = Config.Bind("0Enabled Weapons", "b_ModersSorrowEnabled", true, "Enable Recipe for Moder's Sorrow");
            EnabledWeaponRecipes[6] = Config.Bind("0Enabled Weapons", "b_FlametalGreatSwordEnabled", true, "Enable Recipe for Great Flametal Sword");

            //Silver Great Sword
            SwordSilverGreatName = Config.Bind("Silver Great Sword", "s_SSGName", "Silver Great Sword");
            SwordSilverGreatDescription = Config.Bind("Silver Great Sword", "s_SSGDescription", "A large silvered iron sword, favored for its range for cutting foes down, and for the silver to banish the undead.");

            SwordSilverGreatCraftingStation = Config.Bind("Silver Great Sword", "s_SSGCraftingStationName", "forge", "Crafting Station Piece Name valid options are : forge, piece_workbench, piece_cauldron, piece_stonecutter, piece_artisanstation");

            SwordSilverGreatCraftingLevels[0] = Config.Bind("Silver Great Sword", "i_SSGMinCraftingLevel", 1, "Min Crafting Level for Recipe");
            SwordSilverGreatCraftingLevels[1] = Config.Bind("Silver Great Sword", "i_SSGMaxCraftingLevel", 4, "Max Crafting Level for Upgrades");

            SwordSilverGreatDamages[0] = Config.Bind("Silver Great Sword", "f_SSGBlunt", 0f, "Blunt Damage");
            SwordSilverGreatDamages[1] = Config.Bind("Silver Great Sword", "f_SSGSlash", 105f, "Slash Damage");
            SwordSilverGreatDamages[2] = Config.Bind("Silver Great Sword", "f_SSGPierce", 0f, "Pierce Damage");
            SwordSilverGreatDamages[3] = Config.Bind("Silver Great Sword", "f_SSGChop", 0f, "Chop Damage");
            SwordSilverGreatDamages[4] = Config.Bind("Silver Great Sword", "f_SSGPickaxe", 0f, "Pickaxe Damage");
            SwordSilverGreatDamages[5] = Config.Bind("Silver Great Sword", "f_SSGFire", 0f, "Fire Damage");
            SwordSilverGreatDamages[6] = Config.Bind("Silver Great Sword", "f_SSGFrost", 0f, "Frost Damage");
            SwordSilverGreatDamages[7] = Config.Bind("Silver Great Sword", "f_SSGLightning", 0f, "Lightning Damage");
            SwordSilverGreatDamages[8] = Config.Bind("Silver Great Sword", "f_SSGPoison", 0f, "Poison Damage");
            SwordSilverGreatDamages[9] = Config.Bind("Silver Great Sword", "f_SSGSpirit", 42f, "Spirit Damage");

            SwordSilverGreatDamagesPerUpgrade[0] = Config.Bind("Silver Great Sword", "f_SSGBluntPerLevel", 0f, "Blunt Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[1] = Config.Bind("Silver Great Sword", "f_SSGSlashPerLevel", 8f, "Slash Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[2] = Config.Bind("Silver Great Sword", "f_SSGPiercePerLevel", 0f, "Pierce Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[3] = Config.Bind("Silver Great Sword", "f_SSGChopPerLevel", 0f, "Chop Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[4] = Config.Bind("Silver Great Sword", "f_SSGPickaxePerLevel", 0f, "Pickaxe Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[5] = Config.Bind("Silver Great Sword", "f_SSGFirePerLevel", 0f, "Fire Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[6] = Config.Bind("Silver Great Sword", "f_SSGFrostPerLevel", 0f, "Frost Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[7] = Config.Bind("Silver Great Sword", "f_SSGLightningPerLevel", 0f, "Lightning Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[8] = Config.Bind("Silver Great Sword", "f_SSGPoisonPerLevel", 0f, "Poison Damage Per Level");
            SwordSilverGreatDamagesPerUpgrade[9] = Config.Bind("Silver Great Sword", "f_SSGSpiritPerLevel", 7f, "Spirit Damage Per Level");

            //Maces
            GreatCoreMaceName = Config.Bind("Great Core Mace", "s_MCGName", "Great Core Mace");
            GreatCoreMaceDescription = Config.Bind("Great Core Mace", "s_MCGDescription", "A hefty mace powered by a surtling core to allow you to immolate your foes as you crush them.");

            GreatCoreMaceCraftingStation = Config.Bind("Great Core Mace", "s_MCGCraftingStationName", "forge", "Crafting Station Piece Name valid options are : forge, piece_workbench, piece_cauldron, piece_stonecutter, piece_artisanstation");

            GreatCoreMaceCraftingLevels[0] = Config.Bind("Great Core Mace", "i_MCGMinCraftingLevel", 1, "Min Crafting Level for Recipe");
            GreatCoreMaceCraftingLevels[1] = Config.Bind("Great Core Mace", "i_MCGMaxCraftingLevel", 4, "Max Crafting Level for Upgrades");

            GreatCoreMaceDamages[0] = Config.Bind("Great Core Mace", "f_MCGBlunt", 105f, "Blunt Damage");
            GreatCoreMaceDamages[1] = Config.Bind("Great Core Mace", "f_MCGSlash", 0f, "Slash Damage");
            GreatCoreMaceDamages[2] = Config.Bind("Great Core Mace", "f_MCGPierce", 0f, "Pierce Damage");
            GreatCoreMaceDamages[3] = Config.Bind("Great Core Mace", "f_MCGChop", 0f, "Chop Damage");
            GreatCoreMaceDamages[4] = Config.Bind("Great Core Mace", "f_MCGPickaxe", 0f, "Pickaxe Damage");
            GreatCoreMaceDamages[5] = Config.Bind("Great Core Mace", "f_MCGFire", 45f, "Fire Damage");
            GreatCoreMaceDamages[6] = Config.Bind("Great Core Mace", "f_MCGFrost", 0f, "Frost Damage");
            GreatCoreMaceDamages[7] = Config.Bind("Great Core Mace", "f_MCGLightning", 0f, "Lightning Damage");
            GreatCoreMaceDamages[8] = Config.Bind("Great Core Mace", "f_MCGPoison", 0f, "Poison Damage");
            GreatCoreMaceDamages[9] = Config.Bind("Great Core Mace", "f_MCGSpirit", 0f, "Spirit Damage");

            GreatCoreMaceDamagesPerUpgrade[0] = Config.Bind("Great Core Mace", "f_MCGBluntPerLevel", 8f, "Blunt Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[1] = Config.Bind("Great Core Mace", "f_MCGSlashPerLevel", 0f, "Slash Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[2] = Config.Bind("Great Core Mace", "f_MCGPiercePerLevel", 0f, "Pierce Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[3] = Config.Bind("Great Core Mace", "f_MCGChopPerLevel", 0f, "Chop Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[4] = Config.Bind("Great Core Mace", "f_MCGPickaxePerLevel", 0f, "Pickaxe Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[5] = Config.Bind("Great Core Mace", "f_MCGFirePerLevel", 15f, "Fire Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[6] = Config.Bind("Great Core Mace", "f_MCGFrostPerLevel", 0f, "Frost Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[7] = Config.Bind("Great Core Mace", "f_MCGLightningPerLevel", 0f, "Lightning Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[8] = Config.Bind("Great Core Mace", "f_MCGPoisonPerLevel", 0f, "Poison Damage Per Level");
            GreatCoreMaceDamagesPerUpgrade[9] = Config.Bind("Great Core Mace", "f_MCGSpiritPerLevel", 0f, "Spirit Damage Per Level");


            GreatToxicMaceName = Config.Bind("Great Toxic Mace", "s_MTGName", "Great Toxic Mace");
            GreatToxicMaceDescription = Config.Bind("Great Toxic Mace", "s_MTGDescription", "A hefty mace haressing guck to allow you to poison your foes as you crush them.");

            GreatToxicMaceCraftingStation = Config.Bind("Great Toxic Mace", "s_MTGCraftingStationName", "forge", "Crafting Station Piece Name valid options are : forge, piece_workbench, piece_cauldron, piece_stonecutter, piece_artisanstation");

            GreatToxicMaceCraftingLevels[0] = Config.Bind("Great Toxic Mace", "i_MTGMinCraftingLevel", 1, "Min Crafting Level for Recipe");
            GreatToxicMaceCraftingLevels[1] = Config.Bind("Great Toxic Mace", "i_MTGMaxCraftingLevel", 4, "Max Crafting Level for Upgrades");

            GreatToxicMaceDamages[0] = Config.Bind("Great Toxic Mace", "f_MTGBlunt", 105f, "Blunt Damage");
            GreatToxicMaceDamages[1] = Config.Bind("Great Toxic Mace", "f_MTGSlash", 0f, "Slash Damage");
            GreatToxicMaceDamages[2] = Config.Bind("Great Toxic Mace", "f_MTGPierce", 0f, "Pierce Damage");
            GreatToxicMaceDamages[3] = Config.Bind("Great Toxic Mace", "f_MTGChop", 0f, "Chop Damage");
            GreatToxicMaceDamages[4] = Config.Bind("Great Toxic Mace", "f_MTGPickaxe", 0f, "Pickaxe Damage");
            GreatToxicMaceDamages[5] = Config.Bind("Great Toxic Mace", "f_MTGFire", 0f, "Fire Damage");
            GreatToxicMaceDamages[6] = Config.Bind("Great Toxic Mace", "f_MTGFrost", 0f, "Frost Damage");
            GreatToxicMaceDamages[7] = Config.Bind("Great Toxic Mace", "f_MTGLightning", 0f, "Lightning Damage");
            GreatToxicMaceDamages[8] = Config.Bind("Great Toxic Mace", "f_MTGPoison", 45f, "Poison Damage");
            GreatToxicMaceDamages[9] = Config.Bind("Great Toxic Mace", "f_MTGSpirit", 0f, "Spirit Damage");

            GreatToxicMaceDamagesPerUpgrade[0] = Config.Bind("Great Toxic Mace", "f_MTGBluntPerLevel", 8f, "Blunt Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[1] = Config.Bind("Great Toxic Mace", "f_MTGSlashPerLevel", 0f, "Slash Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[2] = Config.Bind("Great Toxic Mace", "f_MTGPiercePerLevel", 0f, "Pierce Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[3] = Config.Bind("Great Toxic Mace", "f_MTGChopPerLevel", 0f, "Chop Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[4] = Config.Bind("Great Toxic Mace", "f_MTGPickaxePerLevel", 0f, "Pickaxe Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[5] = Config.Bind("Great Toxic Mace", "f_MTGFirePerLevel", 0f, "Fire Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[6] = Config.Bind("Great Toxic Mace", "f_MTGFrostPerLevel", 0f, "Frost Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[7] = Config.Bind("Great Toxic Mace", "f_MTGLightningPerLevel", 0f, "Lightning Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[8] = Config.Bind("Great Toxic Mace", "f_MTGPoisonPerLevel", 15f, "Poison Damage Per Level");
            GreatToxicMaceDamagesPerUpgrade[9] = Config.Bind("Great Toxic Mace", "f_MTGSpiritPerLevel", 0f, "Spirit Damage Per Level");


            GreatFrostMaceName = Config.Bind("Great Frost Mace", "s_MFGName", "Great Frost Mace");
            GreatFrostMaceDescription = Config.Bind("Great Frost Mace", "s_MFGDescription", "A hefty mace powered by a golem's crystal to allow you to freeze your foes as you crush them.");

            GreatFrostMaceCraftingStation = Config.Bind("Great Frost Mace", "s_MFGCraftingStationName", "forge", "Crafting Station Piece Name valid options are : forge, piece_workbench, piece_cauldron, piece_stonecutter, piece_artisanstation");

            GreatFrostMaceCraftingLevels[0] = Config.Bind("Great Frost Mace", "i_MFGMinCraftingLevel", 1, "Min Crafting Level for Recipe");
            GreatFrostMaceCraftingLevels[1] = Config.Bind("Great Frost Mace", "i_MFGMaxCraftingLevel", 4, "Max Crafting Level for Upgrades");

            GreatFrostMaceDamages[0] = Config.Bind("Great Frost Mace", "f_MFGBlunt", 105f, "Blunt Damage");
            GreatFrostMaceDamages[1] = Config.Bind("Great Frost Mace", "f_MFGSlash", 0f, "Slash Damage");
            GreatFrostMaceDamages[2] = Config.Bind("Great Frost Mace", "f_MFGPierce", 0f, "Pierce Damage");
            GreatFrostMaceDamages[3] = Config.Bind("Great Frost Mace", "f_MFGChop", 0f, "Chop Damage");
            GreatFrostMaceDamages[4] = Config.Bind("Great Frost Mace", "f_MFGPickaxe", 0f, "Pickaxe Damage");
            GreatFrostMaceDamages[5] = Config.Bind("Great Frost Mace", "f_MFGFire", 0f, "Fire Damage");
            GreatFrostMaceDamages[6] = Config.Bind("Great Frost Mace", "f_MFGFrost", 45f, "Frost Damage");
            GreatFrostMaceDamages[7] = Config.Bind("Great Frost Mace", "f_MFGLightning", 0f, "Lightning Damage");
            GreatFrostMaceDamages[8] = Config.Bind("Great Frost Mace", "f_MFGPoison", 0f, "Poison Damage");
            GreatFrostMaceDamages[9] = Config.Bind("Great Frost Mace", "f_MFGSpirit", 0f, "Spirit Damage");

            GreatFrostMaceDamagesPerUpgrade[0] = Config.Bind("Great Frost Mace", "f_MFGBluntPerLevel", 8f, "Blunt Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[1] = Config.Bind("Great Frost Mace", "f_MFGSlashPerLevel", 0f, "Slash Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[2] = Config.Bind("Great Frost Mace", "f_MFGPiercePerLevel", 0f, "Pierce Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[3] = Config.Bind("Great Frost Mace", "f_MFGChopPerLevel", 0f, "Chop Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[4] = Config.Bind("Great Frost Mace", "f_MFGPickaxePerLevel", 0f, "Pickaxe Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[5] = Config.Bind("Great Frost Mace", "f_MFGFirePerLevel", 0f, "Fire Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[6] = Config.Bind("Great Frost Mace", "f_MFGFrostPerLevel", 15f, "Frost Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[7] = Config.Bind("Great Frost Mace", "f_MFGLightningPerLevel", 0f, "Lightning Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[8] = Config.Bind("Great Frost Mace", "f_MFGPoisonPerLevel", 0f, "Poison Damage Per Level");
            GreatFrostMaceDamagesPerUpgrade[9] = Config.Bind("Great Frost Mace", "f_MFGSpiritPerLevel", 0f, "Spirit Damage Per Level");

            //Black Metal

            SwordBlackMetalGreatName = Config.Bind("Black Metal Great Sword", "s_SBMGName", "Black Metal Great Sword");
            SwordBlackMetalGreatDescription = Config.Bind("Black Metal Great Sword", "s_SBMGDescription", "A large wavy great sword, made from a black metal that has a glossy green sheen to it.");

            SwordBlackMetalGreatCraftingStation = Config.Bind("Black Metal Great Sword", "s_SBMGCraftingStationName", "forge", "Crafting Station Piece Name valid options are : forge, piece_workbench, piece_cauldron, piece_stonecutter, piece_artisanstation");

            SwordBlackMetalGreatCraftingLevels[0] = Config.Bind("Black Metal Great Sword", "i_SBMGMinCraftingLevel", 1, "Min Crafting Level for Recipe");
            SwordBlackMetalGreatCraftingLevels[1] = Config.Bind("Black Metal Great Sword", "i_SBMGMaxCraftingLevel", 4, "Max Crafting Level for Upgrades");

            SwordBlackMetalGreatDamages[0] = Config.Bind("Black Metal Great Sword", "f_SBMGBlunt", 0f, "Blunt Damage");
            SwordBlackMetalGreatDamages[1] = Config.Bind("Black Metal Great Sword", "f_SBMGSlash", 133f, "Slash Damage");
            SwordBlackMetalGreatDamages[2] = Config.Bind("Black Metal Great Sword", "f_SBMGPierce", 0f, "Pierce Damage");
            SwordBlackMetalGreatDamages[3] = Config.Bind("Black Metal Great Sword", "f_SBMGChop", 0f, "Chop Damage");
            SwordBlackMetalGreatDamages[4] = Config.Bind("Black Metal Great Sword", "f_SBMGPickaxe", 0f, "Pickaxe Damage");
            SwordBlackMetalGreatDamages[5] = Config.Bind("Black Metal Great Sword", "f_SBMGFire", 0f, "Fire Damage");
            SwordBlackMetalGreatDamages[6] = Config.Bind("Black Metal Great Sword", "f_SBMGFrost", 0f, "Frost Damage");
            SwordBlackMetalGreatDamages[7] = Config.Bind("Black Metal Great Sword", "f_SBMGLightning", 0f, "Lightning Damage");
            SwordBlackMetalGreatDamages[8] = Config.Bind("Black Metal Great Sword", "f_SBMGPoison", 0f, "Poison Damage");
            SwordBlackMetalGreatDamages[9] = Config.Bind("Black Metal Great Sword", "f_SBMGSpirit", 0f, "Spirit Damage");

            SwordBlackMetalGreatDamagesPerUpgrade[0] = Config.Bind("Black Metal Great Sword", "f_SBMGBluntPerLevel", 0f, "Blunt Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[1] = Config.Bind("Black Metal Great Sword", "f_SBMGSlashPerLevel", 9f, "Slash Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[2] = Config.Bind("Black Metal Great Sword", "f_SBMGPiercePerLevel", 0f, "Pierce Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[3] = Config.Bind("Black Metal Great Sword", "f_SBMGChopPerLevel", 0f, "Chop Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[4] = Config.Bind("Black Metal Great Sword", "f_SBMGPickaxePerLevel", 0f, "Pickaxe Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[5] = Config.Bind("Black Metal Great Sword", "f_SBMGFirePerLevel", 0f, "Fire Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[6] = Config.Bind("Black Metal Great Sword", "f_SBMGFrostPerLevel", 0f, "Frost Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[7] = Config.Bind("Black Metal Great Sword", "f_SBMGLightningPerLevel", 0f, "Lightning Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[8] = Config.Bind("Black Metal Great Sword", "f_SBMGPoisonPerLevel", 0f, "Poison Damage Per Level");
            SwordBlackMetalGreatDamagesPerUpgrade[9] = Config.Bind("Black Metal Great Sword", "f_SBMGSpiritPerLevel", 0f, "Spirit Damage Per Level");

            //Obisidian Sword

            SwordObsidianGreatName = Config.Bind("Obsidian Great Sword", "s_SOGName", "Moder's Sorrow");
            SwordObsidianGreatDescription = Config.Bind("Obsidian Great Sword", "s_SOGDescription", "Moder's sorrow made tangible, those that use this can freeze the very souls of who they hit.");

            SwordObsidianGreatCraftingStation = Config.Bind("Obsidian Great Sword", "s_SOGCraftingStationName", "forge", "Crafting Station Piece Name valid options are : forge, piece_workbench, piece_cauldron, piece_stonecutter, piece_artisanstation");

            SwordObsidianGreatCraftingLevels[0] = Config.Bind("Obsidian Great Sword", "i_SOGMinCraftingLevel", 1, "Min Crafting Level for Recipe");
            SwordObsidianGreatCraftingLevels[1] = Config.Bind("Obsidian Great Sword", "i_SOGMaxCraftingLevel", 4, "Max Crafting Level for Upgrades");

            SwordObsidianGreatDamages[0] = Config.Bind("Obsidian Great Sword", "f_SOGBlunt", 65f, "Blunt Damage");
            SwordObsidianGreatDamages[1] = Config.Bind("Obsidian Great Sword", "f_SOGSlash", 55f, "Slash Damage");
            SwordObsidianGreatDamages[2] = Config.Bind("Obsidian Great Sword", "f_SOGPierce", 0f, "Pierce Damage");
            SwordObsidianGreatDamages[3] = Config.Bind("Obsidian Great Sword", "f_SOGChop", 0f, "Chop Damage");
            SwordObsidianGreatDamages[4] = Config.Bind("Obsidian Great Sword", "f_SOGPickaxe", 0f, "Pickaxe Damage");
            SwordObsidianGreatDamages[5] = Config.Bind("Obsidian Great Sword", "f_SOGFire", 0f, "Fire Damage");
            SwordObsidianGreatDamages[6] = Config.Bind("Obsidian Great Sword", "f_SOGFrost", 90f, "Frost Damage");
            SwordObsidianGreatDamages[7] = Config.Bind("Obsidian Great Sword", "f_SOGLightning", 0f, "Lightning Damage");
            SwordObsidianGreatDamages[8] = Config.Bind("Obsidian Great Sword", "f_SOGPoison", 0f, "Poison Damage");
            SwordObsidianGreatDamages[9] = Config.Bind("Obsidian Great Sword", "f_SOGSpirit", 0f, "Spirit Damage");

            SwordObsidianGreatDamagesPerUpgrade[0] = Config.Bind("Obsidian Great Sword", "f_SOGBluntPerLevel", 8f, "Blunt Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[1] = Config.Bind("Obsidian Great Sword", "f_SOGSlashPerLevel", 8f, "Slash Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[2] = Config.Bind("Obsidian Great Sword", "f_SOGPiercePerLevel", 0f, "Pierce Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[3] = Config.Bind("Obsidian Great Sword", "f_SOGChopPerLevel", 0f, "Chop Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[4] = Config.Bind("Obsidian Great Sword", "f_SOGPickaxePerLevel", 0f, "Pickaxe Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[5] = Config.Bind("Obsidian Great Sword", "f_SOGFirePerLevel", 0f, "Fire Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[6] = Config.Bind("Obsidian Great Sword", "f_SOGFrostPerLevel", 15f, "Frost Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[7] = Config.Bind("Obsidian Great Sword", "f_SOGLightningPerLevel", 0f, "Lightning Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[8] = Config.Bind("Obsidian Great Sword", "f_SOGPoisonPerLevel", 0f, "Poison Damage Per Level");
            SwordObsidianGreatDamagesPerUpgrade[9] = Config.Bind("Obsidian Great Sword", "f_SOGSpiritPerLevel", 0f, "Spirit Damage Per Level");

            //Flametal Sword

            SwordFlametalGreatName = Config.Bind("Flametal Great Sword", "s_SFGName", "Flametal Great Sword");
            SwordFlametalGreatDescription = Config.Bind("Flametal Great Sword", "s_SFGDescription", "The blade crafted from Flametal burns with such heat that it warms the air around it, ");

            SwordFlametalGreatCraftingStation = Config.Bind("Flametal Great Sword", "s_SFGCraftingStationName", "forge", "Crafting Station Piece Name valid options are : forge, piece_workbench, piece_cauldron, piece_stonecutter, piece_artisanstation");

            SwordFlametalGreatCraftingLevels[0] = Config.Bind("Flametal Great Sword", "i_SFGMinCraftingLevel", 1, "Min Crafting Level for Recipe");
            SwordFlametalGreatCraftingLevels[1] = Config.Bind("Flametal Great Sword", "i_SFGMaxCraftingLevel", 4, "Max Crafting Level for Upgrades");

            SwordFlametalGreatDamages[0] = Config.Bind("Flametal Great Sword", "f_SFGBlunt", 0f, "Blunt Damage");
            SwordFlametalGreatDamages[1] = Config.Bind("Flametal Great Sword", "f_SFGSlash", 135f, "Slash Damage");
            SwordFlametalGreatDamages[2] = Config.Bind("Flametal Great Sword", "f_SFGPierce", 0f, "Pierce Damage");
            SwordFlametalGreatDamages[3] = Config.Bind("Flametal Great Sword", "f_SFGChop", 0f, "Chop Damage");
            SwordFlametalGreatDamages[4] = Config.Bind("Flametal Great Sword", "f_SFGPickaxe", 0f, "Pickaxe Damage");
            SwordFlametalGreatDamages[5] = Config.Bind("Flametal Great Sword", "f_SFGFire", 75f, "Fire Damage");
            SwordFlametalGreatDamages[6] = Config.Bind("Flametal Great Sword", "f_SFGFrost", 0f, "Frost Damage");
            SwordFlametalGreatDamages[7] = Config.Bind("Flametal Great Sword", "f_SFGLightning", 0f, "Lightning Damage");
            SwordFlametalGreatDamages[8] = Config.Bind("Flametal Great Sword", "f_SFGPoison", 0f, "Poison Damage");
            SwordFlametalGreatDamages[9] = Config.Bind("Flametal Great Sword", "f_SFGSpirit", 0f, "Spirit Damage");

            SwordFlametalGreatDamagesPerUpgrade[0] = Config.Bind("Flametal Great Sword", "f_SFGBluntPerLevel", 0f, "Blunt Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[1] = Config.Bind("Flametal Great Sword", "f_SFGSlashPerLevel", 16f, "Slash Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[2] = Config.Bind("Flametal Great Sword", "f_SFGPiercePerLevel", 0f, "Pierce Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[3] = Config.Bind("Flametal Great Sword", "f_SFGChopPerLevel", 0f, "Chop Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[4] = Config.Bind("Flametal Great Sword", "f_SFGPickaxePerLevel", 0f, "Pickaxe Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[5] = Config.Bind("Flametal Great Sword", "f_SFGFirePerLevel", 15f, "Fire Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[6] = Config.Bind("Flametal Great Sword", "f_SFGFrostPerLevel", 0f, "Frost Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[7] = Config.Bind("Flametal Great Sword", "f_SFGLightningPerLevel", 0f, "Lightning Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[8] = Config.Bind("Flametal Great Sword", "f_SFGPoisonPerLevel", 0f, "Poison Damage Per Level");
            SwordFlametalGreatDamagesPerUpgrade[9] = Config.Bind("Flametal Great Sword", "f_SFGSpiritPerLevel", 0f, "Spirit Damage Per Level");

            //This Static Class just fills the ItemList and RecipeLists
            ItemManager.BuildLists();

            //locDictionary.Add("htdvmthSwordSilverGreatName", "Silver Great Sword");
            //locDictionary.Add("htdvmthSwordSilverGreatDesc", "A large silvered iron sword, favored for its range for cutting foes down, and for the silver to banish the undead.");
            //locDictionary.Add("htdvmthGreatCoreMaceName", "Great Core Mace");
            //locDictionary.Add("htdvmthGreatCoreMaceDesc", "A hefty mace powered by a surtling core to allow you to immolate your foes and you crush them.");
            //locDictionary.Add("htdvmthGreatCoreMaceGreenName", "Great Core Mace (Green)");
            //locDictionary.Add("htdvmthGreatCoreMaceGreenDesc", "A hefty mace powered by a surtling core to allow you to immolate your foes and you crush them. It has been infused with guck giving it a green hue.");

            //locDictionary.Add("htdvmthSwordObsidianGreatName", "Moder's Sorrow");
            //locDictionary.Add("htdvmthSwordObsidianGreatDesc", "Moder's sorrow made tangible, those that use this can freeze the very souls of who they hit.");

            //locDictionary.Add("htdvmthSwordBlackmetalGreatName", "Black Metal Great Sword");
            //locDictionary.Add("htdvmthSwordBlackmetalGreatDesc", "A large wavy great sword, made from a black metal that has a glossy green sheen to it.");

            // _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
            _Harmony.PatchAll();

        }

        private void OnDestroy()
        {
            if (_Harmony != null) _Harmony.UnpatchAll(GUID);
        }

        //[HarmonyPatch(typeof(Localization), "SetupLanguage")]
        //private static class Localization_SetupLanguage_Patch
        //{
        //    public static Dictionary<string, string> m_translations;
        //    public static void Postfix(ref Dictionary<string, string> ___m_translations)
        //    {
        //        m_translations = ___m_translations;
        //        foreach (KeyValuePair<string, string> kvp in locDictionary)
        //        {
        //            AddWord(kvp.Key, kvp.Value);
        //        }
        //    }

        //    public static void AddWord(string key, string value)
        //    {
        //        m_translations.Remove(key);
        //        m_translations.Add(key, value);
        //    }
        //}

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        private static class ZNetScene_Awake_Patch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance == null)
                {
                    return;
                }

                if (AssetReferences.myItemList.Count > 0)
                {
                    foreach (GameObject gameObject in AssetReferences.myItemList)
                    {
                        if (!__instance.m_prefabs.Contains(gameObject))
                            __instance.m_prefabs.Add(gameObject);
                    }
                }
                else return;
            }

            //public static void Postfix(ZNetScene __instance)
            //{
            //    if (__instance == null)
            //    {
            //        return;
            //    }

            //    foreach (var prefab in __instance.m_prefabs)
            //    {
            //        Log.LogMessage($"Prefab in ZNetScene: {prefab.name}");
            //    }
            //}
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        private static class ObjectDB_CopyOtherDB_Patch
        {
            public static void Postfix()
            {
                if (!IsObjectDBValid()) return;

                GenerateReferenceLists();
                AddNewItems();
                //AddStationExtension("DragonTear", "piece_artisanstation");
                AddNewRecipes();
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        private static class ObjectDB_Awake_Patch
        {
            public static void Postfix()
            {
                if (!IsObjectDBValid()) return;

                GenerateReferenceLists();
                AddNewItems();
                //AddStationExtension("DragonTear", "piece_artisanstation");
                AddNewRecipes();
            }
        }


        //Dick Justice's and RandyKnapp's hack for ensuring the ObjectDB has objects in it and it's ready
        private static bool IsObjectDBValid()
        {
            return ObjectDB.instance != null && ObjectDB.instance.m_items.Count != 0 && ObjectDB.instance.GetItemPrefab("Amber") != null;
        }

        private static void GenerateReferenceLists()
        {

            //Start surfing through all items
            foreach (GameObject go in ObjectDB.instance.m_items)
            {
                //Log.LogMessage($"Prefab in ObjectDB: {go.name}");
                //Add to reference lists if not in their already
                AssetReferences.TryAddToItemList(go);

                ItemDrop id = go.GetComponent<ItemDrop>();
                if (id != null)
                {
                    var shared = id.m_itemData.m_shared;
                    //Start looking for weapon effects (fx, sfx, vfx)
                    if (shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon || shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon || shared.m_itemType == ItemDrop.ItemData.ItemType.Bow)
                    {
                        AssetReferences.TryExtractEffectsFromItemDropShared(shared);
                    }

                    //Check to see if item can also build things
                    if (shared.m_buildPieces != null)
                    {
                        var pieceTable = shared.m_buildPieces.m_pieces;
                        if (pieceTable != null)
                        {
                            foreach (var pieceTableItem in pieceTable)
                            {
                                AssetReferences.TryAddToPieceList(pieceTableItem);
                                CraftingStation craftingStation = pieceTableItem.GetComponent<CraftingStation>();
                                if (craftingStation != null && !AssetReferences.listOfCraftingStations.ContainsKey(pieceTableItem.name)) AssetReferences.listOfCraftingStations.Add(pieceTableItem.name, craftingStation);
                                StationExtension stationExtension = pieceTableItem.GetComponent<StationExtension>();
                                if (stationExtension != null && !AssetReferences.listOfEffects.ContainsKey(stationExtension.m_connectionPrefab.name)) AssetReferences.listOfEffects.Add(stationExtension.m_connectionPrefab.name, stationExtension.m_connectionPrefab);
                            }
                        }
                    }
                }
            }
        }

        //private static void AddStationExtension(string preFabName, string craftingStation)
        //{
        //    try
        //    {
        //        var prefab = AssetReferences.listOfItemPrefabs[preFabName];
        //        if (prefab == null) prefab = ObjectDB.instance.GetItemPrefab(preFabName);
        //        StationExtension se = prefab.GetComponent<StationExtension>();
        //        if (se == null)
        //        {
        //            se = prefab.transform.Find("attach").gameObject.AddComponent<StationExtension>();
        //            se.m_craftingStation = AssetReferences.listOfCraftingStations[craftingStation];
        //            se.m_connectionPrefab = AssetReferences.listOfEffects["vfx_ExtensionConnection"];
        //            Log.LogMessage($"Added Station Extension to {preFabName}'s 'attach' for crafting station {craftingStation}");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Plugin.Log.LogError($"Error in adding StationExtension to {preFabName} for crafting station {craftingStation}");
        //        Plugin.Log.LogError($"Catch Exception details: {e.Message} --- {e.StackTrace}");
        //    }
        //}

        private static void AddNewItems()
        {

            Dictionary<int, GameObject> m_itemsByHash = (Dictionary<int, GameObject>)typeof(ObjectDB).GetField("m_itemByHash", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ObjectDB.instance);

            if (AssetReferences.myItemList.Count > 0)
            {
                foreach (var item in AssetReferences.myItemList)
                {
                    AddItemToObjectDB(item, ref m_itemsByHash);
                }
            }
            else
            {
                foreach (var customItem in AssetReferences.customItems)
                {
                    customItem.FixReferences();
                    var itemGameObject = customItem.gameObject;
                    if (!AssetReferences.myItemList.Contains(itemGameObject)) AssetReferences.myItemList.Add(itemGameObject);
                    AddItemToObjectDB(itemGameObject, ref m_itemsByHash);
                }
                customItemsAssembled = true;
                WipeCustomItemList();
            }
        }

        private static void AddItemToObjectDB(GameObject item, ref Dictionary<int, GameObject> objectDBItemHash)
        {
            var itemDrop = item.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                if (ObjectDB.instance.GetItemPrefab(item.name.GetStableHashCode()) == null)
                {
                    ObjectDB.instance.m_items.Add(item);
                    objectDBItemHash[item.name.GetStableHashCode()] = item;
                }
            }
        }

        public static Recipe ExtractRecipeFromObjectDBByRecipeName(string recipeName)
        {
            return ObjectDB.instance.m_recipes.Where(r => r.name == recipeName).FirstOrDefault();
        }

        private static void AddNewRecipes()
        {

            if (AssetReferences.myRecipeList.Count > 0)
            {
                foreach (var recipe in AssetReferences.myRecipeList)
                {
                    AddRecipeToObjectDB(recipe);
                }
            }
            else
            {
                foreach (var customItem in AssetReferences.customItems)
                {
                    if (customItem.recipeEnabled)
                    {
                        var recipe = customItem.recipe.GetRecipe();
                        if (!AssetReferences.myRecipeList.Contains(recipe)) AssetReferences.myRecipeList.Add(recipe);
                        AddRecipeToObjectDB(recipe);
                    }
                }
                customRecipesAssembled = true;
                WipeCustomItemList();
            }
        }

        private static void AddRecipeToObjectDB(Recipe recipe)
        {
            //Sadly I'm not sure why this is here, but RandyKnapp had it
            var removed = ObjectDB.instance.m_recipes.RemoveAll(x => x.name == recipe.name);
            if (removed > 0)
            {
                Log.LogMessage($"Recipe ({recipe.name}): {removed} duplicated instance(s) removed.");
            }
            ObjectDB.instance.m_recipes.Add(recipe);
        }

        private static void WipeCustomItemList()
        {
            if (customItemsAssembled && customRecipesAssembled) AssetReferences.customItems = null;
        }
    }
}
