using UnityEngine;
using System.Collections;

public enum Tutorials {none,move,sit,standup,interact,follow,firstTile,secondTile,endTutorial};

public class TutorialGui : MonoBehaviour {
	private string[] TutorialDescriptions = {"","Press <b>W,A,S,D</b> to move.",
		"Press <b>Q</b> to sit.",
		"Press <b>Q</b> to stand up again.",
		"Press <b>E</b> to interact.",
		"Follow the <b>rabbits</b>.",
		"The world is <b>big</b> – go explore.",
		"Discover the world <b>and</b> yourself.",
		"Walk a way."};
	// gui bools
	private bool tutMoveDone=false;
	private bool tutSitDone=false;
	private bool tutStandUpDone=false;
	private bool tutInteractDone=false;
	private bool tutFollowDone=true;// QUICK FIX to not show the last tutorial
	private bool tutFirstTileDone=false;//
	private bool tutSecondFirstTileDone=false;//
	private bool tutDone=false;//

	public bool firstTileDone(){return tutFirstTileDone;}

	private const float DELAY_TIME=2.5f;
	private const float FADE_TIME=1.0f;
	private const float DARKOVERLAY_ALPHA=0.15f;
	private GameObject credits;
	private GUIText textOverlay;
	private GUITexture darkOverlay;
	private GameObject walkaway;
	private GameObject currObj;
	private bool shown = true;
	private bool fade = false;
	private float currentTime=0.0f;
	private float fadeOutEndTutorial=5.0f;
	private bool shouldShowCredits = false;

	private Tutorials nextTut = Tutorials.none;
	private Tutorials currTut = Tutorials.move;
	void Awake()
	{		
		textOverlay = transform.FindChild("TextOverlay").gameObject.transform.FindChild("Text").guiText;
		darkOverlay = transform.FindChild("TextOverlay").gameObject.transform.FindChild("DarkOverlay").guiTexture;
		credits = transform.FindChild("Credits").gameObject;
		walkaway = transform.FindChild("walkaway").gameObject;
		Restart();
	}
	private void resetTextOverlay( string inStr )
	{
		textOverlay.text = inStr;
		GUIStyle style = new GUIStyle();
		style.font = textOverlay.font;
		style.fontSize = 24;

		Vector2 size = style.CalcSize(new GUIContent(textOverlay.text));
		Rect newPixelInset = new Rect( -(size.x+9)/2, darkOverlay.pixelInset.y,size.x+10,darkOverlay.pixelInset.height);
		darkOverlay.pixelInset = newPixelInset;

	}
	void Update()
	{
		Debug.Log ( currTut.ToString()+ " "+nextTut.ToString()+ " " + fade.ToString() +" "+ tutFirstTileDone.ToString() );
		if( fade )
		{
			currentTime += Time.deltaTime;
			if (shown) // fade out
			{
				fadeOutOverlay();
			}
			else if( !shown) // fade in
			{
				fadeInOverlay();
			}
		}
		if( !fade && !shown && (nextTut != Tutorials.none))
		{
			currentTime += Time.deltaTime;
			if( currentTime > DELAY_TIME)
			{
				//Prepare for the next tutorial
				chooseNextTutorial();
				resetTextOverlay( TutorialDescriptions[(int)nextTut] );
				fade = true;
				currentTime = 0.0f;
			}
		}
		if ( currTut == Tutorials.endTutorial)
		{
			fadeOutEndTutorial-=Time.deltaTime;
			if ( fadeOutEndTutorial < 0.0f)
			{
				fadeOutWalkaway();
			}
		}
	}
	public void Restart()
	{
		tutMoveDone=false;
		tutSitDone=false;
		tutStandUpDone=false;
		tutInteractDone=false;
		tutFollowDone=true;// QUICK FIX to not show the last tutorial
		tutFirstTileDone=false;//
		tutSecondFirstTileDone=false;//
		tutDone=false;//
		
		shown = true;
		fade = false;
		currentTime=0.0f;
		fadeOutEndTutorial=5.0f;
		
		nextTut = Tutorials.none;
		currTut = Tutorials.move;
		credits.SetActive(false);
		walkaway.SetActive(false);
		resetTextOverlay(TutorialDescriptions[(int)currTut]);
	}

	private void fadeOutOverlay()
	{
		float percent = (currentTime / FADE_TIME);
		if( percent > 1.0f)
		{
			percent = 1.0f;
			currentTime=0.0f;
			fade = false;
			shown = false;			
			currTut = Tutorials.none;
		}
		// DARKOVERLAY FADE OUT
		Color newDoColor = new Color(0.0f,0.0f,0.0f,DARKOVERLAY_ALPHA*(1.0f-percent));
		darkOverlay.color = newDoColor;
		// TEXT FADE OUT
		Color newTextColor = new Color(1.0f,1.0f,1.0f,(1.0f-percent));
		textOverlay.color = newTextColor;

	}
	private void fadeInOverlay()
	{
		if( nextTut == Tutorials.none)
			return;
		if( nextTut == Tutorials.endTutorial )
		{
			currTut = Tutorials.endTutorial;
			nextTut = Tutorials.none;
		}
		if((nextTut == Tutorials.interact && tutInteractDone) // break off fade in if it changes during the fade in
			||(nextTut == Tutorials.sit && tutSitDone) 
			|| (nextTut == Tutorials.standup && tutStandUpDone))
		{
			shown = true;
			currentTime = 0.0f;
			return;
		}
		float percent = (currentTime / FADE_TIME);
		if( percent > 1.0f)
		{
			percent = 1.0f;
			currentTime=0.0f;
			fade = false;
			shown = true;
			currTut = nextTut;
			nextTut = Tutorials.none;
		}
		// DARKOVERLAY FADE OUT
		Color newDoColor = new Color(0.0f,0.0f,0.0f,DARKOVERLAY_ALPHA*(percent));
		darkOverlay.color = newDoColor;
		// TEXT FADE OUT
		Color newTextColor = new Color(1.0f,1.0f,1.0f,percent);
		textOverlay.color = newTextColor;
	}
	public void doneMove()		{ tutMoveDone=true; 	 if(currTut==Tutorials.move){ fade = true;nextTut=Tutorials.firstTile;}}
	public void doneFirstTile()	{ if(currTut==Tutorials.firstTile) { tutFirstTileDone = true; fade = true;nextTut=Tutorials.interact;}}	
	public void doneInteract()	{ tutInteractDone = true; if(currTut==Tutorials.interact) { fade = true;nextTut=Tutorials.secondTile;}}
	public void doneSecondTile(){ if(currTut==Tutorials.secondTile) { tutSecondFirstTileDone = true; fade = true;nextTut=Tutorials.sit;}}
	public void doneSit()		{ tutSitDone = true; 	 if(currTut==Tutorials.sit) { fade = true;nextTut=Tutorials.standup;}}
	public void doneStandingUp(){ tutStandUpDone = true; if(currTut==Tutorials.standup) { fade = true;nextTut=Tutorials.endTutorial;}}
	public void doneTutorial(){ tutDone = true; if(currTut==Tutorials.endTutorial) { fade = true;nextTut=Tutorials.none;}}
	public void doneFollow()	{ tutFollowDone = true;
									GameObject.Find("GUI").transform.FindChild("Arrow").gameObject.SetActive(false); 
								  if(currTut==Tutorials.follow){ fade = true;nextTut=Tutorials.sit;}}


	private void chooseNextTutorial()
	{
		if(!tutFirstTileDone)
		{
			nextTut = Tutorials.firstTile;
			fade = true;
			return;
		}
		else if(!tutInteractDone)
		{
			nextTut = Tutorials.interact;
			fade = true;
			return;
		}
		else if(!tutSecondFirstTileDone)
		{
			nextTut = Tutorials.secondTile;
			fade = true;
			return;
		}
		else if(!tutSitDone)
		{
			nextTut = Tutorials.sit;
			fade = true;
			return;
		}
		else if(!tutStandUpDone)
		{
			nextTut = Tutorials.standup;
			fade = true;
			return;
		}
		else if(!tutFollowDone)
		{
			nextTut = Tutorials.follow;
			fade = true;
			return;
		}
		else if(!tutDone)
		{
			//nextTut = Tutorials.endTutorial;
			//fade = true;
			//return;
			
			nextTut = Tutorials.endTutorial;
			fadeInWalkaway();
			return;
		}
		else
		{
			nextTut = Tutorials.none;
			fade = true;
		}

	}
	private void fadeOutWalkaway()
	{
		walkaway.SetActive(true);
		walkaway.guiTexture.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		StartCoroutine(Fade.use.Alpha(walkaway.guiTexture, 1.0f, 0.0f, 3.0f));		
		currTut = Tutorials.none;
	}
	private void fadeInWalkaway()
	{		
		walkaway.SetActive(true);
		walkaway.guiTexture.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		StartCoroutine(Fade.use.Alpha(walkaway.guiTexture, 0.0f, 1.0f, 3.0f));		
	}
	public void temporarilyShowCredits()
	{
		if(shouldShowCredits)
			credits.SetActive(true);
	}
	public void temporarilyHideCredits()
	{
		if(shouldShowCredits)
			credits.SetActive(false);
	}
	public void showCredits()
	{		
		shouldShowCredits = true;
		credits.SetActive(true);
		credits.guiTexture.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		StartCoroutine(DelayFadeIn(credits.guiTexture,1.0f));
	}
	
	IEnumerator DelayFadeIn(GUITexture fadein, float delay)
	{		
		yield return new WaitForSeconds(delay);
		if( fadein != null)
			StartCoroutine(Fade.use.Alpha(fadein, 0.0f, 1.0f, 3.0f));		
    }
	
}
