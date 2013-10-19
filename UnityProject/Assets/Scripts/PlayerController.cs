using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections;

//public enum for use in ActiveTile and WorldGeneration as well
public enum Direction {North,South,East,West,None};

public class PlayerController : MonoBehaviour {
	
	//publics
	public float speed;
	public ActiveTile actTile;
	//privates
	private bool isSitting=false;
	private bool isInteracting=false;
	private SphereCollider thisCollider = null;
	
	//get the collider component once, because the GetComponent-call is expansive
	void Awake()
	{
		thisCollider = this.GetComponent<SphereCollider>();		
		//rigidbody.freezeRotation = true;//would push the sphere over the landscape
	}
	
	// Update is called once per frame before the physics are computed
	void FixedUpdate () 
	{
		// Cache the inputs.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool sit = Input.GetButtonDown("Sit");
		bool interact = Input.GetButtonDown("Interact");
		
		if( sit )
		{
			if(!isSitting) 
			{
				//stop the motion
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;		
				//sit down
				thisCollider.radius = 0.2f; 
				isSitting = true;
			}
			else
			{
				//stand up again
				thisCollider.radius = 0.5f;
				isSitting = false;
			}
		}	
		
        Vector3 movement = new Vector3(h,0.0f,v);
		if( Input.GetButton("AlternateMovement"))
			movement= new Vector3(h+v,0.0f,v-h);
		
		if ( !isSitting)
		{
			rigidbody.AddForce (movement*speed*Time.deltaTime);
		}
		//trying out different stuffs // doesnt really work
		//transform.position = speed * moveHorizontal * Time.deltaTime;
		//rigidbody.MovePosition(movement * Time.deltaTime * speed);		
	}
	
	void OnTriggerEnter (Collider other)
	{
		if( other.gameObject.tag == "NextTileTriggers")
		{
			// stop motion
			//rigidbody.velocity = Vector3.zero;
			//rigidbody.angularVelocity = Vector3.zero;
			
			//teleport to new position
			Direction dir=Direction.None;
			if( other.name == "WestTrigger")
			{
				dir = Direction.West;
				rigidbody.MovePosition(new Vector3(-(other.gameObject.transform.position.x+2.0f),rigidbody.position.y,rigidbody.position.z)); // move to east 
			}
			else if( other.name == "EastTrigger")
			{
				dir = Direction.East;
				rigidbody.MovePosition(new Vector3(-(other.gameObject.transform.position.x-2.0f),rigidbody.position.y,rigidbody.position.z)); // move to west 
			}
			else if( other.name == "NorthTrigger")
			{
				dir = Direction.North;
				rigidbody.MovePosition(new Vector3(rigidbody.position.x,rigidbody.position.y,-(other.gameObject.transform.position.z-2.0f))); // move to south 
			}
			else if( other.name == "SouthTrigger")
			{
				dir = Direction.South;
				rigidbody.MovePosition(new Vector3(rigidbody.position.x,rigidbody.position.y,-(other.gameObject.transform.position.z+2.0f))); // move to north 
			}
			
			// update tile, pass the direction along
			actTile.showNextTile(dir);			
		}
	}
    void OnGUI()
    {
        speed = GUI.HorizontalSlider(new Rect(25, 35, 300, 30), speed, 0f, 2000.0f);
        GUI.Label(new Rect(25,15,60,20), speed.ToString(CultureInfo.InvariantCulture));
    }
}