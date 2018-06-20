using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    PlayerStateMachine psm;
    PlayerInput playerInput;
    public CharacterAttributes charAttrib;
    public CharacterController Ccontroller;
    public GameObject cameraRig;
    public Camera playerCam;

    public bool debugDrawGround;
    public bool isGrounded;
    public Vector3 groundCheckOffset;
    public Vector3 groundCheckOffsetAir;
    public Vector3 groundCheckSize;
    public Vector3 groundCheckSizeAir;
    public LayerMask groundLayerMask;

    public bool debugDrawRail;
    //public bool isRailing;//lol YOUR BOOLEAN IS MINE IN THE FORM OF onRail
    public Vector3 railGroundOffset;
    public Vector3 railAirOffset;
    public float railGroundSize;
    public float railAirSize;
    public LayerMask railLayerMask;
    public Rail rail;


    public Vector3 velocity;
    public float currentSpeed;
    public float cornering;
    [Header("Debug ")]
    public Text currentStateText;

    RailLogic currentRail;
    [Header("Rail Logic")]
    [SerializeField]
    bool onRail;
    [SerializeField]
    float railHeightOffset;
    [SerializeField]
    float railCooldown; // Amount of time that must pass before player can grind on another rail
    float railCooldownTimer = -1;

    // Use this for initialization
    void Start () {
        playerInput = GetComponent<PlayerInput>();
        psm = new PlayerStateMachine(this, playerInput);
        psm.AddState("idle", new IdleState(this, psm, playerInput));
        psm.AddState("walk", new WalkState(this, psm, playerInput));
        psm.AddState("run", new RunState(this, psm, playerInput));
        psm.AddState("fall", new FallState(this, psm, playerInput));
        psm.AddState("jump", new JumpState(this, psm, playerInput));
        psm.AddState("runskid", new RunSkidState(this, psm, playerInput));
        psm.AddState("rail", new RailState(this, psm, playerInput));
        psm.Set("idle");
        velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    /*
     * Rail Logic
     * */
    public void SetRailStatus(bool _bool)
    {
        onRail = _bool;
        if (!onRail)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            currentRail = null;
            railCooldownTimer = railCooldown;
        }
    }

    public bool isOnRail()
    {
        return onRail;
    }

    public bool canGrind()
    {
        return (railCooldown > 0) ? false : true;
    }

    /*
     * Returns the character's height offset as a Vector3
     * Used for rail logic
     * */
    public Vector3 GetHeightOffset()
    {
        return new Vector3(0, railHeightOffset, 0);
    }

    // Update is called once per frame
    void Update () {
        currentSpeed = velocity.magnitude;

        if (isGrounded)
        {
            isGrounded = Physics.CheckBox(transform.position+groundCheckOffset, groundCheckSize*.5f, Quaternion.identity, groundLayerMask);
        }
        else
        {
            isGrounded = Physics.CheckBox(transform.position + groundCheckOffsetAir, groundCheckSizeAir * .5f, Quaternion.identity, groundLayerMask);
        }

        Collider[] rails;
        if (isGrounded)
        {
            rails = Physics.OverlapSphere(transform.position + railGroundOffset, railGroundSize);

        } else
        {
            rails = Physics.OverlapSphere(transform.position + railAirOffset, railAirSize);

        }

        if (railCooldownTimer > 0)
        {
            railCooldownTimer -= Time.deltaTime;
        }

        if (rails.Length > 0)
        {
            float distance = 0;
            Collider nearest = rails[0];
            foreach (Collider c in rails)
            {
                //Rail logic moved from OnTriggerEnter
                if (c.transform.root.tag == "Rail")
                {
                    if (c.transform.root.GetComponent<RailLogic>() != currentRail)
                    {
                        if (railCooldownTimer < 0)
                        {
                            currentRail = c.transform.root.GetComponent<RailLogic>();
                            currentRail.SetReferenceRail(c.transform);
                            currentRail.SetOnRail(true);
                            currentRail.SetPlayerRailSpeed(currentSpeed);
                            railCooldownTimer = railCooldown;
                            SetRailStatus(true);
                        }
                    }
                }

                float dist = Vector3.SqrMagnitude(c.ClosestPoint(transform.position));
                if (dist < distance)
                {
                    distance = dist;
                    nearest = c;

                }
            }
        }

        psm.HandleInput();
        psm.UpdateState();
        currentStateText.text = psm.currentState.ToString();
        DebugDraw();
	}

   public Rail GetRail()
    {
        return rail;
    }

    public RailLogic GetCurrentRail()
    {
        return currentRail;
    }

    public void SetVelocity(Vector3 _vel)
    {
        velocity = _vel;
    }

    public void DebugDraw()
    {

        Bounds bound1 = new Bounds(transform.position + groundCheckOffset, groundCheckSize);
        Bounds bound2 = new Bounds(transform.position + groundCheckOffsetAir, groundCheckSizeAir);
        if (isGrounded)
            DebugExtension.DebugBounds(bound1, Color.red);
        else
            DebugExtension.DebugBounds(bound2, Color.red);

        DebugExtension.DebugCone(transform.position,-transform.forward);
    }

    public void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.red;
            if (debugDrawGround)
                Gizmos.DrawWireCube(transform.position+groundCheckOffset, groundCheckSize);
            Gizmos.color = Color.green;
            if (debugDrawRail)
                Gizmos.DrawSphere(transform.position + railGroundOffset, railGroundSize);

        }
        else
        {
            Gizmos.color = Color.red;
            if (debugDrawGround)
                Gizmos.DrawWireCube(transform.position+groundCheckOffsetAir, groundCheckSizeAir);
            Gizmos.color = Color.green;
            if (debugDrawRail)
                Gizmos.DrawSphere(transform.position + railAirOffset, railAirSize);
        }

        Gizmos.color = Color.green;
    }
}
