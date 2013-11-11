using UnityEngine;
using System.Collections;
using System.Linq;

public class RabbitMovement : MonoBehaviour
{
    public Quaternion InitialFace { get; set; }

    public int activated = 0;
    public int deactivated = 0;

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
            GetComponentsInChildren<MeshRenderer>().ToList().ForEach(e => e.renderer.enabled = false);
            //Destroy(this);
        }
    }

    /*public void Activate(Vector3 look)
    {
        activated++;
        gameObject.transform.LookAt(look+transform.position);
    }*/

    public void Look(Vector3 look)
    {
        Vector3 lookPos = transform.position + look;// -transform.localPosition;
        //lookPos.y *= 0;
        gameObject.transform.LookAt(lookPos);
    }

    public void Bliss()
    {
        transform.rotation = InitialFace;
    }

    /*public void Deactivate(RabbitBehaviour behaviour)
    {
        deactivated++;
        switch (behaviour)
        {
            case RabbitBehaviour.Ignore:
            case RabbitBehaviour.Observe:
                
                break;
            default:
                break;
        }
    }*/
}
