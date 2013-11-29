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
    }

    public abstract class ReactableBehaviour : ActableBehaviour
    {
        public abstract void React(float playerProgress, Vector3 playerPos);
    }

    public abstract class InteractableBehaviour : ActableBehaviour
    {
        public abstract CarryObject Activate(float playerProgress, Vector3 playerPos);
        public abstract string customInteractiveText();
        
    }
}
