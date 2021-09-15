using System;
using System.Collections.Generic;
using UnityEngine;

namespace ValheimHTDArmory
{
    public class PrefabNodeManager
    {
        // TODO : What do I really want this to do? I have the MaterialManager. Should I combine them having nodes to stuff like Parent, materials, and meshes?
        private class GameObjectNode
        {
            public string myNode;
            public string targetPrefab;
            public string targetNode;
            public bool copyMesh;
            public bool copyMaterial;
            public bool copyParticles;
            public Color mainTextureColor;
            public Color emissionColor;
            public Color metalColor;
            public bool replaceColor = false;
            public bool replaceEmissionColor = false;
            public bool replaceMetalColor = false;
            public bool UpdateTextureScaleOffset = false;
            public bool useMyTextures = false;
            public bool copyShaderOnly = false;
            public float textureScaleX = 1f;
            public float textureScaleY = 1f;
            public float TextureOffsetX = 0f;
            public float TextureOffsetY = 0f;
            public int myMaterialIndex = 0;
            public int targetMaterialIndex = 0;

            public GameObjectNode()
            {
            }

            public bool IsValid()
            {
                return myNode != null && targetPrefab != null && targetNode != null && (copyMesh || copyMaterial); //check to make sure at least a mesh or material is being copied
            }


            public class SnapPointSetting
            {

            }
        }

        GameObjectNode currentNode = new GameObjectNode();
        List<GameObjectNode> pendingNodes = new List<GameObjectNode>();

        //Found Data
        Dictionary<string, GameObject> targetPrefabNodes = new Dictionary<string, GameObject>();
        Dictionary<string, Renderer> targetMeshRenderers = new Dictionary<string, Renderer>();
        Dictionary<string, MeshFilter> targetMeshFilters = new Dictionary<string, MeshFilter>();
        Dictionary<string, Material> targetMaterials = new Dictionary<string, Material>();
        Dictionary<string, Material> newMaterials = new Dictionary<string, Material>();
        Dictionary<string, ParticleSystemRenderer> targetParticleSystemRenderers = new Dictionary<string, ParticleSystemRenderer>();

        public void StartNewNode()
        {
            if (currentNode.IsValid()) pendingNodes.Add(currentNode);
            currentNode = new GameObjectNode();
            //return this;
        }

        public PrefabNodeManager SetNode(string myNode, string targetPrefab, string targetNode)
        {
            currentNode.myNode = myNode;
            currentNode.targetPrefab = targetPrefab;
            currentNode.targetNode = targetNode;
            return this;
        }

        public PrefabNodeManager SetMyMateiralIndex(int index)
        {
            currentNode.myMaterialIndex = index;
            return this;
        }

        public PrefabNodeManager SetTargetMateiralIndex(int index)
        {
            currentNode.targetMaterialIndex = index;
            return this;
        }

        public PrefabNodeManager CopyTargetMaterial(bool swapTextures = false)
        {
            currentNode.copyMaterial = true;
            currentNode.useMyTextures = swapTextures;
            return this;
        }

        public PrefabNodeManager CopyTargetShader()
        {
            currentNode.copyShaderOnly = true;
            return this;
        }

        public PrefabNodeManager CopyTargetMesh()
        {
            currentNode.copyMesh = true;
            return this;
        }

        public PrefabNodeManager CopyTargetParticle()
        {
            currentNode.copyParticles = true;
            return this;
        }

        public PrefabNodeManager ReplaceMainColor(Color newColor)
        {
            currentNode.mainTextureColor = newColor;
            currentNode.replaceColor = true;
            return this;
        }

        public PrefabNodeManager ReplaceEmissionColor(Color newColor)
        {
            currentNode.emissionColor = newColor;
            currentNode.replaceEmissionColor = true;
            return this;
        }

        public PrefabNodeManager ReplaceMetalColor(Color newColor)
        {
            currentNode.metalColor = newColor;
            currentNode.replaceMetalColor = true;
            return this;
        }

        public PrefabNodeManager ChangeTextureScaleOffset(float sX, float sY, float oX, float oY)
        {
            currentNode.textureScaleX = sX;
            currentNode.textureScaleY = sY;
            currentNode.TextureOffsetX = oX;
            currentNode.TextureOffsetY = oY;
            currentNode.UpdateTextureScaleOffset = true;
            return this;
        }

        public static Transform RecursiveChildNodeFinder(Transform target, string nodeName)
        {
            var foundNode = target.Find(nodeName);
            if (foundNode != null)
            {
                return foundNode;
            }
            else if (target.childCount > 0)
            {
                for (int i = 0; i < target.childCount; i++)
                {
                    foundNode = RecursiveChildNodeFinder(target.GetChild(i), nodeName);
                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
            }

            return null;
        }

        public static void RecursiveAllChildNodeFinder(ref List<Transform> transforms, Transform target, string nodeName, bool absoluteScan = false)
        {
            if (target.childCount > 0)
            {
                foreach (Transform child in target)
                {
                    if (!absoluteScan && child.name.Contains(nodeName))
                    {
                        transforms.Add(child);
                    }
                    if (absoluteScan && child.name == nodeName)
                    {
                        transforms.Add(child);
                    }
                    if (child.childCount > 0)
                    {
                        RecursiveAllChildNodeFinder(ref transforms, child, nodeName, absoluteScan);
                    }
                }
            }
        }

        public static MeshRenderer RecursiveMeshRendererComponetFinder(GameObject target)
        {
            var foundComponet = target.GetComponent<MeshRenderer>();
            if (foundComponet != null)
            {
                return foundComponet;
            }
            else if (target.transform.childCount > 0)
            {
                for (int i = 0; i < target.transform.childCount; i++)
                {
                    foundComponet = RecursiveMeshRendererComponetFinder(target.transform.GetChild(i).gameObject);
                    if (foundComponet != null)
                    {
                        return foundComponet;
                    }
                }
            }

            return null;
        }

        private void WipeLists()
        {
            targetMeshFilters = null;
            targetMeshRenderers = null;
            pendingNodes = null;
            currentNode = null;
            targetParticleSystemRenderers = null;
        }

        public void ApplyNodeChanges(GameObject gameObject)
        {
            if (currentNode.IsValid()) pendingNodes.Add(currentNode);
            currentNode = null;
            if (pendingNodes.Count > 0)
            {
                foreach (GameObjectNode n in pendingNodes)
                {
                    FixMeshReferences(gameObject, n);
                    FixMaterialReferences(gameObject, n);
                    FixParticleSystemRendererReferences(gameObject, n);
                }
                WipeLists();
            }
        }

        private void FixParticleSystemRendererReferences(GameObject gameObject, GameObjectNode node)
        {
            if (node.copyParticles)
            {
                ParticleSystemRenderer myPSR;
                ParticleSystemRenderer targetPSR = null;

                myPSR = RecursiveChildNodeFinder(gameObject.transform, node.myNode).gameObject.GetComponent<ParticleSystemRenderer>();

                if (targetParticleSystemRenderers.ContainsKey(node.targetPrefab + node.targetNode))
                {
                    targetPSR = targetParticleSystemRenderers[node.targetPrefab + node.targetNode];
                }
                else
                {
                    GameObject targetedNode;
                    if (targetPrefabNodes.ContainsKey(node.targetPrefab + node.targetNode))
                    {
                        targetedNode = targetPrefabNodes[node.targetPrefab + node.targetNode];
                        targetPSR = targetedNode.GetComponent<ParticleSystemRenderer>();
                        if (targetPSR != null) targetParticleSystemRenderers.Add(node.targetPrefab + node.targetNode, targetPSR);
                    }
                    else
                    {
                        var referencedGameObject = MyReferences.listOfAllGameObjects[node.targetPrefab];
                        targetedNode = RecursiveChildNodeFinder(referencedGameObject.transform, node.targetNode).gameObject;
                        if (targetedNode != null)
                        {
                            targetPrefabNodes.Add(node.targetPrefab + node.targetNode, targetedNode);
                            targetPSR = targetedNode.GetComponent<ParticleSystemRenderer>();
                        }
                    }
                }

                if (node.copyShaderOnly)
                {
                    myPSR.material.shader = targetPSR.material.shader;
                    return;
                }

                myPSR.material = targetPSR.material;
            }


        }

        private void FixMeshReferences(GameObject gameObject, GameObjectNode node)
        {
            if (node.copyMesh)
            {
                MeshFilter myFilter;
                MeshFilter targetFilter = null;

                myFilter = RecursiveChildNodeFinder(gameObject.transform, node.myNode).gameObject.GetComponent<MeshFilter>();

                if (targetMeshFilters.ContainsKey(node.targetPrefab + node.targetNode))
                {
                    targetFilter = targetMeshFilters[node.targetPrefab + node.targetNode];
                }
                else
                {
                    GameObject targetedNode;
                    if (targetPrefabNodes.ContainsKey(node.targetPrefab + node.targetNode))
                    {
                        targetedNode = targetPrefabNodes[node.targetPrefab + node.targetNode];
                        targetFilter = targetedNode.GetComponent<MeshFilter>();
                        if (targetFilter != null) targetMeshFilters.Add(node.targetPrefab + node.targetNode, targetFilter);
                    }
                    else
                    {
                        var referencedGameObject = MyReferences.listOfAllGameObjects[node.targetPrefab];
                        targetedNode = RecursiveChildNodeFinder(referencedGameObject.transform, node.targetNode).gameObject;
                        if (targetedNode != null)
                        {
                            targetPrefabNodes.Add(node.targetPrefab + node.targetNode, targetedNode);
                            targetFilter = targetedNode.GetComponent<MeshFilter>();
                        }
                    }
                }

                myFilter.mesh = targetFilter.mesh;
            }
        }

        private void FixMaterialReferences(GameObject gameObject, GameObjectNode node)
        {
            try
            {
                if (node.copyMaterial)
                {
                    //Plugin.Log.LogMessage($"Material to Fix for Prefab: {node.myNode}");
                    //Plugin.Log.LogMessage($"Material to Fix for Prefab: {gameObject.name}");

                    Material myMaterialReference = null;
                    Material targetMaterialReference = null;
                    Material newMat = null;
                    Material[] newMatArray = null;
                    string mySavedNewMatName = gameObject.name + node.targetPrefab + "_newMat_" + node.myMaterialIndex;
                    Transform myNode;
                    Renderer myRenderer = null;
                    Renderer targetRenderer = null;
                    myNode = RecursiveChildNodeFinder(gameObject.transform, node.myNode);

                    //If Material Exists Extract it
                    if (newMaterials.ContainsKey(mySavedNewMatName))
                    {
                        //Plugin.Log.LogMessage($"Found saved material: {mySavedNewMatName}");
                        newMat = newMaterials[mySavedNewMatName];
                    }

                    //Find the Mesh or Skinned Mesh Renderer, set newMat if it's not null
                    //myRenderer = myNode.gameObject.GetComponent<MeshRenderer>();
                    if (myNode.gameObject.GetComponent<MeshRenderer>() != null)
                    //if (myRenderer != null)
                    {
                        //Plugin.Log.LogMessage($"Found Mesh Renderer");
                        myRenderer = myNode.gameObject.GetComponent<MeshRenderer>();
                        //Plugin.Log.LogMessage($"Number of Materials: {myRenderer.materials.Length}");
                        if (newMat != null)
                        {
                            //Plugin.Log.LogMessage($"Using saved material MR");
                            //myNode.gameObject.GetComponent<MeshRenderer>().materials[node.myMaterialIndex] = new Material[] { newMat };
                            if (myRenderer.materials.Length == 1) myRenderer.material = newMat;
                            else myRenderer.materials = AssembleNewMatArray(myRenderer.materials, node.myMaterialIndex, newMat);// new Material[] { newMat };
                            return;
                        }

                        myMaterialReference = myRenderer.materials[node.myMaterialIndex];
                    }
                    else if (myNode.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
                    {
                        //Plugin.Log.LogMessage($"Found Skinned Mesh Renderer");
                        myRenderer = myNode.gameObject.GetComponent<SkinnedMeshRenderer>();
                        //Plugin.Log.LogMessage($"Number of Materials: {myRenderer.materials.Length}");
                        if (newMat != null)
                        {
                            //Plugin.Log.LogMessage($"Using saved material SMR");
                            if (myRenderer.materials.Length == 1) myRenderer.material = newMat;
                            else myRenderer.materials = AssembleNewMatArray(myRenderer.materials, node.myMaterialIndex, newMat);// new Material[] { newMat };
                            return;
                        }
                        myMaterialReference = myRenderer.materials[node.myMaterialIndex];
                    }

                    //Start looking for the TargetMaterial
                    string targetNodeName = node.targetPrefab + node.targetNode;
                    string targetMaterialName = targetNodeName + "_" + node.targetMaterialIndex;

                    //if (targetMaterials.ContainsKey(targetMaterialName))
                    //{
                    //    Plugin.Log.LogMessage($"Found saved target material: {targetMaterialName}");
                    //    targetMaterialReference = targetMaterials[targetMaterialName];
                    //}
                    //else
                    //{
                    GameObject targetedNode;
                    if (targetPrefabNodes.ContainsKey(targetNodeName))
                    {
                        //Plugin.Log.LogMessage($"Found saved target node: {targetNodeName}");
                        targetedNode = targetPrefabNodes[targetNodeName];
                        if (targetedNode.GetComponent<MeshRenderer>() != null)
                        {
                            //Plugin.Log.LogMessage($"Found Skinned Mesh Renderer");
                            targetRenderer = targetedNode.GetComponent<MeshRenderer>();
                            targetMaterialReference = targetRenderer.materials[node.targetMaterialIndex];
                        }
                        else if (targetedNode.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            //Plugin.Log.LogMessage($"Found Skinned Mesh Renderer");
                            targetRenderer = targetedNode.GetComponent<SkinnedMeshRenderer>();
                            targetMaterialReference = targetRenderer.materials[node.targetMaterialIndex];
                        }
                        if (!targetMaterials.ContainsKey(targetMaterialName))
                            targetMaterials.Add(targetMaterialName, targetMaterialReference);
                    }
                    else
                    {
                        //Plugin.Log.LogMessage($"Searching for target node: {targetNodeName}");
                        var referencedGameObject = MyReferences.listOfAllGameObjects[node.targetPrefab];
                        targetedNode = RecursiveChildNodeFinder(referencedGameObject.transform, node.targetNode).gameObject;
                        if (targetedNode != null)
                        {
                            if (targetedNode.GetComponent<MeshRenderer>() != null)
                            {
                                //Plugin.Log.LogMessage($"Found Mesh Renderer");
                                targetRenderer = targetedNode.GetComponent<MeshRenderer>();
                                targetMaterialReference = targetRenderer.materials[node.targetMaterialIndex];
                            }
                            else if (targetedNode.GetComponent<SkinnedMeshRenderer>() != null)
                            {
                                //Plugin.Log.LogMessage($"Found Skinned Mesh Renderer");
                                targetRenderer = targetedNode.GetComponent<SkinnedMeshRenderer>();
                                targetMaterialReference = targetRenderer.materials[node.targetMaterialIndex];
                            }
                            if (!targetMaterials.ContainsKey(targetMaterialName))
                                targetMaterials.Add(targetMaterialName, targetMaterialReference);
                        }
                    }
                    // }


                    if (node.copyShaderOnly)
                    {
                        myMaterialReference.shader = targetMaterialReference.shader;
                        return;
                    }


                    if (node.useMyTextures || node.replaceColor || node.replaceEmissionColor || node.replaceMetalColor || node.UpdateTextureScaleOffset)
                    {
                        /* Valid Texture Names
                         * _BumpMap (normals)
                         * _DetailAlbedoMap
                         * _DetailMask
                         * _DetailNormalMap
                         * _EmissionMap
                         * _MainTex (diffuse,albedo)
                         * _MetallicGlossMap
                         * _OcclusionMap
                         * _ParallaxMap
                         */

                        /* Valid Color Ids
                         * _Color
                         * _EmissionColor
                         * _MetalColor *for 'Custom/Creature'
                         */

                        newMat = new Material(targetMaterialReference);
                        newMat.name = mySavedNewMatName;
                        if (node.useMyTextures)
                        {
                            newMat.SetTexture("_MainTex", myMaterialReference.GetTexture("_MainTex"));
                            newMat.SetTexture("_MetallicGlossMap", myMaterialReference.GetTexture("_MetallicGlossMap"));
                            newMat.SetTexture("_BumpMap", myMaterialReference.GetTexture("_BumpMap"));
                        }

                        if (node.replaceColor)
                            newMat.SetColor("_Color", node.mainTextureColor);
                        if (node.replaceEmissionColor)
                            newMat.SetColor("_EmissionColor", node.emissionColor);
                        if (node.replaceMetalColor)
                            newMat.SetColor("_MetalColor", node.metalColor);

                        if (node.UpdateTextureScaleOffset)
                        {
                            newMat.mainTextureScale = new Vector2(node.textureScaleX, node.textureScaleY);
                            newMat.mainTextureOffset = new Vector2(node.TextureOffsetX, node.TextureOffsetY);
                        }

                        if (!newMaterials.ContainsKey(mySavedNewMatName)) newMaterials.Add(mySavedNewMatName, newMat);

                        //Plugin.Log.LogMessage($"Building newMat array");
                        //RendererTest
                        if (myRenderer.materials.Length > 1)
                        {
                            //Plugin.Log.LogMessage($"Setting material array with newMat array");
                            myRenderer.materials = AssembleNewMatArray(myRenderer.materials, node.myMaterialIndex, newMat);// newMatArray;
                        }
                        else myRenderer.materials = new Material[] { newMat };
                    }
                    else //Just 100% use the same materials
                    {
                        //Will have to revist this, As I could have more options for say 3 material items.
                        //Plugin.Log.LogMessage($"Setting material array with target material array");
                        if(myRenderer.materials.Length > 1)
                        {
                            myRenderer.materials = AssembleNewMatArray(myRenderer.materials, node.myMaterialIndex, targetRenderer.materials[node.targetMaterialIndex]);
                        }
                        else myRenderer.materials = targetRenderer.materials;
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error trying to fix material reference for {gameObject.name}");
                Plugin.Log.LogError(e.Message);
                Plugin.Log.LogError(e.StackTrace);
            }
        }
        private Material[] AssembleNewMatArray(Material[] myMaterialArray, int myIndex, Material newMat)
        {
            //Plugin.Log.LogMessage($"Number of Materials to sort through: {myMaterialArray.Length}");
            List<Material> outputList = new List<Material>();
            for (int index = 0; index < myMaterialArray.Length; index++)
            {
                //Plugin.Log.LogMessage($"Ordering material: {myMaterialArray[index]} as index {index} compared to {myIndex} against {newMat}");
                if (index == myIndex) outputList.Add(newMat); 
                else outputList.Add(myMaterialArray[index]);
            }
            //Plugin.Log.LogMessage($"Size of Ordered List material: {outputList.Count}");
            //foreach(Material m in outputList)
            //    Plugin.Log.LogMessage($"Ordered material: {m}");
            return outputList.ToArray();
        }
    }
}
