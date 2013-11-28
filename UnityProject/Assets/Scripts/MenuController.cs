using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour {

	private bool shown = false;
	private GameObject elements = null;
	private GUITexture darkBgr = null;
	private GUIText controlText = null;
	private List<GUIText> selectables;
	private TutorialGui tutorial;

	private int selected = 0;
	// Use this for initialization
	void Start () {
		tutorial = (TutorialGui)GameObject.Find("GUI").GetComponent("TutorialGui");
		
		elements = this.transform.FindChild("MenuElements").gameObject;
		darkBgr = elements.transform.FindChild("DarkBgr").guiTexture;
		selectables = new List<GUIText>();
		selectables.Add(elements.transform.FindChild("Continue").guiText);
		selectables.Add(elements.transform.FindChild("Controls").guiText);
		selectables.Add(elements.transform.FindChild("Restart Tutorial").guiText);
		selectables.Add(elements.transform.FindChild("Quit").guiText);
		controlText = elements.transform.FindChild("ControlsShortExplain").guiText;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			shown = !shown;
		}

		elements.SetActive(shown);
		if( shown )
		{
			Time.timeScale = 0.0f; // pause the game
			scaleBgr();	//scale the Background to full Screen

			//change selection on KeyPresses
			if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
			{
				changeSelected(1);
			}
			else if (Input.GetKeyDown(KeyCode.UpArrow)|| Input.GetKeyDown(KeyCode.W))
			{
				changeSelected(-1);
			}
			controlText.gameObject.SetActive ( selected==1?true:false );

			// reset all Highlights on GUITexts
			foreach( GUIText tmp in selectables)
			{
				tmp.fontStyle = FontStyle.Normal;
			}
			// Highlight selected GUIText
			selectables[selected].fontStyle = FontStyle.Bold;
			// On Enter
			if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				menuAction();
			}
			//Debug.Log ( "selected"+selected);
		}
		else
			Time.timeScale = 1.0f; // continue the game
	}
	private void menuAction()
	{
		switch(selected)
		{
			case 0: // Continue
				shown = false;
				break;
			case 1: // Controls
				shown = false;
				controlText.gameObject.SetActive ( true );
				break;
			case 2: // Restart tutorial
				tutorial.Restart();
				shown = false;
				break;
			case 3: // Exit
				Application.Quit();
				break;
		}

	}
	private void changeSelected(int inVal)
	{
		selected += inVal;
		if ( selected < 0)
		{
			selected = selectables.Count-1;
		}
		else if ( selected >= selectables.Count)
		{
			selected = 0;
		}
	}
	private void scaleBgr()
	{
		Rect newPixelInset = new Rect( -(Screen.width)/2, -(Screen.height)/2,Screen.width,Screen.height);
		darkBgr.pixelInset = newPixelInset;
	}
}
