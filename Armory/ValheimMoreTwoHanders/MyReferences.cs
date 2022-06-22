using System;
using System.Collections.Generic;
using UnityEngine;
using static EffectList;

namespace ValheimHTDArmory
{
    //This class is to hold all references (Base Game, and Mod) 
    public static class MyReferences
    {
        //saved refrences to base game assets 
        public static Dictionary<int, GameObject> listOfAllGameObjects = new(); // all base game items and pieces

        public static Dictionary<int, GameObject> listOfItemPrefabs = new(); //Sub-list of 'listOfAllGameObjects' for just base game items
        public static Dictionary<int, GameObject> listOfPieces = new(); //Sub-list of 'listOfAllGameObjects' for just Pieces
        public static Dictionary<int, CraftingStation> listOfCraftingStations = new(); //For Recipes/Pieces extracted from valid Pieces
        public static Dictionary<int, CookingStation> listOfCookingStations = new(); //For Recipes/Pieces extracted from valid Pieces
        public static Dictionary<int, GameObject> listOfEffects = new(); //For weapons/Attacks extracted from valid items
        public static Dictionary<int, Material> listOfMaterials = new();

        //Saved used References

        //my custom lists 
        //public static List<int> listHashOfSMRWeapons = new List<int>(); //Fixed Referenced Compiled Items
        
               
        
        public static Dictionary<int, StatusEffect> myStatusEffects = new();
        public static List<int> prefabsThatUpgradeAtLevelOneAlways = new();

        //Node data
        public static Dictionary<int, GameObject> targetPrefabNodes = new(); //Names are PrefabNameNodeName "GameObject.name + node.name"
        public static Dictionary<int, MeshFilter> targetMeshFilters = new();
        public static Dictionary<int, Material> newMaterials = new();
        public static Dictionary<int, ParticleSystemRenderer> targetParticleSystemRenderers = new();

        //NewAttacks
        public static Dictionary<int, Attack> myListOfExtraAttacks = new();

        //List related methods
        //
        //
        //
        public static void TryAddToPieceList(GameObject go)
        {
            if (!listOfAllGameObjects.ContainsKey(go.name.GetStableHashCode())) listOfAllGameObjects.Add(go.name.GetStableHashCode(), go);
            if (!listOfPieces.ContainsKey(go.name.GetStableHashCode())) listOfPieces.Add(go.name.GetStableHashCode(), go);
        }

        public static void TryAddToItemList(GameObject go)
        {
            if (!listOfAllGameObjects.ContainsKey(go.name.GetStableHashCode())) listOfAllGameObjects.Add(go.name.GetStableHashCode(), go);
            if (!listOfItemPrefabs.ContainsKey(go.name.GetStableHashCode())) listOfItemPrefabs.Add(go.name.GetStableHashCode(), go);
        }

        public static void TryAddToAttackList(GameObject go, Attack atk)
        {
            if (!myListOfExtraAttacks.ContainsKey(go.name.GetStableHashCode())) myListOfExtraAttacks.Add(go.name.GetStableHashCode(), atk);
        }

        public static void TryAddToStatusEffectList(StatusEffect effect)
        {
            if (!myStatusEffects.ContainsKey(effect.name.GetStableHashCode())) myStatusEffects.Add(effect.name.GetStableHashCode(), effect);
        }

        public static GameObject GetTargetPrefabNode(string prefabname, string nodeName)
        {
            try
            {
                return targetPrefabNodes[(prefabname + nodeName).GetStableHashCode()];
            }
            catch { }
            return null;
        }

        public static void TryAddToTargetPrefabNodeList(GameObject go, string nodeName)
        {
            if (!targetPrefabNodes.ContainsKey((go.name + nodeName).GetStableHashCode())) targetPrefabNodes.Add((go.name + nodeName).GetStableHashCode(), go);
        }

        public static void TryAddToMaterialList(Material mat)
        {
            //Plugin.Log.LogMessage($"Trying to add {mat.name} into MaterialList.");
            if (!listOfMaterials.ContainsKey(mat.name.GetStableHashCode()))
            {
                listOfMaterials.Add(mat.name.GetStableHashCode(), mat);
                //Plugin.Log.LogMessage($"material {mat.name} Added to List of Materials.");
            }
        }

        public static void TryAddToMaterialList(Material mat, string keyName)
        {
            //Plugin.Log.LogMessage($"Trying to add {mat.name} into MaterialList using key {keyName}.");
            if (!listOfMaterials.ContainsKey(mat.name.GetStableHashCode()))
            {
                listOfMaterials.Add(keyName.GetStableHashCode(), mat);
                //Plugin.Log.LogMessage($"material {mat.name} Added to List of Materials.");
            }
        }


        //Extraction Related Methods
        //
        //
        public static void TryExtractEffectsFromItemDropShared(ItemDrop.ItemData.SharedData shared)
        {
            CollectEffectsFromItemDrop(shared.m_hitEffect);
            CollectEffectsFromItemDrop(shared.m_hitTerrainEffect);
            CollectEffectsFromItemDrop(shared.m_holdStartEffect);
            CollectEffectsFromItemDrop(shared.m_startEffect);
            CollectEffectsFromItemDrop(shared.m_triggerEffect);
            CollectEffectsFromItemDrop(shared.m_blockEffect);
            CollectEffectsFromItemDrop(shared.m_trailStartEffect);            
        }

        

        private static void CollectEffectsFromItemDrop(EffectList list)
        {
            if (list != null && list.m_effectPrefabs.Length > 0)
            {
                foreach (EffectData ed in list.m_effectPrefabs)
                {
                    if (ed.m_prefab != null && !listOfEffects.ContainsKey(ed.m_prefab.name.GetStableHashCode()))
                    {
                        listOfEffects.Add(ed.m_prefab.name.GetStableHashCode(), ed.m_prefab);
                    }
                }
            }
        }

    }
}
