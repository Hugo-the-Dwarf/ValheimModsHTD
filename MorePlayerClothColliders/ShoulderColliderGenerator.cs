using System.Collections.Generic;
using UnityEngine;

namespace MorePlayerClothColliders
{
    public static class ShoulderColliderGenerator
    {
        public static List<CapsuleCollider> GenerateColliders(VisEquipment visEquipment)
        {
            var armature = visEquipment.gameObject.transform;
            GameObject colliderObject = null;
            CapsuleCollider currentCollider;
            List<CapsuleCollider> shoulderColliders = new List<CapsuleCollider>();

            var targetMain = Plugin.RecursiveChildNodeFinder(armature, "Hips");
            var target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderHipsCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderHipsCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.5f;
                currentCollider.height = 1.6f;
                currentCollider.direction = 0;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0f, 0.00029f, -0.00081f);
                colliderObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
                colliderObject.transform.localScale = new Vector3(-0.004395234f, -0.002306957f, -0.004395236f);
                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftUpLeg");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderLeftUpLegCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderLeftUpLegCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.6f;
                currentCollider.height = 1.7f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0.00022f, 0.0032f, -0.00019f);
                colliderObject.transform.localRotation = new Quaternion(0.0002136376f, -0.00388667f, 0.999223f, 0.03922009f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);
                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightUpLeg");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderRightUpLegCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderRightUpLegCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.6f;
                currentCollider.height = 1.7f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(-0.00022f, 0.0032f, -0.00019f);
                colliderObject.transform.localRotation = new Quaternion(-0.0002136376f, -0.00388667f, 0.999223f, -0.03922009f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);
                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "Spine1");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderMidCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderMidCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.45f;
                currentCollider.height = 1.5f;
                currentCollider.direction = 0;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0f, -0.00067f, -0.0009f);
                colliderObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
                colliderObject.transform.localScale = new Vector3(-0.003994316f, -0.003345685f, -0.003994316f);
                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftArm");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderLeftUpArmCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderLeftUpArmCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.4f;
                currentCollider.height = 1.25f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0f, 0.001750002f, -0.0001800021f);
                colliderObject.transform.localRotation = new Quaternion(0.0001740195f, -0.02164068f, -0.999733f, 0.0081048f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightArm");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderRightUpArmCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderRightUpArmCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.4f;
                currentCollider.height = 1.25f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0f, 0.001750002f, -0.0001800021f);
                colliderObject.transform.localRotation = new Quaternion(-0.0001740195f, -0.02164068f, -0.999733f, -0.0081048f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftForeArm");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderLeftLowerArmCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderLeftLowerArmCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.5f;
                currentCollider.height = 1.7f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0f, 0.00199f, 0f);
                colliderObject.transform.localRotation = new Quaternion(-0.003467927f, -0.01334951f, -0.9999034f, -0.001760909f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightForeArm");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderRightLowerArmCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderRightLowerArmCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.5f;
                currentCollider.height = 1.7f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0f, 0.00199f, 0f);
                colliderObject.transform.localRotation = new Quaternion(0.003467927f, -0.01334951f, -0.9999034f, 0.001760909f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "Head");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderHeadCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderHeadCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.5f;
                currentCollider.height = 1.1f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(-0.00037f, 0.0013f, 0f);
                colliderObject.transform.localRotation = new Quaternion(1.750886E-07f, -1.490116E-07f, -0.2588191f, 0.9659258f);
                colliderObject.transform.localScale = new Vector3(-0.003059157f, -0.003533579f, -0.003058948f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftLeg");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderLeftLowerLegCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderLeftLowerLegCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.5f;
                currentCollider.height = 1.7f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0.00022f, 0.00256f, -0.00042f);
                colliderObject.transform.localRotation = new Quaternion(0.03379562f, 0.00197151f, 0.0199473f, 0.9992278f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightLeg");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderRightLowerLegCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderRightLowerLegCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.5f;
                currentCollider.height = 1.7f;
                currentCollider.direction = 1;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(-0.00022f, 0.00256f, -0.00042f);
                colliderObject.transform.localRotation = new Quaternion(-0.03379562f, 0.00197151f, 0.0199473f, -0.9992278f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533604f, -0.003058974f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }



            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "LeftFoot");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderLeftFootCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderLeftFootCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.4f;
                currentCollider.height = 1.8f;
                currentCollider.direction = 2;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0f, 0.00075f, 0.00038f);
                colliderObject.transform.localRotation = new Quaternion(-0.5f, 0f, 0f, 0.8660254f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533605f, -0.003058974f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }

            //
            targetMain = Plugin.RecursiveChildNodeFinder(armature, "RightFoot");
            target = Plugin.RecursiveChildNodeFinder(targetMain, "ClothColliderRightFootCape");
            if (target == null)
            {
                colliderObject = new GameObject();
                colliderObject.name = "ClothColliderRightFootCape";
                currentCollider = colliderObject.AddComponent<CapsuleCollider>();

                currentCollider.radius = 0.4f;
                currentCollider.height = 1.8f;
                currentCollider.direction = 2;

                colliderObject.transform.SetParent(targetMain);
                colliderObject.transform.localPosition = new Vector3(0f, 0.00075f, 0.00038f);
                colliderObject.transform.localRotation = new Quaternion(0.5f, 0f, 0f, -0.8660254f);
                colliderObject.transform.localScale = new Vector3(-0.003059162f, -0.003533605f, -0.003058974f);

                colliderObject.SetActive(false);
                shoulderColliders.Add(currentCollider);
            }
            else
            {
                shoulderColliders.Add(target.gameObject.GetComponent<CapsuleCollider>());
            }




            return shoulderColliders;
        }
    }
}

