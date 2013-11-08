using UnityEngine;
using System.Collections;

public enum Tutorials {move,sit,interact};


public class TutorialGui : MonoBehaviour {
	// gui bools
	private bool tutMoveDone=false;
	private bool tutSitDone=false;
	private bool tutInteractDone=false;
	
	void Awake()
	{
		transform.FindChild("tut_sit").gameObject.SetActive(false);
		transform.FindChild("tut_interact").gameObject.SetActive(false);
		transform.FindChild("tut_movement").gameObject.SetActive(true); // start with movement		
	}
	public bool isMoveDone(){return tutMoveDone;}
	public bool isSitDone(){return tutSitDone;}
	public bool isInteractDone(){return tutInteractDone;}
	
	public void fadeOutGuiElement(Tutorials inWhich)
	{	
		foreach (GUITexture child in GetComponentsInChildren<GUITexture>()) {
			if(child.name == "tut_movement" && inWhich==Tutorials.move)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 1.0f));
				child.GetComponent<Animation>().Play();		
				
				GameObject t = transform.FindChild("tut_sit").gameObject;
				t.SetActive(true);
				GUITexture nextChild = t.GetComponent<GUITexture>();
				{
					nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
					StartCoroutine(Fade.use.Alpha(nextChild, 0.0f, 1.0f, 3.0f));
				}
				tutMoveDone = true;
			}
           	else if(child.name == "tut_sit"  && inWhich==Tutorials.sit)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 1.0f));
				child.GetComponent<Animation>().Play();
				GameObject t = transform.FindChild("tut_interact").gameObject;
				t.SetActive(true);
				GUITexture nextChild = t.GetComponent<GUITexture>();
				{
					nextChild.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
					StartCoroutine(Fade.use.Alpha(nextChild, 0.0f, 1.0f, 3.0f));
				}
				tutSitDone=true;
			}
           	else if(child.name == "tut_interact"  && inWhich==Tutorials.interact)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 3.0f));
				child.GetComponent<Animation>().Play();
				tutInteractDone = true;
			}
           
        }
		//StartCoroutine(Fade.use.Alpha(tut_movement, 1.0f, 0.0f, 3.0f));
	}
	
}
