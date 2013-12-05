using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;
using Debug = System.Diagnostics.Debug;

public class CarryElements : MonoBehaviour {

    //Carried object
    //public CarryObject Obj { get; set; }
    internal List<Transform> carryList;
    public float fadeCarry;
    public Transform TBouquet;
    private bool firstAfter = true;
    public Transform FlowerHead;
    private int currentRot;
    private const int ADD_ROT = 135;

	// Use this for initialization
    void Awake()
    {
        carryList = new List<Transform>();
        //carryList = GetComponentsInChildren<Transform>().Where(e => e.tag == "CarryObject").ToList();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void UpdateCarry(float progress)
    {
        if (progress < FlowerBehaviour.RealFlowerPick)
        {
            FadeCarryUpdate();
            firstAfter = true;
        }
        else if (firstAfter)
        {
            List<Transform> flowers = carryList.ToList();
            flowers.ForEach(e => ChangeTransparancy(0.5f, e));
            firstAfter = false;
        }
    }

    private void FadeCarryUpdate()
    {
        if (fadeCarry > 0)
        {
            List<Transform> flowers = carryList.ToList();
            flowers.ForEach(e => ChangeTransparancy(fadeCarry, e));
            fadeCarry -= Time.deltaTime;
        }

        if (fadeCarry <= 0)
        {
            fadeCarry = 0;
            ClearFlowers();
        }
    }

    private void ChangeTransparancy(float fadeRemain, Transform carryObject)
    {
        Color extColor = carryObject.renderer.material.color;
        carryObject.renderer.material.color = new Color(extColor.r, extColor.g, extColor.b, (fadeRemain / 5));
    }

    private float GetNewFadeCarryDuration(float progress)
    {
        //0.0 => 2 sec
        //0.3 => 5 sec
        return 2 + (progress * 10);
    }
    
    /*
    public void PickUpObject(CarryObject pickedObject, float progress)
    {
        switch (pickedObject)
        {
            case CarryObject.Flower:
                AddFlower();
                else
                {

                }
                break;
            case CarryObject.Clear:
                //Remove existing flowers
                //Set carry to nothing
                break;
            case CarryObject.Nothing:
                //Do nothing?
                break;


        }
        {

        }

        //SetCarryShow();

        
        if (progress <= FlowerBehaviour.RealFlowerPick & pickedObject == CarryObject.Flower)
        {
            
            Obj = pickedObject;
        }
        else if (progress > FlowerBehaviour.RealFlowerPick || pickedObject != CarryObject.Flower)//&&(pickedObject == CarryObject.Nothing && Obj == CarryObject.Flower))
        {
            //pickedObject == CarryObject.Nothing
            //ThrowBouquet();
            //Check combination
            CarryObject nObj = CombineObject(pickedObject);

            //Set CarryObject to new object
            Obj = nObj;
        }
        //else
        //{
        //    //Extreme jury-rigging
        //    CarryObject nObj = CombineObject(pickedObject);
        //    //Set CarryObject to new object
        //    Obj = nObj;
        //}
        //Fade out flower before 0.3


        
    }

    private CarryObject CombineObject(CarryObject newObject)
    {
        switch (Obj)
        {
            case CarryObject.Clear:
                return CarryObject.Nothing;
            case CarryObject.Nothing:
                return newObject;
            case CarryObject.Bouquet:
            case CarryObject.Flower:
                switch (newObject)
                {
                    case CarryObject.Flower:
                        return CarryObject.Bouquet;
                    default:
                        return Obj;
                }
            default:
                return CarryObject.Nothing;
        }
    }

    private void SetCarryShow()
    {
        string ObjName;

        switch (Obj)
        {
            case CarryObject.Nothing:
                ObjName = "Nothing";
                break;
            case CarryObject.Leaf:
                ObjName = "Leaf";
                break;
            case CarryObject.Flower:
                ObjName = "SingleFlower";
                break;
            case CarryObject.Bouquet:
                ObjName = "Bouquet";
                break;
            default:
                ObjName = "Nothing";
                break;
        }

        carryList.ForEach(e => e.gameObject.SetActive(false));

        Transform newRend = carryList.FirstOrDefault(e => e.name == ObjName);

        newRend.gameObject.SetActive(true);
    }
    */

    public void PickFlower(float progress)
    {
        if (progress <= FlowerBehaviour.RealFlowerPick)
        {
            fadeCarry = GetNewFadeCarryDuration(progress);
        }

        Quaternion rota = new Quaternion();
        Transform flower = Instantiate(FlowerHead, transform.position, rota) as Transform;
        Debug.Assert(flower != null, "flower != null");
        flower.parent = transform;

        //flower.rotation = Quaternion.LookRotation(new Vector3(0,1,0));
        //flower.rotation *= Quaternion.Euler(0, GetNextAngle(), 0);
        //flower.rotation *= Quaternion.Euler(75, 0, 0);

        //Vector3 dirVec = new Vector3(flower.rotation.x, flower.rotation.y, flower.rotation.z);
        //dirVec.Normalize();
        //flower.position += dirVec * 0.2f;

        carryList.Add(flower);
    }

    private float GetNextAngle()
    {
        currentRot += ADD_ROT;
        return currentRot;
    }

    private void ClearFlowers()
    {
        Transform[] arr = carryList.ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            Destroy(arr[i].gameObject);
        }
        carryList.Clear();
        currentRot = 0;
    }

    public void ThrowBouquet()
    {
        ClearFlowers();
        Instantiate(TBouquet, transform.position, new Quaternion());
    }

}
