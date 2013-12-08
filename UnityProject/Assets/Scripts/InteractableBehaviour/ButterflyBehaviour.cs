using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InteractableBehaviour
{

    public class ButterflyBehaviour : ReactableBehaviour {

        // Use this for initialization
        void Start ()
        {
            Speed = 1.5f;
        }
	
        // Update is called once per frame
        void Update () {
            if (PlayerInRange)
            {
                switch (Behaviour)
                {
                    case AnimalBehaviour.Ignore:
                        break;
                    case AnimalBehaviour.Observe:
                    case AnimalBehaviour.Curious:
                        //GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(PlayerPos + transform.position));
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
        }

        public override void React(float playerProgress, Vector3 playerPos)
        {
            base.React(playerProgress,playerPos);

            if (playerProgress < 0.25)
            {
                Behaviour = AnimalBehaviour.Ignore;
            }
            else if(playerProgress < 0.75)
            {
                Behaviour = AnimalBehaviour.Curious;
            }
            else
            {
                Behaviour = AnimalBehaviour.Flee;
            }
        }
    }
}
