//#pragma strict

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
var xOff = 0;
var yOff = 0;
var zOff = 0;

var tileChangeFlag = 0;

var flower : GameObject;
var bush : GameObject;
var pebble : GameObject;

var currentScat = new Array();
var lastScat = new Array();



function Start() {

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

function GenerateGround() {

	gameObject.AddComponent(MeshFilter);
	gameObject.AddComponent(MeshRenderer);
	
	renderer.material.color = Color(0.3, 0.7, 0.1);
	
	ground = GetComponent(MeshFilter).mesh;
	
	var vertices = new Vector3[width*height];
	
	var uvs = new Vector2[vertices.Length];
	
	var triCount = (width-1)*(height-1)*2;

	var triangles = new int[triCount*3];
	
	//Generates vertices
	var i = 0;
	for(var y=0;y<height;y++) {
		for(var x=0;x<width;x++) {
			vertices[i++] = Vector3(x,0,y);
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
	GetComponent(MeshCollider).material = frictionMap; 
	GetComponent(MeshCollider).smoothSphereCollisions = true; 
	
	//Do some scattering
	lastScat = currentScat;
	currentScat = Scatter(flower, -0.2, 30);
	
	currentScat= currentScat.concat( Scatter(bush, -0.1, 10));
	
	
	//GENERATE DEPTH - Does not work yet
	//var depthGen: DepthGen = GetComponent(DepthGen); ; 
	//Debug.Log(depthGen);

}

function Scatter(prefab, offset, quantity) {

	var scat = new Array();
	
	for (var i=0;i<quantity;i++) {
		scat[i] = Instantiate (prefab, Vector3(Random.Range(1,18),0, Random.Range(1,18)), Quaternion.identity);
		scat[i].transform.position.y = returnPlayerPos(scat[i].transform.position.x,scat[i].transform.position.z)+offset;
		
		//Rotate randomly around x-axis
		scat[i].transform.eulerAngles.y = Random.Range(0, 360);
	}
	return scat;
}


