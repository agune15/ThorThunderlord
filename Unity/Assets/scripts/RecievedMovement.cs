using UnityEngine;
using System.Collections;

public class RecievedMovement : MonoBehaviour  {/*
	
    
	Vector3 newposition;
	public float speed;
	public float walkRange;

	public GameObject graphics;


	void Start () {

		newposition = this.transform.position;

	}
	

	void Update () {

		if (Vector3.Distance (newposition, this.transform.position) > walkRange) {
			this.transform.position = Vector3.MoveTowards(this.transform.position, newposition, speed * Time.deltaTime);
			Quaternion transRot = Quaternion.LookRotation(newposition - this.transform.position, Vector3.up);
			graphics.transform.rotation = Quaternion.Slerp(transRot, graphics.transform.rotation, 0.7f);
		}
	}	

	[PunRPC]
	public void RecievedMove(Vector3 movePos){
		newposition = movePos;
	}
    */
}
