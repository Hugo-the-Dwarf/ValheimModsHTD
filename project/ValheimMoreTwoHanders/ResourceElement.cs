namespace ValheimHTDArmory
{
    public class ResourceElement
    {
        public string prefabItemName; //Res Item - To be Converted to correct prefab when ObjectDB is ready
        public int amount;
        public int amountPerLevel;
        public bool recover = true; //For crafting this is always true, there is no scrap option for items in base game anyways

        public ResourceElement(string prefabItemName, int amount, int amountPerLevel, bool recover = false)
        {
            this.prefabItemName = prefabItemName;
            this.amount = amount;
            this.amountPerLevel = amountPerLevel;
            this.recover = recover;
        }
    }
}
