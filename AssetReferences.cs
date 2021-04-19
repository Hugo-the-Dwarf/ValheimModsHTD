using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EffectList;

namespace ValheimMoreTwoHanders
{
    //This class is to hold all references (Base Game, and Mod) 
    public static class AssetReferences
    {
        //saved refrences to base game assets 
        public static Dictionary<string, GameObject> listOfAllGameObjects = new Dictionary<string, GameObject>(); // all base game items and pieces

        public static Dictionary<string, GameObject> listOfItemPrefabs = new Dictionary<string, GameObject>(); //Sub-list of 'listOfAllGameObjects' for just base game items
        public static Dictionary<string, GameObject> listOfPieces = new Dictionary<string, GameObject>(); //Sub-list of 'listOfAllGameObjects' for just Pieces
        public static Dictionary<string, CraftingStation> listOfCraftingStations = new Dictionary<string, CraftingStation>(); //For Recipes/Pieces extracted from valid Pieces
        public static Dictionary<string, GameObject> listOfEffects = new Dictionary<string, GameObject>(); //For weapons/Attacks extracted from valid items

        //my custom lists 
        public static List<GameObject> myItemList = new List<GameObject>(); //Fixed Referenced Compiled Items
        public static List<Recipe> myRecipeList = new List<Recipe>(); // Fixed Referenced Compiled Recipes
        public static List<CustomItem> customItems = new List<CustomItem>(); // Uncompiled Items+Recipes

        //Node data
        public static Dictionary<string, GameObject> targetPrefabNodes = new Dictionary<string, GameObject>(); //Names are PrefabNameNodeName "GameObject.name + node.name"
        public static Dictionary<string, MeshRenderer> targetPrefabMeshRenderers = new Dictionary<string, MeshRenderer>();
        public static Dictionary<string, MeshFilter> targetPrefabMeshFilters = new Dictionary<string, MeshFilter>();



        //List related methods
        //
        //
        //
        public static void TryAddToPieceList(GameObject go)
        {
            if (!listOfAllGameObjects.ContainsKey(go.name)) listOfAllGameObjects.Add(go.name, go);
            if (!listOfPieces.ContainsKey(go.name)) listOfPieces.Add(go.name, go);
        }

        public static void TryAddToItemList(GameObject go)
        {
            if (!listOfAllGameObjects.ContainsKey(go.name)) listOfAllGameObjects.Add(go.name, go);
            if (!listOfItemPrefabs.ContainsKey(go.name)) listOfItemPrefabs.Add(go.name, go);
        }

        public static GameObject GetTargetPrefabNode(string prefabname, string nodeName)
        {
            try
            {
                return targetPrefabNodes[prefabname + nodeName];
            }
            catch { }
            return null;
        }

        public static void TryAddToTargetPrefabNodeList(GameObject go, string nodeName)
        {
            if (!targetPrefabNodes.ContainsKey(go.name + nodeName)) targetPrefabNodes.Add(go.name + nodeName, go);
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
                    if (ed.m_prefab != null && !listOfEffects.ContainsKey(ed.m_prefab.name))
                    {
                        listOfEffects.Add(ed.m_prefab.name, ed.m_prefab);
                    }
                }
            }
        }

    }
}
