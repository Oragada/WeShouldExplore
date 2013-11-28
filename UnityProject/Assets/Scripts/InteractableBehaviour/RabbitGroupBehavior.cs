using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;
using System.Linq;

public enum RabbitBehaviour{Ignore, Observe, Move, Flee}

public class RabbitGroupBehavior : ReactableBehaviour
{
	public RabbitBehaviour Behaviour; //{ get; private set; }
    public Vector3 PlayerPos; // { get; set; }
	public bool PlayerInRange = false;
	public float Speed = 3.5f;
	//private Vector3 initialFace;
	public float CurrentSpeed = 0f;
	public float Decel = -0.05f;

	public RabbitGroupBehavior()
	{
		Behaviour = RabbitBehaviour.Ignore; 
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
				case RabbitBehaviour.Ignore:
					break;
				case RabbitBehaviour.Observe:
					GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(PlayerPos + transform.position));
					break;
				case RabbitBehaviour.Move:
				case RabbitBehaviour.Flee:
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
				case RabbitBehaviour.Ignore:
					break;
				case RabbitBehaviour.Observe:
					GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Bliss());
					break;
				case RabbitBehaviour.Move:
					if (CurrentSpeed > 0) { CurrentSpeed += Decel; }
					break;
				case RabbitBehaviour.Flee:
					break;
			}
		}

		Vector3 move = PlayerPos*-1;
		//move.y *= 0;

		move.Normalize();

		transform.position += (move * CurrentSpeed * Time.deltaTime);
	}

	public void Deactivate()
	{
		//GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Deactivate(Behaviour));
		PlayerInRange = false;

	}

    public override void React(float playerProgress, Vector3 playerPos)
    {
        PlayerInRange = true;
        PlayerPos = playerPos;

        if (playerProgress < 0.1f)
        {
            Behaviour = RabbitBehaviour.Ignore;
        }
        else if (playerProgress < 0.3f)
        {
            Behaviour = RabbitBehaviour.Observe;
        }
        else if (playerProgress < 0.7f)
        {
            Behaviour = RabbitBehaviour.Move;
        }
        else
        {
            Behaviour = RabbitBehaviour.Flee;
        }
    }
}
