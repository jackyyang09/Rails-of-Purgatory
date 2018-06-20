using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    [Header("Gameplay Modifiers")]
    public bool onRail;

    [Header("Control Settings")]
    public float camTurnSpeed;

    [Header("Physics")]
    public float maxSpeed; // Skating speed is 60% of this value
    public float speedDecay;
    public float antiBumpFactor;
    [SerializeField]
    float currentSpeed;
    public float jumpForce;
    public float jumpDuration;
    public float groundCheckDistance;

    [Header("Components")]
    public Transform camStand;
    CharacterController controller;
    RailLogic currentRail;

    [SerializeField]
    bool isGrounded;
    bool stickReleased;
    float stickDistance;
    float lastWorkingX;
    float lastWorkingY;
    float jumpTimer;

    Vector3 moveDir; //Direction of movement, fed into Character Controller

    float heightOffset;

    public enum States
    {

    }

    // Use this for initialization
    void Start() {
        controller = GetComponent<CharacterController>();
        heightOffset = (transform.up * controller.height / 2).y;
    }

    //Mainly used for things corresponding to input
    void Update() {

    }

    //Mainly for physics stuff
    private void FixedUpdate()
    {
        //Keep the cameraStand on top of the player
        camStand.position = transform.position;

        isGrounded = (controller.Move(new Vector3(0, moveDir.y, 0) * Time.deltaTime) & CollisionFlags.Below) != 0;

        controller.Move(moveDir * Time.deltaTime);

        HandleMovement();

        JumpLogic();

        ApplyPhysics();

        //CheckGroundStatus();

        CalculateSpeed();

        //Handles both movement and ground status
        if (stickReleased && isGrounded)
        {
            moveDir *= speedDecay;
        }

    }

    void HandleMovement()
    {
        if (isGrounded)
        {
            //Distance of the left joystick from center
            stickDistance = Vector2.Distance(Vector2.zero, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            if (stickDistance > 1) stickDistance = 1; //Distance is capped at 1

            Vector3 h = Vector3.zero;
            Vector3 v = Vector3.zero;

            if (stickDistance > 0)
            {

                stickReleased = false;
                Vector3 movement = Vector3.zero;

                h = Input.GetAxis("Horizontal") * camStand.right;
                v = Input.GetAxis("Vertical") * camStand.forward;

                //This prevents the player from going extra fast when moving diagonally
                float inputModifyFactor = (Input.GetAxis("Horizontal") != 0.0f && Input.GetAxis("Vertical") != 0.0f) ? .7071f : 1.0f;

                lastWorkingX = Input.GetAxis("Horizontal");
                lastWorkingY = Input.GetAxis("Vertical");
                movement = (v * inputModifyFactor + h * inputModifyFactor) * maxSpeed * 0.6f;
                movement.y = -antiBumpFactor;

                if (movement != Vector3.zero && stickDistance != 0)
                {
                    moveDir = new Vector3(0, controller.velocity.y, 0) + movement;
                }

                float rotationY = camTurnSpeed * lastWorkingX;
                //camStand.Rotate(0, rotationY, 0);
                transform.Rotate(0, rotationY, 0);
            }
            else
            {
                stickReleased = true;
            }
        }
    }

    /*
     * Apply everything from gravity to speed deacceleration
     * */
    void ApplyPhysics()
    {
        if (!isGrounded)
        {
            moveDir.y -= -Physics.gravity.y * Time.deltaTime;
        }
    }

    /*
    * Handles jumping logic
    * */
    void JumpLogic()
    {
        if (onRail)
        {
            if (Input.GetButtonDown("Jump"))
            {
                currentRail.LeaveRail();
                moveDir.y = jumpForce;
            }
        }
        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                moveDir.y = jumpForce;
            }
        }
        if (!isGrounded)
        {
            if (Input.GetButton("Jump") && jumpTimer < jumpDuration)
            {
                moveDir.y = jumpForce;
                jumpTimer += Time.deltaTime;
            }
            else if (Input.GetButtonUp("Jump") && jumpTimer <= jumpDuration)
            {
                moveDir.y = 0;
                jumpTimer = jumpDuration;
            }
        }
    }

    /*
 * Copied a lot of this from Unity's thirdpersoncontroller script
 * */
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            isGrounded = true;
            jumpTimer = 0;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //We've hit a rail object
        if (other.transform.root.tag == "Rail")
        {
            if (other.transform.root.GetComponent<RailLogic>() != currentRail)
            {
                currentRail = other.transform.root.GetComponent<RailLogic>();
                currentRail.SetReferenceRail(other.transform);
                currentRail.SetOnRail(true);
                currentRail.SetPlayerRailSpeed(currentSpeed);
                SetRailStatus(true);
            }
        }
    }

    public void SetRailStatus(bool _bool)
    {
        onRail = _bool;
        if (!onRail)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            currentRail = null;
        }
    }

    public bool IsOnRail()
    {
        return onRail;
    }

    void CalculateSpeed()
    {
        currentSpeed = Mathf.Sqrt(Mathf.Pow(controller.velocity.x, 2) + Mathf.Pow(controller.velocity.z, 2));
    }

    /*
     * Returns a percentage value of current speed relative to the max speed
     * */
    public float GetSpeed()
    {
        return currentSpeed;
    }

    /*
     * Manually set a velocity
     * Usually used when player gets off a rail
     * */
    public void SetVelocity(Vector3 newVelocity)
    {
        moveDir = newVelocity;
    }

    /*
     * Returns the character's height offset as a Vector3
     * */
    public Vector3 GetHeightOffset()
    {
        return new Vector3(0, heightOffset, 0);
    }
}