using UnityEngine;

[ExecuteInEditMode]
public class SMRReorder : MonoBehaviour
{
    public SkinnedMeshRenderer parent;
    public SkinnedMeshRenderer myself;
    private bool ranUpdate = false;

    void Start()
    {

    }

    void Update()
    {
        myself.rootBone = parent.rootBone;
        if (!ranUpdate && parent != null && myself != null)
        {
            //Transform[] origBones = myself.bones;
            //Transform[] newBones = new Transform[parent.bones.Length];
            //for (int b = 0; b < newBones.Length; b++)
            //{
            //    if (b >= origBones.Length)
            //    {
            //        newBones[b] = parent.bones[b];
            //    }
            //    else
            //    {
            //        Transform t = Array.Find<Transform>(parent.bones, c => c.name == origBones[b].name);
            //        newBones[b] = t ? t : origBones[b];
            //    }
            //}
            //myself.bones = newBones;
            myself.bones = parent.bones;
            ranUpdate = true;
        }
    }
}
