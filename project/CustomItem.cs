using UnityEngine;

namespace ValheimMoreTwoHanders
{
    public class CustomItem
    {
        public RecipeHelper recipe;        
        public GameObject gameObject;
        public WeaponEffectsManager effectHandler = new WeaponEffectsManager();
        public PrefabNodeManager prefabNodeManager = new PrefabNodeManager();
        //public MaterialManager materialManager = new MaterialManager();

        public CustomItem(GameObject gameObject, RecipeHelper recipe)
        {
            this.gameObject = gameObject;
            this.recipe = recipe;
        }
        public void FixReferences()
        {
            effectHandler.ApplyEffects(gameObject);
            prefabNodeManager.ApplyNodeChanges(gameObject);
            //materialManager.ApplyMaterial(gameObject);
        }
    }
}
