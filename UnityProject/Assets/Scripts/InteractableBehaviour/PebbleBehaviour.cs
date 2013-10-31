using UnityEngine;

namespace Assets.Scripts.InteractableBehaviour
{
    public class PebbleBehaviour : InteractBehaviour {	
	
        private bool isActive=false;
        private Component pebble;
	
        public override void customAwake()
        {
            //SphereCollider trigger = GetComponent<SphereCollider>();
            //trigger.radius = triggerRadius;
            pebble = GetGhildComponent("PebbleObject");
            pebble.rigidbody.isKinematic = true;
        }

        public override void activate(float playerProgress)
        {
            pebble.rigidbody.isKinematic = !isActive;
            isActive = !isActive;
		
            //transform.position = new Vector3(transform.position.x,transform.position.y+5.0f, transform.position.z);
        }

        public override string customInteractiveText()
        {
            //Press E
            return "to pick up the pebble.";
        }
	
        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
