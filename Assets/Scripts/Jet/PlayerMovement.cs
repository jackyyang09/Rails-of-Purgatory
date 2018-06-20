using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 lstick, rstick;
    CharacterController controller;
    public float maxWalkSpeed, walkAccel;
    public float maxRunSpeed, runAccel;
    public float deaccel;
    public float currentSpeed;
    public Vector3 currentVelocity;

    [Range(0, 1)]
    public float runThreshold;
    [Range(0, 1)]
    public float walkThreshold;
    [Range(0, 1)]
    public float turnThreshold;
    [Range(0, 1)]
    public float stopThreshold;
    public Quaternion targetRotation;

    [Range(0, 1)]
    public float turnTime;

    public Camera cam;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        lstick = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        currentSpeed = currentVelocity.magnitude;
        //stop
        if (lstick.sqrMagnitude <= stopThreshold)
        {
            currentVelocity *= deaccel;
        }

        //turn
        if (lstick.sqrMagnitude > stopThreshold && lstick.sqrMagnitude <= turnThreshold)
        {
            lstick = cam.transform.rotation * lstick;
            lstick.y = 0;
            targetRotation = Quaternion.LookRotation(lstick);
            transform.rotation = targetRotation;
        }

        //walk
        if (lstick.sqrMagnitude <= walkThreshold && lstick.sqrMagnitude > turnThreshold)
        {
            lstick = cam.transform.rotation * lstick;
            lstick.y = 0;
            targetRotation = Quaternion.LookRotation(lstick);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnTime);
            if (currentSpeed < maxWalkSpeed)
             currentVelocity = (transform.forward * (walkAccel+currentSpeed));
            else
            {
                currentVelocity = transform.forward * currentSpeed;
            }
        }

        //run
        if (lstick.sqrMagnitude > walkThreshold)
        {
            lstick = cam.transform.rotation * lstick;
            lstick.y = 0;
            targetRotation = Quaternion.LookRotation(lstick);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnTime);
            if (currentSpeed < maxRunSpeed)
            {
                currentVelocity += (transform.forward * runAccel);

            } else
            {
                currentVelocity = transform.forward * currentSpeed;
            }
        }

        transform.position += currentVelocity;
    }   
}
