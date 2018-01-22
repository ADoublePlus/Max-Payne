using UnityEngine;

namespace MaxPayne
{
    public class AnimationControl : MonoBehaviour
    {
        Animator anim;
        Rigidbody rigid;
        Transform cam;
        Vector3 targetUpDistance; // The position we want to jump towards
        Vector3 jumpDirection;

        bool storeTargetUpDis;
        bool onAir;
        bool aimingJump;
        bool AddJumpForce;
        bool rotateToJump;

        float airTimer;

        // Use this for initialization
        void Start ()
        {
            anim = GetComponent<Animator>();

            rigid = GetComponent<Rigidbody>();

            cam = Camera.main.transform;
        }

        void FixedUpdate ()
        {
            FindAngles(); 

            if (onAir)
            {
                JumpingLogic();
            }
            else
            {
                // If we are on the ground
                if (aimingJump)
                {
                    // Player hits a movement button
                    float h = Input.GetAxis("Horizontal");
                    float v = Input.GetAxis("Vertical");

                    if (Mathf.Abs(h) + Mathf.Abs(v) > 0)
                    {
                        anim.SetTrigger("GetUp");

                        onAir = false;

                        aimingJump = false;
                    }
                }
            }

            anim.SetBool("onAir", onAir);
        }

        void FindAngles ()
        {
            // Get a point forward of the character
            Ray ray = new Ray(transform.position, transform.forward);

            Vector3 dir = ray.GetPoint(100) - cam.position; // Find the direction towards the camera

            // Convert it into the local space of the camera
            Vector3 relativePosition = cam.InverseTransformDirection(dir).normalized;

            // Where we want the character to look towards
            anim.SetFloat("AimFront", relativePosition.z, .1f, Time.deltaTime);
            anim.SetFloat("AimSides", relativePosition.x, .1f, Time.deltaTime);
        }

        // Called from the animation event of Jump Start
        public void GoAirborne ()
        {
            onAir = true;

            aimingJump = true;
        }

        void JumpingLogic ()
        {
            airTimer += Time.deltaTime; // Add to the air time

            if (airTimer < .5f)
            {
                rigid.drag = 0;

                // Get a position we want to jump towards
                if (!storeTargetUpDis)
                {
                    targetUpDistance = transform.position + transform.forward * 1.5f + new Vector3(0, 1.5f, 0);

                    storeTargetUpDis = true;
                }

                // Add force on the direction between our position and where we want to jump to
                if (!AddJumpForce)
                {
                    rigid.AddForce((targetUpDistance - transform.position).normalized * 8, ForceMode.Impulse);

                    AddJumpForce = true;
                }
            }
            else
            {
                // Do a raycast down to see if we hit the ground
                RaycastHit hit;

                if (Physics.Raycast(transform.position, -transform.up, out hit, 0.3f))
                {
                    onAir = false;

                    airTimer = 0;

                    rigid.drag = 4;

                    storeTargetUpDis = false;

                    AddJumpForce = false;
                }
            }
        }
    }
}