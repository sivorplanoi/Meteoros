using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleViewController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void GameStart () {

		GameObject.Find ("/Canvas/Panel/StartButton/StartButtonText").GetComponent<Text> ().text = "Now Loading  ・・・";
		SceneManager.LoadScene ("GameScene");
	}

	public void RankingView () {

		SceneManager.LoadScene ("RankingScene");
	}

}
