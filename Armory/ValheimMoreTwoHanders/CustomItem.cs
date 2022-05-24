using UnityEngine;

namespace ValheimHTDArmory
{
    public class CustomItem
    { 
        public GameObject gameObject;
        public EffectsManager effectHandler = new();
        public PrefabNodeManager prefabNodeManager = new();

        public CustomItem(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
        public void FixReferences()
        {
            effectHandler.ApplyEffects(gameObject);
            prefabNodeManager.ApplyNodeChanges(gameObject);
        }
    }
}
