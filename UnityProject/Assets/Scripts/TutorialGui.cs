using UnityEngine;
using System.Collections;

public enum Tutorials {move,sit,interact};


public class TutorialGui : MonoBehaviour {

	void Awake()
	{
		
	}
	
	public void fadeOutGuiElement(Tutorials inWhich)
	{	
		foreach (GUITexture child in GetComponentsInChildren<GUITexture>()) {
			if(child.name == "tut_movement" && inWhich==Tutorials.move)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 3.0f));
				child.GetComponent<Animation>().Play();
			}
           	else if(child.name == "tut_interact"  && inWhich==Tutorials.interact)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 3.0f));
				child.GetComponent<Animation>().Play();
			}
           	else if(child.name == "tut_sit"  && inWhich==Tutorials.sit)
			{
				StartCoroutine(Fade.use.Alpha(child, 1.0f, 0.0f, 3.0f));
				//Debug.Log(child.GetComponent<Animation>().ToString());
				child.GetComponent<Animation>().Play();
			}
           
        }
		//StartCoroutine(Fade.use.Alpha(tut_movement, 1.0f, 0.0f, 3.0f));
	}
	
}
