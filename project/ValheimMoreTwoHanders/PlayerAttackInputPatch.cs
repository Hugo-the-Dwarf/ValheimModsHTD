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

        public class DamageContainer
        {
            public HitData.DamageTypes damage;
            public HitData.DamageTypes damagePerLevel;

            public DamageContainer(ItemDrop.ItemData weapon)
            {
                var shared = weapon.m_shared;
                damage = new HitData.DamageTypes
                {
                    m_blunt = shared.m_damages.m_blunt,
                    m_pierce = shared.m_damages.m_pierce,
                    m_slash = shared.m_damages.m_slash,
                    m_chop = shared.m_damages.m_chop,
                    m_pickaxe = shared.m_damages.m_pickaxe,
                    m_fire = shared.m_damages.m_fire,
                    m_frost = shared.m_damages.m_frost,
                    m_lightning = shared.m_damages.m_lightning,
                    m_poison = shared.m_damages.m_poison,
                    m_spirit = shared.m_damages.m_spirit
                };
                damagePerLevel = new HitData.DamageTypes
                {
                    m_blunt = shared.m_damagesPerLevel.m_blunt,
                    m_pierce = shared.m_damagesPerLevel.m_pierce,
                    m_slash = shared.m_damagesPerLevel.m_slash,
                    m_chop = shared.m_damagesPerLevel.m_chop,
                    m_pickaxe = shared.m_damagesPerLevel.m_pickaxe,
                    m_fire = shared.m_damagesPerLevel.m_fire,
                    m_frost = shared.m_damagesPerLevel.m_frost,
                    m_lightning = shared.m_damagesPerLevel.m_lightning,
                    m_poison = shared.m_damagesPerLevel.m_poison,
                    m_spirit = shared.m_damagesPerLevel.m_spirit
                };
            }
        }

        public static bool Prefix(Player __instance, float dt, ref DamageContainer __state)
        {

            if (__instance.InPlaceMode())
            {
                return true;
            }

            string weaponName = __instance.m_visEquipment.m_rightItem;
            if (weaponName != null & weaponName.Trim() != "")
            {
                if (MyReferences.myListOfExtraAttacks.ContainsKey(weaponName.GetStableHashCode()))
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
                            return true;
                        }

                        if (__instance.m_currentAttack != null)
                        {
                            __instance.m_currentAttack.Stop();
                            __instance.m_previousAttack = __instance.m_currentAttack;
                            __instance.m_currentAttack = null;
                        }

                        //ItemDrop.ItemData currentWeapon = __instance.m_unarmedWeapon.m_itemData;
                        Attack attack = MyReferences.myListOfExtraAttacks[weaponName.GetStableHashCode()].Clone();
                        var currentWeapon = __instance.GetCurrentWeapon();
                        //if(currentWeapon.m_shared.m_icons[0].name.Contains("MaceCoreGreatHTD_Icon"))
                        //{
                            //__state = new DamageContainer(currentWeapon);
                            //currentWeapon.m_shared.m_damages = new HitData.DamageTypes { m_frost = 100f };
                            //currentWeapon.m_shared.m_damagesPerLevel = new HitData.DamageTypes { m_frost = 30f };
                        //}


                        if (attack.Start(__instance, __instance.m_body, __instance.m_zanim, __instance.m_animEvent, __instance.m_visEquipment, currentWeapon, __instance.m_previousAttack, __instance.m_timeSinceLastAttack, 0F))
                        {
                            __instance.m_currentAttack = attack;
                            __instance.m_lastCombatTimer = 0f;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static void Postfix(Player __instance, float dt, ref DamageContainer __state)
        {
            if(__state != null)
            {
                var currentWeapon = __instance.GetCurrentWeapon();
                currentWeapon.m_shared.m_damages = __state.damage;
                currentWeapon.m_shared.m_damagesPerLevel = __state.damagePerLevel;
            }
        }
    }

}
