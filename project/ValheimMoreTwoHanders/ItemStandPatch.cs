using HarmonyLib;
using UnityEngine;

namespace ValheimHTDArmory
{
    //Thanks to Sarcen, and Mixone for helping assemble this
    [HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
    public class ItemStandPatch
    {
        //[HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
        //[HarmonyPrefix]
        private static bool Prefix(ItemStand __instance, GameObject item, ref GameObject __state)
        {
            Transform attach_stand = PrefabNodeManager.RecursiveChildNodeFinder(item.transform, "attach_stand");
            if (attach_stand)
            {
                Transform newAttach = attach_stand;
                string standType = __instance.transform.gameObject.GetComponent<Piece>()?.m_description;
                if (standType != null && standType.Trim().Length > 0)
                {
                    if (standType.Contains("vertical"))
                    {
                        newAttach = PrefabNodeManager.RecursiveChildNodeFinder(attach_stand, "v");
                    }
                    else if (standType.Contains("horizontal"))
                    {
                        newAttach = PrefabNodeManager.RecursiveChildNodeFinder(attach_stand, "h");
                    }
                }
                __state = newAttach.gameObject;
            }
            // didn't find my custom one, just perform the normal code
            return true;
        }

        //[HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
        //[HarmonyPostfix]
        private static void Postfix(ItemStand __instance, GameObject item, ref GameObject __result, ref GameObject __state)
        {
            if (__state != null) __result = __state;
        }
    }
}