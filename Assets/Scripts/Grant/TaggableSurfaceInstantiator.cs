using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* instantiates taggable surfaces based on how many tag spots they are made up of
 * and other things
 */
public class TaggableSurfaceInstantiator : MonoBehaviour {
	public GameObject taggableSurfacePrefab;
	public int size; /* number of tag spots this surface will be made of */

	// Use this for initialization
	void Start () {
		GameObject parent = gameObject.transform.parent.gameObject;

		/* split the tag surface along the horizontal direction */
		float intervalWidth = parent.transform.localScale.x / size;
		Vector3 horizontalDirection = parent.transform.right;
		Vector3 horizontalOffset = parent.transform.position;
		horizontalOffset.x -= (parent.transform.localScale.x - intervalWidth)/2.0f;
		
		Vector3 scale = new Vector3(1.0f / size, 1, 1);

		for (int i = 0; i < size; ++i)
		{
			Vector3 newPosition = (horizontalDirection * intervalWidth * i) + horizontalOffset;
			GameObject tagSurface = Instantiate(taggableSurfacePrefab, newPosition,parent.transform.rotation,parent.transform);
			/* rescale the tag surface */
			tagSurface.transform.localScale = scale;
			TaggableSurfaceController tagScript = tagSurface.GetComponentInChildren<TaggableSurfaceController>();
			if(size > 1)
			{
				tagScript.InGroup = true;
				tagScript.SurfaceIndex = i;
			}
		}
		parent.GetComponent<MeshRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
