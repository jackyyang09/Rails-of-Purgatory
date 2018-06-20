using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayCanBehaviour : MonoBehaviour {
	
	public int pickupValue; // value of can on pickup

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			GameObject player = other.gameObject;
			player.GetComponent<PlayerBehaviourCanTest>().AddSprayCans(this.pickupValue);
			Object.Destroy(this.gameObject);
		}
	}
}
