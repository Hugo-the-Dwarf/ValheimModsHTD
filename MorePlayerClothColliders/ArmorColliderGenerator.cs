using System.Collections.Generic;
using UnityEngine;

namespace MorePlayerClothColliders
{
    public static class ArmorColliderGenerator
    {

        public static List<CapsuleCollider> GenerateColliders(VisEquipment visEquipment)
        {
            List<CapsuleCollider> armorClothColliders = new List<CapsuleCollider>();
            var armature = visEquipment.gameObject.transform;
            GameObject colliderObject;
            CapsuleCollider currentCollider;
            //Edit Existing Colliders and Create Shoulder Versions

            //1st is hips
            //2nd is LeftUpLeg
            //3rd is RightUpLeg
            //4th is LowerBack
            //5th is Chest
            var targetMain = Plugin.RecursiveChildNodeFinder(armature, "Hips");
            var target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothCollider");
            target.name = "ClothColliderHips";
            target.transform.localPosition = new Vector3(0f, 0.00069f, 0);//-0.00028f
            target.transform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
            target.transform.localScale = new Vector3(-0.004395234f, -0.002306957f, -0.004395236f);

            visEquipment.m_clothColliders[0].radius = 0.45f;
            visEquipment.m_clothColliders[0].height = 1f;
            visEquipment.m_clothColliders[0].direction = 0;



            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftUpLeg");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothCollider");
            target.name = "ClothColliderLeftUpLeg";
            target.transform.localPosition = new Vector3(0.0002200005f, 0.003200019f, 0.00041f);
            target.transform.localRotation = new Quaternion(0.0002136376f, -0.00388667f, 0.999223f, 0.03922009f);
            target.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

            visEquipment.m_clothColliders[1].radius = 0.4f;
            visEquipment.m_clothColliders[1].height = 1.7f;
            visEquipment.m_clothColliders[1].direction = 1;

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightUpLeg");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothCollider");
            target.name = "ClothColliderRightUpLeg";
            target.transform.localPosition = new Vector3(-0.0002200005f, 0.003200019f, 0.00041f);
            target.transform.localRotation = new Quaternion(-0.0002136376f, -0.00388667f, 0.999223f, -0.03922009f);
            target.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

            visEquipment.m_clothColliders[2].radius = 0.4f;
            visEquipment.m_clothColliders[2].height = 1.7f;
            visEquipment.m_clothColliders[2].direction = 1;

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "Spine1");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothCollider (3)");
            target.name = "ClothColliderMid";
            target.transform.localPosition = new Vector3(0f, -0.00067f, 0f);
            target.transform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
            target.transform.localScale = new Vector3(-0.003994316f, -0.003345685f, -0.003994316f);

            visEquipment.m_clothColliders[3].radius = 0.45f;
            visEquipment.m_clothColliders[3].height = 1f;
            visEquipment.m_clothColliders[3].direction = 0;

            //Add existing colliders and capes versions to the new lists
            armorClothColliders.AddRange(visEquipment.m_clothColliders);

            //New Colliders for missing parts
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftArm");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderLeftUpArm";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.35f;
            currentCollider.height = 1.25f;
            currentCollider.direction = 1;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(0f, 0.001750002f, -0.0001800021f);
            colliderObject.transform.localRotation = new Quaternion(0.0001740195f, -0.02164068f, -0.999733f, 0.0081048f);
            colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightArm");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderRightUpArm";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.35f;
            currentCollider.height = 1.25f;
            currentCollider.direction = 1;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(0f, 0.001750002f, -0.0001800021f);
            colliderObject.transform.localRotation = new Quaternion(-0.0001740195f, -0.02164068f, -0.999733f, -0.0081048f);
            colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftForeArm");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderLeftLowerArm";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.35f;
            currentCollider.height = 1.5f;
            currentCollider.direction = 1;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(0f, 0.00175f, 0f);
            colliderObject.transform.localRotation = new Quaternion(-0.003666301f, -0.01329636f, -0.9998187f, 0.01312859f);
            colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightForeArm");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderRightLowerArm";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.35f;
            currentCollider.height = 1.5f;
            currentCollider.direction = 1;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(0f, 0.00175f, 0f);
            colliderObject.transform.localRotation = new Quaternion(0.003666301f, -0.01329636f, -0.9998187f, -0.01312859f);
            colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);            

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "Head");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderHead";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.38f;
            currentCollider.height = 0.9f;
            currentCollider.direction = 1;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(-0.00037f, 0.0013f, 0f);
            colliderObject.transform.localRotation = new Quaternion(1.750886E-07f, -1.490116E-07f, -0.2588191f, 0.9659258f);
            colliderObject.transform.localScale = new Vector3(-0.003059157f, -0.003533579f, -0.003058948f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftLeg");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderLeftLowerLeg";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.3f;
            currentCollider.height = 1.7f;
            currentCollider.direction = 1;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(0.00022f, 0.00255f, -0.00022f);
            colliderObject.transform.localRotation = new Quaternion(-0.08638234f, -0.0004369422f, 0.02003973f, 0.9960604f);
            colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);            

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightLeg");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderRightLowerLeg";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.3f;
            currentCollider.height = 1.7f;
            currentCollider.direction = 1;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(-0.00022f, 0.00255f, -0.00022f);
            colliderObject.transform.localRotation = new Quaternion(0.08638234f, -0.0004369422f, 0.02003973f, -0.9960604f);
            colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftFoot");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderLeftFoot";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.3f;
            currentCollider.height = 1.6f;
            currentCollider.direction = 2;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(0f, 0.00098f, 0.00013f);
            colliderObject.transform.localRotation = new Quaternion(-0.5f, 0f, 0f, 0.8660254f);
            colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533605f, -0.003058974f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightFoot");
            colliderObject = new GameObject();
            colliderObject.name = "ClothColliderRightFoot";
            currentCollider = colliderObject.AddComponent<CapsuleCollider>();

            currentCollider.radius = 0.3f;
            currentCollider.height = 1.6f;
            currentCollider.direction = 2;

            colliderObject.transform.SetParent(targetMain);
            colliderObject.transform.localPosition = new Vector3(0f, 0.00098f, 0.00013f);
            colliderObject.transform.localRotation = new Quaternion(0.5f, 0f, 0f, -0.8660254f);
            colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533605f, -0.003058974f);

            colliderObject.SetActive(false);
            armorClothColliders.Add(currentCollider);


            return armorClothColliders;
        }

    }
}
