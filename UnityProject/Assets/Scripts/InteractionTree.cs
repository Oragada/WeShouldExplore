using UnityEngine;
using System.Collections;

public class InteractionTree : MonoBehaviour
{

    public GameObject movePosition;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Interact(PlayerController playerController)
    {
        GUI.Label(new Rect(200,20,50,20),"interacting");
        playerController.transform.position = movePosition.transform.position;

    }
}
