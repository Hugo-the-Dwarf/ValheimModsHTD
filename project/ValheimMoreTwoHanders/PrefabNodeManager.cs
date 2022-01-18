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
            public int numberOfNodes = 1;
            public bool copyMesh;
            public bool copyMaterial;
            public bool copyParticles;
            public bool replaceMeshScale;
            public string targetPrefab;
            public string targetNode;
            public float scaleMeshX;
            public float scaleMeshY;
            public float scaleMeshZ;

            public MaterialTask currentMaterialTask = new MaterialTask();
            public List<MaterialTask> materialTasks = new List<MaterialTask>();

            public class MaterialTask
            {
                public string targetPrefab;
                public string targetNode;
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
            }

            public void NewMaterialTask()
            {
                materialTasks.Add(currentMaterialTask);
                currentMaterialTask = new MaterialTask();
            }

            public GameObjectNode()
            {
            }

            public bool IsValid()
            {
                return myNode != null && (copyMesh || copyMaterial || copyParticles); //check to make sure at least a mesh or material is being copied
            }


            public class SnapPointSetting
            {

            }
        }

        GameObjectNode currentNode = new GameObjectNode();
        List<GameObjectNode> pendingNodes = new List<GameObjectNode>();
        string savedTargetPrefabNode = "";


        public void StartNewNode()
        {
            if (currentNode.IsValid())
            {
                currentNode.NewMaterialTask();
                pendingNodes.Add(currentNode);
            }
            currentNode = new GameObjectNode();
        }

        public PrefabNodeManager SetNode(string myNode, string targetPrefab, string targetNode, int numberOfNodes = 1)
        {
            currentNode.myNode = myNode;
            currentNode.targetPrefab = targetPrefab;
            currentNode.targetNode = targetNode;
            currentNode.numberOfNodes = numberOfNodes;
            savedTargetPrefabNode = targetPrefab + targetNode;
            return this;
        }

        public PrefabNodeManager StartNewMaterialTask()
        {
            currentNode.NewMaterialTask();
            return this;
        }

        public PrefabNodeManager SetMyMateiralIndex(int index)
        {
            currentNode.currentMaterialTask.myMaterialIndex = index;
            return this;
        }

        public PrefabNodeManager ChangeMeshScale(float scale)
        {
            currentNode.replaceMeshScale = true;
            currentNode.scaleMeshX = scale;
            currentNode.scaleMeshY = scale;
            currentNode.scaleMeshZ = scale;
            return this;
        }

        public PrefabNodeManager ChangeMeshScale(float x, float y, float z)
        {
            currentNode.replaceMeshScale = true;
            currentNode.scaleMeshX = x;
            currentNode.scaleMeshY = y;
            currentNode.scaleMeshZ = z;
            return this;
        }

        public PrefabNodeManager SetTargetMateiralIndex(int index)
        {
            currentNode.currentMaterialTask.targetMaterialIndex = index;
            return this;
        }

        public PrefabNodeManager SetTargetMateiralPrefabAndNode(string targetPrefab, string targetNode)
        {
            currentNode.currentMaterialTask.targetPrefab = targetPrefab;
            currentNode.currentMaterialTask.targetNode = targetNode;
            return this;
        }

        public PrefabNodeManager CopyTargetMaterial(bool swapTextures = false)
        {
            currentNode.copyMaterial = true;
            currentNode.currentMaterialTask.useMyTextures = swapTextures;
            return this;
        }

        public PrefabNodeManager CopyTargetShader()
        {
            currentNode.currentMaterialTask.copyShaderOnly = true;
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
            currentNode.currentMaterialTask.mainTextureColor = newColor;
            currentNode.currentMaterialTask.replaceColor = true;
            return this;
        }

        public PrefabNodeManager ReplaceEmissionColor(Color newColor)
        {
            currentNode.currentMaterialTask.emissionColor = newColor;
            currentNode.currentMaterialTask.replaceEmissionColor = true;
            return this;
        }

        public PrefabNodeManager ReplaceMetalColor(Color newColor)
        {
            currentNode.currentMaterialTask.metalColor = newColor;
            currentNode.currentMaterialTask.replaceMetalColor = true;
            return this;
        }

        public PrefabNodeManager ChangeTextureScaleOffset(float sX, float sY, float oX, float oY)
        {
            currentNode.currentMaterialTask.textureScaleX = sX;
            currentNode.currentMaterialTask.textureScaleY = sY;
            currentNode.currentMaterialTask.TextureOffsetX = oX;
            currentNode.currentMaterialTask.TextureOffsetY = oY;
            currentNode.currentMaterialTask.UpdateTextureScaleOffset = true;
            return this;
        }

        public static Transform RecursiveChildNodeFinder(Transform target, string nodeName)
        {
            Transform foundNode;
            if (target.gameObject.name == nodeName)
            {
                return target;
            }

            if (target.childCount > 0)
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

        public static void RecursiveChildNodesFinder(Transform target, string nodeName, int numberOfNodes, ref List<Transform> nestedTargets)
        {
            if (target.name == nodeName && nestedTargets.Count < numberOfNodes)
                nestedTargets.Add(target);

            if (target.childCount > 0 && nestedTargets.Count < numberOfNodes)
            {
                for (int i = 0; i < target.childCount; i++)
                {
                    RecursiveChildNodesFinder(target.GetChild(i), nodeName, numberOfNodes, ref nestedTargets);
                }
            }
        }

        private void WipeLists()
        {
            //targetMeshFilters = null;
            pendingNodes = null;
            currentNode = null;
            //targetParticleSystemRenderers = null;
        }

        public void ApplyNodeChanges(GameObject gameObject)
        {
            //Plugin.Log.LogMessage($"Applying Node Changes for Prefab: {gameObject.name}");
            if (currentNode.IsValid()) pendingNodes.Add(currentNode);
            currentNode = null;
            if (pendingNodes.Count > 0)
            {
                //Plugin.Log.LogMessage($"Pending Nodes Found: {pendingNodes.Count}");
                foreach (GameObjectNode n in pendingNodes)
                {
                    //Plugin.Log.LogMessage($"Going to Try to Fix Meshes {n.copyMesh}");
                    FixMeshReferences(gameObject, n);
                    //Plugin.Log.LogMessage($"Going to Try to Fix Materials  {n.copyMaterial}");
                    FixMaterialReferences(gameObject, n);
                    //Plugin.Log.LogMessage($"Going to Try to Fix Particles  {n.copyParticles}");
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

                if (MyReferences.targetParticleSystemRenderers.ContainsKey(savedTargetPrefabNode.GetStableHashCode()))
                {
                    targetPSR = MyReferences.targetParticleSystemRenderers[savedTargetPrefabNode.GetStableHashCode()];
                }
                else
                {
                    GameObject targetedNode;
                    if (MyReferences.targetPrefabNodes.ContainsKey((node.targetPrefab + node.targetNode).GetStableHashCode()))
                    {
                        targetedNode = MyReferences.targetPrefabNodes[(node.targetPrefab + node.targetNode).GetStableHashCode()];
                        targetPSR = targetedNode.GetComponent<ParticleSystemRenderer>();
                        if (targetPSR != null) MyReferences.targetParticleSystemRenderers.Add(savedTargetPrefabNode.GetStableHashCode(), targetPSR);
                    }
                    else
                    {
                        var referencedGameObject = MyReferences.listOfAllGameObjects[node.targetPrefab.GetStableHashCode()];
                        targetedNode = RecursiveChildNodeFinder(referencedGameObject.transform, node.targetNode).gameObject;
                        if (targetedNode != null)
                        {
                            MyReferences.targetPrefabNodes.Add(savedTargetPrefabNode.GetStableHashCode(), targetedNode);
                            targetPSR = targetedNode.GetComponent<ParticleSystemRenderer>();
                        }
                    }
                }

                myPSR.material = targetPSR.material;
            }


        }

        private void FixMeshReferences(GameObject gameObject, GameObjectNode node)
        {
            if (node.copyMesh)
            {
                MeshFilter targetFilter = null;

                //Gather all child transforms from main prefab
                List<Transform> foundNodes = new List<Transform>();
                RecursiveChildNodesFinder(gameObject.transform, node.myNode, node.numberOfNodes, ref foundNodes);
                if (foundNodes.Count == 0) return; //If this didn't find anything, either via typo or wrong name, just exit

                //Extract all the mesh filters from the found main prefab node(s)
                List<MeshFilter> mfs = new List<MeshFilter>();
                foreach (var foundNode in foundNodes)
                {
                    if (node.replaceMeshScale)
                    {
                        foundNode.localScale = new Vector3(node.scaleMeshX, node.scaleMeshY, node.scaleMeshZ);
                    }
                    MeshFilter mf = foundNode.GetComponent<MeshFilter>();
                    if (mf != null) mfs.Add(mf);
                }
                if (mfs.Count == 0) return; //No Mesh filters, just exit

                //Has the Target prefab's Mesh Filter been looked up before?
                if (MyReferences.targetMeshFilters.ContainsKey(savedTargetPrefabNode.GetStableHashCode()))
                {
                    targetFilter = MyReferences.targetMeshFilters[savedTargetPrefabNode.GetStableHashCode()];
                }
                else
                {
                    GameObject targetedNode;
                    //If this Target Prefab and it's Node been looked up, use that to get the Mesh Filter
                    if (MyReferences.targetPrefabNodes.ContainsKey(savedTargetPrefabNode.GetStableHashCode()))
                    {
                        targetedNode = MyReferences.targetPrefabNodes[savedTargetPrefabNode.GetStableHashCode()];
                        targetFilter = targetedNode.GetComponent<MeshFilter>();
                        //If this reference isn't saved, save it for later
                        if (targetFilter != null) MyReferences.targetMeshFilters.Add(savedTargetPrefabNode.GetStableHashCode(), targetFilter);
                    }
                    else
                    {
                        //Look up Target Prefab and extract the Node
                        var referencedGameObject = MyReferences.listOfAllGameObjects[node.targetPrefab.GetStableHashCode()];
                        targetedNode = RecursiveChildNodeFinder(referencedGameObject.transform, node.targetNode).gameObject;
                        if (targetedNode != null)
                        {
                            MyReferences.targetPrefabNodes.Add(savedTargetPrefabNode.GetStableHashCode(), targetedNode);
                            targetFilter = targetedNode.GetComponent<MeshFilter>();
                        }
                    }
                }
                foreach (var mf in mfs)
                {
                    mf.mesh = targetFilter.mesh;
                }
            }
        }

        private void FixMaterialReferences(GameObject gameObject, GameObjectNode node)
        {
            try
            {
                if (node.copyMaterial)
                {
                    //Plugin.Log.LogMessage($"Material to Fix for Prefab: {gameObject.name} for child: {node.myNode}");

                    Dictionary<int, Material> newMatArray = new Dictionary<int, Material>();
                    List<Transform> myNodes = new List<Transform>();
                    List<Renderer> myRenderers = new List<Renderer>();
                    Renderer targetRenderer = null;

                    RecursiveChildNodesFinder(gameObject.transform, node.myNode, node.numberOfNodes, ref myNodes);

                    //Plugin.Log.LogMessage($"Number of children found: {myNodes.Count}");
                    if (myNodes.Count == 0) return;

                    //Extract all Renderers
                    foreach (Transform myNode in myNodes)
                    {
                        var renderer = myNode.GetComponent<MeshRenderer>();
                        if (renderer != null)
                        {
                            myRenderers.Add(renderer);
                            continue;
                        }
                        var renderer2 = myNode.GetComponent<SkinnedMeshRenderer>();
                        if (renderer2 != null)
                        {
                            myRenderers.Add(renderer2);
                            continue;
                        }
                        Plugin.Log.LogWarning($"Wasn't able to find renderer for {node.myNode} inside of {gameObject.name}");
                    }

                    foreach (var mt in node.materialTasks)
                    {
                        string mySavedNewMatName = gameObject.name + node.targetPrefab + "_newMat_" + mt.myMaterialIndex;
                        //If Material Exists Extract it
                        if (MyReferences.newMaterials.ContainsKey(mySavedNewMatName.GetStableHashCode()))
                        {
                            newMatArray.Add(mt.myMaterialIndex, MyReferences.newMaterials[mySavedNewMatName.GetStableHashCode()]);
                        }
                    }

                    if (newMatArray.Count > 0)
                    {
                        Material[] materials = null;
                        foreach (Renderer myRenderer in myRenderers)
                        {
                            if (materials == null)
                            {
                                materials = myRenderer.materials;
                                foreach (var mt in node.materialTasks)
                                {
                                    materials[mt.myMaterialIndex] = newMatArray[mt.myMaterialIndex];
                                }
                            }
                            myRenderer.materials = materials;
                        }
                    }

                    //Start looking for the TargetMaterial
                    string targetNodeName = node.targetPrefab + node.targetNode;
                    GameObject targetedNode;
                    if (MyReferences.targetPrefabNodes.ContainsKey(targetNodeName.GetStableHashCode()))
                    {
                        //Plugin.Log.LogMessage($"Found saved target node: {targetNodeName}");
                        targetedNode = MyReferences.targetPrefabNodes[targetNodeName.GetStableHashCode()];
                        if (targetedNode.GetComponent<MeshRenderer>() != null)
                        {
                            //Plugin.Log.LogMessage($"Found Skinned Mesh Renderer");
                            targetRenderer = targetedNode.GetComponent<MeshRenderer>();
                        }
                        else if (targetedNode.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            //Plugin.Log.LogMessage($"Found Skinned Mesh Renderer");
                            targetRenderer = targetedNode.GetComponent<SkinnedMeshRenderer>();
                        }
                    }
                    else
                    {
                        //Plugin.Log.LogMessage($"Searching for target node: {targetNodeName}");
                        var referencedGameObject = MyReferences.listOfAllGameObjects[node.targetPrefab.GetStableHashCode()];
                        targetedNode = RecursiveChildNodeFinder(referencedGameObject.transform, node.targetNode).gameObject;
                        if (targetedNode != null)
                        {
                            if (targetedNode.GetComponent<MeshRenderer>() != null)
                            {
                                //Plugin.Log.LogMessage($"Found Mesh Renderer");
                                targetRenderer = targetedNode.GetComponent<MeshRenderer>();
                            }
                            else if (targetedNode.GetComponent<SkinnedMeshRenderer>() != null)
                            {
                                //Plugin.Log.LogMessage($"Found Skinned Mesh Renderer");
                                targetRenderer = targetedNode.GetComponent<SkinnedMeshRenderer>();
                            }
                        }
                    }

                    foreach (var renderer in myRenderers)
                    {
                        Material[] mats = null;
                        if (mats == null)
                        {
                            mats = renderer.materials;
                            foreach (var mt in node.materialTasks)
                            {
                                if (mt.copyShaderOnly)
                                {
                                    mats[mt.myMaterialIndex].shader = targetRenderer.materials[mt.targetMaterialIndex].shader;
                                    continue;
                                }

                                if (mt.useMyTextures || mt.replaceColor || mt.replaceEmissionColor || mt.replaceMetalColor || mt.UpdateTextureScaleOffset)
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

                                    string mySavedNewMatName = gameObject.name + node.targetPrefab + "_newMat_" + mt.myMaterialIndex;
                                    Material newMat = new Material(targetRenderer.materials[mt.targetMaterialIndex]);
                                    newMat.name = mySavedNewMatName;
                                    if (mt.useMyTextures)
                                    {
                                        newMat.SetTexture("_MainTex", renderer.materials[mt.myMaterialIndex].GetTexture("_MainTex"));
                                        newMat.SetTexture("_MetallicGlossMap", renderer.materials[mt.myMaterialIndex].GetTexture("_MetallicGlossMap"));
                                        newMat.SetTexture("_BumpMap", renderer.materials[mt.myMaterialIndex].GetTexture("_BumpMap"));
                                    }

                                    if (mt.replaceColor)
                                        newMat.SetColor("_Color", mt.mainTextureColor);
                                    if (mt.replaceEmissionColor)
                                        newMat.SetColor("_EmissionColor", mt.emissionColor);
                                    if (mt.replaceMetalColor)
                                        newMat.SetColor("_MetalColor", mt.metalColor);

                                    if (mt.UpdateTextureScaleOffset)
                                    {
                                        newMat.mainTextureScale = new Vector2(mt.textureScaleX, mt.textureScaleY);
                                        newMat.mainTextureOffset = new Vector2(mt.TextureOffsetX, mt.TextureOffsetY);
                                    }

                                    if (!MyReferences.newMaterials.ContainsKey(mySavedNewMatName.GetStableHashCode())) MyReferences.newMaterials.Add(mySavedNewMatName.GetStableHashCode(), newMat);

                                    //Plugin.Log.LogMessage($"Building newMat array");
                                    //RendererTest
                                    if (mats.Length > 1)
                                    {
                                        //Plugin.Log.LogMessage($"Setting material array with newMat array");
                                        mats[mt.myMaterialIndex] = newMat;
                                    }
                                    else mats = new Material[] { newMat };
                                }
                                else //Just 100% use the same materials
                                {
                                    //Will have to revist this, As I could have more options for say 3 material items.
                                    //Plugin.Log.LogMessage($"Setting material array with target material array");
                                    if (mats.Length > 1)
                                    {
                                        mats[mt.myMaterialIndex] = targetRenderer.materials[mt.targetMaterialIndex];
                                    }
                                    else mats = targetRenderer.materials;
                                }
                            }
                        }
                        renderer.materials = mats;


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

    }
}
