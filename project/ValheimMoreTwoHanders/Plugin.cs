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
        public const string Version = "5.0.3";
        public const string ModName = "Hugo's Armory";
        public const string GUID = "htd.armory";
        //public static readonly string MyDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static ServerSync.ConfigSync configSync = new ServerSync.ConfigSync(GUID) { DisplayName = ModName, CurrentVersion = Version };

        Harmony _Harmony;
        public static ManualLogSource Log;

        public readonly Harmony harmony = new Harmony(GUID);

        public static CustomConfig cc = new CustomConfig();
        public static CustomLocalization cl = new CustomLocalization();
        public static bool disableFlametalFlames = false;

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
            disableFlametalFlames = Config.Bind<bool>("2Options", "disable_FlametalSwordFlames", false, "Disable the fire and smoke effect from the Flametal Great Sword.").Value;
            PlayerAttackInputPatch.attack3Hotkey = Config.Bind<string>("1Hotkeys", "hotkey_ThirdAttack", "mouse 3", "Customizable hotkey so you can use the third attack of the weapon. If you want to use a mouse key, include a space: mouse 3, for example. Valid inputs: https://docs.unity3d.com/ScriptReference/KeyCode.html");

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
            if (_Harmony != null) _Harmony.UnpatchAll(GUID);
        }

        private void Start()
        {
            //AddNewRecipes();
        }

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        private static class ZNetScene_Awake_Patch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance == null)
                {
                    return;
                }

                if (MyReferences.myItemList.Count > 0)
                {
                    foreach (GameObject gameObject in MyReferences.myItemList)
                    {
                        if (!__instance.m_prefabs.Contains(gameObject))
                            __instance.m_prefabs.Add(gameObject);
                    }
                }
                else return;
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        private static class ObjectDB_CopyOtherDB_Patch
        {
            public static void Postfix()
            {
                if (!IsObjectDBValid()) return;

                GenerateReferenceLists();
                AddNewItems();
                AddNewRecipes();
                AddNewStatusEffects();
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
                AddNewRecipes();
                AddNewStatusEffects();
            }
        }

        //[HarmonyPatch(typeof(FejdStartup), "OnJoinStart")]
        //private static class FejdStartup_OnJoinStart_Patch
        //{
        //    public static void PreFix()
        //    {
        //        AddNewRecipes();
        //    }
        //}

        public static void RebuildRecipes()
        {
            if (IsObjectDBValid())
            {
                foreach (var recipeToApply in cc.recipeConfigs)
                {
                    if (recipeToApply.Enabled)
                    {
                        GameObject go = MyReferences.myItemList.Where(mil => mil.name == recipeToApply.ItemPrefab).FirstOrDefault();
                        if (go == null) continue;
                        Recipe updatedRecipe = recipeToApply.LoadConfigedRecipeHelper(go).GetRecipe();
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
            //AssetReferences.customItems = new List<CustomItem>();
            MyReferences.myRecipeList = new List<Recipe>();
            MyReferences.myItemList = new List<GameObject>();
            //customItemsAssembled = false;
            //customRecipesAssembled = false;
            //ItemManager.BuildLists();

            if (IsObjectDBValid())
            {
                //GenerateReferenceLists();
                //AddNewItems();
                AddNewRecipes();
            }
        }

        //Dick Justice's and RandyKnapp's hack for ensuring the ObjectDB has objects in it and it's ready
        public static bool IsObjectDBValid()
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
                MyReferences.TryAddToItemList(go);

                //if (!AssetReferences.listOfMaterials.ContainsKey("item_particle"))
                //{
                //    Log.LogMessage($"Searching for ParticleSystemRenderer in {go.name}");                    
                //    ParticleSystemRenderer psr = go.GetComponent<ParticleSystemRenderer>();
                //    Log.LogMessage($"ParticleSystemRenderer in {go.name} Found.");
                //    AssetReferences.TryAddToMaterialList(psr.material, "item_particle");
                //    Log.LogMessage($"PSR material {psr.material.name} Found.");
                //}

                ItemDrop id = go.GetComponent<ItemDrop>();
                if (id != null)
                {
                    ParticleSystemRenderer ps = go.GetComponent<ParticleSystemRenderer>();
                    if (ps != null)
                    {
                        //Log.LogMessage($"ParticleSystemRenderer in {go.name} Found.");
                        if (!MyReferences.listOfMaterials.ContainsKey("item_particle"))
                        {
                            //Log.LogMessage($"ParticleSystemRenderer material in {go.name}, {ps.material.name} Found.");
                            MyReferences.TryAddToMaterialList(ps.material, "item_particle");
                        }
                    }

                    var shared = id.m_itemData.m_shared;
                    //Start looking for weapon effects (fx, sfx, vfx)
                    if (shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon
                        || shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon
                        || shared.m_itemType == ItemDrop.ItemData.ItemType.Bow)
                    {
                        if (!MyReferences.listOfMaterials.ContainsKey("club_trail"))
                        {
                            //Log.LogMessage($"MeleeWeaponTrail in {go.name} Found.");
                            Transform trail = PrefabNodeManager.RecursiveChildNodeFinder(go.transform, "trail");
                            if (trail != null)
                            {
                                MeleeWeaponTrail mwt = trail.gameObject.GetComponent<MeleeWeaponTrail>();
                                MyReferences.TryAddToMaterialList(mwt._material, "club_trail");
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
                                CraftingStation craftingStation = pieceTableItem.GetComponent<CraftingStation>();
                                if (craftingStation != null && !MyReferences.listOfCraftingStations.ContainsKey(pieceTableItem.name)) MyReferences.listOfCraftingStations.Add(pieceTableItem.name, craftingStation);
                                StationExtension stationExtension = pieceTableItem.GetComponent<StationExtension>();
                                if (stationExtension != null && !MyReferences.listOfEffects.ContainsKey(stationExtension.m_connectionPrefab.name)) MyReferences.listOfEffects.Add(stationExtension.m_connectionPrefab.name, stationExtension.m_connectionPrefab);
                            }
                        }
                    }
                }
            }
        }

        private static void AddNewItems()
        {

            Dictionary<int, GameObject> m_itemsByHash = (Dictionary<int, GameObject>)typeof(ObjectDB).GetField("m_itemByHash", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ObjectDB.instance);

            if (MyReferences.myItemList.Count > 0)
            {
                foreach (var item in MyReferences.myItemList)
                {
                    AddItemToObjectDB(item, ref m_itemsByHash);
                }
            }

            if (MyReferences.customItems.Count > 0)
            {
                foreach (var customItem in MyReferences.customItems)
                {
                    customItem.FixReferences();
                    var itemGameObject = customItem.gameObject;
                    if (!MyReferences.myItemList.Contains(itemGameObject)) MyReferences.myItemList.Add(itemGameObject);
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

            if (MyReferences.myRecipeList.Count > 0)
            {
                foreach (var recipe in MyReferences.myRecipeList)
                {
                    AddRecipeToObjectDB(recipe);
                }
            }

            if (MyReferences.myRecipeHelperList.Count > 0)
            {
                foreach (var recipeHelper in MyReferences.myRecipeHelperList)
                {
                    if (recipeHelper.recipeEnabled)
                    {
                        var recipe = recipeHelper.GetRecipe();
                        if (!MyReferences.myRecipeList.Contains(recipe)) MyReferences.myRecipeList.Add(recipe);
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
            // It removes the old recipe and adds the new one. Randy has this to make sure that reloading the configuration while the game is running works.
            ObjectDB.instance.m_recipes.RemoveAll(x => x.name == recipe.name);
            ObjectDB.instance.m_recipes.Add(recipe);
        }

        private static void AddNewStatusEffects()
        {

            if (MyReferences.myStatusEffects.Count > 0)
            {
                foreach (var effect in MyReferences.myStatusEffects)
                {
                    AddStatusEffectToObjectDB(effect);
                }
            }
        }

        private static void AddStatusEffectToObjectDB(StatusEffect effect)
        {
            //Sadly I'm not sure why this is here, but RandyKnapp had it
            // It removes the old recipe and adds the new one. Randy has this to make sure that reloading the configuration while the game is running works.
            ObjectDB.instance.m_StatusEffects.RemoveAll(x => x.name == effect.name);
            ObjectDB.instance.m_StatusEffects.Add(effect);
        }

        private static void WipeCustomItemList()
        {
            if (customItemsAssembled && customRecipesAssembled)
            {
                MyReferences.customItems = new List<CustomItem>();
                MyReferences.myRecipeHelperList = new List<RecipeHelper>();
            }
        }
    }
}
