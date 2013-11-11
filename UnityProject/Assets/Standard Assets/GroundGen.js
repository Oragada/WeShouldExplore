//#pragma strict

//PUBLIC

var depthGenR: GameObject;
var depthGenL: GameObject;

var ground : Mesh;
var groundCol : MeshCollider;
var frictionMap: PhysicMaterial;
var width : int;
var height : int;

//Noise properties
@Range (0.0, 1.0)
var frequency = 0.2; //Noise frequency

@Range (-1.0, 10.0)
var scale = 1.0; //Noise scale

//Tile Offsets
var flower : GameObject;
var bush : GameObject;
var pebble : GameObject;
var rabbits : GameObject;
var grass : GameObject;
var grass1 : GameObject;
var grass2 : GameObject;
var tree: GameObject;

var groundMat : Material;

//PRIVATE
var xOff;
var yOff;
var zOff;

var tileChangeFlag;
var currentScat = new Array();
var lastScat = new Array();



function Start() {

	xOff = 0;
	yOff = 0;
	zOff = 0;
	
	tileChangeFlag = 0;

	GenerateGround();
	ChangeTerrain();
	
	
}

function Update() {

	/*This is in the update function because the previous tile collider gets destroyed
	no sooner than the next update. In other words: right about now */
	
	if(tileChangeFlag==1) {	
		ChangeTerrain();
		tileChangeFlag=0;
	}

}

////////////////////////////////////////////
//										  //
//          FUNCTION DEFINITIONS		  //
//										  //		
////////////////////////////////////////////

function GenerateGround() {

	gameObject.AddComponent(MeshFilter);
	gameObject.AddComponent(MeshRenderer);
	
	renderer.material = groundMat;
	
	ground = GetComponent(MeshFilter).mesh;
	
	var vertices = new Vector3[width*height];
	
	var uvs = new Vector2[vertices.Length];
	
// 	var tangents = new Vector4[vertices.Length];
	
	var triCount = (width-1)*(height-1)*2;

	var triangles = new int[triCount*3];
	
	//Generates vertices
	var i = 0;
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
	
	//Generates triangles
	i = 0;
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
	
	//Generates uvs
	for(var u=0; u < uvs.Length; u++) {

		uvs[u] = Vector2(vertices[u].x, vertices[u].z);
	
	}
	
	//Assigns vertices, triangles and uvs to the ground mesh
	
	ground.vertices = vertices;
	ground.triangles = triangles;
	ground.uv = uvs;
	
	ground.RecalculateNormals();
	
	//tangents
	//ground.tangents = tangents;
	//TangentSolver(ground);
	
	
	//Scale the mesh so it's the same size on the screen no matter what BREAKS OTHER SHIT IF NOT 20x20 RIGHT NOW
	transform.localScale.x = 20/(width*1.0);
	transform.localScale.z = 20/(height*1.0);
	transform.localScale.y = transform.localScale.x;
	

}


function returnPlayerPos(x,z) {

	return returnGroundY(x,z)+0.2;
	
}

function returnGroundY(x,z) {
	
	return Mathf.PerlinNoise(z*frequency+(zOff*frequency),x*frequency+(xOff*frequency))*scale;

}


function showNextTile(dir) {

	switch(dir) {
		case 0: //North
			xOff += 19; 
		break;
		case 1: //South
			xOff -= 19;
		break;
		case 2:
			zOff -= 19;
		break;
		case 3:
			zOff += 19;
		break;
	}
	
	Destroy(GetComponent(MeshCollider));
	
	tileChangeFlag = 1;
	
}

function ChangeTerrain() {

	//Clear currectScat for the new tile
	if (currentScat.length>0) {

		for(var k=0;k<currentScat.length;k++) {
			var toKill = currentScat[k];
			//currentScat.RemoveAt(k);
			Destroy(toKill);
		}
	}

	//transfer height map data on mesh
	ground = GetComponent(MeshFilter).mesh;
	
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
	
	//Generate collision mesh for new tile
	gameObject.AddComponent(MeshCollider);
	
	//Make depth planes
	depthGenR.GetComponent(DepthGenR).Generate();
	depthGenL.GetComponent(DepthGenL).Generate();
	
	//
	//SCATTERING - very nasty, should be rewriten
	
	lastScat = currentScat;
	
	var availablePositions = new Array();
	
	for (var c=0; c < vertices.length; c++) {
		availablePositions[c] = 0;
	}
	
	currentScat = ScatterTrees(availablePositions, tree, 0.0, 10, 30, 0, 1);
	
	currentScat = currentScat.concat(Scatter(availablePositions, bush, -0.1, 3, 10, 0, 1));
	
	currentScat = currentScat.concat(Scatter(availablePositions, flower, -0.2, 10, 30, 0, 1));

    currentScat = currentScat.concat(ScatterRabbits(availablePositions, rabbits, 0, 1, 2, 0, 1));
    
    currentScat = currentScat.concat(Scatter(availablePositions, pebble, 0, 10, 20, 1, 1));
    
    currentScat = currentScat.concat(ScatterGrass(availablePositions, grass,/*offsetY*/ 0.4,/*quantityMin*/ 400,/*quantityMax*/ 500,/*rigidBody*/ 0,/*randomRotation*/ 0));
	
	


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
			/*
			ap[arrPointer-1] = 2;
			ap[arrPointer+1] = 2;
			ap[arrPointer-width] = 2;
			ap[arrPointer-width-1] = 2;
			ap[arrPointer-width+1] = 2;
			ap[arrPointer+width] = 2;
			ap[arrPointer+width-1] = 2;
			ap[arrPointer+width+1] = 2;
			*/
			
			
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
			/*
			ap[arrPointer-1] = 2;
			ap[arrPointer+1] = 2;
			ap[arrPointer-width] = 2;
			ap[arrPointer-width-1] = 2;
			ap[arrPointer-width+1] = 2;
			ap[arrPointer+width] = 2;
			ap[arrPointer+width-1] = 2;
			ap[arrPointer+width+1] = 2;
			*/
			
			
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
			/*
			ap[arrPointer-1] = 2;
			ap[arrPointer+1] = 2;
			ap[arrPointer-width] = 2;
			ap[arrPointer-width-1] = 2;
			ap[arrPointer-width+1] = 2;
			ap[arrPointer+width] = 2;
			ap[arrPointer+width-1] = 2;
			ap[arrPointer+width+1] = 2;
			*/
			
			
		}
	}
	return scat;
}

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