using UnityEngine;
using System.Collections;
using UnityEditor.Events;

public enum Weapon
{
    UNARMED = 0,
    RELAX = 8
}

public class RPGCharacterControllerFREE : MonoBehaviour
{
    #region Variables

    //Components
    Rigidbody rb;
    protected Animator animator;
    public GameObject target;
    [HideInInspector]
    public Vector3 targetDashDirection;
    public Camera sceneCamera;
    public bool useNavMesh = false;
    private UnityEngine.AI.NavMeshAgent agent;
    private float navMeshSpeed;

    //jumping variables
    public float gravity = -9.8f;

    //movement variables
    [HideInInspector]
    public bool canMove = true;
    public float walkSpeed = 1.35f;
    float moveSpeed;
    public float runSpeed = 6f;
    float rotationSpeed = 40f;
    Vector3 inputVec;
    Vector3 newVelocity;

    //Weapon and Shield
    [HideInInspector]
    public Weapon weapon;
    int rightWeapon = 0;
    int leftWeapon = 0;
    [HideInInspector]
    public bool isRelax = false;

    //isStrafing/action variables
    [HideInInspector]
    public bool canAction = true;
    [HideInInspector]
    public bool isStrafing = false;
    [HideInInspector]
    public bool isDead = false;
    public float knockbackMultiplier = 1f;
    bool isKnockback;

    //inputs variables
    float inputHorizontal = 0f;
    float inputVertical = 0f;

    #endregion

    #region Initialization and Inputs

    void Start()
    {
        //set the animator component
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = false;
    }

    void Inputs()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal") * Time.deltaTime * 3.0f;
        inputVertical = Input.GetAxisRaw("Vertical") * Time.deltaTime * 3.0f;
        transform.Translate(inputHorizontal, 0, inputVertical);
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            animator.SetBool("Moving", true);
        }
    }

    #endregion

    #region Updates

    void Update()
    {
        //make sure there is animator on character
        if (animator)
        {
            Inputs();
            if (canMove && !isDead && !useNavMesh)
            {
                CameraRelativeMovement();
            }

            //if strafing
            if (Input.GetKey(KeyCode.F))
            {
                animator.SetBool("Attack", true);
            }
            else
            {
                isStrafing = false;
                animator.SetBool("Strafing", false);
            }
        }
        else
        {
            Debug.Log("ERROR: There is no animator for character.");
        }
        if (useNavMesh)
        {
            agent.enabled = true;
            navMeshSpeed = agent.velocity.magnitude;
        }
        else
        {
            agent.enabled = false;
        }
        //Slow time
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0.15f;
            }
        }
        //Pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        //apply gravity force
        rb.AddForce(0, gravity, 0, ForceMode.Acceleration);
        //check if character can move
        if (canMove && !isDead)
        {
            moveSpeed = UpdateMovement();
        }
    }

    //get velocity of rigid body and pass the value to the animator to control the animations
    void LateUpdate()
    {
        if (!useNavMesh)
        {
            //Get local velocity of charcter
            float velocityXel = transform.InverseTransformDirection(rb.velocity).x;
            float velocityZel = transform.InverseTransformDirection(rb.velocity).z;
            //Update animator with movement values
            animator.SetFloat("Velocity X", velocityXel / runSpeed);
            animator.SetFloat("Velocity Z", velocityZel / runSpeed);
            //if character is alive and can move, set our animator
            if (!isDead && canMove)
            {
                if (moveSpeed > 0)
                {
                    animator.SetBool("Moving", true);
                }
                else
                {
                    animator.SetBool("Moving", false);
                    animator.SetBool("Attack", false);
                }
            }
        }
        else
        {
            animator.SetFloat("Velocity X", agent.velocity.sqrMagnitude);
            animator.SetFloat("Velocity Z", agent.velocity.sqrMagnitude);
            if (navMeshSpeed > 0)
            {
                animator.SetBool("Moving", true);
            }
            else
            {
                animator.SetBool("Moving", false);
                animator.SetBool("Attack", false);
            }
        }
    }

    #endregion

    #region UpdateMovement

    void CameraRelativeMovement()
    {
        //converts control input vectors into camera facing vectors
        Transform cameraTransform = sceneCamera.transform;
        //Forward vector relative to the camera along the x-z plane   
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        //Right vector relative to the camera always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        inputVec = inputHorizontal * right + inputVertical * forward;
    }

    float UpdateMovement()
    {
        if (!useNavMesh)
        {
            CameraRelativeMovement();
        }
        Vector3 motion = inputVec;
        if (isStrafing && !isRelax)
        {
            //make character point at target
            Quaternion targetRotation;
            Vector3 targetPos = target.transform.position;
            targetRotation = Quaternion.LookRotation(targetPos - new Vector3(transform.position.x, 0, transform.position.z));
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
        }
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
        //return a movement value for the animator
        return inputVec.magnitude;
    }

    #endregion
}