using System;
using System.Collections.Generic;
using UnityEngine;
using static EffectList;

namespace ValheimHTDArmory
{
    public class EffectsManager
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

        public enum StatusEffectTarget
        {
            SET,
            EQUIP,
            ATTACK,
            ODB
        }

        private class PendingStatusEffect
        {
            public string prefabTarget;
            public StatusEffectTarget targetFrom;
            public StatusEffectTarget targetTo;

            public PendingStatusEffect(string targetPrefabName, StatusEffectTarget targetFrom, StatusEffectTarget targetTo)
            {
                prefabTarget = targetPrefabName;
                this.targetFrom = targetFrom;
                this.targetTo = targetTo;
            }
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
        List<PendingStatusEffect> pendingStatusEffects = new List<PendingStatusEffect>();

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



            ParticleSystemRenderer ps = gameObject.GetComponent<ParticleSystemRenderer>();
            if (ps != null)
            {
                ps.sharedMaterial = MyReferences.listOfMaterials["item_particle".GetStableHashCode()];
            }

            Transform thing = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(gameObject.transform, "attach");
            if (thing != null)
            {
                thing = RecursiveSearchFunctions.ChildNodeFinderBreadthFirst(thing, "equiped");
                if (thing != null)
                {
                    Transform trail = RecursiveSearchFunctions.ChildNodeFinderDepthFirst(thing, "trail");
                    if (trail != null)
                    {
                        MeleeWeaponTrail mwt = trail.gameObject.GetComponent<MeleeWeaponTrail>();
                        mwt._material = MyReferences.listOfMaterials["club_trail".GetStableHashCode()];
                    }
                }
            }

            var gameObjectShared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

            if (pendingStatusEffects.Count > 0)
            {
                try
                {
                    foreach (PendingStatusEffect pendingStatusEffect in pendingStatusEffects)
                    {
                        StatusEffect EffectToApply = null;
                        GameObject targetPrefab = null;
                        if (pendingStatusEffect.targetFrom != StatusEffectTarget.ODB)
                        {
                            targetPrefab = MyReferences.listOfAllGameObjects[pendingStatusEffect.prefabTarget.GetStableHashCode()];
                            if (targetPrefab == null)targetPrefab = ObjectDB.instance.GetItemPrefab(pendingStatusEffect.prefabTarget);                            
                            if (targetPrefab == null) continue;
                        }

                        switch (pendingStatusEffect.targetFrom)
                        {
                            case StatusEffectTarget.ODB:
                                EffectToApply = ObjectDB.instance.GetStatusEffect(pendingStatusEffect.prefabTarget);
                                break;
                            case StatusEffectTarget.SET:
                                EffectToApply = targetPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_setStatusEffect;
                                break;
                            case StatusEffectTarget.EQUIP:
                                EffectToApply = targetPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_equipStatusEffect;
                                break;
                            case StatusEffectTarget.ATTACK:
                                EffectToApply = targetPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_attackStatusEffect;
                                break;
                        }

                        if (EffectToApply != null)
                        {
                            switch (pendingStatusEffect.targetTo)
                            {
                                case StatusEffectTarget.SET:
                                    gameObjectShared.m_setStatusEffect = EffectToApply;
                                    break;
                                case StatusEffectTarget.EQUIP:
                                    gameObjectShared.m_equipStatusEffect = EffectToApply;
                                    break;
                                case StatusEffectTarget.ATTACK:
                                    gameObjectShared.m_attackStatusEffect = EffectToApply;
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.LogError($"Error Trying to fix status effect reference {e.Message}");
                }
            }

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
                        effectData.m_prefab = MyReferences.listOfEffects[pendingEffect.effectName.GetStableHashCode()];
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

            gameObjectShared.m_hitEffect.m_effectPrefabs = hitEffectsData.ToArray();
            gameObjectShared.m_hitTerrainEffect.m_effectPrefabs = hitterrainEffectsData.ToArray();
            gameObjectShared.m_blockEffect.m_effectPrefabs = blockEffectsData.ToArray();
            gameObjectShared.m_startEffect.m_effectPrefabs = startEffectsData.ToArray();
            gameObjectShared.m_holdStartEffect.m_effectPrefabs = holdEffectsData.ToArray();
            gameObjectShared.m_triggerEffect.m_effectPrefabs = triggerEffectsData.ToArray();
            gameObjectShared.m_trailStartEffect.m_effectPrefabs = trailEffectsData.ToArray();

            WipeLists();
        }

        public EffectsManager AddEffect(string effectName)
        {
            pendingEffects.Add(new PendingEffect(effectName, lastUsedEffectList));
            return this;
        }

        public EffectsManager AddEffect(string effectName, EffectList effectList)
        {
            lastUsedEffectList = effectList;
            pendingEffects.Add(new PendingEffect(effectName, effectList));
            return this;
        }

        public EffectsManager AddStatusEffect(string targetPrefabName, StatusEffectTarget targetFrom, StatusEffectTarget targetTo)
        {
            pendingStatusEffects.Add(new PendingStatusEffect(targetPrefabName, targetFrom, targetTo));
            return this;
        }

        private void WipeLists()
        {
            pendingEffects = null;
            pendingStatusEffects = null;
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
