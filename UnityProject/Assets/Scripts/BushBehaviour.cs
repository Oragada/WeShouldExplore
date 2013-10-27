using UnityEngine;
using System.Collections;

public class BushBehaviour : CustomBehaviour {
	
	private Color inactiveColor;
	private Color activeColor;
	private bool isActive;
	
	public override void customAwake()
	{		
		triggerRadius = bushTriggerRadius;
		inactiveColor = new Color(0.0f, 96.0f/255.0f, 5.0f/255.0f,1.0f);
		activeColor = new Color(90.0f/255.0f, 122.0f/255.0f, 96.0f/255.0f,1.0f);
		isActive = false;
			
		SphereCollider trigger = GetComponent<SphereCollider>();
		trigger.radius = triggerRadius;
	}
	public override void activate(float playerProgress)
	{
		isActive = !isActive;
		if (isActive)
			renderer.material.color = activeColor;
		else 
			renderer.material.color = inactiveColor;
	}
	public override  string customInteractiveText()
	{
		//Press E
		return "to pick a berry from the bush.";
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
