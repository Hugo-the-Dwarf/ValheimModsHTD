using System.Collections.Generic;
using UnityEngine;

namespace ValheimHTDArmory
{
    public class RecipeHelper
    {
        private List<ResourceElement> resources = new();
        private Recipe myRecipe = ScriptableObject.CreateInstance<Recipe>(); //Unitiy will bitch if you don't init recipes this way
        private string myCraftingStation; //string name of the prefab, gets used to do a real lookup later
        private int myCraftingStationHash; 
        private string prefabName;
        public bool recipeEnabled = true;
        private bool referencesFixed = false;

        public RecipeHelper(GameObject prefab, string craftingStationKey, int minCraftingStationLevel, int amount)
        {
            prefabName = prefab.name;
            myRecipe.name = prefabName + "_recipe";
            myRecipe.m_item = prefab.GetComponent<ItemDrop>();
            myRecipe.m_amount = amount;
            myRecipe.m_enabled = true;
            myCraftingStation = craftingStationKey;
            myCraftingStationHash = craftingStationKey.GetStableHashCode();
            myRecipe.m_minStationLevel = minCraftingStationLevel;
            //myRecipe.m_repairStation = null; //Do I even need this for items, Unity has "None" which I'm assuming is NULL
            //myRecipe.m_resources = craftingRequirementList; //Piece.Requirement[] class
        }

        public string GetPrefabName()
        {
            return prefabName;
        }

        public string GetCraftingStation()
        {
            return myCraftingStation;
        }

        public Recipe GetRecipeInstance()
        {
            return myRecipe;
        }

        public ResourceElement[] GetResourceElements()
        {
            return resources.ToArray();
        }

        public RecipeHelper AddResource(string prefabItemName, int amount, int amountPerLevel)
        {
            resources.Add(new ResourceElement(prefabItemName, amount, amountPerLevel, true));
            return this;
        }

        public void FixResources()
        {
            var outPutArray = new List<Piece.Requirement>();
            GameObject resourceGameObject = null;
            foreach (ResourceElement re in resources)
            {
                if (MyReferences.listOfItemPrefabs.ContainsKey(re.prefabItemName.GetStableHashCode())) 
                    resourceGameObject = MyReferences.listOfItemPrefabs[re.prefabItemNameHash];
                else
                {
                    resourceGameObject = ObjectDB.instance.GetItemPrefab(re.prefabItemNameHash);
                    if (resourceGameObject != null && !MyReferences.listOfItemPrefabs.ContainsKey(re.prefabItemNameHash)) 
                        MyReferences.listOfItemPrefabs.Add(re.prefabItemNameHash, resourceGameObject);
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
                if (outPutArray.Count == 4) break;
            }
            myRecipe.m_resources = outPutArray.ToArray();
            //return outPutArray.ToArray();
        }

        public Recipe GetRecipe()
        {
            //Set the CraftingStation via the Global CraftingStation Dictonary that is looked up before this is called
            if (MyReferences.listOfCraftingStations.Count > 0 && MyReferences.listOfCraftingStations.ContainsKey(myCraftingStationHash))
            {
                myRecipe.m_craftingStation = MyReferences.listOfCraftingStations[myCraftingStationHash];
            }

            //Compile the list of requirements which when this is called ObjectDB should be valid
            //myRecipe.m_resources = GetResources();

            //Return the completed Recipe
            return myRecipe;
        }
    }
}
