using UnityEngine;
using System.Collections;

public class ActiveTile : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void showNextTile(Direction inDirection)
	{
		var myMaterial = renderer.material;
		myMaterial.SetColor("_Color",new Color(Random.value,Random.value,Random.value,1.0f));
	}
}
