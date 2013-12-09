using UnityEngine;
using System.Collections;

public class Butterflying : MonoBehaviour {

	private bool isAtPoint = false;
	private Vector3 nextPos;
	private GameObject butterfly;
	private GameObject boundaries;
	//Behaviour
	public float speed = 0.5f;
	//Sphere variables
	private float s = 20.0f; // angles inside the sphere
	private float t = 35.0f;
	private float r = 1.0f;
	// Use this for initialization
	void Start () {
		boundaries = GameObject.Find("Boundaries").gameObject;
		r = boundaries.GetComponent<SphereCollider>().radius;

		butterfly  = GameObject.Find("Butterfly_Model").gameObject;
		nextPos = new Vector3 ( r * Mathf.Cos(s) *Mathf.Sin(t),r * Mathf.Sin(s) * Mathf.Sin(t),r * Mathf.Cos(t));
		nextPos += boundaries.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(  Vector3.Distance(butterfly.transform.position, nextPos) < 0.05f)
		{
			//compute next point on the sphere to fly to
			s = Random.value*360.0f;
			t = Random.value*360.0f;
			nextPos = new Vector3 ( r * Mathf.Cos(s) *Mathf.Sin(t),r * Mathf.Sin(s) * Mathf.Sin(t),r * Mathf.Cos(t));
			nextPos += boundaries.transform.position;
		}
		Vector3 dir = nextPos - butterfly.transform.position;//comput the direction towards the nextPos;
		dir.Normalize();
		butterfly.transform.position += dir*speed;//move towards the nextPos;

		//rotate the butterfly, not perfect but ok
		Vector3 offset = new Vector3(270.0f,0.0f, 0.0f);
		Quaternion rotation = Quaternion.LookRotation(dir,offset);
		butterfly.transform.rotation=rotation;
	}
}
