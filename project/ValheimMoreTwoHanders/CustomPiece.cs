using System.Collections.Generic;
using UnityEngine;

namespace ValheimHTDArmory
{
    public class CustomPiece
    {
        public GameObject myPiece;
        private bool lookUpToolPrefab = false;
        private string buildToolPrefabName; //Hammer is an example
        private ItemDrop buildToolID;
        /*
        Crafting Station forge --- forge (CraftingStation)
        Crafting Station piece_workbench --- piece_workbench (CraftingStation)
        Crafting Station piece_cauldron --- piece_cauldron (CraftingStation)
        Crafting Station piece_stonecutter --- piece_stonecutter (CraftingStation)
        Crafting Station piece_artisanstation --- piece_artisanstation (CraftingStation)
        */
        public string craftingStationName; //piece_workbench or a Workbench is needed to be nearby to build it
        public List<string> placeEffectNames = new List<string>();
        //Probably can have a better data storage option but just making this real quick
        public List<string> destoryedEffectNames = new List<string>();
        public List<string> hitEffectNames = new List<string>();
        public List<string> switchEffectNames = new List<string>();
        //Since the Behemoth Rug can already access the Behemoth Hide I can ignore this for now
        private List<ResourceElement> craftingResources = new List<ResourceElement>();
        //public RequireComponent[] craftingResources;

        public enum EffectListEnum
        {
            PLACEMENT,
            DESTORYED,
            HIT,
            SWITCH
        }

        public CustomPiece SetBuildTool(string buildToolPrefabName)
        {
            this.buildToolPrefabName = buildToolPrefabName;
            this.lookUpToolPrefab = true;
            return this;
        }

        public CustomPiece SetBuildTool(ItemDrop buildToolID)
        {
            this.buildToolID = buildToolID;
            return this;
        }

        public CustomPiece AddEffect(EffectListEnum list, string effectName)
        {
            switch (list)
            {
                case EffectListEnum.PLACEMENT:
                    placeEffectNames.Add(effectName);
                    break;
                case EffectListEnum.DESTORYED:
                    destoryedEffectNames.Add(effectName);
                    break;
                case EffectListEnum.HIT:
                    hitEffectNames.Add(effectName);
                    break;
                case EffectListEnum.SWITCH:
                    switchEffectNames.Add(effectName);
                    break;
            }

            return this;
        }

        public CustomPiece AddEffect(EffectListEnum list, string[] effectNames)
        {
            switch (list)
            {
                case EffectListEnum.PLACEMENT:
                    foreach (string s in effectNames)
                    {
                        placeEffectNames.Add(s);
                    }
                    break;
                case EffectListEnum.DESTORYED:
                    foreach (string s in effectNames)
                    {
                        destoryedEffectNames.Add(s);
                    }
                    break;
                case EffectListEnum.HIT:
                    foreach (string s in effectNames)
                    {
                        hitEffectNames.Add(s);
                    }
                    break;
                case EffectListEnum.SWITCH:
                    foreach (string s in effectNames)
                    {
                        switchEffectNames.Add(s);
                    }
                    break;
            }

            return this;
        }

        public CustomPiece AddResourceRequirement(string prefabName, int amount, int amountPerLevel, bool recover = true)
        {
            craftingResources.Add(new ResourceElement(prefabName, amount, amountPerLevel, recover));
            return this;
        }

        public void AddPiece()
        {
            //I have a list of references for all items, but I can fix this later
            var buildTool = ObjectDB.instance.GetItemPrefab(buildToolPrefabName); //might have to leave this for sync updates
            //var buildTool = MyReferences.listOfItemPrefabs[buildToolPrefabName.GetStableHashCode()];
            if (buildTool != null)
            {
                ItemDrop id = buildTool.GetComponent<ItemDrop>();
                if (id != null)
                {
                    var shared = id.m_itemData.m_shared;
                    //add to build list
                    if (shared.m_buildPieces != null)
                    {
                        shared.m_buildPieces.m_pieces.RemoveAll(bp => bp.name == myPiece.name);
                        shared.m_buildPieces.m_pieces.Add(myPiece);
                    }
                }
            }
        }

        public void UpdateRequirements()
        {
            if (craftingResources.Count > 0)
            {
                var outPutArray = new List<Piece.Requirement>();
                GameObject resourceGameObject = null;
                foreach (ResourceElement re in craftingResources)
                {
                    if (MyReferences.listOfItemPrefabs.ContainsKey(re.prefabItemName.GetStableHashCode()))
                        resourceGameObject = MyReferences.listOfItemPrefabs[re.prefabItemName.GetStableHashCode()];
                    else
                    {
                        resourceGameObject = ObjectDB.instance.GetItemPrefab(re.prefabItemName);
                        if (resourceGameObject != null && !MyReferences.listOfItemPrefabs.ContainsKey(re.prefabItemName.GetStableHashCode()))
                            MyReferences.listOfItemPrefabs.Add(re.prefabItemName.GetStableHashCode(), resourceGameObject);
                    }
                    outPutArray.Add(new Piece.Requirement
                    {
                        m_resItem = resourceGameObject.GetComponent<ItemDrop>()
                        ,
                        m_amount = re.amount
                        ,
                        m_amountPerLevel = re.amountPerLevel
                        ,
                        m_recover = re.recover
                    });
                    resourceGameObject = null;
                }
                myPiece.GetComponent<Piece>().m_resources = outPutArray.ToArray();
            }
        }

        public void CompileAndAddPiece()
        {
            Piece myPieceData = myPiece.GetComponent<Piece>();
            if (myPieceData != null)
            {
                myPieceData.m_craftingStation = MyReferences.listOfCraftingStations[craftingStationName.GetStableHashCode()];
                //Compile Place Effects
                if (placeEffectNames != null && placeEffectNames.Count > 0)
                {
                    EffectList newList = new EffectList();
                    List<EffectList.EffectData> newEffectData = new List<EffectList.EffectData>();
                    foreach (string effectName in placeEffectNames)
                    {
                        EffectList.EffectData compiledEffectData = new EffectList.EffectData();
                        compiledEffectData.m_prefab = MyReferences.listOfEffects[effectName.GetStableHashCode()];
                        newEffectData.Add(compiledEffectData);
                    }
                    newList.m_effectPrefabs = newEffectData.ToArray();
                    myPieceData.m_placeEffect = newList;
                }
            }

            WearNTear myWearNTear = myPiece.GetComponent<WearNTear>();
            if (myWearNTear != null)
            {
                //Compile Destroyed, Hit, and Switch Effects
                if (destoryedEffectNames != null && destoryedEffectNames.Count > 0)
                {
                    EffectList newList = new EffectList();
                    List<EffectList.EffectData> newEffectData = new List<EffectList.EffectData>();
                    foreach (string effectName in destoryedEffectNames)
                    {
                        EffectList.EffectData compiledEffectData = new EffectList.EffectData();
                        compiledEffectData.m_prefab = MyReferences.listOfEffects[effectName.GetStableHashCode()];
                        newEffectData.Add(compiledEffectData);
                    }
                    newList.m_effectPrefabs = newEffectData.ToArray();
                    myWearNTear.m_destroyedEffect = newList;
                }

                if (hitEffectNames != null && hitEffectNames.Count > 0)
                {
                    EffectList newList = new EffectList();
                    List<EffectList.EffectData> newEffectData = new List<EffectList.EffectData>();
                    foreach (string effectName in hitEffectNames)
                    {
                        EffectList.EffectData compiledEffectData = new EffectList.EffectData();
                        compiledEffectData.m_prefab = MyReferences.listOfEffects[effectName.GetStableHashCode()];
                        newEffectData.Add(compiledEffectData);
                    }
                    newList.m_effectPrefabs = newEffectData.ToArray();
                    myWearNTear.m_hitEffect = newList;
                }

                if (switchEffectNames != null && switchEffectNames.Count > 0)
                {
                    EffectList newList = new EffectList();
                    List<EffectList.EffectData> newEffectData = new List<EffectList.EffectData>();
                    foreach (string effectName in switchEffectNames)
                    {
                        EffectList.EffectData compiledEffectData = new EffectList.EffectData();
                        compiledEffectData.m_prefab = MyReferences.listOfEffects[effectName.GetStableHashCode()];
                        newEffectData.Add(compiledEffectData);
                    }
                    newList.m_effectPrefabs = newEffectData.ToArray();
                    myWearNTear.m_switchEffect = newList;
                }
            }

            UpdateRequirements();

            AddPiece();
        }
    }
}
