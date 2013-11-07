using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;
using System.Linq;

public class RabbitGroupBehavior : InteractBehaviour
{

    public float runDirection { get; set; }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override CarryObject activate(float playerProgress)
    {
        gameObject.GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Activate(runDirection));

        return CarryObject.Nothing;
    }

    public override string customInteractiveText()
    {
        return "Rabbits";
    }
}
