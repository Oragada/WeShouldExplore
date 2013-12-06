using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InteractableBehaviour
{
    public abstract class ActableBehaviour : MonoBehaviour {
        public float triggerRadius=1.0f;
	
        //public abstract void customAwake();
        //public abstract CarryObject activate(float playerProgress);

        protected Component GetGhildComponent(string Name)
        {
            return GetComponentsInChildren<Component>().First(c => c.name == Name);
            //return head;
        }

        private void setObjectYPosition()
        {
            float newYPos = transform.position.y;
            /*try
            {
                newYPos = gameObject.parent.groundTile.GetComponent<GroundGen>().returnPlayerPos(gameObject.transform.position.x, gameObject.transform.position.z);
            }
            catch (System.MissingMethodException e)
            {
                newYPos = gameObject.transform.position.y;
            }
            float diff = newYPos - gameObject.transform.position.y;
            if (Mathf.Abs(diff) > 0.0001f)
                gameObject.transform.Translate(new Vector3(0.0f, newYPos - gameObject.transform.position.y, 0.0f));
            */
        }
    }

    public abstract class ReactableBehaviour : ActableBehaviour
    {
        protected AnimalBehaviour Behaviour;
        public bool PlayerInRange = false;
        public Vector3 PlayerPos; // { get; set; }
        public float Speed = 3.5f;
        public float Decel = -0.1f;
        public float CurrentSpeed = 0f;

        protected ReactableBehaviour()
        {
            Behaviour = AnimalBehaviour.Ignore; 
        }

        public virtual void React(float playerProgress, Vector3 playerPos)
        {

            PlayerInRange = true;
            PlayerPos = playerPos;
        }

        public void Deactivate()
        {
            PlayerInRange = false;
        }
    }

    public abstract class InteractableBehaviour : ActableBehaviour
    {
        public abstract CarryObject Activate(float playerProgress, Vector3 playerPos);
        public abstract string customInteractiveText();
        
    }
}
