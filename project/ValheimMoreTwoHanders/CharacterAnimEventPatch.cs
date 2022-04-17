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
        private static float maxSpeedBonus = 1.2f;
        private static float minSpeedBonus = 0.9f;
        private static float speedIdentifier = 0.0017f;

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

                                float speedMod = 1f;
                                //float baseWeight = 1f;
                                //float baseAttackForce = 1f;
                                //float speedModRatio = 0.9f;
                                switch (item.m_shared.m_skillType)
                                {

                                    //case Skills.SkillType.Axes:
                                    //    speedModRatio = 1f;
                                    //    baseWeight = 2.5f;
                                    //    baseAttackForce = 70f;
                                    //    break;
                                    case Skills.SkillType.Swords:
                                        //baseWeight = 3f;
                                        //baseAttackForce = 60f;
                                        speedMod = 1.10f;
                                        break;

                                    case Skills.SkillType.Clubs:
                                        //baseWeight = 6f;
                                        //baseAttackForce = 120f;
                                        speedMod = 0.95f;
                                        break;

                                }

                                float newSpeed = ___m_animator.speed;
                                //Plugin.Log.LogMessage($"Animation: {keyName}'s speed is: {newSpeed.ToString()}");
                                if (!baseSpeeds.ContainsKey(keyName))
                                {
                                    Dictionary<float, float> oldNewSpeed = new Dictionary<float, float>();
                                    List<float> newSpeedList = new List<float>();
                                    //newSpeed = (___m_animator.speed * CalculateAverageSpeedRatio(baseWeight,baseAttackForce,speedMod, speedModRatio, ref item.m_shared)) + speedIdentifier;
                                    newSpeed = (___m_animator.speed * speedMod) + speedIdentifier;
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
                                            //newSpeed = (___m_animator.speed * CalculateAverageSpeedRatio(baseWeight, baseAttackForce, speedMod, speedModRatio, ref item.m_shared)) + speedIdentifier;
                                            newSpeed = (___m_animator.speed * speedMod) + speedIdentifier;
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
                                //Plugin.Log.LogMessa/*ge(newSpeed);*/
                                ___m_animator.speed = newSpeed;
                            }
                        }
                    }
                    //If Attacking
                }
                //If Player
            }
            //prefix end


            private static float CalculateAverageSpeedRatio(float baseWeight, float baseAttackForce, float speedMod,float speedModRatio, ref ItemDrop.ItemData.SharedData shared)
            {
                return Mathf.Clamp((((speedMod + (baseWeight / baseAttackForce)) + (speedMod + (shared.m_weight / shared.m_attackForce))) / 2) * speedModRatio, minSpeedBonus, maxSpeedBonus);
            }


        }
    }
}
