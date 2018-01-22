using UnityEngine;

namespace MaxPayne
{
    public class IK_ShoulderHandler : MonoBehaviour
    {
        public IKPositions ikPositions;
        public Transform shoulderToTrack;
        public Transform HandTarget;
        public Transform ElbowTarget;

        public bool enableIK = true;
        public bool debugMode;

        public float lerpRate;
        
        [System.Serializable]
        public class IKPositions
        {
            public Vector3 handTargetPos1;
            public Vector3 handTargetPos2;
            public Vector3 handTargetRot1;
            public Vector3 elbowTargetPos1;
            public Vector3 elbowTargetPos2;
        }

        // Use this for initialization
        void Start ()
        {
            HandTarget.position = ikPositions.handTargetPos2;

            ElbowTarget.position = ikPositions.elbowTargetPos1;
        }

        // Update is called once per frame
        void Update ()
        {
            Vector3 positionToCopy = shoulderToTrack.TransformPoint(Vector3.zero);

            transform.position = positionToCopy;

            if (!debugMode)
            {
                HandTarget.localPosition = Vector3.MoveTowards(ikPositions.handTargetPos1, ikPositions.handTargetPos2, lerpRate);
                HandTarget.localRotation = Quaternion.Euler(ikPositions.handTargetRot1);

                ElbowTarget.localPosition = Vector3.MoveTowards(ikPositions.elbowTargetPos1, ikPositions.elbowTargetPos2, lerpRate);
            }
        }
    }
}