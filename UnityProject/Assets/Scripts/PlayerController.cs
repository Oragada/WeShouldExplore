using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.InteractableBehaviour;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections;

//public enum for use in ActiveTile and WorldGeneration as well
public enum Direction {North,South,East,West,None};

public enum CarryObject {Nothing, Flower, Bouquet, Leaf}

public class PlayerController : MonoBehaviour {
	
	//publics
	public float speed;
	public float inertiaMultiplier=0.1f;
	public ActiveTile actTile;
	
	// gui elements
	public TutorialGui gui;
	// gui bools
	private bool tutMoveDone=false;
	private bool tutSitDone=false;
	private bool tutInteractDone=false;
	
	//privates
	private bool isSitting=false;
	private bool isInteracting=false;
	private int movementMode = 0;
	// interactive stuff
	private float progress=0.0f;
	private bool sit = false;
	private bool interact = false;
    private List<InteractBehaviour> inRangeElements;
    private const float THRESH_FOR_NO_COLLISION = 0.1f;
    private const float THRESH_FOR_INERTIA = 0.006f;
    private Vector3 lastMove= new Vector3(0.0f,0.0f,0.0f);

    //Carried object
    private CarryObject Obj { get; set; }
    private List<Transform> carryList;
	
	//get the collider component once, because the GetComponent-call is expansive
	void Awake()
	{
		inRangeElements = new List<InteractBehaviour>();

        //Obj = CarryObject.Nothing;

        carryList = GetComponentsInChildren<Transform>().Where(e => e.tag == "CarryObject").ToList();

        PickUpObject(CarryObject.Flower);
	}	
	
	void Update () 
	{
		// Cache the inputs.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
		// changed the Axis gravity&sensitivity to 1000, for more direct input.
		// for joystick usage however Vince told me to:
		/* just duplicate Horizontal and use gravity 1, dead 0.2 and sensitivity 1 that it works*/		
        sit = Input.GetButtonDown("Sit");
		interact = Input.GetButtonDown("Interact");
		
		checkProgress();
		
		if( sit )
		{
			if(!isSitting) 
			{
				// hide how to sit in the gui
				if(!tutSitDone)
				{
					gui.fadeOutGuiElement(Tutorials.sit);
					tutSitDone=true;
				}		
				//sit down
				gameObject.transform.localScale = new Vector3(1.0f,0.5f,1.0f);				
				gameObject.transform.Translate(new Vector3(0.0f,-0.25f,0.0f));
				isSitting = true;
				PlaySittingSound();
			}
			else
			{
				//stand up again
				gameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
				gameObject.transform.Translate(new Vector3(0.0f,0.25f,0.0f));
				isSitting = false;
			}
		}	
		
		if( interact )
        {
			if(!tutInteractDone)
			{
				gui.fadeOutGuiElement(Tutorials.interact);
				tutInteractDone=true;
			}
			if( inRangeElements.Count > 0)
			{
				CarryObject co = inRangeElements[0].activate(progress);
                PickUpObject(co);
			}
		}
		else
		{ 
			// show in the gui what will happen on the "E" button
			/*if( inRangeElements.Count > 0)
			{
				Debug.Log ( inRangeElements[0].getInteractiveText() );
			}*/
		}
				
        if( Input.GetButtonDown("ToggleMovementMode"))
		{	movementMode++;
			if( movementMode > 2)
				movementMode = 0;
		}
		
		if ( !isSitting)
		{			
			Movement(v,h);
		}	
	}
	
    public void PickUpObject(CarryObject pickedObject)
    {
        //Check combination
        CarryObject nObj = CombineObject(pickedObject);


        //Set CarryObject to new object
        Obj = nObj;

        SetCarryShow();
    }

    private CarryObject CombineObject(CarryObject newObject)
    {
        switch (Obj)
        {
            case CarryObject.Nothing:
            case CarryObject.Leaf:
                return newObject;
            case CarryObject.Bouquet:
            case CarryObject.Flower:
                switch (newObject)
                {
                    case CarryObject.Flower:
                        return CarryObject.Bouquet;
                    default:
                        return newObject;
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

    private void checkProgress()
    {
        rigidbody.isKinematic = progress <= THRESH_FOR_NO_COLLISION;
    }

    private void Movement(float v, float h)
	{
		bool moved=false;
		Vector3 move = new Vector3(0.0f,0.0f,0.0f);
		if( movementMode == 0) // DPAD mode
		{			
			if( v > 0.05f )
			{
				move.x = 0.1f;
				moved = true;
			}
			else if( v < -0.05f )
			{
				move.x = -0.1f;
				moved = true;
			}
			if( h < -0.05f )
			{
				move.z = 0.1f;
				moved = true;
			}
			if( h > 0.05f )
			{
				move.z = -0.1f;
				moved = true;
			}			
		}
		else if( movementMode == 1) // diagonal mode version one
		{			
			if( v > 0.05f )
			{
				move.x += 0.1f;
				move.z += 0.1f;moved = true;
			}
			else if( v < -0.05f )
			{
				move.x -= 0.1f;
				move.z -= 0.1f;moved = true;
			}
			if( h < -0.05f )
			{
				move.x -= 0.1f;
				move.z += 0.1f;moved = true;
			}
			else if( h > 0.05f )
			{
				move.x += 0.1f;
				move.z -= 0.1f;moved = true;
			}
		}
		else if( movementMode == 2) // diagonal mode 
		{
			
			if( v > 0.05f )
			{
				move.x = Mathf.Min(0.1f, move.x+0.1f);
				move.z = Mathf.Min(0.1f, move.z+0.1f);moved = true;
			}
			else if( v < -0.05f )
			{
				move.x =  Mathf.Max(-0.1f, move.x-0.1f);
				move.z =  Mathf.Max(-0.1f, move.z-0.1f);moved = true;					
			}
			if( h < -0.05f )
			{
				move.x =  Mathf.Max(-0.1f, move.x-0.1f);
				move.z = Mathf.Min(0.1f, move.z+0.1f);moved = true;
			}
			else if( h > 0.05f )
			{
				move.x = Mathf.Min(0.1f, move.x+0.1f);
				move.z =  Mathf.Max(-0.1f, move.z-0.1f);moved = true;					
			}
			// when moving diagnoal reduce walk speed by sqrt(2)
			if( move.x != 0.0f && move.z != 0.0f)
			{
				move.x /= Mathf.Sqrt( 2.0f );
				move.z /= Mathf.Sqrt( 2.0f );
			}
		}
		// apply the movement-vector to the player if he moved
		if(moved)			
		{
			gameObject.transform.Translate(move*Time.deltaTime*speed);
			lastMove = new Vector3( move.x,move.y,move.z ); // save last move in case the player moved
			
			// fade out movement tutorial
			if(!tutMoveDone)
			{
				gui.fadeOutGuiElement(Tutorials.move);
				tutMoveDone=true;
			}
		}
		else if(lastMove.magnitude > THRESH_FOR_INERTIA) // inertia
		{
			gameObject.transform.Translate(lastMove*Time.deltaTime*speed);
			//simply reduce inertia quadratic
			lastMove =  new Vector3( lastMove.x* inertiaMultiplier,lastMove.y* inertiaMultiplier,lastMove.z* inertiaMultiplier ); 					
			moved = true;
		}
		
		//set the players Y pos depending on the terrain
		// only if he was moved by player or inertia
		if( moved ) 
		{
			setPlayersYPosition();
		}
	}
	
    private void setPlayersYPosition()
	{
		float newYPos = gameObject.transform.position.y;
		try
		{
			//newYPos = actTile.returnPlayerPos(gameObject.transform.position.x,gameObject.transform.position.z);
			int a = 0;
		}
		catch(System.MissingMethodException e)
		{
			newYPos = gameObject.transform.position.y;
		}
		float diff = newYPos-gameObject.transform.position.y;
		if (Mathf.Abs(diff) > 0.0001f)
			gameObject.transform.Translate(new Vector3(0.0f,newYPos-gameObject.transform.position.y,0.0f));
	
	}

    void OnTriggerEnter (Collider other)
	{
		if( other.gameObject.tag == "NextTileTriggers")
		{
			//teleport to new position
			Direction dir=Direction.None;
			if( other.name == "WestTrigger")
			{
				dir = Direction.West;
				
				//gameObject.transform.Translate(new Vector3((-other.gameObject.transform.position.x*2)-2.0f,0.0f,0.0f),Space.World); // move to east MIKE old code
				
				Vector3 position = gameObject.transform.position;
				position.x = 18;
				gameObject.transform.position = position;
				
			}
			else if( other.name == "EastTrigger")
			{
				dir = Direction.East;
				
				//gameObject.transform.Translate(new Vector3(-(other.gameObject.transform.position.x*2)+2.0f,0.0f,0.0f),Space.World); // move to west 
				
				Vector3 position = gameObject.transform.position;
				position.x = 1;
				gameObject.transform.position = position;
				
			}
			else if( other.name == "NorthTrigger")
			{
				dir = Direction.North;
				
				//gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -(other.gameObject.transform.position.z*2)+2.0f),Space.World); // move to south 
				
				Vector3 position = gameObject.transform.position;
				position.z = 1;
				gameObject.transform.position = position;
			}
			else if( other.name == "SouthTrigger")
			{
				dir = Direction.South;
				//gameObject.transform.Translate(new Vector3(0.0f, 0.0f, (-other.gameObject.transform.position.z*2)-2.0f),Space.World); // move to south 
				
				Vector3 position = gameObject.transform.position;
				position.z = 18;
				gameObject.transform.position = position;
			}
			
			// update tile, pass the direction along
			actTile.showNextTile(dir);
			setPlayersYPosition();
		}
		if( other.gameObject.tag == "Interactable")
		{
			InteractBehaviour addThis = other.GetComponent<InteractBehaviour>();
			inRangeElements.Add(addThis);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if( other.gameObject.tag == "Interactable")
		{
			InteractBehaviour removeThis = other.GetComponent<InteractBehaviour>();
			inRangeElements.Remove(removeThis);
		}
	}

    void OnGUI()
    {
		const int x = 25;
		const int y = 315;

		//progressbar
        GUI.Label(new Rect(x,y,120,20), "Progress: 0"+ progress.ToString("#.##"));
		progress = GUI.HorizontalSlider(new Rect(x, y+20, 300, 10), progress, 0.00f, 0.99f);

		//speed slider
		GUI.Label(new Rect(x,y+40,120,20), "Speed: "+speed.ToString(CultureInfo.InvariantCulture));
        speed = GUI.HorizontalSlider(new Rect(x, y+60, 300, 10), speed, 0f, 500.0f);

		//inertia multiplier slider
		GUI.Label(new Rect(x,y+80,200,20), "InertiaMultiplier: "+inertiaMultiplier.ToString("#.###"));
        inertiaMultiplier = GUI.HorizontalSlider(new Rect(x, y+100, 300, 10), inertiaMultiplier, 0.8f, 1.0f);    
   
		//movement style slider
		GUI.Label(new Rect(x,y+120,150,20), "AlternateMoveStyle:");
		float test = GUI.HorizontalSlider(new Rect(x, y+140, 50, 10), movementMode, 0.0f, 2.0f);

        //in range elements count
        GUI.Label(new Rect(x, y + 160, 100, 20), "Debug:");
        //GUI.Label(new Rect(x, y + 180, 200, 20), inRangeElements[0].ToString());
        //GUI.Label(new Rect(x, y + 200, 200, 20), inRangeElements[1].ToString());
        //GUI.Label(new Rect(x, y + 180, 100, 20), Obj.ToString());
		
		if( test > 1.5f) 
			movementMode = 2;
		else if( test > 0.5f)
			movementMode = 1;
		else
			movementMode = 0;			
    }

	private void PlaySittingSound()
	{
		foreach ( AudioSource sound in GetComponentsInChildren<AudioSource>())
		{
			if ( sound.name == "SittingSound")
			{
				if (sound.audio != null && !sound.audio.isPlaying) 
					sound.audio.Play();				
			}
		}		
	}
}



