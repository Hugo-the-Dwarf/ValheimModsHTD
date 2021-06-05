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
            var id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("FineWood", 8, 0).AddResource("Silver", 60, 30).AddResource("LeatherScraps", 9, 3).AddResource("Iron", 10, 5);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Horizontal;
            Attack3.m_attackAnimation = "battleaxe_secondary";
            Attack3.m_attackStamina = 15f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.5f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 30f;
            Attack3.m_damageMultiplier = 0.5f;
            Attack3.m_forceMultiplier = 3f;
            Attack3.m_staggerMultiplier = 4f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 30f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

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

            //Iron
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordIronGreat");
            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("FineWood", 8, 0).AddResource("Iron", 40, 20).AddResource("LeatherScraps", 9, 3);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Horizontal;
            Attack3.m_attackAnimation = "battleaxe_secondary";
            Attack3.m_attackStamina = 15f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.5f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 30f;
            Attack3.m_damageMultiplier = 0.5f;
            Attack3.m_forceMultiplier = 3f;
            Attack3.m_staggerMultiplier = 4f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 30f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

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

            //Silver Battle axe
            currentItem = ExtractGameObjectFromBundle(assetBundle, "AxeSilverBattle");

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("ElderBark", 20, 0).AddResource("Silver", 60, 30).AddResource("LeatherScraps", 9, 3).AddResource("Iron", 10, 5);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "swing_pickaxe";
            Attack3.m_attackStamina = 25f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.3f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 40f;
            Attack3.m_damageMultiplier = 1.75f;
            Attack3.m_forceMultiplier = 0.75f;
            Attack3.m_staggerMultiplier = 2f;
            Attack3.m_attackRange = 2.5f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 120f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.effectHandler.AddEffect("vfx_clubhit", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_battleaxe_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT_TERRAIN).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_wood_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_battleaxe_swing_wosh", WeaponEffectsManager.EffectList.TRAIL);

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            AssetReferences.customItems.Add(currentCustom);


            //Mace Silver Great
            currentItem = ExtractGameObjectFromBundle(assetBundle, "MaceSilverGreat");

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);//forge
                currentRecipeHelper.AddResource("ElderBark", 20, 0).AddResource("Silver", 60, 30).AddResource("YmirRemains", 10, 0).AddResource("FreezeGland", 10, 0);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "swing_sledge";
            Attack3.m_attackStamina = 35f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.4f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 60f;
            Attack3.m_damageMultiplier = 1.5f;
            Attack3.m_forceMultiplier = 1f;
            Attack3.m_staggerMultiplier = 2f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 90f;
            Attack3.m_attackRayWidth = 4f;
            Attack3.m_maxYAngle = 45f;
            Attack3.m_hitThroughWalls = true;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

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
            //MaceCoreGreat
            currentItem = ExtractGameObjectFromBundle(assetBundle, "MaceCoreGreat");

            float rotX = 0;
            float rotY = 45f;
            float rotZ = 0;
            Transform RotatingElement = currentItem.transform.Find("attach/EffectHolder1/rotator1");
            Rotator cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/v/EffectHolder2/rotator2");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/h/EffectHolder3/rotator3");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);//forge
                currentRecipeHelper.AddResource("TrophySurtling", 1, 0).AddResource("SurtlingCore", 40, 25).AddResource("Iron", 35, 10).AddResource("LeatherScraps", 12, 8);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "swing_sledge";
            Attack3.m_attackStamina = 35f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.4f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 60f;
            Attack3.m_damageMultiplier = 1.5f;
            Attack3.m_forceMultiplier = 1f;
            Attack3.m_staggerMultiplier = 2f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 90f;
            Attack3.m_attackRayWidth = 4f;
            Attack3.m_maxYAngle = 45f;
            Attack3.m_hitThroughWalls = true;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

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

            RotatingElement = currentItem.transform.Find("attach/EffectHolder1/rotator1");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/v/EffectHolder2/rotator2");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;

            RotatingElement = currentItem.transform.Find("stand/h/EffectHolder3/rotator3");
            cir = RotatingElement.gameObject.AddComponent<Rotator>();
            cir.rotateX = rotX;
            cir.rotateY = rotY;
            cir.rotateZ = rotZ;



            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);//forge
                currentRecipeHelper.AddResource("TrophySurtling", 1, 0).AddResource("SurtlingCore", 40, 15).AddResource("Iron", 35, 25).AddResource("LeatherScraps", 8, 12);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "swing_sledge";
            Attack3.m_attackStamina = 35f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.4f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 60f;
            Attack3.m_damageMultiplier = 1.5f;
            Attack3.m_forceMultiplier = 1f;
            Attack3.m_staggerMultiplier = 2f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 90f;
            Attack3.m_attackRayWidth = 4f;
            Attack3.m_maxYAngle = 45f;
            Attack3.m_hitThroughWalls = true;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            Color greenEmission = new Color(0f, 1.4f, 0.13f);
            float sX = 0.6f;
            float sY = 0.75f;
            float oX = .1f;
            float oY = .06f;
            currentCustom.prefabNodeManager.SetNode("core1", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().ReplaceEmissionColor(greenEmission).ChangeTextureScaleOffset(sX, sY, oX, oY).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("core2", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().ReplaceEmissionColor(greenEmission).ChangeTextureScaleOffset(sX, sY, oX, oY).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("core3", "SurtlingCore", "core").CopyTargetMaterial().CopyTargetMesh().ReplaceEmissionColor(greenEmission).ChangeTextureScaleOffset(sX, sY, oX, oY).StartNewNode();

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

            //Grasp
            currentItem = ExtractGameObjectFromBundle(assetBundle, "MaceGraspUndying");

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);//forge
                currentRecipeHelper.AddResource("TrophyBonemass", 1, 0).AddResource("WitheredBone", 20, 10).AddResource("BoneFragments", 50, 25).AddResource("TrollHide", 30, 20);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "swing_sledge";
            Attack3.m_attackStamina = 35f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.4f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 60f;
            Attack3.m_damageMultiplier = 1.5f;
            Attack3.m_forceMultiplier = 1f;
            Attack3.m_staggerMultiplier = 2f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 90f;
            Attack3.m_attackRayWidth = 4f;
            Attack3.m_maxYAngle = 45f;
            Attack3.m_hitThroughWalls = true;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

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
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordBlackMetalGreat");

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("BlackMetal", 60, 30).AddResource("LinenThread", 9, 3);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Horizontal;
            Attack3.m_attackAnimation = "battleaxe_secondary";
            Attack3.m_attackStamina = 15f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.5f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 30f;
            Attack3.m_damageMultiplier = 0.5f;
            Attack3.m_forceMultiplier = 3f;
            Attack3.m_staggerMultiplier = 4f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 30f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

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

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("DragonTear", 1, 0).AddResource("Obsidian", 25, 35).AddResource("FreezeGland", 30, 30).AddResource("Crystal", 20, 10);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "sword_secondary";
            Attack3.m_attackStamina = 30f;
            Attack3.m_speedFactor = 0.2f;
            Attack3.m_speedFactorRotation = 0f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 30f;
            Attack3.m_damageMultiplier = 3f;
            Attack3.m_forceMultiplier = 1f;
            Attack3.m_staggerMultiplier = 1f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 45f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

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

            currentCustom.prefabNodeManager.SetNode("hull1", "DragonTear", "hull").CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("hull2", "DragonTear", "hull").CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("hull3", "DragonTear", "hull").CopyTargetMaterial().StartNewNode();

            currentCustom.prefabNodeManager.SetNode("pixel_flakes1", "DragonTear", "pixel_flakes").CopyTargetParticle().CopyTargetShader().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("pixel_flakes2", "DragonTear", "pixel_flakes").CopyTargetParticle().CopyTargetShader().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("pixel_flakes3", "DragonTear", "pixel_flakes").CopyTargetParticle().CopyTargetShader().StartNewNode();

            currentCustom.prefabNodeManager.SetNode("smoke_expl1", "DragonTear", "smoke_expl").CopyTargetParticle().CopyTargetShader().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("smoke_expl2", "DragonTear", "smoke_expl").CopyTargetParticle().CopyTargetShader().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("smoke_expl3", "DragonTear", "smoke_expl").CopyTargetParticle().CopyTargetShader().StartNewNode();

            currentCustom.prefabNodeManager.SetNode("flare1", "DragonTear", "flare").CopyTargetParticle().CopyTargetShader().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("flare2", "DragonTear", "flare").CopyTargetParticle().CopyTargetShader().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("flare3", "DragonTear", "flare").CopyTargetParticle().CopyTargetShader().StartNewNode();

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            AssetReferences.customItems.Add(currentCustom);


            //SwordFlametalGreat
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordFlametalGreat");

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("Flametal", 35, 20).AddResource("Iron", 25, 15).AddResource("LeatherScraps", 10, 10).AddResource("SurtlingCore", 20, 10);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "sword_secondary";
            Attack3.m_attackStamina = 30f;
            Attack3.m_speedFactor = 0.2f;
            Attack3.m_speedFactorRotation = 0f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 30f;
            Attack3.m_damageMultiplier = 3f;
            Attack3.m_forceMultiplier = 1f;
            Attack3.m_staggerMultiplier = 1f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 45f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT_TERRAIN).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_metal_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_battleaxe_swing_wosh", WeaponEffectsManager.EffectList.TRAIL);

            if (Plugin.disableFlametalFlames)
            {
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames_rise1").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames_rise2").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames_rise3").gameObject.SetActive(false);

                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames_mid1").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames_mid2").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames_mid3").gameObject.SetActive(false);

                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames1").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames2").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "flames3").gameObject.SetActive(false);

                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "embers1").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "embers2").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "embers3").gameObject.SetActive(false);

                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "smoke1").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "smoke2").gameObject.SetActive(false);
                PrefabNodeManager.RecursiveChildNodeFinder(currentItem.transform, "smoke3").gameObject.SetActive(false);

                //currentCustom.prefabNodeManager.SetNode("flames_rise1", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("flames_rise2", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("flames_rise3", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("flames_mid1", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("flames_mid2", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("flames_mid3", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("flames1", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("flames2", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("flames3", "Torch", "flames").CopyTargetParticle().CopyTargetShader().StartNewNode();

                //currentCustom.prefabNodeManager.SetNode("embers1", "Torch", "embers").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("embers2", "Torch", "embers").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("embers3", "Torch", "embers").CopyTargetParticle().CopyTargetShader().StartNewNode();

                //currentCustom.prefabNodeManager.SetNode("smoke1", "Torch", "smoke (1)").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("smoke2", "Torch", "smoke (1)").CopyTargetParticle().CopyTargetShader().StartNewNode();
                //currentCustom.prefabNodeManager.SetNode("smoke3", "Torch", "smoke (1)").CopyTargetParticle().CopyTargetShader().StartNewNode();
            }

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            AssetReferences.customItems.Add(currentCustom);

            //Variant
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordFlametalGreatIron");

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("TrophyDraugr", 1, 0).AddResource("Iron", 60, 25).AddResource("ElderBark", 10, 5).AddResource("LeatherScraps", 20, 10);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "sword_secondary";
            Attack3.m_attackStamina = 30f;
            Attack3.m_speedFactor = 0.2f;
            Attack3.m_speedFactorRotation = 0f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 30f;
            Attack3.m_damageMultiplier = 3f;
            Attack3.m_forceMultiplier = 1f;
            Attack3.m_staggerMultiplier = 1f;
            Attack3.m_attackRange = 3f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 45f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

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

            //Axes


            //Black Metal
            currentItem = ExtractGameObjectFromBundle(assetBundle, "AxeBlackMetalBattle");

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("BlackMetal", 60, 30).AddResource("LinenThread", 9, 3);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "swing_pickaxe";
            Attack3.m_attackStamina = 25f;
            Attack3.m_speedFactor = 0.1f;
            Attack3.m_speedFactorRotation = 0.3f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 40f;
            Attack3.m_damageMultiplier = 1.75f;
            Attack3.m_forceMultiplier = 0.75f;
            Attack3.m_staggerMultiplier = 2f;
            Attack3.m_attackRange = 2.5f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 120f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.prefabNodeManager.SetNode("model1", "SwordBlackmetal", "default").CopyTargetMaterial(true).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("model2", "SwordBlackmetal", "default").CopyTargetMaterial(true).StartNewNode();
            currentCustom.prefabNodeManager.SetNode("model3", "SwordBlackmetal", "default").CopyTargetMaterial(true).StartNewNode();

            currentCustom.effectHandler.AddEffect("vfx_clubhit", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_battleaxe_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("vfx_HitSparks", WeaponEffectsManager.EffectList.HIT_TERRAIN).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
            currentCustom.effectHandler.AddEffect("sfx_wood_blocked", WeaponEffectsManager.EffectList.BLOCK).AddEffect("vfx_blocked").AddEffect("fx_block_camshake");
            currentCustom.effectHandler.AddEffect("fx_swing_camshake", WeaponEffectsManager.EffectList.TRIGGER);
            currentCustom.effectHandler.AddEffect("sfx_battleaxe_swing_wosh", WeaponEffectsManager.EffectList.TRAIL);

            currentCustom.prefabNodeManager.SetNode("stand1mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand1mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh1", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();
            currentCustom.prefabNodeManager.SetNode("stand2mesh2", "wood_wall_roof_top", "top").CopyTargetMesh().CopyTargetMaterial().StartNewNode();

            AssetReferences.customItems.Add(currentCustom);

            //DragonSlayer
            currentItem = ExtractGameObjectFromBundle(assetBundle, "SwordDragonSlayer");

            id = currentItem.GetComponent<ItemDrop>();
            if (!Plugin.cc.UsingDefaultItemConfig(currentItem))
            {
                Plugin.cc.ApplyItemDataFromConfigRecord(ref currentItem);
            }
            else
            {
                Plugin.cc.AddItemDataAsConfigRecord(currentItem);
            }

            if (!Plugin.cc.UsingDefaultRecipeConfig(currentItem))
            {
                currentRecipeHelper = Plugin.cc.ApplyRecipeHelperFromConfigRecord(currentItem);
            }
            else
            {
                currentRecipeHelper = new RecipeHelper(currentItem, "forge", 1, 1);
                currentRecipeHelper.AddResource("TrophyDragonQueen", 1, 0).AddResource("TrophyDraugrElite", 1, 0).AddResource("Iron", 90, 45).AddResource("LinenThread", 20, 10);
                Plugin.cc.AddRecipeAsConfigRecord(currentRecipeHelper);
            }

            Attack3 = id.m_itemData.m_shared.m_secondaryAttack.Clone();
            Attack3.m_attackType = Attack.AttackType.Vertical;
            Attack3.m_attackAnimation = "sword_secondary";
            Attack3.m_attackStamina = 30f;
            Attack3.m_speedFactor = 0.2f;
            Attack3.m_speedFactorRotation = 0f;
            Attack3.m_attackStartNoise = 10f;
            Attack3.m_attackHitNoise = 30f;
            Attack3.m_damageMultiplier = 3f;
            Attack3.m_forceMultiplier = 1f;
            Attack3.m_staggerMultiplier = 1f;
            Attack3.m_attackRange = 3.5f;
            Attack3.m_attackHeight = 1f;
            Attack3.m_attackAngle = 45f;

            AssetReferences.TryAddToAttackList(currentItem, Attack3);

            currentCustom = new CustomItem(currentItem, currentRecipeHelper);

            currentCustom.effectHandler.AddEffect("vfx_clubhit", WeaponEffectsManager.EffectList.HIT).AddEffect("sfx_sword_hit").AddEffect("fx_hit_camshake");
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

        //private static HitData.DamageTypes SetDamageValues(ConfigEntry<float>[] newDamageList)
        //{
        //    HitData.DamageTypes newDamage = new HitData.DamageTypes();
        //    newDamage.m_blunt = newDamageList[0].Value;
        //    newDamage.m_slash = newDamageList[1].Value;
        //    newDamage.m_pierce = newDamageList[2].Value;
        //    newDamage.m_chop = newDamageList[3].Value;
        //    newDamage.m_pickaxe = newDamageList[4].Value;
        //    newDamage.m_fire = newDamageList[5].Value;
        //    newDamage.m_frost = newDamageList[6].Value;
        //    newDamage.m_lightning = newDamageList[7].Value;
        //    newDamage.m_poison = newDamageList[8].Value;
        //    newDamage.m_spirit = newDamageList[9].Value;

        //    return newDamage;
        //}

        //private static AssetBundle GetAssetBundleFromFile(string fileName)
        //{
        //    AssetBundle LoadedAssetBundle = null;
        //    string dllPath = "NO PATH SET";
        //    try
        //    {
        //        dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //        LoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(dllPath, fileName));

        //        if (LoadedAssetBundle == null)
        //        {
        //            Debug.Log("Failed to load AssetBundle!");
        //            return null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Plugin.Log.LogError($"Error Trying to load asset bundle {fileName} in location: {dllPath}");
        //        Plugin.Log.LogError($"Catch Exception details: {e.Message} --- {e.StackTrace}");
        //    }
        //    return LoadedAssetBundle;
        //}

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
