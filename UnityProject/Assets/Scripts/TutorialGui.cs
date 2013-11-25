using UnityEngine;
using System.Collections;

public enum Tutorials {none,move,sit,standup,interact,follow};

public class TutorialGui : MonoBehaviour {
	public string[] TutorialStrings = {"",	"Press <b>ArrowKeys</b> to move.",
										  	"Press <b>Q</b> to sit.",
											"Press <b>Q</b> to stand up again.",
											"Press <b>E</b> to interact.",
											"Follow the <b>rabbits</b>."};
	// gui bools
	private bool tutMoveDone=false;
	private bool tutSitDone=false;
	private bool tutStandUpDone=false;
	private bool tutInteractDone=false;
	private bool tutFollowDone=true;// QUICK FIX to not show the last tutorial
	
	private const float DELAY_TIME=2.5f;
	private const float FADE_TIME=2.5f;
	private const float DARKOVERLAY_ALPHA=0.3f;
	private GameObject credits;
	private GUIText textOverlay;
	private GameObject darkOverlay;
	private GameObject currObj;
	private Tutorials currType;
	private bool shown = true;
	private bool fade = false;
	private float currentTime=0.0f;
	void Awake()
	{
		textOverlay = transform.FindChild("TextOverlay").gameObject.transform.FindChild("Text").guiText;
		darkOverlay = transform.FindChild("TextOverlay").gameObject.transform.FindChild("DarkOverlay").gameObject;
		fade = true;
		//textOverlay.SetActive(false);
		//Color test = new Color(0.0f,0.0f,0.0f,0.1f);
		//textOverlay.transform.FindChild("DarkOverlay").gameObject.renderer.material.color = test;
		resetTextOverlay("Press <b>E</b> to.");
		transform.FindChild("tut_sit").gameObject.SetActive(false);
		transform.FindChild("tut_standup").gameObject.SetActive(false);
		transform.FindChild("tut_interact").gameObject.SetActive(false);
		transform.FindChild("tut_follow").gameObject.SetActive(false);
		currObj = transform.FindChild("tut_movement").gameObject;
		currObj.SetActive(true); // start with movement		
		currType = Tutorials.move;
		
		
		credits = GameObject.Find("GUI").transform.FindChild("Credits").gameObject;
		credits.SetActive(false);
	}
	private void resetTextOverlay( string inStr )
	{
		textOverlay.text = inStr;
		GUIStyle style = new GUIStyle();
		style.font = textOverlay.font;
		
		Vector2 size = style.CalcSize(new GUIContent(textOverlay.text));
		Vector3 overlayScale = darkOverlay.transform.localScale;
		overlayScale.x  = size.x *0.00322f;
		darkOverlay.transform.localScale = overlayScale;
	}
	void Update()
	{
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
	}
	private void fadeOutOverlay()
	{
		float percent = (currentTime / FADE_TIME);
		if( percent > 1.0f)
		{
			percent = 1.0f;
			fade = false;
			shown = false;
		}
		// DARKOVERLAY FADE OUT
		Color newDoColor = new Color(0.0f,0.0f,0.0f,DARKOVERLAY_ALPHA*(1.0f-percent));
		darkOverlay.renderer.material.color = newDoColor;
		// TEXT FADE OUT
		Color newTextColor = new Color(1.0f,1.0f,1.0f,(1.0f-percent));
		textOverlay.color = newTextColor;

	}
	private void fadeInOverlay()
	{
		float percent = (currentTime / FADE_TIME);
		if( percent > 1.0f)
		{
			percent = 1.0f;
			fade = false;
			shown = true;
		}
		// DARKOVERLAY FADE OUT
		Color newDoColor = new Color(0.0f,0.0f,0.0f,DARKOVERLAY_ALPHA*(percent));
		Debug.Log( newDoColor.ToString());
		darkOverlay.renderer.material.color = newDoColor;
		// TEXT FADE OUT
		Color newTextColor = new Color(1.0f,1.0f,1.0f,percent);
		textOverlay.color = newTextColor;
	}
	public void doneMove(){ tutMoveDone=true; fadeOut(Tutorials.move);}
	public void doneSit(){ tutSitDone = true;fadeOut(Tutorials.sit); }
	public void doneStandingUp(){ tutStandUpDone = true; fadeOut(Tutorials.standup);}
	public void doneInteract(){ tutInteractDone = true; fadeOut(Tutorials.interact);}
	public void doneFollow()
	{ 
		tutFollowDone = true;
		GameObject.Find("GUI").transform.FindChild("Arrow").gameObject.SetActive(false); 
		fadeOut(Tutorials.follow);
	}
	
	public void fadeOut(Tutorials inWhich)
	{
		if( inWhich == currType)
		{
			if ( currObj != null)
			{
				StopAllCoroutines();
				if ( currObj.GetComponent<GUITexture>().color.a > 0.1f)
					StartCoroutine(Fade.use.Alpha(currObj.GetComponent<GUITexture>(), 1.0f, 0.0f, 1.0f));				
				currObj.GetComponent<Animation>().Play();		
				currObj = null;
				currType = Tutorials.none;
			}
			showNextTutorial();
		}
	}
	private void showNextTutorial()
	{
		if(!tutInteractDone)
		{
			GameObject t = transform.FindChild("tut_interact").gameObject;
			t.SetActive(true);
			GUITexture nextChild = t.GetComponent<GUITexture>();
			{
				nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
				StartCoroutine(DelayFadeIn(nextChild,DELAY_TIME));		
				currObj = t;
				currType = Tutorials.interact;
			}
		}
		else if(!tutSitDone)
		{
			GameObject t = transform.FindChild("tut_sit").gameObject;
			t.SetActive(true);
			GUITexture nextChild = t.GetComponent<GUITexture>();
			{
				nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
				StartCoroutine(DelayFadeIn(nextChild,DELAY_TIME));		
				currObj = t;
				currType = Tutorials.sit;
			}
		}
		else if(!tutStandUpDone)
		{
			GameObject t = transform.FindChild("tut_standup").gameObject;
			t.SetActive(true);
			GUITexture nextChild = t.GetComponent<GUITexture>();
			{
				nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
				StartCoroutine(DelayFadeIn(nextChild,DELAY_TIME));		
				currObj = t;
				currType = Tutorials.standup;
			}
		}
		else if(!tutFollowDone)
		{
			GameObject t = transform.FindChild("tut_follow").gameObject;
			t.SetActive(true);
			GUITexture nextChild = t.GetComponent<GUITexture>();
			{
				nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
				StartCoroutine(DelayFadeIn(nextChild,DELAY_TIME));		
				currObj = t;
				currType = Tutorials.follow;
			}
		}
	}
	public void showCredits()
	{		
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
