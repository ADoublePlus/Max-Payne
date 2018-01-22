using UnityEngine;

namespace MaxPayne
{
    public class IK_Handler_Player : MonoBehaviour
    {
        PlayerControl playControl;
        IK_ShoulderHandler rightShoulderIKbase;
        Animator anim;
        Transform cam;
        Vector3 targetLookPos;
        Vector3 curLookPos;
        Vector3 shoulderForwardDir = new Vector3(0, 0, 1);

        public Transform debugCube;

        public float bodyWeight = .8f;
        public float clampWeight = 1;
        public float handWeight = 1;
        public float elbowWeight = 1;
        public float slerpSpeed = 5;

        float targetPosLerp;
        float curPosLerp;
        float targetAimWeight;
        float curAimWeight;

        // Use this for initialization
        void Start ()
        {
            playControl = GetComponent<PlayerControl>();

            rightShoulderIKbase = GetComponentInChildren<IK_ShoulderHandler>();

            anim = GetComponent<Animator>();

            cam = Camera.main.transform;
            
        }

        void FixedUpdate ()
        {
            Ray ray = new Ray(cam.position, cam.forward);

            RaycastHit hit;

            LayerMask mask = ~(1 << 8);

            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                targetLookPos = hit.point;
            }
            else
            {
                targetLookPos = ray.GetPoint(100);
            }

            curLookPos = Vector3.Lerp(curLookPos, targetLookPos, Time.deltaTime * 10);

            debugCube.position = Vector3.Lerp(debugCube.position, curLookPos, Time.deltaTime * 10);

            Quaternion desiredRotation = Quaternion.LookRotation(curLookPos - rightShoulderIKbase.transform.position);

            rightShoulderIKbase.transform.rotation = Quaternion.Slerp(rightShoulderIKbase.transform.rotation, desiredRotation, Time.deltaTime * slerpSpeed);

            Vector3 dir = transform.InverseTransformPoint(curLookPos) - shoulderForwardDir;

            float angle = Vector3.Angle(dir, shoulderForwardDir);

            if (angle > 30)
            {
                if (anim.GetBool("CanMove"))
                {
                    targetPosLerp = 1;
                }
                else
                {
                    targetPosLerp = 0;
                } 
            }
            else
            {
                targetPosLerp = 0;
            }

            curPosLerp = Mathf.MoveTowards(curPosLerp, targetPosLerp, Time.deltaTime * 1);

            rightShoulderIKbase.lerpRate = curPosLerp;
        }

        void OnAnimatorIK ()
        {
            if (playControl.prepareToJump)
            {
                targetAimWeight = 0;
            }
            else
            {
                targetAimWeight = 1;
            }

            curAimWeight = Mathf.MoveTowards(curAimWeight, targetAimWeight, Time.deltaTime * 5);

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, curAimWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightShoulderIKbase.HandTarget.position);

            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, curAimWeight);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightShoulderIKbase.HandTarget.rotation);

            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, curAimWeight);
            anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightShoulderIKbase.ElbowTarget.position);

            anim.SetLookAtWeight(curAimWeight, bodyWeight, 1, 1, clampWeight);
            anim.SetLookAtPosition(curLookPos);
        }
    }
}