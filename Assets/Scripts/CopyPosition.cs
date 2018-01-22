using UnityEngine;

namespace MaxPayne
{
    public class CopyPosition : MonoBehaviour
    {
        public Transform target;

        public float lerpRate;

        void FixedUpdate ()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * lerpRate);
        }
    }
}