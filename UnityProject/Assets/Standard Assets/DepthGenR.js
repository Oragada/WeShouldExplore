#pragma strict

var groundObject : GameObject;
var depth : Mesh;
var mat : Material;

var renderInit = 0;

function Start () {

	

}

function Update () {

}



function Generate() {

	if(renderInit == 0) {
		gameObject.AddComponent(MeshFilter);
		gameObject.AddComponent(MeshRenderer);
		renderInit = 1;
	}

	var heightD = 2;
	var width = 20;

	renderer.material = mat;
	
	var ground = groundObject.GetComponent(MeshFilter).mesh;
	
	depth = GetComponent(MeshFilter).mesh;
	
	var vertices = new Vector3[width*heightD];
	
	var uvs = new Vector2[vertices.Length];
	
	var triCount = (width-1)*(heightD-1)*2;

	var triangles = new int[triCount*3];
	
	//Generates vertices
	var i = 0;
	for(var x=0;x<width;x++) {
	
		vertices[i++] = Vector3(x,ground.vertices[i-1].y-0,0);
		
	}
	
	for(var x2=0;x2<width;x2++) {
	
		vertices[i++] = Vector3(x2,0,0);
		
	}

	
/*	
	var i = 0;
	for(var y=0;y<heightD;y++) {
		for(var x=0;x<width;x++) {
		
			vertices[i++] = Vector3(x,ground.vertices[i-1].y-y,0);
			
		}
	}
	
*/	
	
	
	//Generates triangles
	i = 0;
	for(var yt=0;yt<heightD-1;yt++) {
		for(var xt=0;xt<width-1;xt++) {
		
			triangles[i++] = (yt * width) + xt;
			triangles[i++] = (yt * width) + xt+1;
			triangles[i++] = ((yt + 1) * width) + xt;
			
			
			triangles[i++] = ((yt+1)*width)+xt;
			triangles[i++] = (yt * width) + xt+1;
			triangles[i++] = ((yt+1)* width) + xt+1;
			
			
		}
	}
	
	//Generates uvs
	for(var u=0; u < uvs.Length; u++) {

		//uvs[u] = Vector2(vertices[u].x, vertices[u].z);
		uvs[u] = Vector2(vertices[u].x, vertices[u].y);
	}
	
	//Assigns vertices, triangles and uvs to the ground mesh
	
	depth.vertices = vertices;
	depth.triangles = triangles;
	depth.uv = uvs;
	
	depth.RecalculateNormals();
	
// 	for (var b=0; b<depth.normals.length; b++) {
// 	
// 		depth.normals[b] = -depth.normals[b];
// 	
// 	}
	


}

