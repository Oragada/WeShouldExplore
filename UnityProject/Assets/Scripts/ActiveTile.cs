using UnityEngine;
using System.Collections;

public class ActiveTile : MonoBehaviour {

    public InteractionTree tree;

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

    public void Interact(Vector3 pos, PlayerController playerController)
    {
        if ((tree.transform.position - pos).magnitude < 20)
        {
            tree.Interact(playerController);
        }
    }
}
