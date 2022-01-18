using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ValheimHTDArmory
{
    class CharacterAnimEventPatch
    {
        public static Dictionary<string, Dictionary<float, float>> baseSpeeds = new Dictionary<string, Dictionary<float, float>>();

        [HarmonyPatch(typeof(CharacterAnimEvent), "FixedUpdate")]
        static class CharacterAnimEvent_Awake_Patch
        {
            static void Prefix(ref Animator ___m_animator, Character ___m_character)
            {
                //if (1 == 1) return; //Idk how to do this yet, so disabled.

                if (___m_character is Player)
                {
                    if (___m_animator?.GetCurrentAnimatorClipInfo(0)?.Any() == true && ___m_character.InAttack())
                    {

                        //Plugin.Log.LogMessage($"Animation Clip: {___m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name}");
                        Player player = (Player)___m_character;
                        var item = player.GetCurrentWeapon();

                        if (item != null && item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon)
                        {
                            if (item.m_shared.m_skillType == Skills.SkillType.Swords
                                || item.m_shared.m_skillType == Skills.SkillType.Clubs
                                || item.m_shared.m_skillType == Skills.SkillType.Axes)
                            {
                                string clipName = ___m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                                if (clipName.ToLower().Contains("idle")) return;
                                string keyName = (item.m_shared.m_name.Replace(' ','-')) + clipName;
                                float speedBonus = 1f;
                                switch (item.m_shared.m_skillType)
                                {
                                    case Skills.SkillType.Swords:
                                        speedBonus = 1.5f;
                                        break;
                                    case Skills.SkillType.Clubs:
                                        var attackForce = item.m_shared.m_attackForce;
                                        if (attackForce > 100) speedBonus = 0.8f;
                                        else speedBonus = 0.90f;
                                        break;
                                }

                                float newSpeed = ___m_animator.speed;
                                Plugin.Log.LogMessage($"Animation: {keyName}'s speed is: {newSpeed}");
                                if (!baseSpeeds.ContainsKey(keyName))
                                {
                                    Dictionary<float, float> newOldSpeed = new Dictionary<float, float>();
                                    newSpeed = ___m_animator.speed * speedBonus;
                                    newOldSpeed.Add(newSpeed, ___m_animator.speed);
                                    baseSpeeds.Add(keyName, newOldSpeed);
                                    Plugin.Log.LogMessage($"Animation: {keyName}'s speed is changed to: {newSpeed}");
                                }
                                else
                                {
                                    if (!baseSpeeds[keyName].ContainsKey(___m_animator.speed) 
                                        && baseSpeeds[keyName].ContainsKey(___m_animator.speed * speedBonus))
                                    {
                                        newSpeed = ___m_animator.speed * speedBonus;
                                        baseSpeeds[keyName].Add(newSpeed, ___m_animator.speed);
                                        Plugin.Log.LogMessage($"Animation: {keyName}'s speed is changed to: {newSpeed}");
                                    }

                                }

                                //___m_animator.speed = ___m_animator.speed * speedBonus;
                                ___m_animator.speed = newSpeed;
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }

            //static void Postfix(ref Animator ___m_animator, Character ___m_character, ref float __state)
            //{
            //    if (___m_animator?.GetCurrentAnimatorClipInfo(0)?.Any() == true && ___m_character.InAttack())
            //    {
            //        if (__state != -1f)
            //        {
            //            ___m_animator.speed = __state;
            //        }
            //    }
            //}
        }
    }
}
