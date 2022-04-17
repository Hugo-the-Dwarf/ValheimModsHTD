using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;
using System;

namespace ValheimHTDArmory
{
    [BepInPlugin(Plugin.GUID, Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "7.1.0";
        public const string ModName = "Hugos Armory";
        public const string GUID = "htd.armory";
        public static ServerSync.ConfigSync configSync = new ServerSync.ConfigSync(GUID) { DisplayName = ModName, CurrentVersion = Version };

        Harmony _Harmony;
        public static ManualLogSource Log;
        public readonly Harmony harmony = new Harmony(GUID);


        public static CustomConfig cc = new CustomConfig();
        public static CustomLocalization cl = new CustomLocalization();

        public static List<GameObject> myItemList = new List<GameObject>(); //Fixed Referenced Compiled Items
        public static List<CustomItem> customItems = new List<CustomItem>(); // Uncompiled Items
        // public static List<CustomItem> customArmor = new List<CustomItem>(); // Uncompiled Items

        public static List<Recipe> myRecipeList = new List<Recipe>(); // Fixed Referenced Compiled Recipes
        public static List<RecipeHelper> myRecipeHelperList = new List<RecipeHelper>(); // uncompiled recipes

        public static List<CustomPiece> customPieces = new List<CustomPiece>();
        public static List<CookingRecipe> myCookingRecipes = new List<CookingRecipe>();


        //BepinEx Config Values
        public static bool disableAttackSpeedModule = false;
        public static bool disableFlametalFlames = false;
        public static bool disableSilverBattleaxeLights = false;

        public static bool holdIronGreatswordByBlade = true;
        public static bool holdSilverGreatswordByBlade = true;
        public static bool holdBlackMetalAltGreatswordByBlade = true;


        private static bool fixedReferences = false;

        private void Awake()
        {
            _Harmony = new Harmony(GUID);
#if DEBUG
            Log = Logger;
#else
            Log = new ManualLogSource(null);
#endif
            disableAttackSpeedModule = Config.Bind<bool>("0Modules", "disable_AttackSpeedModule", false, "Disable the attack speed module, you may want to do this if you find conflicts with other mods that change attack speeds.").Value;

            PlayerAttackInputPatch.attack3Hotkey = Config.Bind<string>("1Hotkeys", "hotkey_ThirdAttack", "mouse 3", "Customizable hotkey so you can use the third attack of the weapon. If you want to use a mouse key, include a space: mouse 3, for example. Valid inputs: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            disableFlametalFlames = Config.Bind<bool>("2Options", "disable_FlametalSwordFlames", false, "Disable the fire and smoke effect from the Flametal Great Sword.").Value;
            disableSilverBattleaxeLights = Config.Bind<bool>("2Options", "disable_SilverBattleaxeLights", false, "Disable the flickering lights from the Silver Battleaxe.").Value;


            holdIronGreatswordByBlade = Config.Bind<bool>("2Options", "hold_IronGreatswordByBlade", true, "If this is true, holds the sword like one would a battleaxe. If this is false hold the sword resting on the shoulder.").Value;
            holdSilverGreatswordByBlade = Config.Bind<bool>("2Options", "hold_SilverGreatswordByBlade", true, "If this is true, holds the sword like one would a battleaxe. If this is false hold the sword resting on the shoulder.").Value;
            holdBlackMetalAltGreatswordByBlade = Config.Bind<bool>("2Options", "hold_BlackMetalAltGreatswordByBlade", true, "If this is true, holds the sword like one would a battleaxe. If this is false hold the sword resting on the shoulder.").Value;

            string path = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), GUID);
            cc.LoadInitialConfigs(path);
            cl.LoadLocalization(path);

            //This Static Class just fills the ItemList and RecipeLists
            try
            {
                ItemManager.BuildLists();
            }
            catch (Exception e)
            {
                Log.LogError("Problem with Setting up item prefabs. ItemManager.");
                Log.LogError(e.Message);
                Log.LogError(e.StackTrace);
            }

            cc.WriteConfigs(path);

            // _Harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);


            /*
                // add null checks to the following lines, they are omitted for clarity
                var original = typeof(TheClass).GetMethod("TheMethod");
                var prefix = typeof(MyPatchClass1).GetMethod("SomeMethod");
                var postfix = typeof(MyPatchClass2).GetMethod("SomeMethod");

                harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

                // You can use named arguments to specify certain patch types only:
                harmony.Patch(original, postfix: new HarmonyMethod(postfix));
                harmony.Patch(original, prefix: new HarmonyMethod(prefix), transpiler: new HarmonyMethod(transpiler));
            */

            _Harmony.PatchAll();
        }

        private void OnDestroy()
        {
            //if (_Harmony != null) _Harmony.UnpatchAll(GUID);
        }

        //TODO make fixing references a repeating invoked method that will stop it's repeating
        private void Update()
        {
            if (fixedReferences) return;
            if (!IsObjectDBValid()) return;
            GenerateReferenceLists();
            FixRecipeReferences();
            AddNewItems();
            AddNewRecipes();
            AddNewStatusEffects();
            FixAndAddNewPieces();
            //WipeReferenceLists();
            fixedReferences = true;
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        private static class ZNetScene_Awake_Patch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance == null)
                {
                    return;
                }

                if (customItems.Count > 0)
                {
                    foreach (var gameObject in customItems)
                    {
                        __instance.m_prefabs.RemoveAll(i => i.name == gameObject.gameObject.name);
                        __instance.m_prefabs.Add(gameObject.gameObject);
                    }
                }

                if (myItemList.Count > 0)
                {
                    foreach (GameObject gameObject in myItemList)
                    {
                        __instance.m_prefabs.RemoveAll(i => i.name == gameObject.name);
                        __instance.m_prefabs.Add(gameObject);
                    }
                }

            }
        }

        //[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        //private static class ZNetScene_Awake_PostPatch
        //{
        //    public static void Postfix(ZNetScene __instance)
        //    {
        //        if (__instance == null)
        //        {
        //            return;
        //        }

        //        var blastFurnace = __instance.GetPrefab("blastfurnace");
        //        if (blastFurnace != null)
        //        {
        //            var smelterComponent = blastFurnace.GetComponent<Smelter>();
        //            if (smelterComponent != null)
        //            {
        //                List<Smelter.ItemConversion> conversions = new List<Smelter.ItemConversion>();
        //                conversions.AddRange(smelterComponent.m_conversion);

        //                Smelter.ItemConversion blueItemConversion = new Smelter.ItemConversion();
        //                blueItemConversion.m_from = OreBlue.GetComponent<ItemDrop>();
        //                blueItemConversion.m_to = MetalBlue.GetComponent<ItemDrop>();
        //                conversions.Add(blueItemConversion);

        //                Smelter.ItemConversion greenItemConversion = new Smelter.ItemConversion();
        //                greenItemConversion.m_from = OreGreen.GetComponent<ItemDrop>();
        //                greenItemConversion.m_to = MetalGreen.GetComponent<ItemDrop>();
        //                conversions.Add(greenItemConversion);

        //                Smelter.ItemConversion redItemConversion = new Smelter.ItemConversion();
        //                redItemConversion.m_from = OreRed.GetComponent<ItemDrop>();
        //                redItemConversion.m_to = MetalRed.GetComponent<ItemDrop>();
        //                conversions.Add(redItemConversion);

        //                Smelter.ItemConversion purpleItemConversion = new Smelter.ItemConversion();
        //                purpleItemConversion.m_from = OrePurple.GetComponent<ItemDrop>();
        //                purpleItemConversion.m_to = MetalPurple.GetComponent<ItemDrop>();
        //                conversions.Add(purpleItemConversion);

        //                smelterComponent.m_conversion = conversions;

        //            }
        //        }
        //    }

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
        private static class ObjectDB_CopyOtherDB_Patch
        {
            public static void Postfix()
            {
                if (!IsObjectDBValid()) return;
                if (!fixedReferences)
                {
                    GenerateReferenceLists();
                    FixItemReferences();
                }
                AddNewItems();
                AddNewRecipes();
                AddNewStatusEffects();
            }
        }

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
        private static class ObjectDB_Awake_Patch
        {
            public static void Postfix()
            {
                if (!IsObjectDBValid()) return;
                if (!fixedReferences)
                {
                    GenerateReferenceLists();
                    FixItemReferences();
                }
                AddNewItems();
                AddNewRecipes();
                AddNewStatusEffects();
            }
        }

        public static void WipeReferenceLists()
        {
            MyReferences.listOfAllGameObjects = null;
            MyReferences.listOfCookingStations = null;
            MyReferences.listOfCraftingStations = null;
            MyReferences.listOfEffects = null;
            MyReferences.listOfItemPrefabs = null;
            MyReferences.listOfMaterials = null;
            MyReferences.listOfPieces = null;
        }

        public static void RebuildItems()
        {
            if (IsObjectDBValid())
            {
                ItemManager.ApplySyncedItemConfigData();
                AddNewItems();
            }
        }

        public static void RebuildRecipes()
        {
            if (IsObjectDBValid())
            {
                foreach (var recipeToApply in cc.recipeConfigs)
                {
                    if (recipeToApply.Enabled)
                    {
                        GameObject go = myItemList.Where(mil => mil.name == recipeToApply.ItemPrefab).FirstOrDefault();
                        if (go == null) continue;
                        RecipeHelper rh = recipeToApply.LoadConfigedRecipeHelper(go);
                        rh.FixResources();
                        Recipe updatedRecipe = rh.GetRecipe();
                        if (updatedRecipe != null)
                        {
                            AddRecipeToObjectDB(updatedRecipe);
                            continue;
                        }
                    }
                }
            }
        }

        public static void RebuildCustomAssetLists()
        {
            //MyReferences.customItems = new List<CustomItem>();
            myRecipeList = new List<Recipe>();
            myItemList = new List<GameObject>();
            //customItemsAssembled = false;
            //customRecipesAssembled = false;
            //ItemManager.BuildLists();

            if (IsObjectDBValid())
            {
                //GenerateReferenceLists();
                AddNewItems();
                AddNewRecipes();
            }
        }

        //Dick Justice's and RandyKnapp's hack for ensuring the ObjectDB has objects in it and it's ready
        public static bool IsObjectDBValid()
        {
            return ObjectDB.instance != null && ObjectDB.instance.m_items.Count != 0 && ObjectDB.instance.GetItemPrefab("Wood") != null;
        }

        private static void GenerateReferenceLists()
        {
            //Start surfing through all items
            foreach (GameObject go in ObjectDB.instance.m_items)
            {
                //Add to reference lists if not in their already
                MyReferences.TryAddToItemList(go);

                ItemDrop id = go.GetComponent<ItemDrop>();
                if (id != null)
                {
                    //var s = id.m_itemData.m_shared;
                    //Log.LogMessage($"{id.name},{s.m_weight},{s.m_attackForce}");
                    ParticleSystemRenderer ps = go.GetComponent<ParticleSystemRenderer>();
                    if (ps != null)
                    {
                        if (!MyReferences.listOfMaterials.ContainsKey("item_particle".GetStableHashCode()))
                        {
                            MyReferences.TryAddToMaterialList(ps.sharedMaterial, "item_particle");
                        }
                    }

                    var shared = id.m_itemData.m_shared;
                    //Start looking for weapon effects (fx, sfx, vfx)
                    if (shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon
                        || shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon
                        || shared.m_itemType == ItemDrop.ItemData.ItemType.Bow)
                    {
                        if (!MyReferences.listOfMaterials.ContainsKey("club_trail".GetStableHashCode()))
                        {
                            Transform thing = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(go.transform, "attach");
                            if (thing != null)
                            {
                                thing = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(thing, "equiped");
                                if (thing != null)
                                {
                                    Transform trail = RecursiveSearchFunctions.ChildNodeFinderDepthFirst(thing, "trail");
                                    if (trail != null)
                                    {
                                        MeleeWeaponTrail mwt = trail.gameObject.GetComponent<MeleeWeaponTrail>();
                                        MyReferences.TryAddToMaterialList(mwt._material, "club_trail");
                                    }
                                }
                            }

                        }
                        MyReferences.TryExtractEffectsFromItemDropShared(shared);
                    }

                    //Check to see if item can also build things
                    if (shared.m_buildPieces != null)
                    {
                        var pieceTable = shared.m_buildPieces.m_pieces;
                        if (pieceTable != null)
                        {
                            foreach (var pieceTableItem in pieceTable)
                            {
                                MyReferences.TryAddToPieceList(pieceTableItem);

                                //One off capture of a station extension's line effect
                                StationExtension stationExtension = pieceTableItem.GetComponent<StationExtension>();
                                if (stationExtension != null && !MyReferences.listOfEffects.ContainsKey(stationExtension.m_connectionPrefab.name.GetStableHashCode()))
                                    MyReferences.listOfEffects.Add(stationExtension.m_connectionPrefab.name.GetStableHashCode(), stationExtension.m_connectionPrefab);

                                //Collect this for items and pieces, for proper referencing
                                CraftingStation craftingStation = pieceTableItem.GetComponent<CraftingStation>();
                                if (craftingStation != null && !MyReferences.listOfCraftingStations.ContainsKey(pieceTableItem.name.GetStableHashCode()))
                                    MyReferences.listOfCraftingStations.Add(pieceTableItem.name.GetStableHashCode(), craftingStation);

                                CookingStation cookingStation = pieceTableItem.GetComponent<CookingStation>();
                                if (cookingStation != null && !MyReferences.listOfCookingStations.ContainsKey(pieceTableItem.name.GetStableHashCode()))
                                    MyReferences.listOfCookingStations.Add(pieceTableItem.name.GetStableHashCode(), cookingStation);


                                //Extracting any Piece Placement Effects
                                var PieceScript = pieceTableItem.GetComponent<Piece>();
                                if (PieceScript != null)
                                {
                                    ExtractEffectsFromPiece(PieceScript.m_placeEffect);
                                }

                                //Extracting WearNTear effects
                                var WearNTearScript = pieceTableItem.GetComponent<WearNTear>();
                                if (WearNTearScript != null)
                                {
                                    ExtractEffectsFromPiece(WearNTearScript.m_destroyedEffect);
                                    ExtractEffectsFromPiece(WearNTearScript.m_hitEffect);
                                    ExtractEffectsFromPiece(WearNTearScript.m_switchEffect);
                                }

                            }
                        }
                    }

                }
            }
        }

        private static void ExtractEffectsFromPiece(EffectList el)
        {
            if (el.m_effectPrefabs != null && el.m_effectPrefabs.Length > 0)
            {
                foreach (var effect in el.m_effectPrefabs)
                {
                    if (effect.m_prefab != null
                        && !MyReferences.listOfEffects.ContainsKey(effect.m_prefab.name.GetStableHashCode()))
                    {
                        MyReferences.listOfEffects.Add(effect.m_prefab.name.GetStableHashCode(), effect.m_prefab);
                    }
                }
            }

        }

        //Debug version
        private static void ExtractEffectsFromPiece(EffectList el, string listName)
        {
            try
            {
                if (el.m_effectPrefabs != null && el.m_effectPrefabs.Length > 0)
                {
                    foreach (var effect in el.m_effectPrefabs)
                    {
                        if (effect.m_prefab != null
                            && !MyReferences.listOfEffects.ContainsKey(effect.m_prefab.name.GetStableHashCode()))
                        {
                            MyReferences.listOfEffects.Add(effect.m_prefab.name.GetStableHashCode(), effect.m_prefab);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error trying to access piece data in list {listName}");
                Plugin.Log.LogError(e.Message);
                Plugin.Log.LogError(e.StackTrace);
            }
        }

        private static void SyncNewPieces()
        {

            if (customPieces.Count > 0)
            {
                foreach (var piece in customPieces)
                {
                    piece.UpdateRequirements();
                    piece.AddPiece();
                }
                //customPieces = null;
            }
        }

        private static void FixAndAddNewPieces()
        {

            if (customPieces.Count > 0)
            {
                foreach (var piece in customPieces)
                {
                    piece.CompileAndAddPiece();
                }
                //customPieces = null;
            }
        }

        private static void AddNewCookingRecipes()
        {
            if (myCookingRecipes.Count > 0)
            {
                foreach (var cr in myCookingRecipes)
                {
                    var cookingStation = MyReferences.listOfCookingStations[cr.cookingStationName.GetStableHashCode()];
                    if (cookingStation != null)
                    {
                        CookingStation.ItemConversion recipe = new CookingStation.ItemConversion();
                        recipe.m_cookTime = cr.cookingTime;


                        recipe.m_from = cr.GetFromItemDrop();

                        recipe.m_to = cr.GetToItemDrop();
                        //if (!cookingStation.m_conversion.Contains(recipe))
                        cookingStation.m_conversion.RemoveAll(c => c.m_from.gameObject.name == recipe.m_from.gameObject.name);
                        cookingStation.m_conversion.Add(recipe);
                    }
                }
            }
        }

        private static void AddNewItems()
        {

            Dictionary<int, GameObject> m_itemsByHash = (Dictionary<int, GameObject>)typeof(ObjectDB).GetField("m_itemByHash", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ObjectDB.instance);

            if (myItemList.Count > 0)
            {
                foreach (var item in myItemList)
                {
                    AddItemToObjectDB(item, ref m_itemsByHash);
                }
            }

            if (customItems.Count > 0)
            {
                foreach (var customItem in customItems)
                {
                    var itemGameObject = customItem.gameObject;
                    AddItemToObjectDB(itemGameObject, ref m_itemsByHash);
                }
            }
        }

        private static void AddItemToObjectDB(GameObject item, ref Dictionary<int, GameObject> objectDBItemHash)
        {
            var itemDrop = item.GetComponent<ItemDrop>();
            if (itemDrop != null)
            {
                if (ObjectDB.instance.GetItemPrefab(item.name.GetStableHashCode()) == null)
                {
                    ObjectDB.instance.m_items.RemoveAll(i => i.name == item.name);
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

            if (myRecipeList.Count > 0)
            {
                foreach (var recipe in myRecipeList)
                {
                    AddRecipeToObjectDB(recipe);
                }
            }

            if (myRecipeHelperList.Count > 0)
            {
                foreach (var recipeHelper in myRecipeHelperList)
                {
                    if (recipeHelper.recipeEnabled)
                    {
                        var recipe = recipeHelper.GetRecipe();
                        AddRecipeToObjectDB(recipe);
                    }
                    else ObjectDB.instance.m_recipes.RemoveAll(x => x.name == recipeHelper.GetRecipe().name);
                }
            }
        }

        private static void AddRecipeToObjectDB(Recipe recipe)
        {
            ObjectDB.instance.m_recipes.RemoveAll(x => x.name == recipe.name);
            ObjectDB.instance.m_recipes.Add(recipe);
        }

        private static void AddNewStatusEffects()
        {

            if (MyReferences.myStatusEffects.Count > 0)
            {
                foreach (KeyValuePair<int, StatusEffect> effect in MyReferences.myStatusEffects)
                {
                    AddStatusEffectToObjectDB(effect.Value);
                }
            }
        }

        private static void AddStatusEffectToObjectDB(StatusEffect effect)
        {
            ObjectDB.instance.m_StatusEffects.RemoveAll(x => x.name == effect.name);
            ObjectDB.instance.m_StatusEffects.Add(effect);
        }

        private static void FixItemReferences()
        {
            foreach (var item in customItems)
            {
                item.FixReferences();
                myItemList.Add(item.gameObject);
            }

            customItems = new List<CustomItem>();
        }

        private static void FixRecipeReferences()
        {

            foreach (var recipe in myRecipeHelperList)
            {
                if (recipe.recipeEnabled)
                {
                    recipe.FixResources();
                    myRecipeList.Add(recipe.GetRecipe());
                }
            }

            myRecipeHelperList = new List<RecipeHelper>();
        }
    }
}
