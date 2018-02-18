using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeGravity : MonoBehaviour {

	public GameObject homeCore;
	private float GravityAcceleration = 8.0f;

	private bool timeScale0 = false;

	private Vector3 rigidbodyVelo;
	private Vector3 rigidbodyAngVelo;



	// Use this for initialization
	void Start () {
		Vector3 direction = homeCore.transform.position - this.transform.position;
		direction.Normalize ();

		this.gameObject.GetComponent<Rigidbody> ().AddForce (GravityAcceleration * direction, ForceMode.VelocityChange);


	}
	
	// Update is called once per frame
	void Update () {
		//timeScaleが0ならターゲットの動きを止める

		if (Time.timeScale == 0 && !timeScale0) {
			TargetPauseTemp ();
			timeScale0 = true;
		}
		else if (Time.timeScale != 0 && timeScale0) {
			TargetResumeTemp ();
			timeScale0 = false;
		}

		Vector3 direction = homeCore.transform.position - this.transform.position;
		GravityAcceleration = (direction.sqrMagnitude * 1 ) / 1000000000000.0f;
		direction.Normalize ();

		this.gameObject.GetComponent<Rigidbody> ().AddForce (GravityAcceleration * direction, ForceMode.Acceleration);


	}

	void TargetPauseTemp () {
		rigidbodyVelo = this.gameObject.GetComponent<Rigidbody> ().velocity;
		rigidbodyAngVelo = this.gameObject.GetComponent<Rigidbody> ().angularVelocity;
		this.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		this.gameObject.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
	}

	void TargetResumeTemp () {
		this.gameObject.GetComponent<Rigidbody> ().velocity = rigidbodyVelo;
		this.gameObject.GetComponent<Rigidbody> ().angularVelocity = rigidbodyAngVelo;
	}




	void OnCollisionEnter(Collision other) {

		if (other.gameObject.transform.root.gameObject.name == "Home" || other.gameObject.name == "Terrain") {

			if (other.gameObject.transform.parent != null) {
				GameObject otherParentObject = other.gameObject.transform.parent.gameObject;

				if (otherParentObject.tag == "House") {

					GameObject.Find ("/ExplosionPlay").GetComponent<ExplosionPlayer> ().HouseExplosionPlay (otherParentObject);



				}
			}

			GameObject.Find ("/ExplosionPlay").GetComponent<ExplosionPlayer> ().ExpParticleRange = 1;
			GameObject.Find ("/ExplosionPlay").GetComponent<ExplosionPlayer> ().HitExplosionPlay (this.gameObject);

		}

	}
}
