//#pragma strict

var depthGenR: GameObject;																	//PUBLIC VARIABLES
var depthGenL: GameObject;

var ground : Mesh;
var groundCol : MeshCollider;
//var frictionMap: PhysicMaterial;
var width : int;
var height : int;

var xOff : int;	
var yOff: int;
var zOff: int;

@Range (0.0, 1.0)
var frequency = 0.044; 																		//Noise frequency 0.2?, 0.107 orig //0.044 more normal

@Range (-1.0, 10.0)
var scale = 5.0; 																			// 1.0 ?, 3.5 //3.5 more normal

@Range (0.0, 1.0)
var frequencyDetail = 0.309;

@Range (-1.0, 10.0)
var scaleDetail = 0.72;																		//Noise scale


var flower : GameObject;																	//Tile Offsets
var shroom : GameObject;
var bush : GameObject;
var pebble : GameObject;
var rabbits : GameObject;
var butterflies : GameObject;
var grass : GameObject;
var grass1 : GameObject;
var grass2 : GameObject;
var tree: GameObject;

var memorySize = 1;

var groundMat : Material;

																				
var tileChangeFlag;																			//PRIVATE VARIABLES
var currentScat = new Array();
var lastScat = new Array();

var bordersID;
var emptyID;
var treesID;
var areolasID;
var bushesID;
var flowersID;
var animalsID;
var pebblesID;
var grassID;
var shroomsID;
var underTreeID;

var renderedInstances = new Array();

var inMemoryFlag;

var currentScatID;

var tileStack = new Array();

var scatterStack = new Array();



function Start() {																			//-----Start-----//

	xOff = 0;
	yOff = 0;
	zOff = 0;
	
	tileStack[0] = "0x0";
	
	bordersID = -1;
	emptyID = 0;
	treesID = 1;
	areolasID = 2;
	bushesID = 3;
	flowersID = 4;
	shroomsID = 5;
	animalsID = 6;
	pebblesID = 7;
	grassID = 8;
	shroomsID = 9;
	underTreeID = 10;

	
	tileChangeFlag = 0;
	inMemoryFlag = 0;

	GenerateGround();
	ChangeTerrain();
	
	currentScat = ProbabilityScatter();	
	
	scatterStack[0] = currentScat;
	
	ScatterRenderer(currentScat);
	
	
}

function Update() {																			//-----Update-----//

	
	if (tileChangeFlag==1) {
	
		tileChangeFlag=0;	
	
		for(var k=0;k<renderedInstances.length;k++) {
			var toKill = renderedInstances[k];
			Destroy(toKill);
		}			
															
		ChangeTerrain();
		
		CheckMemory();																		// This is in the update function because the previous tile collider gets destroyed no sooner than the next update. In other words: right about now	
		
											
		
		scatterStack.Add(currentScat);														//The current scat gets always saved to the stacked
																														
	}
}



function CheckMemory() {

	inMemoryFlag = 0;

	var xs = xOff/19;
	var zs = zOff/19;
	
	var x = xs.ToString();
	var z = zs.ToString();
	
	currentScatID = String.Format("{0}{1}{2}", x, "x", z);
	
	for (var i = 0; i<tileStack.length; i++) {
	
		if( tileStack[i] == currentScatID) {												//If it's an old one, get it from scatterStack and render it;
						
			currentScat = scatterStack[i];
			inMemoryFlag = 1;
			
		}	
	}
	
	if(tileStack.length<memorySize) {												
		
		tileStack.Add(currentScatID);
		
	} else {
		
		tileStack.Shift();
		
		tileStack.Add(currentScatID);
	}
	
	if (inMemoryFlag == 1) {																//If this tile is in the memory, fish out the scat and
		ScatterRenderer(currentScat);
		inMemoryFlag = 0;
	} else {
		currentScat = ProbabilityScatter();
		ScatterRenderer(currentScat);
	}	
}


function showNextTile(dir) {																//-----ShowNextTile-----//

	switch(dir) {
		case 0: 																				//North
			xOff += 19; 
		break;
		case 1: 																				//South
			xOff -= 19;
		break;
		case 2:																					//East
			zOff -= 19;																		
		break;
		case 3:																					//West
			zOff += 19;																	
		break;
	}	
	
	Destroy(GetComponent(MeshCollider));														//Destroy the mesh collider of the current tile before generating a new one
	
	tileChangeFlag = 1;
	
}

																					
																					

function GenerateGround() {																	//-----GenerateGround-----//

	gameObject.AddComponent(MeshFilter);
	gameObject.AddComponent(MeshRenderer);
	
	renderer.material = groundMat;
	
	ground = GetComponent(MeshFilter).mesh;
	
	var vertices = new Vector3[width*height];
	
	var uvs = new Vector2[vertices.Length];
	
// 	var tangents = new Vector4[vertices.Length];
	
	var triCount = (width-1)*(height-1)*2;

	var triangles = new int[triCount*3];
	
	
	var i = 0;																					//Generate vertices
	for(var y=0;y<height;y++) {
		for(var x=0;x<width;x++) {
		
			vertices[i++] = Vector3(x,0,y);
			
		}
	}
	
	
	i = 0;																						//Generate triangles
	for(var yt=0;yt<height-1;yt++) {
		for(var xt=0;xt<width-1;xt++) {
		
			triangles[i++] = ((yt + 1) * width) + xt;
			triangles[i++] = (yt * width) + xt+1;
			triangles[i++] = (yt * width) + xt;
			
			triangles[i++] = ((yt+1)* width) + xt+1;
			triangles[i++] = (yt * width) + xt+1;
			triangles[i++] = ((yt+1)*width)+xt;
		}
	}
	
	
	for(var u=0; u < uvs.Length; u++) {															//Generate uvs

		uvs[u] = Vector2(vertices[u].x, vertices[u].z);
	
	}
	
	
	
	ground.vertices = vertices;																	//Assign vertices, triangles and uvs to the ground mesh
	ground.triangles = triangles;
	ground.uv = uvs;
	
	ground.RecalculateNormals();
	
																								//tangents
																								//ground.tangents = tangents;
																								//TangentSolver(ground);
	

	transform.localScale.x = 20/(width*1.0);													//Scale the mesh so it's the same size on the screen no matter what BREAKS OTHER SHIT IF NOT 20x20 RIGHT NOW
	transform.localScale.z = 20/(height*1.0);
	transform.localScale.y = transform.localScale.x;
	

}


function returnPlayerPos(x,z) {																//-----returnPlayerPos-----//

	return returnGroundY(x,z)+0.2;
	
}

function returnGroundY(x,z) {																//-----ReturnGroundY-----//
	
// 	return Mathf.PerlinNoise(z*frequency+(zOff*frequency), 										//Single step Perlin noise for the ground
// 							 x*frequency+(xOff*frequency))*scale;

	
	var base = Mathf.PerlinNoise(z*frequency+(zOff*frequency), 									//base Perlin noise for the ground
	 							 x*frequency+(xOff*frequency))*scale;
	
	var detail = Mathf.PerlinNoise(z*frequencyDetail+(zOff*frequencyDetail), 					//detailing Perlin noise for the ground
	    						   x*frequencyDetail+(xOff*frequencyDetail))*scaleDetail;
	    						   
	return base+detail; 							

}

function returnCurvature(x,z) {																//-----ReturnCurvature-----//

	return Mathf.PerlinNoise(z*frequency+(zOff*frequency), 										//base Perlin noise for the ground
	 				  x*frequency+(xOff*frequency))*scale;
	 				  
}

function ChangeTerrain() {																	//-----ChangeTerrain-----//

	
	if (renderedInstances.length>0) {															//Clear renderedInstances for the new tile

		
	}

	ground = GetComponent(MeshFilter).mesh;														//transfer height map data on mesh
	
	var vertices = ground.vertices;
	
	var index = 0;
	
	for(var i=0;i<width;i++) {
		for(var j=0;j<height;j++) {	
			vertices[index].y = returnGroundY(j,i);		
			index++;
		}
	}
	
	ground.vertices = vertices;
	ground.RecalculateNormals();
	
	
	gameObject.AddComponent(MeshCollider);														//Generate collision mesh for new tile
	
	depthGenR.GetComponent(DepthGenR).Generate();												//Make depth planes
	depthGenL.GetComponent(DepthGenL).Generate();


}


function ProbabilityScatter() {																//-------ProbabilityScatter-------//

	var toRender = new Array();
	
	var treeProb = 0.5;																			//Probabilities
	var bushProb = 0.1;
	var grassProb = 0.3;
	var pebbleProb = 0.1;
	var flowerProb = 0.6;
	
	var animalProb = 0.005;
	
	var shroomProb = 0.7;
	var shroomTreeProb = 0.01;
	var shroomCounter = 0;
	
	var index = 0;																				//Fill borders with -1 so nothing can spawn there
	for (var c=0; c < height; c++) {
		for (var f=0; f < width; f++) {

			if (c == 0 || c == height-1) {
				toRender[index] = -1;
			} else if (f == 0 || f == width-1) {
				toRender[index] = -1;
			} else {
			toRender[index] = 0;
			}
			index++;
		}
	}
	
	var i;
	
	for (i=0; i < toRender.length; i++) {														//SCATTER TREES//
		
		if (toRender[i]==0) {
			
			var changeFrequency = 0.003;
			var changeScale = 1.0;
			var probChange = Mathf.PerlinNoise((zOff/*19*/)*changeFrequency+(zOff*changeFrequency), 									//base Perlin noise for the ground
	 							 			   (xOff/*19*/)*changeFrequency+(xOff*changeFrequency)) * changeScale;
	 							 		
			
			var combinedProb = treeProb*probChange;
				
			if (Random.Range(0.0,1.0)<combinedProb) {
			
				TreeAreola(i, toRender, probChange);											//Tree Areola
				toRender[i] = treesID;														
				
				
			}
		}
	}
	
	i = 0;
	
	
	for (i=0; i < toRender.length; i++) {														//SCATTER SHROOMS//
		
		if (toRender[i]==underTreeID) {

			if (shroomCounter == 0) {
				if (Random.Range(0.0,1.0)<shroomTreeProb) {
					shroomCounter=8;
				}
			} else if(shroomCounter>0) {
				if (Random.Range(0.0,1.0)<shroomProb) {
					toRender[i] = shroomsID;
					shroomCounter--;													
				}	
			}
		}
	}
	
	
	for (i=0; i < toRender.length; i++) {														//SCATTER BUSHES//
		
		
		if (toRender[i]==emptyID) {																	//if on empty land
				
			if (Random.Range(0.0,1.0)<bushProb) {
			
				toRender[i] = bushesID;
				
			}
		
		} else if (toRender[i]==areolasID) {															//if under tree, probability just 10% of the normal	
		
			if (Random.Range(0.0,1.0)<bushProb*0.1) {
			
				toRender[i] = bushesID;
				
			}
		}
	}
	
	
	for (i=0; i < toRender.length; i++) {														//SCATTER GRASS//
		
		if (toRender[i]==emptyID || toRender[i]==areolasID) {
				
			if (Random.Range(0.0,1.0)<grassProb) {
			
				toRender[i] = grassID;													
				
				
			}
		}
	}
	
	
	for (i=0; i < toRender.length; i++) {														//SCATTER ANIMALS//
		
		if (toRender[i]==areolasID) {
				
			if (Random.Range(0.0,1.0)<animalProb) {
			
				toRender[i] = animalsID;														
				
			}
		}
	}
	
	
	
	for (i=0; i < toRender.length; i++) {														//SCATTER PEBBLES//
		
		if (toRender[i]==emptyID || toRender[i]==areolasID) {
				
			if (Random.Range(0.0,1.0)<pebbleProb) {
			
				toRender[i] = pebblesID;	
				
				
			}
		}
	}
	
	for (i=0; i < toRender.length; i++) {														//SCATTER FLOWERS//
		
		if (toRender[i]==emptyID || toRender[i]==areolasID) {
				
			if (Random.Range(0.0,1.0)<flowerProb) {
			
				toRender[i] = flowersID;													
				
				
			}
		}
	}
	
	
	return toRender;
}

																							

function ScatterRenderer(toRender) {														//----ScatterRenderer----//

	for (var i=0;i<toRender.length;i++) {
	
		var x = (i % width) + Random.Range(-0.2,0.2);
		var z = Mathf.FloorToInt(i/width) + Random.Range(-0.2,0.2);
		var y = returnGroundY(x,z);
		
		var temPos = Vector3(x, y, z);
	
		switch (toRender[i]) {
		
			case emptyID:
			
			break;
			
			case treesID:																	//render TREES//
			
				temPos.y+=0.2;
			
				renderedInstances[i] = Instantiate(tree, temPos, Quaternion.identity);
				
				renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);
			
			break;
			
			
			case areolasID:																	//render TREE AREOLAS//
			
				//renderedInstances[i] = Instantiate(flower, temPos, Quaternion.identity);		//instantiation
				
				//renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
			break;
			
			case shroomsID:																	//render SHROOMS//
			
				renderedInstances[i] = Instantiate(shroom, temPos, Quaternion.identity);		//instantiation
				
				renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
			break;
			
			
			case bushesID:																	//render BUSHES//
				
				
				temPos.y+=0.0; 																	//vertical offset
				
				renderedInstances[i] = Instantiate(bush, temPos, Quaternion.identity); 			//instantiation
				
				renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
			break;
			
			case flowersID:																	//render FLOWERS//
				
				
				temPos.y+=0.0; 																	//vertical offset
				
				renderedInstances[i] = Instantiate(flower, temPos, Quaternion.identity); 			//instantiation
				
				renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
			break;
			
			case grassID:																	//render GRASS//
				
				
				temPos.y+=0.5; 																	//vertical offset
				
				if (Random.Range(0.0,1.0)<0.3) {
				
					renderedInstances[i] = Instantiate(grass, temPos, Quaternion.identity);						
				
				} else if (Random.Range(0.0,1.0)<0.6) {
					renderedInstances[i] = Instantiate(grass1, temPos, Quaternion.identity);
				} else {
					renderedInstances[i] = Instantiate(grass2, temPos, Quaternion.identity);
				}
				

				renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
			break;
			
			case animalsID:																	//render ANIMALS//
				
				
				//temPos.y+=0.5; 
				
				if (Random.Range(0.0,1.0)<0.3) {
				
					renderedInstances[i] = Instantiate(rabbits, temPos, Quaternion.identity);
				
				} else {
					renderedInstances[i] = Instantiate(butterflies, temPos, Quaternion.identity);
				}
				 		//instantiation
				
				renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
			break;
			
			case pebblesID:																	//render PEBBLES//
				
				temPos.y+=0.2; 																	//vertical offset
				
				renderedInstances[i] = Instantiate(pebble, temPos, Quaternion.identity); 		//instantiation
				
				renderedInstances[i].rigidbody.Sleep();	
															
				
				renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
				
// 				var randScale = Random.Range(0.8, 1.2);
// 				renderedInstances[i].transform.localScale.x = randScale;
// 				renderedInstances[i].transform.localScale.y = randScale;
// 				renderedInstances[i].transform.localScale.z = randScale;
				
			break;
		}
	}
}






function TreeAreola(i, toRender, probChange) {															//----TreeAreola----//

	var areolaSize;
	
	if (probChange>0.6) {				
		areolaSize = 2;
	} else if (probChange>0.5) {
		areolaSize = 3;
	} else if (probChange>0.4) {
		areolaSize = 4;
	} else if (probChange>0.3) {
		areolaSize = 5;
	} else {
		areolaSize = 6;
	}
	
	/*
	if (probChange>0.4) {
		areolaSize = 2;
	} else if (probChange>0.35) {
		areolaSize = 3;
	} else if (probChange>0.3) {
		areolaSize = 4;
	} else if (probChange>0.25) {
		areolaSize = 5;
	} else {
		areolaSize = 6;
	}
	*/
	
	for (var h = areolaSize*-1; h <= areolaSize; h++) {
		for(var w = areolaSize*-1; w <= areolaSize; w++) {
			
			var index = i+w+(width*h);
			
			if (index > 0 && index < toRender.length-1) {									//array bounds check
				if (Mathf.Abs(w)<=1 && Mathf.Abs(h)<=1) {									//if in shroom area
					toRender[index]	= toRender[index]==bordersID ? bordersID : underTreeID; //if not the borders, generate shrooms
				} else {																	//if not in shroom area
					
					if (toRender[index] == emptyID) {										//if empty, make areola
						toRender[index] = areolasID;
					}
				}
			}	
		}
	}
}












/*/ALL SCATTERING GLORY

function Scatter(ap, prefab, offsetY, quantityMin, quantityMax, rigidBody, randomRotation) {

	var scat = new Array();
	
	var quantity = Random.Range(quantityMin,quantityMax);
	
	for (var i=0;i<quantity;i++) {
	
		var temPos = Vector3(Random.Range(2,36)/2, 0, Random.Range(2,36)/2);
		
		var arrPointer = temPos.x+(temPos.z*width);
		
		temPos.x += Random.Range(-0.2,0.2);
		temPos.z += Random.Range(-0.2,0.2);
		
		if (ap[arrPointer] == 0 || ap[arrPointer] == 2) {
	
			scat[i] = Instantiate (prefab, temPos, Quaternion.identity);
			scat[i].transform.position.y = returnPlayerPos(scat[i].transform.position.x,scat[i].transform.position.z)+offsetY;
		
			if (randomRotation==1) {
				//Rotate randomly around x-axis
				scat[i].transform.eulerAngles.y = Random.Range(0, 360);
			} else {
				scat[i].transform.eulerAngles.y = 45;
			}
		 	
			if (rigidBody == 1) {
				scat[i].rigidbody.Sleep();
			}
			
			ap[arrPointer] = 1;
			
			
		}
	}
	return scat;
}

function ScatterPebble(ap, prefab, offsetY, quantityMin, quantityMax, rigidBody, randomRotation) {

	var scat = new Array();
	
	var quantity = Random.Range(quantityMin,quantityMax);
	
	for (var i=0;i<quantity;i++) {
	
		var temPos = Vector3(Random.Range(2,36)/2, 0, Random.Range(2,36)/2);
		
		var arrPointer = temPos.x+(temPos.z*width);
		
		temPos.x += Random.Range(-0.2,0.2);
		temPos.z += Random.Range(-0.2,0.2);
		
		if (ap[arrPointer] == 0 || ap[arrPointer] == 2) {
	
			scat[i] = Instantiate (prefab, temPos, Quaternion.identity);
			scat[i].transform.position.y = returnPlayerPos(scat[i].transform.position.x,scat[i].transform.position.z)+offsetY;
		
			if (randomRotation==1) {
				//Rotate randomly around x-axis
				scat[i].transform.eulerAngles.y = Random.Range(0, 360);
			} else {
				scat[i].transform.eulerAngles.y = 45;
			}
		 	
			if (rigidBody == 1) {
				scat[i].rigidbody.Sleep();
			}
			
			ap[arrPointer] = 1;
			
			
		}
	}
	return scat;
}

function ScatterTrees(ap, prefab, offsetY, quantityMin, quantityMax, rigidBody, randomRotation) {

	var scat = new Array();
	
	var quantity = Random.Range(quantityMin,quantityMax);
	
	for (var i=0;i<quantity;i++) {
	
		var temPos = Vector3(Random.Range(2,36)/2, 0, Random.Range(2,36)/2);
		
		var arrPointer = temPos.x+(temPos.z*width);
		
		temPos.x += Random.Range(-0.2,0.2);
		temPos.z += Random.Range(-0.2,0.2);
		
		if (ap[arrPointer] == 0) {
	
			scat[i] = Instantiate (prefab, temPos, Quaternion.identity);
			scat[i].transform.position.y = returnPlayerPos(scat[i].transform.position.x,scat[i].transform.position.z)+offsetY;
		
			if (randomRotation==1) {
				//Rotate randomly around x-axis
				scat[i].transform.eulerAngles.y = Random.Range(0, 360);
			} else {
				scat[i].transform.eulerAngles.y = 45;
			}
		 	
			if (rigidBody == 1) {
				scat[i].rigidbody.Sleep();
			}
			
			ap[arrPointer] = 1;
			ap[arrPointer-1] = 2;
			ap[arrPointer+1] = 2;
			ap[arrPointer-width] = 2;
			ap[arrPointer-width-1] = 2;
			ap[arrPointer-width+1] = 2;
			ap[arrPointer+width] = 2;
			ap[arrPointer+width-1] = 2;
			ap[arrPointer+width+1] = 2;
			
			
		}
	}
	return scat;
}



function ScatterGrass(ap, prefab, offsetY, quantityMin, quantityMax, rigidBody, randomRotation) {

	var scat = new Array();
	
	var quantity = Random.Range(quantityMin,quantityMax);
	
	for (var i=0;i<quantity;i++) {
	
		var grassPick = Random.Range(0,3);
	
		if(grassPick == 0) {
			prefab = grass;
		} else if (grassPick == 1) {
			prefab = grass1;
		} else {
			prefab = grass2;
		}
	
		var temPos = Vector3(Random.Range(2,36)/2, 0, Random.Range(2,36)/2);
		
		var arrPointer = temPos.x+(temPos.z*width);
		
		temPos.x += Random.Range(-0.2,0.2);
		temPos.z += Random.Range(-0.2,0.2);
		
		if (ap[arrPointer] == 0 || ap[arrPointer] == 2) {
	
			scat[i] = Instantiate (prefab, temPos, Quaternion.identity);
			scat[i].transform.position.y = returnPlayerPos(scat[i].transform.position.x,scat[i].transform.position.z)+offsetY;
		
			if (randomRotation==1) {
				//Rotate randomly around x-axis
				scat[i].transform.eulerAngles.y = Random.Range(0, 360);
			} else {
				scat[i].transform.eulerAngles.y = 45;
			}
		 	
			if (rigidBody == 1) {
				scat[i].rigidbody.Sleep();
			}
			
			ap[arrPointer] = 1;
			
			
		}
	}
	return scat;
}


function ScatterRabbits(ap, prefab, offsetY, quantityMin, quantityMax, rigidBody, randomRotation) {

	var scat = new Array();
	
	var quantity = Random.Range(quantityMin,quantityMax);

    if(Random.Range(0.0,1.0) > 0.5){
        return scat;
    }
	
	if (Random.Range(0.0,1.0)>0.5) {
	
	for (var i=0;i<quantity;i++) {
	
		var temPos = Vector3(Random.Range(6,32)/2, 0, Random.Range(6,32)/2);
		
		var arrPointer = temPos.x+(temPos.z*width);
		
		temPos.x += Random.Range(-0.2,0.2);
		temPos.z += Random.Range(-0.2,0.2);
		
		if (ap[arrPointer] == 0 || ap[arrPointer] == 2) {
	
			scat[i] = Instantiate (prefab, temPos, Quaternion.identity);
			scat[i].transform.position.y = returnPlayerPos(scat[i].transform.position.x,scat[i].transform.position.z)+offsetY;
		
			if (randomRotation==1) {
				//Rotate randomly around x-axis
				scat[i].transform.eulerAngles.y = Random.Range(0, 360);
			} else {
				scat[i].transform.eulerAngles.y = 45;
			}
		 	
			if (rigidBody == 1) {
				scat[i].rigidbody.Sleep();
			}
			
			ap[arrPointer] = 1;
			
			
		}
	}
	
	}
	return scat;
}



*/
