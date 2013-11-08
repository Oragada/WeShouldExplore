using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour {
	
	PlayerController myscript;
	// Use this for initialization
	void Start () {
		myscript = transform.parent.GetComponent<PlayerController>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter (Collider other)
	{
		myscript.channeledTriggerEnter(other);
	}
	void OnTriggerExit (Collider other)
	{
		myscript.channeledTriggerExit(other);
	}
}
