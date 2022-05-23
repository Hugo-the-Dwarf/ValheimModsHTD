using UnityEngine;

[ExecuteInEditMode]
public class PrintBoneOrder : MonoBehaviour
{
    public SkinnedMeshRenderer parent;
    public SkinnedMeshRenderer myself;
    public bool ranUpdate = false;

    void Start()
    {

    }

    void Update()
    {
        if (!ranUpdate && parent != null && myself != null)
        {
            string outputMessage;
            int currentIndex = 0;
            print($"Parent Root Bone {parent.rootBone.name}, My Root Bone {myself.rootBone.name}");
            print("Everything is going to be Index# : parentBone - myBone");
            while (currentIndex < parent.bones.Length || currentIndex < myself.bones.Length)
            {
                if (currentIndex < parent.bones.Length) outputMessage = "Index " + currentIndex + " : " + parent.bones[currentIndex].name;
                else outputMessage = "Index " + currentIndex + " : Nil";

                if (currentIndex < myself.bones.Length) outputMessage += " - " + myself.bones[currentIndex].name;
                else outputMessage += " - Nil";
                print(outputMessage);
                outputMessage = "";
                currentIndex++;
            }
            ranUpdate = true;
        }
    }
}

