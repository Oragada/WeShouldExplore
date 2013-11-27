using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Linq;
using System.Collections;
using Assets.Scripts;

//public enum for use in ActiveTile and WorldGeneration as well
public enum Direction {North,South,East,West,None,NorthEast,NorthWest,SouthEast,SouthWest};
public enum LayerList {Default,b,c,d,e,f,g,h,ignorePebbleCollision,withPebbleCollision};
public enum CarryObject {Nothing, Flower, Bouquet, Leaf}

public class PlayerController : MonoBehaviour {
	
	//publics
	public float speed;
	public ActiveTile actTile;
	public Material playerMat;
	//public GroundGen groundGen;
	
	// gui elements
	public TutorialGui gui;
	private ProgressManager progressMng;
	private GUIText interactionTooltip;
	
	//privates
	private GameObject groundTile;
	private bool isSitting=false;
	private int movementMode = -1;
	protected Animator animator;
	// interactive stuff
	private float progress=0.0f;
	private bool sit = false;
	private bool interact = false;
	private bool dead = false;
    private List<InteractBehaviour> inRangeElements;
    private const float THRESH_FOR_NO_COLLISION = 0.1f;
	private const float THRESH_FOR_PEBBLE_KICKING = 0.1f;
	private const float THRESH_FOR_TRUE_INTERACTION_TO_COUNT = 0.1f;
	private const float THRESH_FOR_SITTING_SOUND_FADEOUT = 5.0f; // in seconds
	private float currSittingTime = 0.0f; //100.0f for testing
	//Inertia
    private Direction lastDir = Direction.None;
	private float start = 0.0f;
	private float distance = 0.1f;
	private float duration = 1.0f;
	private float elapsedTime = 0.0f;
	// Meshes
	//private GameObject sittingPlayerMesh;
	//private GameObject standingPlayerMesh;
	//private GameObject deadPlayerMesh;
	public SphereCollider collisionHelper;
	private List<SphereCollider> collidingObj;
	//Sounds
	private AudioSource sittingSound;
	private AudioSource dyingSound;
	private float fadingSittingVolume;
    //Carried object
    private CarryObject Obj { get; set; }
    private List<Transform> carryList;
	private Camera isoCam;
	private Color background;
	// Debug
	private bool fadeOut=false;
	private float fadeOutFactor=0.2f;
	//get the collider component once, because the GetComponent-call is expansive
	void Awake()
	{
		//groundGen = this.GetComponent<GroundGen>;
		inRangeElements = new List<InteractBehaviour>();
		animator = transform.FindChild("animations").GetComponent<Animator>();
		groundTile = GameObject.Find("GroundTile");
		progressMng = (ProgressManager)GameObject.Find("Progression").GetComponent("ProgressManager");
		interactionTooltip = GameObject.Find("ContextSensitiveInteractionText").guiText;
		interactionTooltip.text = "";
        //Obj = CarryObject.Nothing;

        carryList = GetComponentsInChildren<Transform>().Where(e => e.tag == "CarryObject").ToList();
		
        PickUpObject(CarryObject.Nothing);
		// find sounds
		sittingSound = GameObject.Find("AudioSit").audio;
		dyingSound = GameObject.Find("AudioDeath").audio;
		// find camera
		isoCam = GameObject.Find("IsoCamera").camera;
		background = isoCam.backgroundColor;
		// set to no collisions with pebbles (via Layers)		
		SetLayerRecursively(gameObject,(int)(LayerList.ignorePebbleCollision)); // ignorePebbleCollision
		// colliding stuffs
        collisionHelper = transform.FindChild("ObstacleCollider").gameObject.GetComponent<SphereCollider>();
		collidingObj = new List<SphereCollider>();
	}	

	
	void Update () {
		if (dead)
			return;
		
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
				gui.doneSit();
				//sit down
				currSittingTime = 0.0f;
				animator.SetBool("sitting", true );
				//change the carrying position when sitting down
				transform.FindChild("CarryingPosition").gameObject.transform.Translate(0.0f,-0.15f,0.0f);
				isSitting = true;
				PlaySittingSound();
				elapsedTime = duration;
			}
			else
			{
				gui.doneStandingUp();
				//stand up again
				
				animator.SetBool("sitting", false );
				//change the carrying position when standing up again
				transform.FindChild("CarryingPosition").gameObject.transform.Translate(0.0f,+0.15f,0.0f);
				StopSittingSound(currSittingTime);
				isSitting = false;
				progressMng.usedMechanic(ProgressManager.Mechanic.Sitting, currSittingTime);
				currSittingTime = 0.0f;
			}
		}	
		
		if( interact )
        {
			InteractBehaviour closest = FindClosestInteractable();
			if( closest != null) 
	        {
				animator.SetBool("picking", true );

				CarryObject co = closest.activate(progress);
                PickUpObject(co);
				gui.doneInteract();
				if( progress >= THRESH_FOR_TRUE_INTERACTION_TO_COUNT)
					progressMng.usedMechanic( ProgressManager.Mechanic.Interaction );					
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
		else
		{
			currSittingTime += Time.deltaTime; // count seconds spend sitting;
		}
		FadeSounds(Time.deltaTime);		
		DisplayInteractionTooltip();		

	    BunnyCheck();
		animationHandling();
	}
	private void animationHandling()
	{
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
		if(stateInfo.nameHash == Animator.StringToHash("Base.Picking"))
		{
			animator.SetBool("picking", false );
		}
		else if(stateInfo.nameHash == Animator.StringToHash("Base.Kicking"))
		{
			animator.SetBool("kicking", false );
		}
		else if(stateInfo.nameHash == Animator.StringToHash("Base.Dying"))
		{
			animator.SetBool("dead", false);
		}
	}
	private void BunnyCheck()
    {
        foreach (RabbitGroupBehavior rab in inRangeElements.OfType<RabbitGroupBehavior>())
        {
            var rabbit = rab;
            Vector3 toPlayerVec = (transform.position - rabbit.transform.position);
            toPlayerVec.y *= 0;
            //toRabVec.Normalize();
            rabbit.PlayerPos = toPlayerVec;
            rabbit.activate(progress);
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
            //case CarryObject.Leaf:
                //return Obj;
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
	public static void SetLayerRecursively(GameObject go, int layerNumber)
	{
		foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
		{
			trans.gameObject.layer = layerNumber;
		}
	}
    private void checkProgress()
    {
		// compute new progress value:
		progressMng.computeProgress();
		progress = progressMng.getProgress();
		// set attributes accordingly
		float grey = progressMng.getValue(ProgressManager.Values.GreyPlayerColor);
		playerMat.color = new Color(grey,grey,grey,  progressMng.getValue(ProgressManager.Values.Alpha)); // transparency
		//rigidbody.isKinematic = progress <= THRESH_FOR_NO_COLLISION; // starts colliding
		speed =  progressMng.getValue(ProgressManager.Values.Speed); // reduced speed
		duration = progressMng.getValue(ProgressManager.Values.InertiaDuration); // reduced sliding
		distance = progressMng.getValue(ProgressManager.Values.InertiaDistance); // reduced sliding
		
		if (progress > THRESH_FOR_PEBBLE_KICKING)
		{			
			SetLayerRecursively(gameObject,(int)(LayerList.withPebbleCollision));
		}
		
		//start fading the background to black				
		float lower = progressMng.getValue(ProgressManager.Values.BackgroundColorFactor);
		isoCam.backgroundColor = new Color ( background.r*lower , background.g*lower, background.b*lower, 1.0f);
	
		// he dies at progress 1.0f
		if (progress > 1.0f && !dead)
		{
			Die();
		}
    }

    private void Movement(float v, float h)
	{
		Direction moved = Direction.None;
		if( movementMode == 0 || movementMode ==-1) // DPAD mode
		{			
			if( v > 0.05f )
				moved = Direction.North;
			else if( v < -0.05f )
				moved = Direction.South;
			
			if( h < -0.05f )
			{
				if(moved == Direction.North)
					moved = Direction.NorthWest;
				else if ( moved == Direction.South)
					moved = Direction.SouthWest;
				else
					moved = Direction.West;
			}
			else if( h > 0.05f )
			{			
				if(moved == Direction.North)
					moved = Direction.NorthEast;
				else if ( moved == Direction.South)
					moved = Direction.SouthEast;
				else
					moved = Direction.East;
			}
		}
		else if( movementMode == 1) // diagonal mode version
		{			
			if( v > 0.05f )
				moved = Direction.NorthWest;
			else if( v < -0.05f )
				moved = Direction.SouthEast;
			if( h < -0.05f )	
			{
				if(moved == Direction.NorthWest)
					moved = Direction.West;
				else if ( moved == Direction.SouthEast)
					moved = Direction.South;
				else
					moved = Direction.SouthWest;
			}
				
			else if( h > 0.05f )
			{
				if(moved == Direction.NorthWest)
					moved = Direction.North;
				else if ( moved == Direction.SouthEast)
					moved = Direction.East;
				else
					moved = Direction.NorthEast;
			}
		}
		else if( movementMode == 2) // diagonal mode 
		{			
			movementMode=-1;			
		}
		// apply the movement-vector to the player if he moved
		if(moved != Direction.None)			
		{		
			gui.doneMove();
			switch(moved)//rotation
			{
				case Direction.North: gameObject.transform.eulerAngles = new Vector3(0.0f,0.0f,0.0f);
					break;
				case Direction.East: gameObject.transform.eulerAngles = new Vector3(0.0f,90.0f,0.0f);
					break;
				case Direction.South: gameObject.transform.eulerAngles = new Vector3(0.0f,180.0f,0.0f);
					break;
				case Direction.West: gameObject.transform.eulerAngles = new Vector3(0.0f,270.0f,0.0f);
					break;
				case Direction.NorthEast: gameObject.transform.eulerAngles = new Vector3(0.0f,45.0f,0.0f);
					break;
				case Direction.NorthWest: gameObject.transform.eulerAngles = new Vector3(0.0f,315.0f,0.0f);
					break;
				case Direction.SouthEast: gameObject.transform.eulerAngles = new Vector3(0.0f,135.0f,0.0f);
					break;
				case Direction.SouthWest: gameObject.transform.eulerAngles = new Vector3(0.0f,225.0f,0.0f);
					break;
			}		
			lastDir = moved;			
			Vector3 dir = checkForCollisions(moved);			
			gameObject.transform.Translate(dir*Time.deltaTime*speed*0.1f); //move forward a step		
			elapsedTime = 0.0f;
		}
		else if(elapsedTime <= duration && progress < THRESH_FOR_NO_COLLISION) // inertia
		{
			Interpolate.Function test = Interpolate.Ease(Interpolate.EaseType.EaseOutSine);
			float incVal = test(start, distance,elapsedTime, duration);
			//Debug.Log ("val:"+incVal+" s:"+start + " d:"+distance+" elT:"+elapsedTime+" dur:"+duration);			
			gameObject.transform.Translate(new Vector3( distance-incVal,0.0f,0.0f)*Time.deltaTime*speed);
			elapsedTime += Time.deltaTime;			
			moved = lastDir;
		}
		
		collidingObj.Clear();
		//set the players Y pos depending on the terrain
		// only if he was moved by player or inertia
		if( moved != Direction.None) 
		{
			setPlayersYPosition();
		}
	}
	private Vector3 checkForCollisions(Direction moved)
	{
		Vector3 ret = new Vector3(1.0f,0.0f,0.0f);
		float collSizePercent = progressMng.getValue(ProgressManager.Values.CollisionSizePercent);
		if (collSizePercent == 0.0f) // do not compute collisions when the CollisionSizePercent is zero
			return ret;
		foreach( SphereCollider enemy in collidingObj)
		{
			//compute collision-vector between this-SphereCollider and the other-SphereCollider
			Vector3 dif = collisionHelper.transform.position - enemy.transform.position;
			// ignore Y-difference
			dif.y = 0.0f;
			if(((collisionHelper.radius + enemy.radius)*collSizePercent) > dif.magnitude)
			{
				// convert the collision-vector to local space, because player rotates in the movementfunction
				dif = transform.InverseTransformDirection(dif);	
	    		// offset the player frame & speed independent 
				// *0.7f makes sure that diagonal movement should be save ( 1 / sqrt(2) )
				ret.Set((dif.x)/(Time.deltaTime*speed*1.0f), ret.y,(dif.z)/(Time.deltaTime*speed*1.0f));
			}
		}			
		return ret;
	}	
    private void setPlayersYPosition()
	{
		float newYPos = gameObject.transform.position.y;
		try
		{
			newYPos = groundTile.GetComponent<GroundGen>().returnPlayerPos(gameObject.transform.position.x,gameObject.transform.position.z);
		}
		catch(System.MissingMethodException e)
		{
			newYPos = gameObject.transform.position.y;
		}
		float diff = newYPos-gameObject.transform.position.y;
		if (Mathf.Abs(diff) > 0.0001f)
			gameObject.transform.Translate(new Vector3(0.0f,newYPos-gameObject.transform.position.y+0.585f,0.0f));
	
	}	
	public void channeledTriggerStay (Collider other)
	{
		if( other.name == "CollisionCollider")
		{			
			SphereCollider enemy = other.GetComponent<SphereCollider>();
			if (enemy != null)
			{
				collidingObj.Add( enemy );				
			}
		}
	}
    public void channeledTriggerEnter (Collider other)
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
				position.z = 1;
				gameObject.transform.position = position;
                inRangeElements.Clear();
				
			}
			else if( other.name == "EastTrigger")
			{
				dir = Direction.East;
				
				//gameObject.transform.Translate(new Vector3(-(other.gameObject.transform.position.x*2)+2.0f,0.0f,0.0f),Space.World); // move to west 
				
				Vector3 position = gameObject.transform.position;
				position.z = 18;
				gameObject.transform.position = position;
                inRangeElements.Clear();
				
			}
			else if( other.name == "NorthTrigger")
			{
				dir = Direction.North;
				
				//gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -(other.gameObject.transform.position.z*2)+2.0f),Space.World); // move to south 
				
				Vector3 position = gameObject.transform.position;
				position.x = 1;
				gameObject.transform.position = position;
                inRangeElements.Clear();
			}
			else if( other.name == "SouthTrigger")
			{
				dir = Direction.South;
				//gameObject.transform.Translate(new Vector3(0.0f, 0.0f, (-other.gameObject.transform.position.z*2)-2.0f),Space.World); // move to south 
				
				Vector3 position = gameObject.transform.position;
				position.x = 18;
				gameObject.transform.position = position;
                inRangeElements.Clear();
			}
			
			// update tile, pass the direction along
			groundTile.GetComponent<GroundGen>().showNextTile(dir);
			gui.doneFollow();
			progressMng.usedMechanic(ProgressManager.Mechanic.Travel);
			//groundTile.GetComponent<GroundGen>().
			setPlayersYPosition();
		}
		if( other.gameObject.tag == "Interactable")
		{
			Transform colli = other.transform.FindChild("CollisionCollider");
			if( colli != null)
			{
				SphereCollider enemy = colli.GetComponent<SphereCollider>();
				if (enemy != null)
					collidingObj.Add( colli.GetComponent<SphereCollider>() );				
			}
			
			InteractBehaviour addThis = other.GetComponent<InteractBehaviour>();

            /*if (addThis.GetType() == typeof (RabbitGroupBehavior))
            {
                RabbitGroupBehavior rabbit = (RabbitGroupBehavior) addThis;
                Vector3 toRabVec = (rabbit.transform.position - transform.position);
                toRabVec.y *= 0;
                toRabVec.Normalize();
                rabbit.runDirection = toRabVec;
                rabbit.activate(progress);
            }*/

			inRangeElements.Add(addThis);
			
			if( progress < THRESH_FOR_TRUE_INTERACTION_TO_COUNT)
			{
				progressMng.usedMechanic( ProgressManager.Mechanic.Interaction );
			}
		}
		else if( other.name == "CollisionCollider")
		{			
			SphereCollider enemy = other.GetComponent<SphereCollider>();
			if (enemy != null)
			{
				collidingObj.Add( enemy );
			}
		}
	}
	private InteractBehaviour FindClosestInteractable()
	{
		if( inRangeElements.Count < 1)
			return null;
		InteractBehaviour ret = inRangeElements[0];
		float dist = Vector3.Distance(transform.position, inRangeElements[0].transform.position);
		if( inRangeElements.Count == 1)
			return ret;
		foreach (InteractBehaviour i in inRangeElements)
        {
			if( dist > Vector3.Distance(transform.position, i.transform.position))
			{
				dist = Vector3.Distance(transform.position, i.transform.position);
				ret = i;
			}
		}
		return ret;
	}
	private void DisplayInteractionTooltip()
	{
		if ( dead )
			return;
		
		interactionTooltip.text = "";
		InteractBehaviour closest = FindClosestInteractable();
		if( closest != null) 
        {
			if (closest.customInteractiveText() != null)
				interactionTooltip.text = "Press <b>E</b> "+closest.customInteractiveText();
		}				
	}
	public void channeledTriggerExit(Collider other)
	{
		if( other.gameObject.tag == "Interactable")
		{
            InteractBehaviour removeThis = other.GetComponent<InteractBehaviour>();
            if (removeThis.GetType() == typeof(RabbitGroupBehavior))
            {
                ((RabbitGroupBehavior)removeThis).Deactivate();
            }
			inRangeElements.Remove(removeThis);

		}		
	}

    void OnGUI()
    {
		const int x = 25;
		const int y = 300;
        if (movementMode != -1)
        {
            //progressbar
            GUI.Label(new Rect(x, y, 120, 20), "Progress: " + progressMng.getProgress().ToString("#.##"));
            progressMng.prog_offset = GUI.HorizontalSlider(new Rect(x, y + 20, 300, 10),progressMng.prog_offset, -1.01f, 1.01f);

            //speed slider
            GUI.Label(new Rect(x, y + 40, 120, 20), "Speed: " + speed.ToString(CultureInfo.InvariantCulture));
            speed = GUI.HorizontalSlider(new Rect(x, y + 60, 300, 10), speed, 0f, 500.0f);

            //Inertia Duration slider
            GUI.Label(new Rect(x, y + 80, 200, 20), "InertiaDuration: " + duration.ToString("#.###"));
            duration = GUI.HorizontalSlider(new Rect(x, y + 100, 300, 10), duration, 0.0f, 2.0f);
			
			//Inertia Distance slider
			GUI.Label(new Rect(x, y + 120, 200, 20), "InertiaDistance: " + distance.ToString("#.###"));
            distance = GUI.HorizontalSlider(new Rect(x, y + 140, 300, 10), distance, 0.0f, 1.0f);

            //movement style slider
            GUI.Label(new Rect(x, y + 160, 150, 20), "AlternateMoveStyle:");
            float test = GUI.HorizontalSlider(new Rect(x, y + 180, 50, 10), movementMode, 0.0f, 2.0f);

            //in range elements count
            GUI.Label(new Rect(x, y + 220, 100, 20), "Debug:");
            //GUI.Label(new Rect(x, y + 180, 200, 20), inRangeElements[0].ToString());
            //GUI.Label(new Rect(x, y + 200, 200, 20), inRangeElements.OfType<RabbitGroupBehavior>().Count().ToString(CultureInfo.InvariantCulture));
            GUI.Label(new Rect(x, y + 240, 100, 20), Obj.ToString());

            if (test > 1.5f)
                movementMode = 2;
            else if (test > 0.5f)
                movementMode = 1;
            else
                movementMode = 0;
        }
    }
	private void Die()
	{
		dead = true;
		// change mesh to lying 
		Debug.Log ("die only once.");
		animator.SetBool("dead", true );
		// start Death sounds
		PlayDeathSound();
		// clean the interaction Tooltip text
		interactionTooltip.text = "You died.";	
		// show the credits
		gui.showCredits();
	}
	private void FadeSounds(float timeDelta)
	{
		if(sittingSound.audio.isPlaying)
		{
			if (!fadeOut) // fade In
			{
				if ( fadingSittingVolume < 1.0f)
					fadingSittingVolume += timeDelta*0.2f;
				sittingSound.audio.volume = fadingSittingVolume;			
			}
			else // fade Out
			{
				if ( fadingSittingVolume > 0.01f)
					fadingSittingVolume -= timeDelta*fadeOutFactor;
				else 
				{
					sittingSound.audio.Stop();
					fadeOut = false;
				}
				sittingSound.audio.volume = fadingSittingVolume;							
			}
		}
	}
	private void PlaySittingSound()
	{
		sittingSound.audio.time = sittingSound.audio.clip.length*Random.value; // starts at random pos in the track
		sittingSound.audio.Play();
		fadingSittingVolume = 0.0f;
	}
	private void StopSittingSound(float inTimeSpentSitting)
	{		
		if ( inTimeSpentSitting < THRESH_FOR_SITTING_SOUND_FADEOUT)
			fadeOutFactor = 100.0f;
		else
			fadeOutFactor = Mathf.Lerp(0.5f, 0.03f, Mathf.Min (1.0f, (inTimeSpentSitting-THRESH_FOR_SITTING_SOUND_FADEOUT)/20.0f));
		fadingSittingVolume = 1.0f;
		sittingSound.audio.volume = fadingSittingVolume;
		fadeOut = true;
	}
	private void PlayDeathSound()
	{
		StopSittingSound(0.0f);
		dyingSound.Play();
	}
}



