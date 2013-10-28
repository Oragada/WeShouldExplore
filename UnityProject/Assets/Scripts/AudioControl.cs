using UnityEngine;
using System.Collections;

public class AudioControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bool toggleMusic = Input.GetButtonDown("ToggleMusic");
		if( toggleMusic )
		{
			AudioSource test = GetComponent<AudioSource>();
			if (test.audio.isPlaying) 
			{
				test.audio.Stop();
				Debug.Log("Stop Music");
			}
			else
			{
				test.audio.Play();
				Debug.Log("Play Music");
			}
		}
	}
}
