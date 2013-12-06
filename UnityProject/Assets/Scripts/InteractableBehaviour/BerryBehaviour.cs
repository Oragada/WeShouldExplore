using System.Collections.Generic;
using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;
using System.Linq;

public class BerryBehaviour : InteractableBehaviour {

    bool berriesTaken = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override CarryObject Activate(float playerProgress, Vector3 playerPos)
	{
        /*
        if (berriesTaken)//& playerProgress <= RealFlowerPick)
		{
			//fade = GetNewFadeDuration(playerProgress);
			return CarryObject.Nothing;
		}*/

        if (!berriesTaken)//&& playerProgress > RealFlowerPick)
		{
			//isActive = !isActive;
            berriesTaken = true;

		    RemoveBerries();

			return CarryObject.Berry;

			//rigidbody.isKinematic = !isActive;

			//transform.position = new Vector3(transform.position.x,transform.position.y+5.0f, transform.position.z);
		}

		return CarryObject.Nothing;
	}

    private void RemoveBerries()
    {
        List<Transform> list = gameObject.GetComponentsInChildren<Transform>().Where(e => e.gameObject.tag == "Berry").ToList();
        foreach (Transform go in list)
        {
            go.gameObject.SetActive(false);
        }
    }

    public override string customInteractiveText()
	{
		return "to eat the berries";
	}
}
