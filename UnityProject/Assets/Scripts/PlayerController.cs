using UnityEngine;
using System.Collections;

//public enum for use in ActiveTile and WorldGeneration as well
public enum Direction {North,South,East,West,None};

public class PlayerController : MonoBehaviour {
	
	public float speed;
	public ActiveTile actTile;
	
	// Update is called once per frame before the physics are computed
	void FixedUpdate () 
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		
		Vector3 movement = new Vector3(moveHorizontal,0.0f,moveVertical);
		rigidbody.AddForce (movement*speed*Time.deltaTime);
	}
	
	void OnTriggerEnter (Collider other)
	{
		if( other.gameObject.tag == "NextTileTriggers")
		{
			// stop motion
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			
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
}