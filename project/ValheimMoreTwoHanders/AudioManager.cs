

using System.Collections.Generic;
using UnityEngine;

namespace ValheimHTDArmory
{
    class AudioManager
    {
        public enum AudioType
        {
            ZSFX,
            AudioSource
        }

        private class PendingAudioTask
        {
            public GameObject myPrefab;
            List<AudioTask> targets = new List<AudioTask>();
        }


        private class AudioTask
        {
            string myNode;
            bool copyTargetToMe = false;
            AudioType type;
            List<AudioTarget> targets = new List<AudioTarget>();

            public void ApplyTask(GameObject go)
            {
                /*
                 * 
                 */
                if (targets.Count > 0)
                {
                    Transform myTransform = RecursiveSearchFunctions.ChildNodeFinderDepthFirst(go.transform, myNode);
                    List<AudioClip> acList = new List<AudioClip>();
                    if (myTransform != null)
                    {
                        foreach (var target in targets)
                        {




                            switch (type)
                            {
                                case AudioType.ZSFX:
                                    var myZSFX = myTransform.GetComponent<ZSFX>();
                                    if (myZSFX != null)
                                    {
                                        if (myTransform != null)
                                        {
                                            var targetZSFX = myTransform.GetComponent<ZSFX>();
                                            if (targetZSFX != null)
                                            {
                                                if (target.targetIndex != -1)
                                                {

                                                }
                                                else
                                                {
                                                    foreach (var ac in targetZSFX.m_audioClips)
                                                    {
                                                        acList.Add(ac);
                                                    }
                                                }
                                                //myZSFX.m_audioClips[0]
                                            }
                                        }
                                    }
                                    break;
                                case AudioType.AudioSource:
                                    break;
                            }

                        }
                    }
                }
            }
        }

        private class AudioTarget
        {
            public string targetPrefab;
            public string targetNode;
            public int targetIndex = -1;
        }


        private List<PendingAudioTask> pendingAudioTasks = new List<PendingAudioTask>();
    }
}
