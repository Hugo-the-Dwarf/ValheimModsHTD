using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace ValheimHTDArmory
{
    [HarmonyPatch]
    public static class VisEquipmentPatch
    {
        [HarmonyPatch(typeof(VisEquipment), "AttachItem")]
        [HarmonyPostfix]
        public static void AttachItemPatchPost(VisEquipment __instance, int itemHash, int variant, Transform joint, ref GameObject __result, bool enableEquipEffects = true)
        {
            if (__instance.m_isPlayer)
            {
                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);

                //Adding other attachs (sheathes, etc)
                if (itemPrefab != null
                    && (__instance.m_rightHand == joint
                    || __instance.m_backMelee == joint
                    || __instance.m_backTwohandedMelee == joint))
                {
                    Transform attachOtherOld = __instance.m_backMelee.Find("attach_other(Clone)");
                    if (attachOtherOld == null)
                    {
                        attachOtherOld = __instance.m_backTwohandedMelee.Find("attach_other(Clone)");
                        if (attachOtherOld != null) Object.Destroy(attachOtherOld.gameObject);
                    }
                    else Object.Destroy(attachOtherOld.gameObject);


                    ItemDrop id = itemPrefab.GetComponent<ItemDrop>();
                    Transform attachOther = PrefabNodeManager.RecursiveChildNodeFinder(itemPrefab.transform, "attach_other");

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
                                gameObjectOther.transform.localPosition = Vector3.zero;
                                gameObjectOther.transform.localRotation = Quaternion.identity;
                                break;
                            case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
                                gameObjectOther.transform.SetParent(__instance.m_backTwohandedMelee.transform);
                                gameObjectOther.transform.localPosition = Vector3.zero;
                                gameObjectOther.transform.localRotation = Quaternion.identity;
                                break;
                        }
                    }
                }
            }
        }

        //[HarmonyPatch(typeof(VisEquipment), "AttachItem")]
        //[HarmonyPrefix]
        //public static bool AttachItemPatch(VisEquipment __instance, int itemHash, int variant, Transform joint, ref GameObject __result, bool enableEquipEffects = true)
        //{
        //    bool isMyModSMRWeapon = MyReferences.listHashOfSMRWeapons.Contains(itemHash);            

        //    if (__instance.m_rightHand == joint && isMyModSMRWeapon)
        //    {

        //        if (itemPrefab == null)
        //        {
        //            //ZLog.Log("Missing attach item: " + itemHash + "  ob:" + base.gameObject.name + "  joint:" + (joint ? joint.name : "none"));
        //            return true;
        //        }

        //        GameObject gameObject = null;
        //        int childCount = itemPrefab.transform.childCount;
        //        for (int i = 0; i < childCount; i++)
        //        {
        //            Transform child = itemPrefab.transform.GetChild(i);
        //            if (child.gameObject.name == "attach_skin")
        //            {
        //                //Plugin.Log.LogMessage("Found attach_skin, Fist Weapon");
        //                gameObject = child.gameObject;
        //                break;
        //            }
        //        }
        //        if (gameObject == null)
        //        {
        //            return true;
        //        }
        //        GameObject gameObject2 = Object.Instantiate(gameObject);
        //        gameObject2.SetActive(true);
        //        __instance.CleanupInstance(gameObject2);
        //        if (enableEquipEffects)
        //        {
        //            __instance.EnableEquipedEffects(gameObject2);
        //        }
        //        if (gameObject.name == "attach_skin")
        //        {
        //            gameObject2.transform.SetParent(__instance.m_bodyModel.transform.parent);
        //            gameObject2.transform.localPosition = Vector3.zero;
        //            gameObject2.transform.localRotation = Quaternion.identity;
        //            SkinnedMeshRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>();
        //            foreach (SkinnedMeshRenderer obj in componentsInChildren)
        //            {
        //                obj.rootBone = __instance.m_bodyModel.rootBone;
        //                obj.bones = __instance.m_bodyModel.bones;
        //            }
        //        }
        //        else
        //        {
        //            gameObject2.transform.SetParent(joint);
        //            gameObject2.transform.localPosition = Vector3.zero;
        //            gameObject2.transform.localRotation = Quaternion.identity;
        //        }
        //        gameObject2.GetComponentInChildren<IEquipmentVisual>()?.Setup(variant);
        //        __result = gameObject2;
        //        return false;
        //    }
        //    return true;
        //}

    }
}