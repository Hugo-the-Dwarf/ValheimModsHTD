using BepInEx.Configuration;
using HarmonyLib;
using System;
using UnityEngine;

namespace ValheimHTDArmory
{

    [HarmonyPatch(typeof(Player), "PlayerAttackInput")]
    public static class PlayerAttackInputPatch
    {
        //Loaded config values 
        public static ConfigEntry<string> attack3Hotkey;
        public static bool invalidKeyAttack3 = false;

        public static void Postfix(Player __instance, float dt)
        {

            if (__instance.InPlaceMode())
            {
                return;
            }            

            string weaponName = __instance.m_visEquipment.m_rightItem;
            if (weaponName != null & weaponName.Trim() != "")
            {
                if (MyReferences.listOfExtraAttacks.ContainsKey(weaponName))
                {

                    bool attack3Input = false;
                    try
                    {
                        if (attack3Hotkey.Value.Length > 0 && Input.GetKey(attack3Hotkey.Value.ToLower()))
                        {
                            attack3Input = true;
                        }
                    }
                    catch (Exception)
                    {
                        if (!invalidKeyAttack3)
                        {
                            Plugin.Log.LogError("You bound an invalid key code for 'attack 3' that Unity cannot recognize. Check the config file.");
                            invalidKeyAttack3 = true;
                        }
                    }

                    if (attack3Input)//__instance.IsBlocking() && __instance.m_secondaryAttack
                    {
                        __instance.AbortEquipQueue();
                        if ((__instance.InAttack() && !__instance.HaveQueuedChain()) 
                            || __instance.InDodge() || !__instance.CanMove() 
                            || __instance.IsKnockedBack() || __instance.IsStaggering() 
                            || __instance.InMinorAction())
                        {
                            return;
                        }

                        if (__instance.m_currentAttack != null)
                        {
                            __instance.m_currentAttack.Stop();
                            __instance.m_previousAttack = __instance.m_currentAttack;
                            __instance.m_currentAttack = null;
                        }

                        //ItemDrop.ItemData currentWeapon = __instance.m_unarmedWeapon.m_itemData;
                        Attack attack = MyReferences.listOfExtraAttacks[weaponName].Clone();

                        if (attack.Start(__instance, __instance.m_body, __instance.m_zanim, __instance.m_animEvent, __instance.m_visEquipment, __instance.GetCurrentWeapon(), __instance.m_previousAttack, __instance.m_timeSinceLastAttack, 0F))
                        {
                            __instance.m_currentAttack = attack;
                            __instance.m_lastCombatTimer = 0f;
                        }
                    }
                }
            }


        }
    }

}
