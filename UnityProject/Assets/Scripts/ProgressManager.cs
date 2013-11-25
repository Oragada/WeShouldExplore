﻿using UnityEngine;
using System.Collections;

public class ProgressManager : MonoBehaviour {
	
	public enum Mechanic {Sitting, Travel, Interaction}
	public enum Values {Alpha, Speed, InertiaDuration, InertiaDistance, GreyPlayerColor, BackgroundColorFactor, CollisionSizePercent}
	
	private float progress= 0.0f;	
	private float totalSittingTime = 0.0f; //100.0f for testing
	private uint nearInteractionCounter = 0; // 45 for testing
	private uint totalTilesTraveled = 0; // 45 for testing
	// Debug
	public float prog_offset = 0.0f;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void computeProgress()
	{
		//Debug.Log ( "progress: "+progress+" totalSittingTime:"+totalSittingTime+" nearInteractionCounter:"+nearInteractionCounter+" totalTilesTraveled:" + totalTilesTraveled+" offset:"+prog_offset);
		progress = ((Mathf.Sqrt( totalSittingTime * (float)(nearInteractionCounter)))/100.0f)+prog_offset;
		progress += Mathf.Log10( totalTilesTraveled + 1)*0.1f;
		progress = Mathf.Max(0.0f, Mathf.Min(1.01f, progress));		
	}
	public float getProgress()
	{
		return progress;
	}
	public void usedMechanic(Mechanic inMech, float inVal=0.0f)
	{
		switch(inMech)
		{
		    case Mechanic.Interaction:
		        nearInteractionCounter++;
		        break;
		    case Mechanic.Sitting:		        
				totalSittingTime  += inVal;
		        break;
			case Mechanic.Travel:
		        totalTilesTraveled++;
		        break;		    
		    default:
		        Debug.Log("unknown Mechanic");
		        break;
		}		
	}
	public float getValue(Values inVal)
	{
		switch(inVal)
		{
			case Values.Alpha:
		        return Mathf.Min(1.0f, 0.3f+progress*5.0f);
		        break;
		    case Values.Speed:		
			
				Vector2[] speedValues = {
					new Vector2(0.0f, 45.0f), //fast in the beginning
					new Vector2(0.25f, 30.0f), //constant in the middle
					new Vector2(0.75f, 30.0f), //constant in the middle
					new Vector2(1.0f, 20.0f) //slow in the end
				};
				return multipointInterpolation(speedValues,progress);
		        break;
			case Values.InertiaDuration:
		        return Mathf.Max(0.0f, 1.0f - progress*10.0f);
		        break;	
			case Values.InertiaDistance:
		        return Mathf.Max(0.0f, 0.1f - progress);
		        break;		    
			case Values.GreyPlayerColor:			
		        return Mathf.Min(1.0f, -0.4f*progress+0.5f);
		        break;	
			case Values.BackgroundColorFactor:
				return linearInterpolationBetween(1.0f,0.9f,progress);
				break;	
			case Values.CollisionSizePercent: // 0.25 = zero, 0.5 = one
				return linearInterpolationBetween(0.20f,0.5f,progress);
				break;	
		    default:
		        Debug.Log("unknown Value");
		        break;
		}		
		
		return progress;
	}
	public float multipointInterpolation(Vector2[] inVal, float inProgress) 
	{
		// find 2 closest points in the array
		Vector2 lower = inVal[0];
		Vector2 higher = inVal[inVal.Length-1];
		foreach( Vector2 p in inVal)
		{	
			if( p.x == inProgress)
			{
				return p.y;
			}		
			if( p.x > lower.x && p.x < inProgress)
			{
				lower = p;
			}
			if( p.x < higher.x && p.x > inProgress)
			{
				higher = p;
			}
		}
		float tmp = linearInterpolationBetween(lower.x, higher.x, inProgress);

		return lower.y*(1-tmp) + higher.y*(tmp);
	}
	public float linearInterpolationBetween( float zeroTill, float oneAt, float t)
	{
		return Mathf.Max(0.0f, Mathf.Min(1.0f,(t-zeroTill) / (oneAt-zeroTill)));
	}
}