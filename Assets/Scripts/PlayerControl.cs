using UnityEngine;

namespace MaxPayne
{
    public class PlayerControl : MonoBehaviour
    {
        #region Component Variables

        [SerializeField] PhysicMaterial zfriction; // Zero friction when we move
        [SerializeField] PhysicMaterial mfriction; // Maximum friction when we are stationary

        Animator anim;
        CapsuleCollider capCol;
        Rigidbody rigid;
        Transform cam;
        Transform camHolder;

        #endregion

        [SerializeField] float speed = 0.8f; // How fast we move
        [SerializeField] float turnSpeed = 5; // How fast we turn

        Vector3 directionPos; // The direction we look at
        Vector3 storeDir;

        public bool prepareToJump;

        float horizontal;
        float vertical;
        float targetTurnAmount;
        float curTurnAmount;

        bool jumpOnce;
        bool rotateToLookCamDirection;
        bool canMove;

        // Use this for initialization
        void Start ()
        {
            //Setup the references
            capCol = GetComponent<CapsuleCollider>();

            rigid = GetComponent<Rigidbody>();

            cam = Camera.main.transform;
            camHolder = cam.parent.parent;
            
            SetupAnimator();
        }

        // Update is called once per frame
        void Update ()
        {
            HandleFriction();

            if (Input.GetMouseButtonUp(1))
            {
                prepareToJump = true;
            }

        }

        void FixedUpdate ()
        {
            // Inputs
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            storeDir = camHolder.right;

            canMove = anim.GetBool("CanMove");

            Vector3 dirForward = storeDir * horizontal;
            Vector3 dirSides = camHolder.forward * vertical;

            if (canMove)
                rigid.AddForce((dirForward + dirSides).normalized * speed / Time.deltaTime);

            Ray ray = new Ray(cam.position, cam.forward);

            directionPos = ray.GetPoint(100);

            // Find the direction from that position     
            Vector3 dir = directionPos - transform.position;
            dir.y = 0;

            float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));

            if (!prepareToJump)
            {
                jumpOnce = false;

                anim.SetFloat("Forward", vertical);
                anim.SetFloat("Sideways", horizontal);

                RotateWhileAimingOrMoving(angle, dir);
            }
            else
            {
                FindDirectionToJumpTo();
            }
        }

        void FindDirectionToJumpTo ()
        {
            anim.SetFloat("Sideways", 0, 0.1f, Time.deltaTime);
            anim.SetFloat("Forward", Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            directionPos = transform.position + (camHolder.right * horizontal) + (camHolder.forward * vertical);

            Vector3 dir = directionPos - transform.position;
            dir.y = 0;

            // Find the angle between the characters rotation and where the camera is looking
            float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));

            // If it's not zero 
            if (angle != 0 && canMove) // Look towards the camera
                rigid.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 2 * turnSpeed * Time.deltaTime);

            if (!jumpOnce)
            {
                anim.SetTrigger("Jump");

                jumpOnce = true;
            }
        }

        void RotateWhileAimingOrMoving (float angle, Vector3 dir)
        {
            if (angle > 75)
                rotateToLookCamDirection = true;

            if (!rotateToLookCamDirection)
            {
                if (horizontal != 0 || vertical != 0)
                {
                    if (angle != 0 && canMove)
                    {
                        rigid.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
                    }
                }
            }
            else if (canMove)
            {
                targetTurnAmount = 1;

                if (angle != 0)
                {
                    rigid.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
                }

                if (angle < 1)
                {
                    rotateToLookCamDirection = false;

                    targetTurnAmount = 0;
                }
            }

            curTurnAmount = Mathf.MoveTowards(curTurnAmount, targetTurnAmount, Time.deltaTime * 3);

            anim.SetFloat("PivotTurn", curTurnAmount);
        }

        void HandleFriction ()
        {
            // If there's no input
            if (horizontal == 0 && vertical == 0)
            {
                // Stationary, so we want maximum friction
                capCol.material = mfriction;
            }
            else
            {
                // If we are moving, then we don't want any friction
                capCol.material = zfriction;
            }
        }

        void SetupAnimator ()
        {
            // This is a reference to the animator component on the root component.
            anim = GetComponent<Animator>();

            // We use the avatar from a child animator component if present
            // This is to enable easy swapping of the character model as a child node
            foreach (var childAnimator in GetComponentsInChildren<Animator>())
            {
                if (childAnimator != anim)
                {
                    anim.avatar = childAnimator.avatar;

                    Destroy(childAnimator);

                    break; 
                }
            }
        }
    }
}