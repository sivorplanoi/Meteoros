using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("DestroyThisGold", 60);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void DestroyThisGold () {
		Destroy (this.gameObject);
	}

}
