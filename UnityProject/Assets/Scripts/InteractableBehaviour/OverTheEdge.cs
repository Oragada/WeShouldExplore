using System.Linq;
using UnityEngine;
using System.Collections;

public class OverTheEdge : MonoBehaviour
{
	public int hit = 0;
	void OnTriggerEnter(Collider other)
	{
		hit++;
		if (other.tag == "NextTileTriggers")
		{
			//MeshRenderer rend = GetComponent<MeshRenderer>();
			//if(rend != null)rend.renderer.enabled = false;
			gameObject.SetActive(false);
			//GetComponentsInChildren<MeshRenderer>().ToList().ForEach(e => e.renderer.enabled = false);
			//GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(e => e.renderer.enabled = false);
			//Destroy(this);
		}
	}
}
