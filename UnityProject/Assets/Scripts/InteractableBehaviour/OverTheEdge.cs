using System.Linq;
using UnityEngine;
using System.Collections;

public class OverTheEdge : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NextTileTriggers")
        {
            //MeshRenderer rend = GetComponent<MeshRenderer>();
            //if(rend != null)rend.renderer.enabled = false;
            GetComponentsInChildren<MeshRenderer>().ToList().ForEach(e => e.renderer.enabled = false);
            //Destroy(this);
        }
    }
}
