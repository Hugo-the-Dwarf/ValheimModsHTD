using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace ValheimHTDArmory
{
    [HarmonyPatch]    
    public static class VisEquipmentPatch
    {
        //[HarmonyPatch(typeof(VisEquipment), "SetRightHandEquiped")]
        //[HarmonyPrefix]
        //public static void SetRightHandEquipedPatch(VisEquipment __instance, int hash)
        //{
        //    if (AssetReferences.myItemHashList.ContainsKey(hash))
        //    {
        //        __instance.SetLeftItem(AssetReferences.myItemHashList[hash], 0);
        //    }
        //}

        [HarmonyPatch(typeof(VisEquipment), "AttachItem")]
        [HarmonyPrefix]
        public static bool AttachItemPatch(VisEquipment __instance, int itemHash, int variant, Transform joint, ref GameObject __result, bool enableEquipEffects = true)
        {
            bool isMyModItemHash = MyReferences.listHashOfSMRWeapons.Contains(itemHash);
            if (__instance.m_rightHand == joint && isMyModItemHash)
            {
                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);
                if (itemPrefab == null)
                {
                    //ZLog.Log("Missing attach item: " + itemHash + "  ob:" + base.gameObject.name + "  joint:" + (joint ? joint.name : "none"));
                    return true;
                }
                GameObject gameObject = null;
                int childCount = itemPrefab.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = itemPrefab.transform.GetChild(i);
                    if (child.gameObject.name == "attach_skin")
                    {
                        Plugin.Log.LogMessage("Found attach_skin, Fist Weapon");
                        gameObject = child.gameObject;
                        break;
                    }
                }
                if (gameObject == null)
                {
                    return true;
                }
                GameObject gameObject2 = Object.Instantiate(gameObject);
                gameObject2.SetActive(true);
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
                //if (gameObject.name == "attach_skin")
                //{
                //    gameObject2.transform.SetParent(__instance.m_bodyModel.transform.parent);
                //    gameObject2.transform.localPosition = Vector3.zero;
                //    gameObject2.transform.localRotation = Quaternion.identity;
                //    SkinnedMeshRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>();
                //    foreach (SkinnedMeshRenderer obj in componentsInChildren)
                //    {
                //        obj.rootBone = __instance.m_bodyModel.rootBone;
                //        if (MyReferences.dictionaryHashOfSMRReorders.ContainsKey(itemHash))
                //        {
                //            obj.bones = MyReferences.dictionaryHashOfSMRReorders[itemHash];
                //        }
                //        else
                //        {
                //            Transform[] origBones = obj.bones;
                //            Transform[] newBones = new Transform[__instance.m_bodyModel.bones.Length];

                //            for (int b = 0; b < newBones.Length; b++)
                //            {
                //                if (b >= origBones.Length)
                //                {
                //                    newBones[b] = __instance.m_bodyModel.bones[b];
                //                }
                //                else
                //                {
                //                    Transform t = System.Array.Find<Transform>(__instance.m_bodyModel.bones, c => c.name == origBones[b].name);
                //                    newBones[b] = t ? t : origBones[b];
                //                }
                //            }
                //            obj.bones = newBones;
                //            MyReferences.dictionaryHashOfSMRReorders.Add(itemHash, newBones);
                //        }
                //    }
                //}
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
            else if (isMyModItemHash)
            {
                GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);
                if (itemPrefab == null)
                {
                    //ZLog.Log("Missing attach item: " + itemHash + "  ob:" + base.gameObject.name + "  joint:" + (joint ? joint.name : "none"));
                    return true;
                }
                GameObject gameObject = null;
                int childCount = itemPrefab.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = itemPrefab.transform.GetChild(i);
                    if (child.gameObject.name == "attach")
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
                    gameObject2.transform.SetParent(joint);
                    gameObject2.transform.localPosition = Vector3.zero;
                    gameObject2.transform.localRotation = Quaternion.identity;
                
                gameObject2.GetComponentInChildren<IEquipmentVisual>()?.Setup(variant);
                __result = gameObject2;
                return false;
            }
            return true;
        }

    }
    
}
//       AttachItem but patch mode
//GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);
//if (itemPrefab == null)
//{
//    //ZLog.Log("Missing attach item: " + itemHash + "  ob:" + base.gameObject.name + "  joint:" + (joint ? joint.name : "none"));
//    return true;
//}
//GameObject gameObject = null;
//int childCount = itemPrefab.transform.childCount;
//for (int i = 0; i < childCount; i++)
//{
//    Transform child = itemPrefab.transform.GetChild(i);
//    if (child.gameObject.name == "attach" || child.gameObject.name == "attach_skin")
//    {
//        gameObject = child.gameObject;
//        break;
//    }
//}
//if (gameObject == null)
//{
//    return true;
//}
//GameObject gameObject2 = Object.Instantiate(gameObject);
//gameObject2.SetActive(value: true);
//__instance.CleanupInstance(gameObject2);
//if (enableEquipEffects)
//{
//    __instance.EnableEquipedEffects(gameObject2);
//}
//if (gameObject.name == "attach_skin")
//{
//    gameObject2.transform.SetParent(__instance.m_bodyModel.transform.parent);
//    gameObject2.transform.localPosition = Vector3.zero;
//    gameObject2.transform.localRotation = Quaternion.identity;
//    SkinnedMeshRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>();
//    foreach (SkinnedMeshRenderer obj in componentsInChildren)
//    {
//        obj.rootBone = __instance.m_bodyModel.rootBone;
//        obj.bones = __instance.m_bodyModel.bones;
//    }
//}
//else
//{
//    gameObject2.transform.SetParent(joint);
//    gameObject2.transform.localPosition = Vector3.zero;
//    gameObject2.transform.localRotation = Quaternion.identity;
//}
//gameObject2.GetComponentInChildren<IEquipmentVisual>()?.Setup(variant);
//return gameObject2;


//      old dual left hand stuff
//GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemHash);
//if (itemPrefab == null)
//{
//    //ZLog.Log("Missing attach item in patch for join: "+joint.name);
//    return true;
//}
//GameObject gameObject = null;
//int childCount = itemPrefab.transform.childCount;
//for (int i = 0; i < childCount; i++)
//{
//    Transform child = itemPrefab.transform.GetChild(i);
//    if (child.gameObject.name == "attachleft" || child.gameObject.name == "attach_skin")
//    {
//        gameObject = child.gameObject;
//        break;
//    }
//}
//if (gameObject == null)
//{
//    return true;
//}
//GameObject gameObject2 = Object.Instantiate(gameObject);
//gameObject2.SetActive(value: true);
//__instance.CleanupInstance(gameObject2);
//if (enableEquipEffects)
//{
//    __instance.EnableEquipedEffects(gameObject2);
//}
//if (gameObject.name == "attach_skin")
//{
//    gameObject2.transform.SetParent(__instance.m_bodyModel.transform.parent);
//    gameObject2.transform.localPosition = Vector3.zero;
//    gameObject2.transform.localRotation = Quaternion.identity;
//    SkinnedMeshRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>();
//    foreach (SkinnedMeshRenderer obj in componentsInChildren)
//    {
//        obj.rootBone = __instance.m_bodyModel.rootBone;
//        obj.bones = __instance.m_bodyModel.bones;
//    }
//}
//else
//{
//    gameObject2.transform.SetParent(joint);
//    gameObject2.transform.localPosition = Vector3.zero;
//    gameObject2.transform.localRotation = Quaternion.identity;
//}
//gameObject2.GetComponentInChildren<IEquipmentVisual>()?.Setup(variant);
//__result = gameObject2;
//return false;