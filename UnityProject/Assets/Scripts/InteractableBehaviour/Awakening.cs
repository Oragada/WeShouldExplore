using UnityEngine;
using System.Linq;

namespace Assets.Scripts.InteractableBehaviour
{
    public class Awakening : MonoBehaviour {


        // Use this for initialization
        void Start () {
	
        }

        void Awake()
        {
            //GetComponentsInChildren<InteractBehaviour>().ToList().ForEach(c => c.customAwake());
        }

        // Update is called once per frame
        void Update () {
	
        }
    }
}
