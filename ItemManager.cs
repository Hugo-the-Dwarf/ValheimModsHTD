using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.IO;

namespace ValheimMoreTwoHanders
{
    /*
        Crafting Station forge --- forge (CraftingStation)
        Crafting Station piece_workbench --- piece_workbench (CraftingStation)
        Crafting Station piece_cauldron --- piece_cauldron (CraftingStation)
        Crafting Station piece_stonecutter --- piece_stonecutter (CraftingStation)
        Crafting Station piece_artisanstation --- piece_artisanstation (CraftingStation)
     */
    public static class ItemManager
    {
        private static AssetBundle assetBundle;
        public static string ResourcePath = "Assets/CustomItems/";

        private static RecipeHelper currentRecipeHelper;
        private static GameObject currentItem;
        private static CustomItem currentCustom;

        public static void BuildLists()
        {
            //weapon resources
            assetBundle = GetAssetBundleFromFile("twohandedweapons");

            //Silver Great Sword    
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordSilverGreat");
            currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
            currentRecipeHelper.AddResource("FineWood", 3, 0).AddResource("Silver", 60, 30).AddResource("LeatherScraps", 9, 3).AddResource("Iron", 10, 5);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_metal_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_sword_swing", WeaponEffectsManager.EffectList.TRAIL);

            AssetReferences.customItems.Add(currentCustom);


            //MaceCoreGreat
            currentItem = ExtractGameObjectFromBundle(assetBundle, "MaceCoreGreat");
            float rotX = 7.5f;
            float rotY = 12.75f;
            float rotZ = -5.25f;
            Transform RotatingElement = currentItem.transform.Find("attach/model1/rotator1");
            Rotator cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/v/model2/rotator2");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/h/model3/rotator3");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;
            currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);//forge
            currentRecipeHelper.AddResource("Flametal", 15, 5).AddResource("SurtlingCore", 30, 15).AddResource("Iron", 10, 5);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.prefabNodeManager.SetNode("core1", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("core2", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("core3", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().StartNewNode();

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            currentCustom.effectHandler.AddEffect("vfx_clubhit", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_club_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("vfx_sledge_hit", WeaponEffectsManager.EffectList.HIT_TERRAIN).AddEffect("sfx_sledge_iron_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_metal_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_battleaxe_swing_wosh", WeaponEffectsManager.EffectList.TRAIL);

            AssetReferences.customItems.Add(currentCustom);
            //Green
            currentItem = ExtractGameObjectFromBundle(assetBundle, "MaceCoreGreatGreen");
            RotatingElement = currentItem.transform.Find("attach/model1/rotator1");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/v/model2/rotator2");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/h/model3/rotator3");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);//forge
            currentRecipeHelper.AddResource("MaceCoreGreat", 1, 0).AddResource("Flametal", 0, 5).AddResource("Guck", 1, 0).AddResource("BlackMetal", 5, 5);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.prefabNodeManager.SetNode("core1", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().ReplaceEmissionColor(new Color(0f, 0.8490f, 0.4117f)).ChangeTextureScaleOffset(0.6f,0.6f,0.15f,0.2f).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("core2", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("core3", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().StartNewNode();

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            currentCustom.effectHandler.AddEffect("vfx_clubhit", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_club_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("vfx_sledge_hit", WeaponEffectsManager.EffectList.HIT_TERRAIN).AddEffect("sfx_sledge_iron_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_metal_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_battleaxe_swing_wosh", WeaponEffectsManager.EffectList.TRAIL);

            AssetReferences.customItems.Add(currentCustom);

            //SwordBlackmetalGreat
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordBlackmetalGreat");
            currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
            currentRecipeHelper.AddResource("BlackMetal", 60, 30).AddResource("LinenThread", 9, 3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.prefabNodeManager.SetNode("model1", "SwordBlackmetal", "default").CopyTargetMaterial(true).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("model2", "SwordBlackmetal", "default").CopyTargetMaterial(true).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("model3", "SwordBlackmetal", "default").CopyTargetMaterial(true).StartNewNode();

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_metal_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_sword_swing", WeaponEffectsManager.EffectList.TRAIL);

            //currentCustom.materialManager.SetCloneTargetSwapTextures("SwordBlackmetal");

            AssetReferences.customItems.Add(currentCustom);


            //SwordObsidianGreat
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordObsidianGreat");

            rotX = 13f;
            rotY = -15.75f;
            rotZ = 9.25f;
            RotatingElement = currentItem.transform.Find("attach/rotator1");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/v/rotator2");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/h/rotator3");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
            currentRecipeHelper.AddResource("DragonTear", 1, 0).AddResource("Obsidian", 25, 35).AddResource("FreezeGland", 30, 30).AddResource("Crystal", 20, 10);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.effectHandler.AddEffect("vfx_clubhit", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_club_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT_TERRAIN).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_metal_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_battleaxe_swing_wosh", WeaponEffectsManager.EffectList.TRAIL);

            currentCustom.prefabNodeManager.SetNode("wrap1", "LinenThread", "model").CopyTargetMaterial(false).ReplaceMainColor(new Color(0.3647f, .2431f, .145f)).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("wrap2", "LinenThread", "model").CopyTargetMaterial(false).ReplaceMainColor(new Color(0.3647f, .2431f, .145f)).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("wrap3", "LinenThread", "model").CopyTargetMaterial(false).ReplaceMainColor(new Color(0.3647f, .2431f, .145f)).StartNewNode();

            currentCustom.prefabNodeManager.SetNode("inner1", "DragonTear", "inner").CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("inner2", "DragonTear", "inner").CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("inner3", "DragonTear", "inner").CopyTargetMaterial().StartNewNode();

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            AssetReferences.customItems.Add(currentCustom);

            assetBundle.Unload(false);
        }

        private static AssetBundle GetAssetBundleFromFile(string fileName)
        {
            AssetBundle LoadedAssetBundle = null;
            string dllPath = "NO PATH SET";
            try
            {
                dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                LoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(dllPath, fileName));

                if (LoadedAssetBundle == null)
                {
                    Debug.Log("Failed to load AssetBundle!");
                    return null;
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error Trying to load asset bundle {fileName} in location: {dllPath}");
                Plugin.Log.LogError($"Catch Exception details: {e.Message} --- {e.StackTrace}");
            }
            return LoadedAssetBundle;
        }

        private static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));

            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }

        private static GameObject ExtractGameObjectFromBundle(AssetBundle bundle, string prefabName)
        {
            try
            {
                return bundle.LoadAsset<GameObject>(ResourcePath + prefabName + ".prefab");
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error Trying to load asset '{prefabName}.prefab' from assetbundle '{bundle.name}'");
                Plugin.Log.LogError($"Catch Exception details: {e.Message} --- {e.StackTrace}");
            }
            return null;
        }
        
    }
}
