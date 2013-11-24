using UnityEngine;
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
				return Mathf.Max(20.0f, 45.0f - (progress*40.0f));
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
				return Mathf.Min(1.0f,(1.0f - (progress))/0.1f);
				break;	
			case Values.CollisionSizePercent: // 0.25 = zero, 0.5 = one
				return Mathf.Max(0.0f, Mathf.Min(1.0f, (progress-0.25f)/0.25f));
				break;	
		    default:
		        Debug.Log("unknown Value");
		        break;
		}		
		
		return progress;
	}
}
