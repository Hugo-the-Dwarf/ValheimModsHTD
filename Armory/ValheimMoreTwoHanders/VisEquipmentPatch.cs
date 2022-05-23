using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ValheimHTDArmory
{
    //[HarmonyPatch]
    public static class VisEquipmentPatch
    {
        /*
        [HarmonyPatch(typeof(VisEquipment), "AttachItem")]
        [HarmonyPostfix]
        public static void AttachItemPatchPost(VisEquipment __instance, int itemHash, int variant, Transform joint, ref GameObject __result, bool enableEquipEffects = true)
        {
            if (__instance.m_isPlayer)
            {

                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);
                ClearAttachOthers(ref __instance);
                //Adding other attachs (sheathes, etc)
                if (itemPrefab != null)
                {

                    ItemDrop id = itemPrefab.GetComponent<ItemDrop>();
                    Transform attachOther = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(itemPrefab.transform, "attach_other");

                    if (id != null && attachOther != null)
                    {
                        GameObject gameObjectOther = Object.Instantiate(attachOther.gameObject);
                        gameObjectOther.SetActive(true);
                        __instance.CleanupInstance(gameObjectOther);

                        var shared = id.m_itemData.m_shared;
                        var attachpoint = shared.m_attachOverride;
                        if (attachpoint == ItemDrop.ItemData.ItemType.None) attachpoint = shared.m_itemType;
                        switch (attachpoint)
                        {
                            case ItemDrop.ItemData.ItemType.OneHandedWeapon:
                                gameObjectOther.transform.SetParent(__instance.m_backMelee.transform);
                                break;
                            case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
                                gameObjectOther.transform.SetParent(__instance.m_backTwohandedMelee.transform);
                                break;
                            case ItemDrop.ItemData.ItemType.Bow:
                                gameObjectOther.transform.SetParent(__instance.m_backBow.transform);
                                break;
                            case ItemDrop.ItemData.ItemType.Attach_Atgeir:
                                gameObjectOther.transform.SetParent(__instance.m_backAtgeir.transform);
                                break;
                            case ItemDrop.ItemData.ItemType.Shield:
                                gameObjectOther.transform.SetParent(__instance.m_backShield.transform);
                                break;
                            case ItemDrop.ItemData.ItemType.Tool:
                                gameObjectOther.transform.SetParent(__instance.m_backTool.transform);
                                break;
                        }
                        gameObjectOther.transform.localPosition = Vector3.zero;
                        gameObjectOther.transform.localRotation = Quaternion.identity;
                    }
                }
            }
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

        //GoldenJude's Patch for male and female stuff for armors
        [HarmonyPatch(typeof(VisEquipment), "AttachArmor")]
        [HarmonyPostfix]
        public static void AttachArmorPostFix(VisEquipment __instance, ref List<GameObject> __result)
        {
            int modelIndex = __instance.GetModelIndex();
            foreach (var go in __result)
            {
                var smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (smrs != null && smrs.Length > 0 && modelIndex == 1)
                {
                    foreach (var smr in smrs)
                        smr.SetBlendShapeWeight(0, 100f);
                }

                if (go.transform.childCount > 0)
                {
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        var child = go.transform.GetChild(i);
                        char firstChildNameChar = child.name[0];
                        if (modelIndex == 0 && (firstChildNameChar == 'f' || firstChildNameChar == 'F'))
                            go.SetActive(false);
                        if (modelIndex == 1 && (firstChildNameChar == 'm' || firstChildNameChar == 'M'))
                            go.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "SetChestEquiped")]
        [HarmonyPostfix]
        public static void SetChestEquipedPostFix(VisEquipment __instance, int hash)
        {

        }
        */
    }
}