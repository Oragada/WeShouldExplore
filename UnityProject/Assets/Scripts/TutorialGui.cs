using UnityEngine;
using System.Collections;

public enum Tutorials {none,move,sit,standup,interact,follow};

public class TutorialGui : MonoBehaviour {
	// gui bools
	private bool tutMoveDone=false;
	private bool tutSitDone=false;
	private bool tutStandUpDone=false;
	private bool tutInteractDone=false;
	private bool tutFollowDone=true;// QUICK FIX to not show the last tutorial
	
	private const float DELAY_TIME=2.5f;
	private GameObject credits;
	
	private GameObject currObj;
	private Tutorials currType;
	void Awake()
	{
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
