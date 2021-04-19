using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimMoreTwoHanders
{
    public class MaterialManager
    {
        private MaterialObject currentMaterialObject;
        private List<MaterialObject> materials = new List<MaterialObject>();

        private class MaterialObject
        {
            //private Texture2D color;
            //private Texture2D metal;
            //private Texture2D normal;
            //private Texture2D emission;
            public string prefabToGetMaterialFrom;
            public string myNode;
            public string targetNode;
            public Color texColor;
            public bool replaceTextures;
            public bool cloneTargetMaterial;
            public bool replaceColor = false;
        }

        public MaterialManager()
        {
            currentMaterialObject = new MaterialObject();
        }

        public void SetMyNode(string node)
        {
            currentMaterialObject.myNode = node;
        }

        public void SetTargetNode(string node)
        {
            currentMaterialObject.targetNode = node;
        }

        public void SetCloneTargetSwapTextures(string prefabName)
        {
            currentMaterialObject.prefabToGetMaterialFrom = prefabName;
            currentMaterialObject.replaceTextures = true;
        }

        public void SetCloneTargetSwapTextures(string prefabName, Color newColor)
        {
            currentMaterialObject.prefabToGetMaterialFrom = prefabName;
            currentMaterialObject.texColor = newColor;
            currentMaterialObject.replaceTextures = true;
            currentMaterialObject.replaceColor = true;
        }

        public void SetCloneTarget(string prefabName)
        {
            currentMaterialObject.prefabToGetMaterialFrom = prefabName;
            currentMaterialObject.cloneTargetMaterial = true;
        }

        public void SetCloneTarget(string prefabName, Color newColor)
        {
            currentMaterialObject.prefabToGetMaterialFrom = prefabName;
            currentMaterialObject.texColor = newColor;
            currentMaterialObject.cloneTargetMaterial = true;
            currentMaterialObject.replaceColor = true;
        }

        public void StartNewMaterial()
        {
            materials.Add(currentMaterialObject);
            currentMaterialObject = new MaterialObject();
        }

        public void ApplyMaterial(GameObject go)
        {
            materials.Add(currentMaterialObject);
            foreach (MaterialObject mo in materials)
            {
                if (mo.prefabToGetMaterialFrom != null)
                {
                    GameObject rgo;
                    if (Plugin.listOfItemPrefabs.ContainsKey(mo.prefabToGetMaterialFrom)) rgo = Plugin.listOfItemPrefabs[mo.prefabToGetMaterialFrom];
                    else
                    {
                        rgo = ObjectDB.instance.GetItemPrefab(mo.prefabToGetMaterialFrom);
                        if (rgo != null && !Plugin.listOfItemPrefabs.ContainsKey(mo.prefabToGetMaterialFrom)) Plugin.listOfItemPrefabs.Add(mo.prefabToGetMaterialFrom, rgo);
                    }

                    //RecursiveMeshRendererComponetFinder just scans through the transforms.GameObjects trying to get a MeshRenderer Componet
                    MeshRenderer goRenderer;
                    Material newMat;
                    Material oldMat = null;

                    if (mo.targetNode != null)
                    {
                        newMat = ItemManager.RecursiveMeshRendererComponetFinderUsingNodeName(rgo, mo.targetNode).material;
                    }
                    else
                    {
                        newMat = new Material(ItemManager.RecursiveMeshRendererComponetFinder(rgo).material);
                    }

                    if (mo.myNode != null)
                    {
                        goRenderer = ItemManager.RecursiveMeshRendererComponetFinderUsingNodeName(go, mo.myNode);
                        if (!mo.cloneTargetMaterial) oldMat = goRenderer.material;
                    }
                    else
                    {
                        goRenderer = ItemManager.RecursiveMeshRendererComponetFinder(go);
                        if (!mo.cloneTargetMaterial) oldMat = goRenderer.material;
                    }

                    /*
                    if (color != null) newMat.SetTexture("_MainTex", color);
                    if (metal != null) newMat.SetTexture("_MetallicGlossMap", metal);
                    if (normal != null) newMat.SetTexture("_BumpMap", normal);
                    if (emission != null) newMat.SetTexture("_EmissionMap", emission);
                    */

                    newMat.name = rgo.name + "_newmat";
                    if (mo.replaceTextures)
                    {
                        newMat.color = oldMat.color;
                        newMat.SetTexture("_MainTex", oldMat.GetTexture("_MainTex"));
                        newMat.SetTexture("_MetallicGlossMap", oldMat.GetTexture("_MetallicGlossMap"));
                        newMat.SetTexture("_BumpMap", oldMat.GetTexture("_BumpMap"));
                    }
                    if (mo.replaceColor)
                    {
                        newMat.color = mo.texColor;
                    }

                    goRenderer.material = newMat;
                }
            }
        }
    }
}


//try
//{
//    string searchPath;
//    Debug.Log($"[{Plugin.GUID}] texture name is {textureName}");
//    searchPath = ItemManager.ResourcePath + textureName + "_d.png";
//    Debug.Log($"[{Plugin.GUID}] trying to look for texture with: '{searchPath}'");
//    color = bundle.LoadAsset<Texture2D>(searchPath);
//    Debug.Log($"[{Plugin.GUID}] color name is {color.name}");
//    searchPath = ItemManager.ResourcePath + textureName + "_m.png";
//    Debug.Log($"[{Plugin.GUID}] trying to look for texture with: '{searchPath}'");
//    metal = bundle.LoadAsset<Texture2D>(searchPath);
//    Debug.Log($"[{Plugin.GUID}] color metal is {metal.name}");
//    searchPath = ItemManager.ResourcePath + textureName + "_n.png";
//    Debug.Log($"[{Plugin.GUID}] trying to look for texture with: '{searchPath}'");
//    normal = bundle.LoadAsset<Texture2D>(searchPath);
//    Debug.Log($"[{Plugin.GUID}] color normal is {normal.name}");

//    searchPath = ItemManager.ResourcePath + textureName + "_e.png";
//    Debug.Log($"[{Plugin.GUID}] trying to look for texture with: '{searchPath}'");
//    emission = bundle.LoadAsset<Texture2D>(searchPath);
//    Debug.Log($"[{Plugin.GUID}] color normal is {emission.name}");
//}
//catch (Exception e)
//{
//    Debug.Log($"[{Plugin.GUID}] Error happened extracting textures {e.Message}");
//}