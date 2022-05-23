using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MorePlayerClothColliders
{
    [BepInPlugin(Plugin.GUID, Plugin.ModName, Plugin.Version)]
    class Plugin : BaseUnityPlugin
    {
        public const string Version = "3.0.1";
        public const string ModName = "More Player Cloth Colliders";
        public const string GUID = "htd.mpcc";
        Harmony _Harmony;
        public static ManualLogSource Log;
        public readonly Harmony harmony = new Harmony(GUID);

        private void Awake()
        {
            _Harmony = new Harmony(GUID);
#if DEBUG
            Log = Logger;
#else
            Log = new ManualLogSource(null);
#endif
            _Harmony.PatchAll();
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
        
    }
}
