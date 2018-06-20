using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour {
    public List<Vector3> points;
    public float radius;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnDrawGizmos()
    {
        foreach(Vector3 c in points)
        {
            Gizmos.DrawSphere(c, radius);
        }
    }
}
