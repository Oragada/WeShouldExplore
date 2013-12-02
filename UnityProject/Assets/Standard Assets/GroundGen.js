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
var bush : GameObject;
var pebble : GameObject;
var rabbits : GameObject;
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
var shroomsID;
var animalsID;
var pebblesID;
var grassID;

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

																								// my tangents solution - didn't work		
																								// 			var vertexL = Vector3( x-1, 0, y );
																								// 			var vertexR = Vector3( x+1, 0, y );
																								//	
																								// 			var tan = Vector3.Scale( Vector3(1,1,1), vertexR - vertexL ).normalized;
																								// 			tangents[i-1] = Vector4( tan.x, tan.y, tan.z, 1 );
																								// 
																								// 			var tan = vertexR - vertexL;
																								// 			tan = tan.normalized;
																								//  		tangents[i-1] = Vector4( tan.x, tan.y, tan.z, -1 );
			
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
	
	var treeProb = 0.1;																			//Probabilities
	var bushProb = 0.1;
	var grassProb = 0.6;
	var pebbleProb = 0.4;
	var flowerProb = 0.3;
	
	
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
				
			if (Random.Range(0.0,1.0)<treeProb) {
			
				toRender[i] = treesID;
				
				TreeAreola(i, toRender);														//Tree Areola
				
				
			}
		}
	}
	
	i = 0;
	
	
	for (i=0; i < toRender.length; i++) {														//SCATTER BUSHES//
		
		
		if (toRender[i]==0) {																	//if on empty land
				
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
	
	for (i=0; i < toRender.length; i++) {														//SCATTER PEBBLES//
		
		if (toRender[i]==emptyID) {
				
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
				
				renderedInstances[i] = Instantiate(grass1, temPos, Quaternion.identity); 		//instantiation
				
				renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
			break;
			
			case pebblesID:																	//render PEBBLES//
				
				temPos.y+=0.2; 																	//vertical offset
				
				renderedInstances[i] = Instantiate(pebble, temPos, Quaternion.identity); 		//instantiation
				
				renderedInstances[i].rigidbody.Sleep();	
															
				//renderedInstances[i].transform.eulerAngles.y = Random.Range(0, 360);			//random rotation
				
			break;
		}
	}
}






function TreeAreola(i, toRender) {															//----TreeAreola----//

	toRender[i-1] = toRender[i-1]==bordersID ? bordersID : areolasID;
	toRender[i+1] = toRender[i+1]==bordersID ? bordersID : areolasID;
	toRender[i-width] = toRender[i-width]==bordersID ? bordersID : areolasID;
	toRender[i-width-1] = toRender[i-width-1]==bordersID ? bordersID : areolasID;
	toRender[i-width+1] = toRender[i-width+1]==bordersID ? bordersID : areolasID;
	toRender[i+width] = toRender[i+width]==bordersID ? bordersID : areolasID;
	toRender[i+width-1] = toRender[i+width-1]==bordersID ? bordersID : areolasID;
	toRender[i+width+1] = toRender[i+width+1]==bordersID ? bordersID : areolasID;

	toRender[i-2] = toRender[i-2]==bordersID ? bordersID : areolasID;
	toRender[i+2] = toRender[i+2]==bordersID ? bordersID : areolasID;

	if (i-width-2>=0) {
		toRender[i-width-2] = toRender[i-width-2]==bordersID ? bordersID : areolasID;
	}
	
	if (i-width+2>=0) {
		toRender[i-width+2] = toRender[i-width+2]==bordersID ? bordersID : areolasID;
	}

	if (i+width-2<toRender.length) {
		toRender[i+width-2] = toRender[i+width-2]==bordersID ? bordersID : areolasID;
	}
	
	if (i+width+2<toRender.length) {
		toRender[i+width+2] = toRender[i+width+2]==bordersID ? bordersID : areolasID;
	}


	for (var rng = -1; rng<=1; rng++) {
		if (i-width*2+rng>=0) {
			toRender[i-width*2+rng] = toRender[i-width*2+rng]==bordersID ? bordersID : areolasID;
		}
	}

	for (rng = -1; rng<=1; rng++) {
		if (i+width*2+rng<toRender.length) {
			toRender[i+width*2+rng] = toRender[i+width*2+rng]==bordersID ? bordersID : areolasID;
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


/*Derived from
Lengyel, Eric. Computing Tangent Space Basis Vectors for an Arbitrary Mesh. Terathon Software 3D Graphics Library, 2001.
http://www.terathon.com/code/tangent.html
*/

/* 
function TangentSolver(theMesh : Mesh)
    {
        vertexCount = theMesh.vertexCount;
        vertices = theMesh.vertices;
        normals = theMesh.normals;
        texcoords = theMesh.uv;
        triangles = theMesh.triangles;
        triangleCount = triangles.length/3;
        tangents = new Vector4[vertexCount];
        tan1 = new Vector3[vertexCount];
        tan2 = new Vector3[vertexCount];
        tri = 0;
        for ( i = 0; i < (triangleCount); i++)
        {
            i1 = triangles[tri];
            i2 = triangles[tri+1];
            i3 = triangles[tri+2];
 
            v1 = vertices[i1];
            v2 = vertices[i2];
            v3 = vertices[i3];
 
            w1 = texcoords[i1];
            w2 = texcoords[i2];
            w3 = texcoords[i3];
 
            x1 = v2.x - v1.x;
            x2 = v3.x - v1.x;
            y1 = v2.y - v1.y;
            y2 = v3.y - v1.y;
            z1 = v2.z - v1.z;
            z2 = v3.z - v1.z;
 
            s1 = w2.x - w1.x;
            s2 = w3.x - w1.x;
            t1 = w2.y - w1.y;
            t2 = w3.y - w1.y;
 
            r = 1.0 / (s1 * t2 - s2 * t1);
            sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
 
            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;
 
            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
 
            tri += 3;
        }
 
        for (i = 0; i < (vertexCount); i++)
        {
            n = normals[i];
            t = tan1[i];
 
            // Gram-Schmidt orthogonalize
            Vector3.OrthoNormalize( n, t );
 
            tangents[i].x  = t.x;
            tangents[i].y  = t.y;
            tangents[i].z  = t.z;
 
            // Calculate handedness
            tangents[i].w = ( Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0 ) ? -1.0 : 1.0;
        }       
    theMesh.tangents = tangents;
}

*/