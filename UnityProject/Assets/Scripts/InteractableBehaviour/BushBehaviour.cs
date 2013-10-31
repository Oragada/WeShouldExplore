using UnityEngine;

namespace Assets.Scripts.InteractableBehaviour
{
    public class BushBehaviour : InteractBehaviour {

        private Color inactiveColor;
        private Color activeColor;
        private bool isActive;
	
        public override void customAwake()
        {
            inactiveColor = new Color(0.0f, 96.0f/255.0f, 5.0f/255.0f,1.0f);
            activeColor = new Color(90.0f/255.0f, 122.0f/255.0f, 96.0f/255.0f,1.0f);
            isActive = false;
			
            SphereCollider trigger = GetComponentInChildren<SphereCollider>();
            trigger.radius = triggerRadius;
        }
        public override CarryObject activate(float playerProgress)
        {
            isActive = !isActive;
            Component bush = GetGhildComponent("BushObject");
            bush.renderer.material.color = isActive ? activeColor : inactiveColor;

            return CarryObject.Leaf;
        }

        public override  string customInteractiveText()
        {
            //Press E
            return "to pick a berry from the bush.";
        }

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
