using HarmonyLib;
using UnityEngine;

namespace ValheimHTDArmory
{
    [HarmonyPatch]
    class RecipePatch
    {
        //I am not actually using this, maybe one day in the far future.
        //[HarmonyPatch(typeof(Recipe), "GetRequiredStationLevel")]
        //[HarmonyPostfix]
        //private static void GetRequiredStationLevelPost(Recipe __instance, int quality, ItemDrop ___m_item, ref int __result)
        //{
        //    __result = MyReferences.prefabsThatUpgradeAtLevelOneAlways.Contains(___m_item.name.GetStableHashCode()) ? 1 : __result;
        //}
    }
}
