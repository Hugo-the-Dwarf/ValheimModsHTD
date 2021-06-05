using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BepInEx.Configuration;
using System.IO;
using System;

namespace ValheimMoreTwoHanders
{
    [BepInPlugin(Plugin.GUID, Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "4.0";
        public const string ModName = "More Two Handed Weapons";
        public const string GUID = "htd.moretwohanders";
        //public static readonly string MyDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Harmony _Harmony;
        public static ManualLogSource Log;

        public readonly Harmony harmony = new Harmony(GUID);

        public static CustomConfig cc = new CustomConfig();
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


            cc.LoadConfigs(Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath),GUID));

            //This Static Class just fills the ItemList and RecipeLists
            try
            {
                ItemManager.BuildLists();
            }
            catch(Exception e)
            {
                Log.LogError("Problem with Setting up item prefabs. ItemManager.");
                Log.LogError(e.Message);
                Log.LogError(e.StackTrace);
            }

            cc.WriteConfigs(Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), GUID));

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

            //    public static void Postfix(ZNetScene __instance)
            //    {
            //        if (__instance == null)
            //        {
            //            return;
            //        }

            //        var portalPrefab = __instance.m_prefabs.Where(p => p.name == "portal_wood").FirstOrDefault();
            //        if (portalPrefab != null)
            //        {
            //            var pieceCom = portalPrefab.GetComponent<Piece>();
            //            if (pieceCom != null) //this may not be needed but I like to check for nulls
            //            {
            //                List<Piece.Requirement> requirements = new List<Piece.Requirement>();

            //                Piece.Requirement req1 = new Piece.Requirement();
            //                req1.m_amount = 123;
            //                req1.m_recover = true; //false means you won't get it on deconstruct
            //                req1.m_resItem = __instance.m_prefabs.Where(p => p.name == "itemprefabnamehere").FirstOrDefault().GetComponent<ItemDrop>();
            //                req1.m_amountPerLevel = 0; //for pieces (buildings) this doesn't matter

            //                requirements.Add(req1);

            //                pieceCom.m_resources = requirements.ToArray();
            //            }
            //        }

            //        foreach (var prefab in __instance.m_prefabs)
            //        {
            //            Log.LogMessage($"Prefab in ZNetScene: {prefab.name}");
            //        }
            //    }
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
                    var shared = id.m_itemData.m_shared;
                    //Start looking for weapon effects (fx, sfx, vfx)
                    if (shared.m_itemType == ItemDrop.ItemData.ItemType.OneHandedWeapon || shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon || shared.m_itemType == ItemDrop.ItemData.ItemType.Bow)
                    {
                        //if (!AssetReferences.listOfMaterials.ContainsKey("club_trail"))
                        //{
                        //    Log.LogMessage($"ParticleSystemRenderer in {go.name} Found.");
                        //    Transform trail = PrefabNodeManager.RecursiveChildNodeFinder(go.transform, "trail");
                        //    if (trail != null)
                        //    {
                        //        MeleeWeaponTrail mwt = trail.gameObject.GetComponent<MeleeWeaponTrail>();
                        //        AssetReferences.TryAddToMaterialList(mwt._material, "club_trail");
                        //    }
                        //}
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
                    if (customItem.recipe.recipeEnabled)
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
