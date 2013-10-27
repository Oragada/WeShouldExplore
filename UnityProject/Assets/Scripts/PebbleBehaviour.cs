using UnityEngine;
using System.Collections;

public class PebbleBehaviour : CustomBehaviour {	
	
	private bool isActive=false;
	
	public override void customAwake()
	{
		SphereCollider trigger = GetComponent<SphereCollider>();		
		trigger.radius = triggerRadius;
		rigidbody.isKinematic = true;
	}
	public override void activate(float playerProgress)
	{
		isActive = !isActive;
		rigidbody.isKinematic = !isActive;
		
		//transform.position = new Vector3(transform.position.x,transform.position.y+5.0f, transform.position.z);
	}
	public override  string customInteractiveText()
	{
		//Press E
		return "to pick up the pebble.";
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
