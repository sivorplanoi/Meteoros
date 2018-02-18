using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPgageOperation : MonoBehaviour {

	private GameObject rotateCamera;

	// Use this for initialization
	void Start () {
		rotateCamera = GameObject.Find ("FirstPersonCharacter");
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt (rotateCamera.transform);
	}

}
