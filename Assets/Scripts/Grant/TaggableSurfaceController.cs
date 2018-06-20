using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggableSurfaceController : MonoBehaviour {
	private GameObject parent;
	private bool IsTagged;
	const int sprayCost = 1;

	[System.ComponentModel.DefaultValue(false)]
	public bool InGroup { get; set; }

	[System.ComponentModel.DefaultValue(-1)]
	public int SurfaceIndex { get; set; }

	// Use this for initialization
	void Start () {
		parent = gameObject.transform.parent.gameObject;
		IsTagged = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			GameObject player = other.gameObject;
			player.GetComponent<PlayerBehaviourCanTest>().SetNearestTagSurface(gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			GameObject player = other.gameObject;
			player.GetComponent<PlayerBehaviourCanTest>().RemoveTagSurface(gameObject);
		}
	}

	/**
	 * Spray a tag onto the parent object. Returns true on success, false on
	 * failure
	 */
	public bool SprayTag(Texture2D tag)
	{
		if (!IsTagged)
		{
			Renderer pRenderer = parent.GetComponent<Renderer>();
			pRenderer.material.mainTexture = tag;
			IsTagged = true;
			gameObject.SetActive(false);
			return true;
		}
		return false;
	}
}
