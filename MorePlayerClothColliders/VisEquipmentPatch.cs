using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MorePlayerClothColliders
{
    [HarmonyPatch]
    class VisEquipmentPatch
    {
        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.Start))]
        private static class VisEquipment_Start_Patch
        {
            public static void Prefix(VisEquipment __instance)
            {
                if (__instance.m_isPlayer)
                {
                    Ragdoll rd = __instance.gameObject.GetComponent<Ragdoll>();
                    if (rd == null && __instance.m_clothColliders.Length > 0)
                    {
                        __instance.m_clothColliders = ArmorColliderGenerator.GenerateColliders(__instance).ToArray();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.AttachArmor))]
        private static class VisEquipment_AttachArmor_Patch
        {
            public static void Prefix(VisEquipment __instance, int itemHash, ref List<CapsuleCollider> __state)
            {

                if (__instance.m_isPlayer)
                {
                    Ragdoll rd = __instance.gameObject.GetComponent<Ragdoll>();
                    if (rd == null && __instance.m_clothColliders.Length > 0)
                    {
                        if (__instance.m_currentShoulderItemHash == itemHash)
                        {
                            __state = new List<CapsuleCollider>();
                            __state.AddRange(__instance.m_clothColliders);
                            List<CapsuleCollider> shoulderColliders = new List<CapsuleCollider>();
                            shoulderColliders.Add(__instance.m_clothColliders[4]);
                            //1st is hips
                            //2nd is LeftUpLeg
                            //3rd is RightUpLeg
                            //4th is LowerBack
                            //5th is Chest
                            shoulderColliders.AddRange(ShoulderColliderGenerator.GenerateColliders(__instance));

                            __instance.m_clothColliders = shoulderColliders.ToArray();
                        }
                    }
                }
            }

            public static void Postfix(VisEquipment __instance, int itemHash, ref List<CapsuleCollider> __state)
            {
                //Reset the old collider list if the state has data
                if (__state != null && __state.Count > 0)
                {
                    __instance.m_clothColliders = __state.ToArray();
                }
            }
        }
    }
}
