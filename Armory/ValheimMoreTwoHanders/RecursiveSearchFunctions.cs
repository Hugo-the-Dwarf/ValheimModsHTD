using System.Collections.Generic;
using UnityEngine;

namespace ValheimHTDArmory
{
    public static class RecursiveSearchFunctions
    {
        public static Transform ChildNodeFinderDepthFirst(Transform target, ref string nodeName)
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
                    foundNode = ChildNodeFinderDepthFirst(target.GetChild(i), nodeName);
                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
            }

            return null;
        }

        public static Transform ChildNodeFinderDepthFirst(Transform target, string nodeName)
        {
            return ChildNodeFinderDepthFirst(target, ref nodeName);
        }

        public static Transform ChildNodeFinderBreadthFirst(Transform[] targets, ref string nodeName)
        {
            Transform foundNode;
            List<Transform[]> branches = new();
            foreach (var target in targets)
            {
                if (target.gameObject.name == nodeName)
                {
                    return target;
                }
                else if (target.childCount > 0)
                {
                    Transform[] children = new Transform[target.childCount];
                    for (int i = 0; i < target.childCount; i++) children[i] = target.GetChild(i);
                    branches.Add(children);
                }
            }

            if (branches.Count > 0)
            {
                foreach (var branchTargets in branches)
                {
                    foundNode = ChildNodeFinderBreadthFirst(branchTargets, ref nodeName);
                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
            }

            return null;

        }

        public static Transform ChildNodeFinderBreadthFirst(Transform target, string nodeName)
        {
            return ChildNodeFinderBreadthFirst(new Transform[] { target }, ref nodeName);
        }

        public static void ChildNodesFinderDepthFirst(Transform target, ref string nodeName, ref int numberOfNodes, ref List<Transform> nestedTargets)
        {
            if (target.name == nodeName && nestedTargets.Count < numberOfNodes)
                nestedTargets.Add(target);

            if (target.childCount > 0 && nestedTargets.Count < numberOfNodes)
            {
                for (int i = 0; i < target.childCount; i++)
                {
                    ChildNodesFinderDepthFirst(target.GetChild(i), ref nodeName, ref numberOfNodes, ref nestedTargets);
                }
            }
        }

        public static void ChildNodesFinderDepthFirst(Transform target, string nodeName, int numberOfNodes, ref List<Transform> nestedTargets)
        {
            ChildNodesFinderDepthFirst(target, ref nodeName, ref numberOfNodes, ref nestedTargets);
        }

        public static void ChildNodesFinderBreadthFirst(Transform[] targets, ref string nodeName, ref int numberOfNodes, ref List<Transform> nestedTargets)
        {
            List<Transform[]> branches = new();
            foreach (var target in targets)
            {
                if (target.name == nodeName && nestedTargets.Count < numberOfNodes)
                {
                    nestedTargets.Add(target);
                    if (nestedTargets.Count >= numberOfNodes) return;
                }

                if (target.childCount > 0)
                {
                    Transform[] children = new Transform[target.childCount];
                    for (int i = 0; i < target.childCount; i++) children[i] = target.GetChild(i);
                    branches.Add(children);
                }
            }


            if (branches.Count > 0 && nestedTargets.Count < numberOfNodes)
            {
                foreach (var branchTargets in branches)
                {
                    ChildNodesFinderBreadthFirst(branchTargets, ref nodeName, ref numberOfNodes, ref nestedTargets);
                    if (nestedTargets.Count >= numberOfNodes) return;
                }
            }
        }

        public static void ChildNodesFinderBreadthFirst(Transform[] targets, string nodeName, int numberOfNodes, ref List<Transform> nestedTargets)
        {
            ChildNodesFinderBreadthFirst(targets, ref nodeName, ref numberOfNodes, ref nestedTargets);
        }

        public static void ChildNodesFinderBreadthFirst(Transform target, string nodeName, int numberOfNodes, ref List<Transform> nestedTargets)
        {
            ChildNodesFinderBreadthFirst(new Transform[] { target }, ref nodeName, ref numberOfNodes, ref nestedTargets);
        }


    }
}
