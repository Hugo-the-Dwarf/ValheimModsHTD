using System;
using System.Collections.Generic;
using UnityEngine;
using static EffectList;

namespace ValheimMoreTwoHanders
{
    public class WeaponEffectsManager
    {
        public enum EffectList
        {
            HIT,
            HIT_TERRAIN,
            BLOCK,
            START,
            HOLD,
            TRIGGER,
            TRAIL
        }

        private class PendingEffect
        {
            public string effectName;
            public EffectList list;

            public PendingEffect(string effectName, EffectList list)
            {
                this.effectName = effectName;
                this.list = list;
            }
        }

        EffectList lastUsedEffectList = EffectList.HIT;

        List<PendingEffect> pendingEffects = new List<PendingEffect>();

        List<EffectData> hitEffectsData = new List<EffectData>();
        List<EffectData> hitterrainEffectsData = new List<EffectData>();
        List<EffectData> blockEffectsData = new List<EffectData>();
        List<EffectData> startEffectsData = new List<EffectData>();
        List<EffectData> holdEffectsData = new List<EffectData>();
        List<EffectData> triggerEffectsData = new List<EffectData>();
        List<EffectData> trailEffectsData = new List<EffectData>();
        public void ApplyEffects(GameObject gameObject)
        {
            //Set item particle material
            //try
            //{
            //    gameObject.GetComponent<ParticleSystemRenderer>().material = AssetReferences.listOfMaterials["item_particle"];
            //}
            //catch (Exception e)
            //{
            //    Plugin.Log.LogError($"Error getting item spark particle effect material from material list");
            //    Plugin.Log.LogError($"Catch Exception details: {e.Message} --- {e.StackTrace}");
            //}
            if (pendingEffects.Count > 0)
            {

                //Set item attack trail material
                //try
                //{
                //    PrefabNodeManager.RecursiveChildNodeFinder(gameObject.transform, "trail").gameObject.GetComponent<MeleeWeaponTrail>()._material = AssetReferences.listOfMaterials["club_trail"];
                //}
                //catch (Exception e)
                //{
                //    Plugin.Log.LogError($"Error getting weapon trail effect material from material list");
                //    Plugin.Log.LogError($"Catch Exception details: {e.Message} --- {e.StackTrace}");
                //}
                foreach (PendingEffect pendingEffect in pendingEffects)
                {
                    EffectData effectData = new EffectData();
                    effectData.m_enabled = true;
                    try
                    {
                        effectData.m_prefab = AssetReferences.listOfEffects[pendingEffect.effectName];
                    }
                    catch (Exception e)
                    {
                        Plugin.Log.LogError($"Error getting effect {pendingEffect.effectName} from effect list");
                        Plugin.Log.LogError($"Catch Exception details: {e.Message} --- {e.StackTrace}");
                    }
                    switch (pendingEffect.list)
                    {
                        case EffectList.HIT:
                            hitEffectsData.Add(effectData);
                            break;
                        case EffectList.HIT_TERRAIN:
                            hitterrainEffectsData.Add(effectData);
                            break;
                        case EffectList.BLOCK:
                            blockEffectsData.Add(effectData);
                            break;
                        case EffectList.START:
                            startEffectsData.Add(effectData);
                            break;
                        case EffectList.HOLD:
                            holdEffectsData.Add(effectData);
                            break;
                        case EffectList.TRIGGER:
                            triggerEffectsData.Add(effectData);
                            break;
                        case EffectList.TRAIL:
                            trailEffectsData.Add(effectData);
                            break;
                    }
                }
            }
            var gameObjectShared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            gameObjectShared.m_hitEffect.m_effectPrefabs = hitEffectsData.ToArray();
            gameObjectShared.m_hitTerrainEffect.m_effectPrefabs = hitterrainEffectsData.ToArray();
            gameObjectShared.m_blockEffect.m_effectPrefabs = blockEffectsData.ToArray();
            gameObjectShared.m_startEffect.m_effectPrefabs = startEffectsData.ToArray();
            gameObjectShared.m_holdStartEffect.m_effectPrefabs = holdEffectsData.ToArray();
            gameObjectShared.m_triggerEffect.m_effectPrefabs = triggerEffectsData.ToArray();
            gameObjectShared.m_trailStartEffect.m_effectPrefabs = trailEffectsData.ToArray();

            WipeLists();
        }

        public WeaponEffectsManager AddEffect(string effectName)
        {
            pendingEffects.Add(new PendingEffect(effectName, lastUsedEffectList));
            return this;
        }

        public WeaponEffectsManager AddEffect(string effectName, EffectList effectList)
        {
            lastUsedEffectList = effectList;
            pendingEffects.Add(new PendingEffect(effectName, effectList));
            return this;
        }

        private void WipeLists()
        {
            pendingEffects = null;
            hitEffectsData = null;
            hitterrainEffectsData = null;
            blockEffectsData = null;
            startEffectsData = null;
            holdEffectsData = null;
            triggerEffectsData = null;
            trailEffectsData = null;
        }
    }
}
