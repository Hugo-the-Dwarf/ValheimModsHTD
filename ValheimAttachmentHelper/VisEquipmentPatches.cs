using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ValheimSkapekeysAndMore
{
    [HarmonyPatch]
    public class VisEquipmentPatches
    {

        private static void TryApplyFemaleShapeKey(SkinnedMeshRenderer smr)
        {
            int femaleShapeKeyIndex = -1;
            Mesh mesh = smr.sharedMesh;
            //int femaleBlendShapeIndex = mesh.GetBlendShapeIndex("Female"); //idk if this is case insenstivie or what the return value is when it fails to find it.
            if (mesh.blendShapeCount > 0)
            {
                for (int i = 0; i < mesh.blendShapeCount; i++)
                {
                    if (mesh.GetBlendShapeName(i).IndexOf("female", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        femaleShapeKeyIndex = i;
                        break;
                    }
                }
                if (femaleShapeKeyIndex >= 0)
                    smr.SetBlendShapeWeight(femaleShapeKeyIndex, 100f);
            }
        }

        private static void EnableSexBasedChildren(GameObject go, int modelIndex)
        {
            if (go.transform.childCount > 0)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    //Plugin.Log.LogMessage($"Enabling Sex Based Children for {go.name}");
                    var child = go.transform.GetChild(i);
                    string childName = child.name;
                    //Plugin.Log.LogMessage($"Child #{i}: {childName}");
                    bool isMale = (childName.IndexOf("male", System.StringComparison.OrdinalIgnoreCase) >= 0 && childName.IndexOf("female", System.StringComparison.OrdinalIgnoreCase) == -1);
                    bool isSexless = childName.IndexOf("male", System.StringComparison.OrdinalIgnoreCase) == -1;
                    //Plugin.Log.LogMessage($"Child #{i}: {childName}, Is Male {isMale}, is Sexless {isSexless}");

                    //male model and the child has at least 'male' in it
                    if (modelIndex == 0 && !isSexless)
                    {
                        if (isMale)
                        {
                            child.gameObject.SetActive(true);
                            //Plugin.Log.LogMessage("Active");
                        }
                        else
                        {
                            child.gameObject.SetActive(false);
                            //Plugin.Log.LogMessage("Deactive");
                        }
                    }

                    if (modelIndex == 1 && !isSexless)
                    {
                        if (!isMale)
                        {
                            child.gameObject.SetActive(true);
                            //Plugin.Log.LogMessage("Active");
                        }
                        else
                        {
                            child.gameObject.SetActive(false);
                            //Plugin.Log.LogMessage("Deactive");
                        }
                    }

                    //    if (modelIndex == 0 && childName.Equals("male", System.StringComparison.OrdinalIgnoreCase))
                    //    go.SetActive(true);
                    //if (modelIndex == 0 && childName.Equals("female", System.StringComparison.OrdinalIgnoreCase))
                    //    go.SetActive(false);
                    //if (modelIndex == 1 && childName.Equals("male", System.StringComparison.OrdinalIgnoreCase))
                    //    go.SetActive(false);
                    //if (modelIndex == 1 && childName.Equals("female", System.StringComparison.OrdinalIgnoreCase))
                    //    go.SetActive(true);
                }
            }
        }

        private static void TryAttachOther(VisEquipment vis, ItemDrop.ItemData id)
        {
            Transform attachOther = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(id.m_dropPrefab.transform, "attach_other");
            if (attachOther != null)
            {
                GameObject gameObjectOther = Object.Instantiate(attachOther.gameObject);
                gameObjectOther.SetActive(true);
                vis.CleanupInstance(gameObjectOther);
                AttachOther(vis, id.m_shared, gameObjectOther.transform);
            }
        }

        private static void AttachOther(VisEquipment vis, ItemDrop.ItemData.SharedData shared, Transform objectTransform)
        {
            var attachpoint = shared.m_attachOverride;
            if (attachpoint == ItemDrop.ItemData.ItemType.None) attachpoint = shared.m_itemType;
            switch (attachpoint)
            {
                case ItemDrop.ItemData.ItemType.OneHandedWeapon:
                    objectTransform.SetParent(vis.m_backMelee.transform);
                    break;
                case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
                    objectTransform.SetParent(vis.m_backTwohandedMelee.transform);
                    break;
                case ItemDrop.ItemData.ItemType.Bow:
                    objectTransform.SetParent(vis.m_backBow.transform);
                    break;
                case ItemDrop.ItemData.ItemType.Attach_Atgeir:
                    objectTransform.SetParent(vis.m_backAtgeir.transform);
                    break;
                case ItemDrop.ItemData.ItemType.Shield:
                    objectTransform.SetParent(vis.m_backShield.transform);
                    break;
                case ItemDrop.ItemData.ItemType.Tool:
                    objectTransform.SetParent(vis.m_backTool.transform);
                    break;
            }
            objectTransform.localPosition = Vector3.zero;
            objectTransform.localRotation = Quaternion.identity;
        }

        private static void ClearAttachOthers(ref VisEquipment vs)
        {
            ClearAttach(vs.m_backAtgeir);
            ClearAttach(vs.m_backBow);
            ClearAttach(vs.m_backMelee);
            ClearAttach(vs.m_backShield);
            ClearAttach(vs.m_backTool);
            ClearAttach(vs.m_backTwohandedMelee);
        }

        private static void ClearAttach(Transform t)
        {
            Transform attachOtherOld = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(t, "attach_other(Clone)");
            if (attachOtherOld != null) Object.Destroy(attachOtherOld.gameObject);
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.UpdateEquipmentVisuals))]
        [HarmonyPrefix]
        public static void UpdateEquipmentVisualsPrefix(VisEquipment __instance)
        {
            if (__instance.m_isPlayer)
            {
                var player = __instance.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    if (player.m_hiddenRightItem == null && player.m_rightItem == null)
                        ClearAttachOthers(ref __instance);

                    if (__instance.m_currentRightItemHash != 0)
                    {
                        var prefab = ObjectDB.instance.GetItemPrefab(__instance.m_currentRightItemHash);
                        if(prefab != null)
                        {
                            if (RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(prefab.transform, "attach_other") != null)
                                ClearAttachOthers(ref __instance);
                        }
                        
                    }
                }
            }

        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.UpdateEquipmentVisuals))]
        [HarmonyPostfix]
        public static void UpdateEquipmentVisualsPostfix(VisEquipment __instance)
        {
            //do attach other adds
            if (__instance.m_isPlayer)
            {
                var player = __instance.gameObject.GetComponent<Player>();

                if (player != null)
                {
                    if (player.m_rightItem != null && player.m_rightItem.m_dropPrefab != null)
                    {
                        TryAttachOther(__instance, player.m_rightItem);
                    }

                    if (player.m_hiddenRightItem != null && player.m_hiddenRightItem.m_dropPrefab != null)
                    {
                        TryAttachOther(__instance, player.m_hiddenRightItem);
                    }

                    if (player.m_leftItem != null && player.m_leftItem.m_dropPrefab != null)
                    {
                        TryAttachOther(__instance, player.m_leftItem);
                    }

                    if (player.m_hiddenLeftItem != null && player.m_hiddenLeftItem.m_dropPrefab != null)
                    {
                        TryAttachOther(__instance, player.m_hiddenLeftItem);
                    }
                }
            }

        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.AttachItem))]
        [HarmonyPostfix]
        //public static void AttachItemPatchPost(VisEquipment __instance, int itemHash, int variant, Transform joint, ref GameObject __result, bool enableEquipEffects = true)
        public static void AttachItemPatchPost(VisEquipment __instance, ref GameObject __result)
        {
            try
            {
                if (__instance.m_isPlayer)
                {
                    var smr = __result.GetComponent<SkinnedMeshRenderer>();
                    if (smr != null)
                    {
                        int modelIndex = __instance.GetModelIndex();
                        if (modelIndex == 1)
                        {
                            TryApplyFemaleShapeKey(smr);
                        }
                        EnableSexBasedChildren(__result, modelIndex);
                    }
                }
            }
            catch(System.Exception ex)
            {
                Plugin.Log.LogError("Error in AttachItemPatchPost, exception handled. Below is the stacktrace.");
                Plugin.Log.LogError(ex.StackTrace);
            }
        }

        //[HarmonyPatch(typeof(VisEquipment), "AttachItem")]
        //[HarmonyPostfix]
        //public static void AttachItemPatchPost(VisEquipment __instance, int itemHash, int variant, Transform joint, ref GameObject __result, bool enableEquipEffects = true)
        //{
        //    if (__instance.m_isPlayer)
        //    {

        //        GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);
        //        //ClearAttachOthers(ref __instance);
        //        //Adding other attachs (sheathes, etc)
        //        if (itemPrefab != null)
        //        {

        //            ItemDrop id = itemPrefab.GetComponent<ItemDrop>();
        //            Transform attachOther = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(itemPrefab.transform, "attach_other");

        //            if (id != null && attachOther != null)
        //            {
        //                GameObject gameObjectOther = Object.Instantiate(attachOther.gameObject);
        //                gameObjectOther.SetActive(true);
        //                __instance.CleanupInstance(gameObjectOther);

        //                var shared = id.m_itemData.m_shared;
        //                var attachpoint = shared.m_attachOverride;
        //                if (attachpoint == ItemDrop.ItemData.ItemType.None) attachpoint = shared.m_itemType;
        //                switch (attachpoint)
        //                {
        //                    case ItemDrop.ItemData.ItemType.OneHandedWeapon:
        //                        gameObjectOther.transform.SetParent(__instance.m_backMelee.transform);
        //                        break;
        //                    case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
        //                        gameObjectOther.transform.SetParent(__instance.m_backTwohandedMelee.transform);
        //                        break;
        //                    case ItemDrop.ItemData.ItemType.Bow:
        //                        gameObjectOther.transform.SetParent(__instance.m_backBow.transform);
        //                        break;
        //                    case ItemDrop.ItemData.ItemType.Attach_Atgeir:
        //                        gameObjectOther.transform.SetParent(__instance.m_backAtgeir.transform);
        //                        break;
        //                    case ItemDrop.ItemData.ItemType.Shield:
        //                        gameObjectOther.transform.SetParent(__instance.m_backShield.transform);
        //                        break;
        //                    case ItemDrop.ItemData.ItemType.Tool:
        //                        gameObjectOther.transform.SetParent(__instance.m_backTool.transform);
        //                        break;
        //                }
        //                gameObjectOther.transform.localPosition = Vector3.zero;
        //                gameObjectOther.transform.localRotation = Quaternion.identity;
        //            }
        //        }
        //    }
        //}




        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.AttachArmor))]
        [HarmonyPostfix]
        public static void AttachArmorPostFix(VisEquipment __instance, ref List<GameObject> __result)
        {
            int modelIndex = __instance.GetModelIndex();
            foreach (var go in __result)
            {

                //Check to see if there is a "Female" Shapekey to enable
                var smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (smrs != null && smrs.Length > 0 && modelIndex == 1)
                {
                    foreach (var smr in smrs)
                    {
                        TryApplyFemaleShapeKey(smr);
                    }
                }
                //Perhaps the 'attach_skin' has a male or female model
                //GoldenJude's gave me this code here, which a lot of the other M/F stuff is based on.
                EnableSexBasedChildren(go, modelIndex);
            }
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetChestEquiped))]
        [HarmonyPostfix]
        public static void SetChestEquipedPostFix(VisEquipment __instance, int hash)
        {
            if (__instance.m_isPlayer)
            {
                if (__instance.m_currentChestItemHash == hash)
                {
                    int modelIndex = __instance.GetModelIndex();
                    if (modelIndex == 1)
                    {
                        GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(hash);
                        if (itemPrefab == null)
                        {
                            //ZLog.Log("Missing chest item " + hash);
                            return;
                        }
                        ItemDrop component = itemPrefab.GetComponent<ItemDrop>();
                        if ((bool)component.m_itemData.m_shared.m_armorMaterial)
                        {
                            var armorMaterial = component.m_itemData.m_shared.m_armorMaterial;
                            if (armorMaterial.HasProperty("_ChestTexFemale") && armorMaterial.GetTexture("_ChestTexFemale") != null)
                            {
                                __instance.m_bodyModel.material.SetTexture("_ChestTex", armorMaterial.GetTexture("_ChestTexFemale"));
                                __instance.m_bodyModel.material.SetTexture("_ChestBumpMap", armorMaterial.GetTexture("_ChestBumpMapFemale"));
                                __instance.m_bodyModel.material.SetTexture("_ChestMetal", armorMaterial.GetTexture("_ChestMetalFemale"));
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetLegEquiped))]
        [HarmonyPostfix]
        public static void SetLegEquipedPostFix(VisEquipment __instance, int hash)
        {
            if (__instance.m_isPlayer)
            {
                if (__instance.m_currentLegItemHash == hash)
                {
                    int modelIndex = __instance.GetModelIndex();
                    if (modelIndex == 1)
                    {
                        GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(hash);
                        if (itemPrefab == null)
                        {
                            //ZLog.Log("Missing chest item " + hash);
                            return;
                        }
                        ItemDrop component = itemPrefab.GetComponent<ItemDrop>();
                        if ((bool)component.m_itemData.m_shared.m_armorMaterial)
                        {
                            var armorMaterial = component.m_itemData.m_shared.m_armorMaterial;
                            if (armorMaterial.HasProperty("_LegTexFemale") && armorMaterial.GetTexture("_LegTexFemale") != null)
                            {
                                __instance.m_bodyModel.material.SetTexture("_LegTex", armorMaterial.GetTexture("_LegTexFemale"));
                                __instance.m_bodyModel.material.SetTexture("_LegBumpMap", armorMaterial.GetTexture("_LegBumpMapFemale"));
                                __instance.m_bodyModel.material.SetTexture("_LegMetal", armorMaterial.GetTexture("_LegMetalFemale"));
                            }


                        }
                    }
                }
            }
        }




    }
}
