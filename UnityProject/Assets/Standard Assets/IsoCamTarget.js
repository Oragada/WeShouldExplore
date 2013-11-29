#pragma strict

var player: GameObject; 

var groundGen: GroundGen;

function Start () {



}

function Update () {

		transform.position.y = groundGen.returnCurvature(player.transform.position.x, player.transform.position.z)*0.5;	

}