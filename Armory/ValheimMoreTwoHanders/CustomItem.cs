using UnityEngine;

namespace ValheimHTDArmory
{
    public class CustomItem
    { 
        public GameObject gameObject;
        public EffectsManager effectHandler = new EffectsManager();
        public PrefabNodeManager prefabNodeManager = new PrefabNodeManager();

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
