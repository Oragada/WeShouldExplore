using UnityEngine;
using System.Linq;

namespace Assets.Scripts.InteractableBehaviour
{
    public class FlowerBehaviour : InteractBehaviour {

        private bool isActive = false;

        bool flowerPicked;

        readonly Color[] colors = new []{new Color(1,0,0), new Color(0,0,1) };

        void Awake()
        {
            Component head = GetGhildComponent("Head");
            head.renderer.material.color = colors[0];
            //SphereCollider trigger = GetComponent<SphereCollider>();
            //trigger.radius = triggerRadius;
            //rigidbody.isKinematic = true;
        }

        public override CarryObject activate(float playerProgress)
        {
            if (!flowerPicked && playerProgress > 0.1f)
            {
                //isActive = !isActive;
                flowerPicked = true;

                Component head = GetGhildComponent("Head");

                head.gameObject.SetActive(false);

                return CarryObject.Flower;

                //rigidbody.isKinematic = !isActive;

                //transform.position = new Vector3(transform.position.x,transform.position.y+5.0f, transform.position.z);
            }

            return CarryObject.Nothing;
        }

        public override string customInteractiveText()
        {
            //Press E
            return "to pick the Flower.";
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
