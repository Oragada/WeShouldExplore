using UnityEngine;

namespace Assets.Scripts.InteractableBehaviour
{
    public class PebbleBehaviour : InteractableBehaviour
    {
        public int ForceMultiplier = 2000;
        //public float HeightLevel = 20.0f;

        //private bool isActive=false;
        private Component pebble;
	
        void Awake()
        {
            //SphereCollider trigger = GetComponent<SphereCollider>();
            //trigger.radius = triggerRadius;

            //pebble = GetGhildComponent("PebbleObject");
            //pebble.rigidbody.isKinematic = true;
        }

        public override CarryObject Activate(float playerProgress, Vector3 playerPos)
        {
            //pebble.rigidbody.isKinematic = !isActive;
            //isActive = !isActive;

            PerformKick(playerProgress, playerPos);
		
            //transform.position = new Vector3(transform.position.x,transform.position.y+5.0f, transform.position.z);

            return CarryObject.Nothing;
        }

        private void PerformKick(float playerProgress, Vector3 playerPos)
        {
            playerPos.Normalize();
            //playerPos.y += HeightLevel;
            transform.position += (new Vector3(0,0.1f,0));
            transform.rigidbody.AddForce(-playerPos*ForceMultiplier);
        }

        public override string customInteractiveText()
        {
            //Press E
            return "to kick the pebble.";
        }
	
        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
