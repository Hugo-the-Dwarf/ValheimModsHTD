using System;
using HarmonyLib;
using UnityEngine;

namespace ValheimHTDArmory
{
    [HarmonyPatch]
    class RecipePatch
    {
        [HarmonyPatch(typeof(Recipe), "GetRequiredStationLevel")]
        [HarmonyPostfix]
        private static void GetRequiredStationLevelPost(Recipe __instance, int quality, ItemDrop ___m_item, ref int __result)
        {
            if (___m_item == null || ___m_item.name == null)
            {
                return;
            }

            try
            {
                __result = MyReferences.prefabsThatUpgradeAtLevelOneAlways.Contains(___m_item.name.GetStableHashCode()) ? 1 : __result;
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
