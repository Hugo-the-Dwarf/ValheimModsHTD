using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.IO;
using BepInEx.Configuration;

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
            assetBundle = GetAssetBundleFromResources("twohandedweapons");

            //Silver Great Sword    
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordSilverGreat");
            currentRecipeHelper = new RecipeHelper(currentItem, Plugin.SwordSilverGreatCraftingStation.Value, Plugin.SwordSilverGreatCraftingLevels[0].Value, 1); 
            currentRecipeHelper.AddResource("FineWood", 3, 0).AddResource("Silver", 60, 30).AddResource("LeatherScraps", 9, 3).AddResource("Iron", 10, 5);

            var id = currentItem.GetComponent<ItemDrop>().m_itemData.m_shared;
            id.m_name = Plugin.SwordSilverGreatName.Value;
            id.m_description = Plugin.SwordSilverGreatDescription.Value;
            id.m_maxQuality = Plugin.SwordSilverGreatCraftingLevels[1].Value;
            id.m_damages = SetDamageValues(Plugin.SwordSilverGreatDamages);
            id.m_damagesPerLevel = SetDamageValues(Plugin.SwordSilverGreatDamagesPerUpgrade);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.recipeEnabled = Plugin.EnabledWeaponRecipes[0].Value;

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

            currentRecipeHelper = new RecipeHelper(currentItem, Plugin.GreatCoreMaceCraftingStation.Value, Plugin.GreatCoreMaceCraftingLevels[0].Value, 1);//forge
            currentRecipeHelper.AddResource("SurtlingCore", 30, 15).AddResource("Iron", 25, 10);

            id = currentItem.GetComponent<ItemDrop>().m_itemData.m_shared;
            id.m_name = Plugin.GreatCoreMaceName.Value;
            id.m_description = Plugin.GreatCoreMaceDescription.Value;
            id.m_maxQuality = Plugin.GreatCoreMaceCraftingLevels[1].Value;
            id.m_damages = SetDamageValues(Plugin.GreatCoreMaceDamages);
            id.m_damagesPerLevel = SetDamageValues(Plugin.GreatCoreMaceDamagesPerUpgrade);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.recipeEnabled = Plugin.EnabledWeaponRecipes[1].Value;

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

            currentRecipeHelper = new RecipeHelper(currentItem, Plugin.GreatToxicMaceCraftingStation.Value, Plugin.GreatToxicMaceCraftingLevels[0].Value, 1);//forge
            currentRecipeHelper.AddResource("Guck", 30, 15).AddResource("Iron", 25, 10);

            id = currentItem.GetComponent<ItemDrop>().m_itemData.m_shared;
            id.m_name = Plugin.GreatToxicMaceName.Value;
            id.m_description = Plugin.GreatToxicMaceDescription.Value;
            id.m_maxQuality = Plugin.GreatToxicMaceCraftingLevels[1].Value;
            id.m_damages = SetDamageValues(Plugin.GreatToxicMaceDamages);
            id.m_damagesPerLevel = SetDamageValues(Plugin.GreatToxicMaceDamagesPerUpgrade);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.recipeEnabled = Plugin.EnabledWeaponRecipes[2].Value;

            currentCustom.prefabNodeManager.SetNode("guck1", "Guck", "model").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("guck2", "Guck", "model").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("guck3", "Guck", "model").CopyTargetMaterial().CopyTargetMesh().StartNewNode();

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
            //Blue
            currentItem = ExtractGameObjectFromBundle(assetBundle, "MaceCoreGreatBlue");
            RotatingElement = currentItem.transform.Find("attach/model1/rotator1");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();            
            cir.rotateY = rotY;
            //cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/v/model2/rotator2");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();            
            cir.rotateY = rotY;
            //cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/h/model3/rotator3");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();            
            cir.rotateY = rotY;
            //cir.rotateZ = rotZ;

            currentRecipeHelper = new RecipeHelper(currentItem, Plugin.GreatFrostMaceCraftingStation.Value, Plugin.GreatFrostMaceCraftingLevels[0].Value, 1);//forge
            currentRecipeHelper.AddResource("Crystal", 30, 15).AddResource("Iron", 25, 10);

            id = currentItem.GetComponent<ItemDrop>().m_itemData.m_shared;
            id.m_name = Plugin.GreatFrostMaceName.Value;
            id.m_description = Plugin.GreatFrostMaceDescription.Value;
            id.m_maxQuality = Plugin.GreatFrostMaceCraftingLevels[1].Value;
            id.m_damages = SetDamageValues(Plugin.GreatFrostMaceDamages);
            id.m_damagesPerLevel = SetDamageValues(Plugin.GreatFrostMaceDamagesPerUpgrade);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.recipeEnabled = Plugin.EnabledWeaponRecipes[3].Value;

            currentCustom.prefabNodeManager.SetNode("outercrystal1", "Crystal", "Cube").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("innercystal1", "Crystal", "interior").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("outercrystal2", "Crystal", "Cube").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("innercystal2", "Crystal", "interior").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("outercrystal3", "Crystal", "Cube").CopyTargetMaterial().CopyTargetMesh().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("innercystal3", "Crystal", "interior").CopyTargetMaterial().CopyTargetMesh().StartNewNode();

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            currentCustom.effectHandler.AddEffect("vfx_clubhit", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_club_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("vfx_sledge_iron_hit", WeaponEffectsManager.EffectList.HIT_TERRAIN).AddEffect("sfx_sledge_iron_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_metal_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_battleaxe_swing_wosh", WeaponEffectsManager.EffectList.TRAIL);

            AssetReferences.customItems.Add(currentCustom);

            //SwordBlackmetalGreat
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordBlackmetalGreat");
            currentRecipeHelper = new RecipeHelper(currentItem, Plugin.SwordBlackMetalGreatCraftingStation.Value, Plugin.SwordBlackMetalGreatCraftingLevels[0].Value, 1);
            currentRecipeHelper.AddResource("BlackMetal", 60, 30).AddResource("LinenThread", 9, 3);

            id = currentItem.GetComponent<ItemDrop>().m_itemData.m_shared;
            id.m_name = Plugin.SwordBlackMetalGreatName.Value;
            id.m_description = Plugin.SwordBlackMetalGreatDescription.Value;
            id.m_maxQuality = Plugin.SwordBlackMetalGreatCraftingLevels[1].Value;
            id.m_damages = SetDamageValues(Plugin.SwordBlackMetalGreatDamages);
            id.m_damagesPerLevel = SetDamageValues(Plugin.SwordBlackMetalGreatDamagesPerUpgrade);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.recipeEnabled = Plugin.EnabledWeaponRecipes[4].Value;

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

            currentRecipeHelper = new RecipeHelper(currentItem, Plugin.SwordObsidianGreatCraftingStation.Value, Plugin.SwordObsidianGreatCraftingLevels[0].Value, 1);
            currentRecipeHelper.AddResource("DragonTear", 1, 0).AddResource("Obsidian", 25, 35).AddResource("FreezeGland", 30, 30).AddResource("Crystal", 20, 10);

            id = currentItem.GetComponent<ItemDrop>().m_itemData.m_shared;
            id.m_name = Plugin.SwordObsidianGreatName.Value;
            id.m_description = Plugin.SwordObsidianGreatDescription.Value;
            id.m_maxQuality = Plugin.SwordObsidianGreatCraftingLevels[1].Value;
            id.m_damages = SetDamageValues(Plugin.SwordObsidianGreatDamages);
            id.m_damagesPerLevel = SetDamageValues(Plugin.SwordObsidianGreatDamagesPerUpgrade);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.recipeEnabled = Plugin.EnabledWeaponRecipes[5].Value;

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


            //SwordFlametalGreat
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordFlametalGreat");

            currentRecipeHelper = new RecipeHelper(currentItem, Plugin.SwordFlametalGreatCraftingStation.Value, Plugin.SwordFlametalGreatCraftingLevels[0].Value, 1);
            currentRecipeHelper.AddResource("Flametal", 35, 20).AddResource("Iron", 25, 15).AddResource("LeatherScraps", 10, 10).AddResource("SurtlingCore", 20, 10);

            id = currentItem.GetComponent<ItemDrop>().m_itemData.m_shared;
            id.m_name = Plugin.SwordFlametalGreatName.Value;
            id.m_description = Plugin.SwordFlametalGreatDescription.Value;
            id.m_maxQuality = Plugin.SwordFlametalGreatCraftingLevels[1].Value;
            id.m_damages = SetDamageValues(Plugin.SwordFlametalGreatDamages);
            id.m_damagesPerLevel = SetDamageValues(Plugin.SwordFlametalGreatDamagesPerUpgrade);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.recipeEnabled = Plugin.EnabledWeaponRecipes[6].Value;

            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT_TERRAIN).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_metal_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_battleaxe_swing_wosh", WeaponEffectsManager.EffectList.TRAIL);

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            AssetReferences.customItems.Add(currentCustom);

            assetBundle.Unload(false);
        }

        private static HitData.DamageTypes SetDamageValues(ConfigEntry<float>[] newDamageList)
        {
            HitData.DamageTypes newDamage = new HitData.DamageTypes();
            newDamage.m_blunt = newDamageList[0].Value;
            newDamage.m_slash = newDamageList[1].Value;
            newDamage.m_pierce = newDamageList[2].Value;
            newDamage.m_chop = newDamageList[3].Value;
            newDamage.m_pickaxe = newDamageList[4].Value;
            newDamage.m_fire = newDamageList[5].Value;
            newDamage.m_frost = newDamageList[6].Value;
            newDamage.m_lightning = newDamageList[7].Value;
            newDamage.m_poison = newDamageList[8].Value;
            newDamage.m_spirit = newDamageList[9].Value;

            return newDamage;
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
