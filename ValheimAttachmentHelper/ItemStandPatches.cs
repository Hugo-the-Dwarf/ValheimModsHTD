using HarmonyLib;
using UnityEngine;

namespace ValheimSkapekeysAndMore
{
    [HarmonyPatch]
    public class ItemStandPatches
    {
        //Thanks to Sarcen, and Mixone for helping me make this patch (it was my 1st patch)
        [HarmonyPatch(typeof(ItemStand), nameof(ItemStand.GetAttachPrefab))]
        [HarmonyPostfix]
        private static void GetAttachPrefabPostfix(ItemStand __instance, GameObject item, ref GameObject __result)
        {
            Transform attach_stand = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(item.transform, "attach_stand");
            if (attach_stand)
            {
                Transform newAttach = attach_stand;
                string standType = __instance.transform.gameObject.GetComponent<Piece>()?.m_description;
                if (standType != null && standType.Trim().Length > 0)
                {
                    if (newAttach.childCount > 0)
                    {
                        for (int i = 0; i < newAttach.childCount; i++)
                        {
                            Transform child = newAttach.GetChild(i);
                            char startingChar = child.gameObject.name[0];
                            if ((startingChar == 'v' || startingChar == 'V') && standType.Contains("vertical"))
                            {
                                newAttach = child;
                                break;
                            }
                            if ((startingChar == 'h' || startingChar == 'H') && standType.Contains("horizontal"))
                            {
                                newAttach = child;
                                break;
                            }
                        }
                    }
                }
                __result = newAttach.gameObject;
            }
        }

    }
}
