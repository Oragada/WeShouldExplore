using UnityEngine;
using System.Collections;

public enum Tutorials {move,sit,standup,interact};

public class TutorialGui : MonoBehaviour {
	// gui bools
	private bool tutMoveDone=false;
	private bool tutSitDone=false;
	private bool tutStandUpDone=false;
	private bool tutInteractDone=false;
	
	private const float DELAY_TIME=2.5f;
	private GameObject credits;
	
	void Awake()
	{
		transform.FindChild("tut_sit").gameObject.SetActive(false);
		transform.FindChild("tut_standup").gameObject.SetActive(false);
		transform.FindChild("tut_interact").gameObject.SetActive(false);
		transform.FindChild("tut_movement").gameObject.SetActive(true); // start with movement		
		credits = GameObject.Find("Credits");
		credits.SetActive(false);
	}
	public bool isMoveDone(){return tutMoveDone;}
	public bool isSitDone(){return tutSitDone;}	
	public bool isStandingUpDone(){return tutStandUpDone;}
	public bool isInteractDone(){return tutInteractDone;}
	
	public void fadeOutGuiElement(Tutorials inWhich)
	{	
		foreach (GUITexture child in GetComponentsInChildren<GUITexture>()) {
			if(child.name == "tut_movement" && inWhich==Tutorials.move)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 1.0f));
				child.GetComponent<Animation>().Play();		
				
				GameObject t = transform.FindChild("tut_interact").gameObject;
				t.SetActive(true);
				GUITexture nextChild = t.GetComponent<GUITexture>();
				{
					nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
					StartCoroutine(DelayFadeIn(nextChild,DELAY_TIME));					
				}
				tutMoveDone = true;
            }
            else if (child.name == "tut_interact" && inWhich == Tutorials.interact)
            {
                StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 3.0f));
                child.GetComponent<Animation>().Play();
                GameObject t = transform.FindChild("tut_sit").gameObject;
                t.SetActive(true);
                GUITexture nextChild = t.GetComponent<GUITexture>();
                {
                    nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    StartCoroutine(DelayFadeIn(nextChild,DELAY_TIME));
                }
                tutInteractDone = true;
            }
           	else if(child.name == "tut_sit"  && inWhich==Tutorials.sit)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 1.0f));
				child.GetComponent<Animation>().Play();
				GameObject t = transform.FindChild("tut_standup").gameObject;
                t.SetActive(true);
                GUITexture nextChild = t.GetComponent<GUITexture>();
                {
                    nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    StartCoroutine(DelayFadeIn(nextChild,DELAY_TIME));
                }
				tutSitDone=true;
			}
			else if(child.name == "tut_standup"  && inWhich==Tutorials.standup)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 1.0f));
				child.GetComponent<Animation>().Play();				
				tutStandUpDone=true;
			}
           
        }
		//StartCoroutine(Fade.use.Alpha(tut_movement, 1.0f, 0.0f, 3.0f));
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
