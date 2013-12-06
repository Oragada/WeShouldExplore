using System.Globalization;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.InteractableBehaviour
{
    public class ShroomBehaviour : InteractableBehaviour {

        public static float RealFlowerPick = 0.3f;
        //private bool isActive = false;
        public float fade = 0f;
        bool flowerPicked;
        private Component head;

        readonly Color[] colors = new []{new Color(1,0,0), new Color(0,0,1) };

        void Awake()
        {
            head = GetGhildComponent("Head");
            //head.renderer.material.color = colors[0];
            //head.renderer.material.shader = Shader.Find("Transparent/Diffuse");
            //SphereCollider trigger = GetComponent<SphereCollider>();
            //trigger.radius = triggerRadius;
            //rigidbody.isKinematic = true;
        }

        public override CarryObject Activate(float playerProgress, Vector3 playerPos)
        {
            if(!flowerPicked & playerProgress <= RealFlowerPick)
            {
                fade = GetNewFadeDuration(playerProgress);
                return CarryObject.Flower;
            }

            if (!flowerPicked && playerProgress > RealFlowerPick)
            {
                //isActive = !isActive;
                flowerPicked = true;


                head.gameObject.SetActive(false);

                return CarryObject.Flower;

                //rigidbody.isKinematic = !isActive;

                //transform.position = new Vector3(transform.position.x,transform.position.y+5.0f, transform.position.z);
            }

            return CarryObject.Nothing;
        }

        private float GetNewFadeDuration(float playerProgress)
        {
            //0.0 => 2 sec
            //0.3 => 5 sec
            return 2 + (playerProgress*10);
        }

        void changeTransparancy(float fadeRemain)
        {
            Color extColor = head.renderer.material.color;
            head.renderer.material.color = new Color(extColor.r, extColor.g, extColor.b,(1 - (fadeRemain/5)));
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
            changeTransparancy(fade);
            if (fade > 0)
            {
                fade -= Time.deltaTime;
            }
            else
            {
                fade = 0;
            }
        }

        void OnGUI()
        {
            //GUI.Label(new Rect(100, y + 240, 200, 20), inRangeElements.Count.ToString(CultureInfo.InvariantCulture));
        }
    }
}
