using UnityEngine;

namespace MaxPayne
{
    public class FreeCameraLook : Pivot
    {
        [SerializeField] private bool lockCursor = false;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSpeed = 1.5f;
        [SerializeField] private float turnsmoothing = .1f;
        [SerializeField] private float tiltMax = 75f;
        [SerializeField] private float tiltMin = 45f;
        
        private const float LookDistance = 100f;

        private float lookAngle;
        private float tiltAngle;
        private float smoothX = 0;
        private float smoothY = 0;
        private float smoothXvelocity = 0;
        private float smoothYvelocity = 0;


        protected override void Awake ()
        {
            base.Awake();

            Cursor.lockState = CursorLockMode.Confined;

            cam = GetComponentInChildren<Camera>().transform;

            pivot = cam.parent;
        }

        protected override void Update ()
        {
            base.Update();

            HandleRotationMovement();

            if (lockCursor && Input.GetMouseButtonUp(0))
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        void OnDisable ()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        protected override void Follow (float deltaTime)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);
        }

        void HandleRotationMovement ()
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            if (turnsmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothXvelocity, turnsmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothYvelocity, turnsmoothing);
            }
            else
            {
                smoothX = x;
                smoothY = y;
            }

            lookAngle += smoothX * turnSpeed;

            transform.rotation = Quaternion.Euler(0f, lookAngle, 0);

            tiltAngle -= smoothY * turnSpeed;

            tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);

            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        }
    }
}