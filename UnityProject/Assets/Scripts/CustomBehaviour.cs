using UnityEngine;
using System.Collections;

public abstract class CustomBehaviour : MonoBehaviour {
	public float triggerRadius=1.0f;
	
	public abstract void customAwake();
	public abstract void activate(float playerProgress);
	public abstract string customInteractiveText();
}
