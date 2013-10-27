using UnityEngine;
using System.Collections;

public class Interactive : MonoBehaviour {
	private CustomBehaviour customBehaviour;
	
	void Awake()
	{
		if( customBehaviour == null)
			customBehaviour = GetComponent<BushBehaviour>();
		if( customBehaviour == null)
			customBehaviour = GetComponent<PebbleBehaviour>();
		
		
		if( customBehaviour != null)
			customBehaviour.customAwake();		
	}
	
	/*void OnTriggerEnter(Collider other) {
        if( other.gameObject.tag == "Player")
		{
			Debug.Log ("collision");
			customBehaviour.customPlayerInInteractionRange();
		}
    }*/
	
	public void activate(float inPlayerProgress)
	{
		customBehaviour.activate(inPlayerProgress);
	}
	public string getInteractiveText()
	{		
		if( customBehaviour != null)
			return customBehaviour.customInteractiveText();
		else //Press E 
			return "to interact.";
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
