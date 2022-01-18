using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimHTDArmory
{
    public class CookingRecipe
    {

        //piece_cookingstation
        //piece_cookingstation_iron
        //piece_oven
        public string fromPrefabName;
        public ItemDrop fromItemDrop;
        public string toPrefabName;
        public ItemDrop toItemDrop;
        public float cookingTime;
        public string cookingStationName;

        public CookingRecipe(string fromPrefabName, string toPrefabName, float cookingTime, string cookingStationName)
        {
            this.fromPrefabName = fromPrefabName;
            this.toPrefabName = toPrefabName;
            this.cookingTime = cookingTime;
            this.cookingStationName = cookingStationName;
        }

        public CookingRecipe(ItemDrop fromItemDrop, ItemDrop toItemDrop, float cookingTime, string cookingStationName)
        {
            this.fromItemDrop = fromItemDrop;
            this.toItemDrop = toItemDrop;
            this.cookingTime = cookingTime;
            this.cookingStationName = cookingStationName;
        }

        public ItemDrop GetFromItemDrop()
        {
            if (fromItemDrop != null) return fromItemDrop;
            ItemDrop returnItemDrop = Plugin.myItemList[fromPrefabName.GetStableHashCode()].GetComponent<ItemDrop>();
            if (returnItemDrop == null)
            {
                returnItemDrop = MyReferences.listOfAllGameObjects[fromPrefabName.GetStableHashCode()].GetComponent<ItemDrop>();
                return returnItemDrop;
            }
            else return returnItemDrop;
        }

        public ItemDrop GetToItemDrop()
        {
            if (toItemDrop != null) return toItemDrop;
            ItemDrop returnItemDrop = Plugin.myItemList[toPrefabName.GetStableHashCode()].GetComponent<ItemDrop>();
            if (returnItemDrop == null)
            {
                returnItemDrop = MyReferences.listOfAllGameObjects[toPrefabName.GetStableHashCode()].GetComponent<ItemDrop>();
                return returnItemDrop;
            }
            else return returnItemDrop;
        }
    }
}
