using UnityEngine;
using System.Collections;
using System.Linq;

public class RabbitMovement : MonoBehaviour
{
    public Quaternion InitialFace { get; set; }

    public Vector3 localC = new Vector3();
    public Vector3 localPos = new Vector3();
    public Vector3 lookP = new Vector3();

    // Use this for initialization
    void Start()
    {
        InitialFace = transform.rotation;
    }
	
	// Update is called once per frame
	void Update () 
    {

	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NextTileTriggers")
        {
            gameObject.SetActive(false);
            //GetComponentsInChildren<MeshRenderer>().ToList().ForEach(e => e.renderer.enabled = false);
            //TODO: call groundgen and inform that a rabbit have moved out
            //Destroy(this);
        }
    }

    public void Look(Vector3 look)
    {
        gameObject.transform.LookAt(look);
    }

    public void Bliss()
    {
        transform.rotation = InitialFace;
    }
}
