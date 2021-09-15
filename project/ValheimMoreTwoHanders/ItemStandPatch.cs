using HarmonyLib;
using UnityEngine;

namespace ValheimHTDArmory
{
    //Thanks to Sarcen, and Mixone for helping assemble this
    [HarmonyPatch]
    public class ItemStandPatch
    {
        [HarmonyPatch(typeof(ItemStand), "GetAttachPrefab")]
        [HarmonyPrefix]
        private static bool GetAttachPrefab(ItemStand __instance, GameObject item, ref GameObject __result)
        {
            Transform attach_stand = item.transform.Find("stand");
            if (attach_stand)
            {
                Transform newAttach = attach_stand;
                string standType = __instance.transform.gameObject.GetComponent<Piece>()?.m_description;
                if (standType != null && standType.Trim().Length > 0)
                {
                    if (standType == "Vertical")
                    {
                        newAttach = attach_stand.Find("v");
                    }
                    else if (standType == "Horizontal")
                    {
                        newAttach = attach_stand.Find("h");
                    }
                    if (newAttach == null) newAttach = attach_stand;
                }

                __result = newAttach.gameObject;
                return false;
            }
            // didn't find my custom one, just perform the normal code
            return true;
        }
    }
}