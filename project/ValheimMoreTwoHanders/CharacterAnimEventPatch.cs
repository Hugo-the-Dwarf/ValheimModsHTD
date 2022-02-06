using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ValheimHTDArmory
{
    class CharacterAnimEventPatch
    {
        public static Dictionary<int, Dictionary<float, float>> baseSpeeds = new Dictionary<int, Dictionary<float, float>>();
        public static Dictionary<int, List<float>> newSpeeds = new Dictionary<int, List<float>>();

        [HarmonyPatch(typeof(CharacterAnimEvent), "FixedUpdate")]
        static class CharacterAnimEvent_Awake_Patch
        {
            static void Prefix(ref Animator ___m_animator, Character ___m_character)
            {
                if (Plugin.disableAttackSpeedModule) return;

                if (___m_character is Player)
                {
                    if (___m_animator?.GetCurrentAnimatorClipInfo(0)?.Any() == true && ___m_character.InAttack())
                    {
                        Player player = (Player)___m_character;
                        var item = player.GetCurrentWeapon();

                        if (item != null && item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.TwoHandedWeapon)
                        {
                            if (item.m_shared.m_skillType == Skills.SkillType.Swords
                                || item.m_shared.m_skillType == Skills.SkillType.Clubs
                                || item.m_shared.m_skillType == Skills.SkillType.Axes)
                            {
                                string clipName = ___m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                                int keyName = ((item.m_shared.m_name.Replace(' ', '-')) + clipName).GetStableHashCode();
                                float speedBonus = 1f;
                                var attackForce = item.m_shared.m_attackForce;
                                switch (item.m_shared.m_skillType)
                                {
                                    case Skills.SkillType.Swords:
                                        if (attackForce >= 90) speedBonus = 0.95f;
                                        else speedBonus = 1.15f;
                                        break;
                                    case Skills.SkillType.Clubs:
                                        if (attackForce >= 100) speedBonus = 0.85f;
                                        break;
                                }

                                float newSpeed = ___m_animator.speed;
                                //Plugin.Log.LogMessage($"Animation: {keyName}'s speed is: {newSpeed.ToString()}");
                                if (!baseSpeeds.ContainsKey(keyName))
                                {
                                    Dictionary<float, float> oldNewSpeed = new Dictionary<float, float>();
                                    List<float> newSpeedList = new List<float>();
                                    newSpeed = ___m_animator.speed * speedBonus;
                                    oldNewSpeed.Add(___m_animator.speed, newSpeed);
                                    baseSpeeds.Add(keyName, oldNewSpeed);
                                    newSpeedList.Add(newSpeed);
                                    newSpeeds.Add(keyName, newSpeedList);
                                    //Plugin.Log.LogMessage($"Animation: {keyName}'s speed is changed to: {newSpeed.ToString()}");
                                }
                                else
                                {
                                    if (!newSpeeds[keyName].Contains(newSpeed))
                                    {
                                        if (!baseSpeeds[keyName].ContainsKey(newSpeed))
                                        {
                                            newSpeed = ___m_animator.speed * speedBonus;
                                            baseSpeeds[keyName].Add(___m_animator.speed, newSpeed);
                                            newSpeeds[keyName].Add(newSpeed);
                                            //Plugin.Log.LogMessage($"Animation: {keyName}'s speed is changed to: {newSpeed.ToString()}");
                                        }
                                        else
                                        {
                                            newSpeed = baseSpeeds[keyName][newSpeed];
                                            //Plugin.Log.LogMessage($"Animation: {keyName}'s speed is extracted to: {newSpeed.ToString()}");
                                        }
                                    }
                                    else
                                    {
                                        //nothing? since the current speed is already saved
                                    }

                                }
                                ___m_animator.speed = newSpeed;
                            }
                        }
                    }
                    //else
                    //{

                    //}
                }
            }

            //private static bool BaseSpeedSavedForClip(string keyName, ref float speed, float speedBonus)
            //{
            //    if (baseSpeeds[keyName].Count > 0)
            //    {
            //        foreach (KeyValuePair<float, float> oldNew in baseSpeeds[keyName])
            //        {
            //            if (oldNew.Value == speed * speedBonus)
            //            {
            //                speed = oldNew.Value;
            //                Plugin.Log.LogMessage($"Animation: {keyName}'s speed is extracted to: {speed} From Lookup");
            //                return true;
            //            }
            //        }
            //    }
            //    return false;
            //}

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
