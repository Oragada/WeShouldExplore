using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;
using System.Linq;

public enum AnimalBehaviour{Ignore, Observe, Curious, Move, Flee}

public class RabbitGroupBehavior : ReactableBehaviour
{
	//public AnimalBehaviour Behaviour; //{ get; private set; }
	//private Vector3 initialFace;

    public RabbitGroupBehavior()
	{
	    //base();
	}


	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {

		//0 -> 1z, 0x
		//90 -> 0z, 1x
		//180 -> -1z, 0x
		//270 -> 0z, -1x
		//cos -> x
		//sin -> z

		if (PlayerInRange)
		{
			switch (Behaviour)
			{
				case AnimalBehaviour.Ignore:
					break;
				case AnimalBehaviour.Observe:
                case AnimalBehaviour.Curious:
					GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(PlayerPos + transform.position));
					break;
				case AnimalBehaviour.Move:
				case AnimalBehaviour.Flee:
                    GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(PlayerPos * -1 + transform.position));
					CurrentSpeed = Speed;
					break;
				default:
					break;
			}
		}
		else
		{
			switch (Behaviour)
			{
				case AnimalBehaviour.Ignore:
					break;
				case AnimalBehaviour.Observe:
                case AnimalBehaviour.Curious:
					GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Bliss());
					break;
				case AnimalBehaviour.Move:
					if (CurrentSpeed > 0) { CurrentSpeed += Decel; }
					break;
				case AnimalBehaviour.Flee:
					break;
			}
		}

		Vector3 move = PlayerPos*-1;
		//move.y *= 0;

		move.Normalize();

		transform.position += (move * CurrentSpeed * Time.deltaTime);
	}

    public override void React(float playerProgress, Vector3 playerPos)
    {
        base.React(playerProgress,playerPos);

        if (playerProgress < 0.1f)
        {
            Behaviour = AnimalBehaviour.Ignore;
        }
        else if (playerProgress < 0.25f)
        {
            Behaviour = AnimalBehaviour.Observe;
        }
        else if (playerProgress < 0.75f)
        {
            Behaviour = AnimalBehaviour.Curious;
        }
        else if (playerProgress < 0.9f)
        {
            Behaviour = AnimalBehaviour.Move;
        }
        else
        {
            Behaviour = AnimalBehaviour.Flee;
        }
    }
}
