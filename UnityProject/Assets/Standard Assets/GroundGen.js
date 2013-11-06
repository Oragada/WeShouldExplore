//#pragma strict

var ground : Mesh;
var groundCol : MeshCollider;
var width : int;
var height : int;

var move = 0; // move offset

//Noises
//var perlin;
//var fractal;
//perlin = new Perlin();
//fractal = new FractalNoise(2,1,1, perlin);

//Noise properties
@Range (0.0, 1.0)
var frequency = 0.2; //Noise frequency

@Range (-1.0, 10.0)
var scale = 1.0; //Noise scale

@Range (0, 19)
var xOff = 0;

@Range (0, 19)
var yOff = 0;

@Range (0, 19)
var zOff = 0;

var tileChangeFlag = 0;



function Start() {

	GenerateGround();
	ChangeTerrain();
	
}

function Update() {

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
		
		//uvs[u] = Vector2(vertices[u].x/(height-1), vertices[u].z/(width-1));
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

	//return fractal.HybridMultifractal(x*frequency,z*frequency,0)*scale+0.5;
	
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
	
	gameObject.AddComponent(MeshCollider);
	
	Scatter(ground);
}

function Scatter(ground) {
	
	var index = 0;
	arraySize = ground.vertices.Length;
	
	// NOTHING TO SEE HERE, MOVE ALONG



}