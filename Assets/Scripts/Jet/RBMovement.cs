using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBMovement : MonoBehaviour {
    PlayerInput pInput;
    public Camera cam;
    public float accel;
    public float speed;
    public float deaccel;
    public float turnRate;

    public float currentSpeed;
    public Vector3 velocity;

	// Use this for initialization
	void Start () {
        pInput = GetComponent<PlayerInput>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 input = cam.transform.rotation * pInput.lstick;
        input.y = 0;
        Quaternion currentRot = transform.rotation;
        Quaternion desiredRotation = transform.rotation;
        if(input != Vector3.zero)
            desiredRotation = Quaternion.LookRotation(input);
        
        transform.rotation = Quaternion.RotateTowards(currentRot,desiredRotation,turnRate);

        Vector3 accelDir = transform.forward * accel * input.magnitude;
        velocity += accelDir;
        velocity = Vector3.ClampMagnitude(velocity, speed);


        if (input.magnitude < 0.1)
        {
            velocity*=deaccel;

        }
        transform.position += velocity;
        currentSpeed = velocity.magnitude;
	}
}
