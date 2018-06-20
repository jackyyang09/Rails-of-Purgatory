using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerSprayData
{
	public Vector3[] sprayPoints;
	public BitArray castSuccess;
	public Vector3 playerPosition;
	public Texture2D tag;

	public PlayerSprayData(Vector3[] sprayPoints,BitArray castSuccess,Vector3 playerPosition, Texture2D tag)
	{
		this.sprayPoints = sprayPoints;
		this.castSuccess = castSuccess;
		this.playerPosition = playerPosition;
		this.tag = tag;
	}
}

/* using brogrammist's script as a base */
public class PlayerBehaviourCanTest : MonoBehaviour {

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

	[Header("Stats")]
	public int sprayCansMax;
	public int sprayCansStart;
	private int sprayCansCurrent;
	public float sprayConeAngle; // angle dictating the width of the spray cone
	public float sprayDirectionMaxAngle; // angle dictating the maximum angle the spray can will be directed at the wall

	[Header("Graffiti")]
	public Texture2D smallTag;
	public Texture2D mediumTag;
	private Texture2D[] mediumTagPieces;
	const byte mediumTagSize = 5;
	private const byte numRayCast = 5;

	[SerializeField]
	bool isGrounded;
	bool stickReleased;
	float stickDistance;
	float lastWorkingX;
	float lastWorkingY;
	float jumpTimer;

	Vector3 moveDir; //Direction of movement, fed into Character Controller
	
	/* TODO: Change this to a datastructure of surfaces we are near and
	 * use euclidean distance to get the closest one when tagging it
	 */
	private GameObject nearestTagSurface; // tag surface that will be tagged when player goes to tag
	

	public enum States
	{

	}

	// Use this for initialization
	void Start()
	{
		controller = GetComponent<CharacterController>();
		sprayCansCurrent = sprayCansStart;
		nearestTagSurface = null;
		mediumTagPieces = new Texture2D[mediumTagSize];
		SubdivideMediumTag();
	}

	//Mainly used for things corresponding to input
	void Update()
	{

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

		CheckGroundStatus();

		CalculateSpeed();

		SprayTag();

		//Handles both movement and ground status
		if (stickReleased)
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

				//Factor in rotation from movement
				float rotationY = camTurnSpeed * lastWorkingX;
				camStand.Rotate(0, rotationY, 0);
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

	void CalculateSpeed()
	{
		currentSpeed = Mathf.Sqrt(Mathf.Pow(controller.velocity.x, 2) + Mathf.Pow(controller.velocity.z, 2));
	}

	/**
	 * If near a taggable surface and we have enough spray cans, spray graffiti
	 * onto the taggable surface. If we were unable to spray the surface for any reason
	 * we do not deduct any spray cans.
	 */
	void SprayTag()
	{
		
		if (nearestTagSurface != null && sprayCansCurrent > 0 && Input.GetButton("Fire3"))
		{
			/*
			bool sprayed;
			TaggableSurfaceController tagScript = nearestTagSurface.GetComponent<TaggableSurfaceController>();
			if (!tagScript.InGroup)
			{
				sprayed = tagScript.SprayTag(smallTag);
			}
			else
			{
				sprayed = tagScript.SprayTag(mediumTagPieces[tagScript.SurfaceIndex]);
			}
			
			if (sprayed)
			{
				--sprayCansCurrent;
			}*/
			RayTagSurfaceController tagScript = nearestTagSurface.GetComponent<RayTagSurfaceController>();
			GameObject surface = tagScript.getParent();

			RaycastHit hitInfo = new RaycastHit();
			Vector3[] castDirections = new Vector3[numRayCast];

			/* get the direction of spraying, tending towards the centre of the surface
			 * but constrained with a max angle that the player can spray at
			 */
			Vector3 toSurfaceCenter = surface.transform.position - transform.position;
			Debug.DrawRay(transform.position, toSurfaceCenter,Color.red);
			Debug.DrawRay(transform.position, surface.transform.forward,Color.blue);
			float angleDifference = Vector3.Angle(toSurfaceCenter, surface.transform.forward);
			if(angleDifference > sprayDirectionMaxAngle)
			{
				toSurfaceCenter = Vector3.RotateTowards(toSurfaceCenter, surface.transform.forward, Mathf.Deg2Rad*(angleDifference - sprayDirectionMaxAngle),0.0f);
			}

			castDirections[0] = toSurfaceCenter;
			castDirections[1] = Quaternion.Euler(sprayConeAngle, 0, 0) * toSurfaceCenter;
			castDirections[2] = Quaternion.Euler(-sprayConeAngle, 0, 0) * toSurfaceCenter;
			castDirections[3] = Quaternion.Euler(0, sprayConeAngle, 0) * toSurfaceCenter;
			castDirections[4] = Quaternion.Euler(0, -sprayConeAngle, 0) * toSurfaceCenter;

			/* TODO: remove debug ray drawing when no longer necessary */
			bool anyRayHit = false;
			Vector3[] castHitPoints = new Vector3[numRayCast];
			BitArray castSuccess = new BitArray(numRayCast);
			for(int i = 0; i < numRayCast; ++i)
			{
				Debug.DrawRay(transform.position, castDirections[i],Color.green);
				if(Physics.Raycast(transform.position,castDirections[i],out hitInfo, 10f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
				{
					anyRayHit = true;
					castHitPoints[i] = ((hitInfo.point - surface.transform.position).normalized + new Vector3(1,1,1)).normalized;

					castSuccess[i] = true;
				}
				else
				{
					castSuccess[i] = false;
				}
			}

			if (anyRayHit)
			{
				PlayerSprayData playerSprayData = new PlayerSprayData(castHitPoints, castSuccess, transform.position, this.mediumTag);
				tagScript.Spray(playerSprayData);
			}
		}
	}

	public void AddSprayCans(int sprayCans)
	{
		int sum = sprayCansCurrent + sprayCans;
		sprayCansCurrent = sum > sprayCansMax ? sprayCansMax : sum;
		Debug.Log("current spray cans is: " + sprayCansCurrent);
	}

	public void SetNearestTagSurface(GameObject tagSurface)
	{
		nearestTagSurface = tagSurface;
	}

	public void RemoveTagSurface(GameObject tagSurface)
	{
		/* if the nearest surface is another surface, do not change it */
		if(nearestTagSurface == tagSurface)
		{
			nearestTagSurface = null;
		}
	}

	/** 
	 * take the medium tag and divide it into smaller pieces 
	 * for use on medium size tag surfaces. This is done by
	 * taking the pixels within slices of the main texture and
	 * putting them into the new slices.
	 */
	private void SubdivideMediumTag()
	{
		int subWidth = mediumTag.width / mediumTagSize;
		int subHeight = mediumTag.height;
		for (int i = 0; i < mediumTagSize; ++i)
		{
			Texture2D subTexture = new Texture2D(subWidth,subHeight);

			for(int x = 0; x < subWidth; ++x)
			{
				int offsetX = i * subWidth;
				Debug.Log("offsetX: " + offsetX + " first pixel: " + mediumTag.GetPixel(offsetX + x, 0));
				for (int y = 0; y < subHeight; ++y)
				{
					subTexture.SetPixel(x, y, mediumTag.GetPixel(offsetX + x, y));
				}
			}
			subTexture.Apply();
			mediumTagPieces[i] = subTexture;
		}
	}
}
