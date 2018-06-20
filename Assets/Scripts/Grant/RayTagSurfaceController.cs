using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTagSurfaceController : MonoBehaviour
{
	private GameObject parent;
	private bool IsTagged;
	const int sprayCost = 1;

	private float[] sprayBins;

	public uint sprayGridX;
	public uint sprayGridY;

	// Use this for initialization
	void Start()
	{
		parent = gameObject.transform.parent.gameObject;
		sprayBins = new float[sprayGridX * sprayGridY];
		for(int i = 0; i < sprayGridX*sprayGridY; ++i)
		{
			sprayBins[i] = 0;
		}
		IsTagged = false;
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			GameObject player = other.gameObject;
			player.GetComponent<PlayerBehaviourCanTest>().SetNearestTagSurface(gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			GameObject player = other.gameObject;
			player.GetComponent<PlayerBehaviourCanTest>().RemoveTagSurface(gameObject);
		}
	}

	public GameObject getParent()
	{
		return parent;
	}

	/**
	 * take in spray paint from the player with a distribution around the centermost
	 * hitpoint (the first hitpoint). hitpoints should be a normalized vector where
	 * (0,0,z) represents the bottom left and (1,1,z) is the top right. z is not used.
	 */
	public void Spray(PlayerSprayData playerSprayData)
	{
		Vector2Int[] binsHit = new Vector2Int[playerSprayData.sprayPoints.Length];
		/* any ray which did not intersect the surface will have their bin set to (-1,-1) */
		for (int i = 0; i < playerSprayData.sprayPoints.Length; ++i)
		{
			if (playerSprayData.castSuccess[i])
			{
				binsHit[i] = new Vector2Int(Mathf.RoundToInt(playerSprayData.sprayPoints[i].x * sprayGridX),
					Mathf.RoundToInt(playerSprayData.sprayPoints[i].y * sprayGridY));
			}
			else
			{
				binsHit[i] = new Vector2Int(-1, -1);
			}
		}	
		
		for(int i = 0; i < binsHit.Length; ++i)
		{
			if(binsHit[i].x >= 0 && binsHit[i].x <= sprayGridX 
				&& binsHit[i].y >= 0 && binsHit[i].y <= sprayGridY)
			{
				// do stuff
			}
		}
	
	}
}