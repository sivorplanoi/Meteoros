using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class front : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.gameObject.GetComponent<Renderer> ().sortingLayerName = "front";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
