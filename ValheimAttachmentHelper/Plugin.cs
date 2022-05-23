using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ValheimSkapekeysAndMore
{
    [BepInPlugin(Plugin.GUID, Plugin.ModName, Plugin.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.1.2";
        public const string ModName = "Shapekeys and More";
        public const string GUID = "htd.sam";

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
    }
}
