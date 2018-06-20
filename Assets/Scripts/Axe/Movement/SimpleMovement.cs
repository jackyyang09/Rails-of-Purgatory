using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour {
    [SerializeField]
    private bool VertAxis, HorAxis;

    [SerializeField]
    private float Speed, Turn;
    [SerializeField]
    private float MaxSpeed, MaxTurn;

    public float h, v;
    private Rigidbody RBod;
	// Use this for initialization
	void Start () {
        RBod = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        VertAxis = Input.GetAxis("Vertical") != 0;
        v = Input.GetAxis("Vertical");

        HorAxis = Input.GetAxis("Horizontal") != 0;
        h = Input.GetAxis("Horizontal");
    }

    public void FixedUpdate()
    {
        if(VertAxis)
        {
            RBod.MovePosition(transform.position + (transform.forward * (Speed * v)));

            VertAxis = false;
        }
        if (HorAxis)
        {
            var turnF = (Turn * Time.deltaTime) * h;

            RBod.MoveRotation(Quaternion.Euler(new Vector3(0,transform.rotation.eulerAngles.y + turnF,0)));

            HorAxis = false;
        }
    }
}
