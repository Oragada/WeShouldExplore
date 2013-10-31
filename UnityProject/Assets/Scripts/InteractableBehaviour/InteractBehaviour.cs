using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InteractableBehaviour
{
    public abstract class InteractBehaviour : MonoBehaviour {
        public float triggerRadius=1.0f;
	
        public abstract void customAwake();
        public abstract CarryObject activate(float playerProgress);
        public abstract string customInteractiveText();


        protected Component GetGhildComponent(string Name)
        {
            return GetComponentsInChildren<Component>().First(c => c.name == Name);
            //return head;
        }
    }
}
