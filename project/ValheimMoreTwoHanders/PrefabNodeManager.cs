using System.Collections.Generic;
using UnityEngine;

namespace ValheimMoreTwoHanders
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
            public bool replaceColor = false;
            public bool replaceEmissionColor = false;
            public bool UpdateTextureScaleOffset = false;
            public bool useMyTextures = false;
            public bool copyShaderOnly = false;
            public float textureScaleX = 1f;
            public float textureScaleY = 1f;
            public float TextureOffsetX = 0f;
            public float TextureOffsetY = 0f;

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
        Dictionary<string, MeshRenderer> targetMeshRenderers = new Dictionary<string, MeshRenderer>();
        Dictionary<string, MeshFilter> targetMeshFilters = new Dictionary<string, MeshFilter>();
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
                        var referencedGameObject = AssetReferences.listOfAllGameObjects[node.targetPrefab];
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
                        var referencedGameObject = AssetReferences.listOfAllGameObjects[node.targetPrefab];
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
            if (node.copyMaterial)
            {
                MeshRenderer myRenderer;
                MeshRenderer targetRenderer = null;

                myRenderer = RecursiveChildNodeFinder(gameObject.transform, node.myNode).gameObject.GetComponent<MeshRenderer>();
                if (newMaterials.ContainsKey(gameObject.name + node.targetPrefab + "_newMat"))
                {
                    myRenderer.material = newMaterials[gameObject.name + node.targetPrefab + "_newMat"];
                    return;
                }

                if (targetMeshRenderers.ContainsKey(node.targetPrefab + node.targetNode))
                {
                    targetRenderer = targetMeshRenderers[node.targetPrefab + node.targetNode];
                }
                else
                {
                    GameObject targetedNode;
                    if (targetPrefabNodes.ContainsKey(node.targetPrefab + node.targetNode))
                    {
                        targetedNode = targetPrefabNodes[node.targetPrefab + node.targetNode];
                        targetRenderer = targetedNode.GetComponent<MeshRenderer>();
                        if (targetRenderer != null) targetMeshRenderers.Add(node.targetPrefab + node.targetNode, targetRenderer);
                    }
                    else
                    {
                        var referencedGameObject = AssetReferences.listOfAllGameObjects[node.targetPrefab];
                        targetedNode = RecursiveChildNodeFinder(referencedGameObject.transform, node.targetNode).gameObject;
                        if (targetedNode != null)
                        {
                            targetPrefabNodes.Add(node.targetPrefab + node.targetNode, targetedNode);
                            targetRenderer = targetedNode.GetComponent<MeshRenderer>();
                        }
                    }
                }

                if (node.copyShaderOnly)
                {
                    myRenderer.material.shader = targetRenderer.material.shader;
                    return;
                }

                if (node.useMyTextures)
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

                    Material newMat = new Material(targetRenderer.material);
                    newMat.name = gameObject.name + node.targetPrefab + "_newMat";
                    newMat.SetTexture("_MainTex", myRenderer.material.GetTexture("_MainTex"));
                    newMat.SetTexture("_MetallicGlossMap", myRenderer.material.GetTexture("_MetallicGlossMap"));
                    newMat.SetTexture("_BumpMap", myRenderer.material.GetTexture("_BumpMap"));
                    if (node.replaceColor)
                        newMat.SetColor("_Color", node.mainTextureColor);
                    if (node.replaceEmissionColor)
                        newMat.SetColor("_EmissionColor", node.emissionColor);
                    if (node.UpdateTextureScaleOffset)
                    {
                        newMat.mainTextureScale = new Vector2(node.textureScaleX, node.textureScaleY);
                        newMat.mainTextureOffset = new Vector2(node.TextureOffsetX, node.TextureOffsetY);
                    }
                    if (!newMaterials.ContainsKey(newMat.name)) newMaterials.Add(newMat.name, newMat);
                    myRenderer.material = newMat;
                }
                else
                {
                    if (node.replaceColor || node.replaceEmissionColor)
                    {
                        /* Valid Color Ids
                         * _Color
                         * _EmissionColor
                         */
                        Material newMat = new Material(targetRenderer.material);
                        newMat.name = gameObject.name + node.targetPrefab + "_newMat";
                        if (node.replaceColor)
                            newMat.SetColor("_Color", node.mainTextureColor);
                        if (node.replaceEmissionColor)
                            newMat.SetColor("_EmissionColor", node.emissionColor);
                        if (node.UpdateTextureScaleOffset)
                        {
                            newMat.mainTextureScale = new Vector2(node.textureScaleX, node.textureScaleY);
                            newMat.mainTextureOffset = new Vector2(node.TextureOffsetX, node.TextureOffsetY);
                        }
                        if (!newMaterials.ContainsKey(newMat.name)) newMaterials.Add(newMat.name, newMat);
                        myRenderer.material = newMat;
                    }
                    else
                        myRenderer.material = targetRenderer.material; //just use the reference 100%
                }

            }
        }
    }
}
