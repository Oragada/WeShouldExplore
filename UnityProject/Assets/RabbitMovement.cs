using UnityEngine;
using System.Collections;
using System.Linq;

public class RabbitMovement : MonoBehaviour
{
    public float RunDirection = 0f;
    public bool Escaping = false;
    public float Speed = 1.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        //0 -> 1z, 0x
        //90 -> 0z, 1x
        //180 -> -1z, 0x
        //270 -> 0z, -1x
        //cos -> x
        //sin -> z

	    if (Escaping)
	    {
            float xMod = Mathf.Cos(RunDirection / 180 * Mathf.PI);
            float zMod = Mathf.Sin(RunDirection / 180 * Mathf.PI);

            transform.position += (new Vector3(zMod, 0, xMod) * Speed * Time.deltaTime);
	    }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NextTileTriggers")
        {
            this.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(e => e.renderer.enabled = false);
        }
    }

    public void Activate(float direction)
    {
        Escaping = true;
        RunDirection = direction;
        transform.transform.eulerAngles = new Vector3(0,direction,0);
    }
}
