using HarmonyLib;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimMoreTwoHanders
{
    [HarmonyPatch]    
    public static class VisEquipmentPatch
    {
        [HarmonyPatch(typeof(VisEquipment), "SetRightHandEquiped")]
        [HarmonyPrefix]
        public static void SetRightHandEquipedPatch(VisEquipment __instance, int hash)
        {
            if (AssetReferences.myItemHashList.ContainsKey(hash))
            {
                __instance.SetLeftItem(AssetReferences.myItemHashList[hash], 0);
            }
        }

        [HarmonyPatch(typeof(VisEquipment), "AttachItem")]
        [HarmonyPrefix]
        public static bool AttachItemPatch(VisEquipment __instance, int itemHash, int variant, Transform joint, ref GameObject __result, bool enableEquipEffects = true)
        {
            if(AssetReferences.myItemList.Where(item => item.name.GetStableHashCode() == itemHash).Any() && __instance.m_leftHand == joint)
            {
                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);
                if (itemPrefab == null)
                {
                    //ZLog.Log("Missing attach item in patch for join: "+joint.name);
                    return true;
                }
                GameObject gameObject = null;
                int childCount = itemPrefab.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = itemPrefab.transform.GetChild(i);
                    if (child.gameObject.name == "attachleft" || child.gameObject.name == "attach_skin")
                    {
                        gameObject = child.gameObject;
                        break;
                    }
                }
                if (gameObject == null)
                {
                    return true;
                }
                GameObject gameObject2 = Object.Instantiate(gameObject);
                gameObject2.SetActive(value: true);
                __instance.CleanupInstance(gameObject2);
                if (enableEquipEffects)
                {
                    __instance.EnableEquipedEffects(gameObject2);
                }
                if (gameObject.name == "attach_skin")
                {
                    gameObject2.transform.SetParent(__instance.m_bodyModel.transform.parent);
                    gameObject2.transform.localPosition = Vector3.zero;
                    gameObject2.transform.localRotation = Quaternion.identity;
                    SkinnedMeshRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer obj in componentsInChildren)
                    {
                        obj.rootBone = __instance.m_bodyModel.rootBone;
                        obj.bones = __instance.m_bodyModel.bones;
                    }
                }
                else
                {
                    gameObject2.transform.SetParent(joint);
                    gameObject2.transform.localPosition = Vector3.zero;
                    gameObject2.transform.localRotation = Quaternion.identity;
                }
                gameObject2.GetComponentInChildren<IEquipmentVisual>()?.Setup(variant);
                __result = gameObject2;
                return false;
            }
            return true;
        }

    }
    
}
