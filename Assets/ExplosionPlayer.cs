using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExplosionPlayer : MonoBehaviour {

	public GameObject HouseExpolosionParticlePrefab;
	public GameObject HitExplosionParticlePrefab;

	public GameObject GameOverCanvas;

	public float ExpParticleRange;

	private int houseExistence = 1;

	private int houseQuantity = 3;

	// Use this for initialization
	void Start () {

		GameObject[] Houses = GameObject.FindGameObjectsWithTag ("House");
		GameObject.Find ("/CanvasScore/BrokenHouseText").GetComponent<Text> ().text = ": " + (5 - Houses.Length) + " / " + houseQuantity;

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//House爆発パーティクル再生
	public void HouseExplosionPlay (GameObject target) {
		GameObject explosionEffect = Instantiate (HouseExpolosionParticlePrefab, target.transform.position, Quaternion.identity) as GameObject;
		explosionEffect.GetComponent<ParticleSystem> ().Play ();
		GameObject.Find ("HouseExSound/" + target.name + "ExSound").GetComponent<AudioSource> ().Play ();
		Destroy (target);
		Destroy (explosionEffect, 5.5f);

		//houseが減ったらいずれかランダムでhouseの位置にHomeCoreを移動する
		//GameObject.Find ("/HomeCore").gameObject.transform.position = GameObject.FindWithTag ("House").transform.position;

		Invoke ("DelayScoreSave", 0.2f);

	}

	//Target爆発パーティクル再生
	public void HitExplosionPlay (GameObject target) {
		GameObject explosionEffect = Instantiate (HitExplosionParticlePrefab, target.transform.position, Quaternion.identity) as GameObject;
		explosionEffect.transform.localScale *= target.GetComponent<Para> ().Scale * ExpParticleRange;
		explosionEffect.GetComponent<ParticleSystem> ().Play ();
		GameObject.Find("FPSController/FirstPersonCharacter").GetComponent<AudioSource> ().Play ();
		Destroy (target);
		Destroy (explosionEffect, 5.0f);
	}

	void DelayScoreSave () {

		//if (GameObject.FindGameObjectWithTag ("House") != null) {
		//	GameObject.Find ("/HomeCore").gameObject.transform.position = GameObject.FindGameObjectWithTag ("House").transform.position;
		//}

		GameObject[] Houses = GameObject.FindGameObjectsWithTag ("House");
		if ((5 - Houses.Length) <= houseQuantity) {
			GameObject.Find ("/CanvasScore/BrokenHouseText").GetComponent<Text> ().text = ": " + (5 - Houses.Length) + " / " + houseQuantity;
		}

		if ((5 - Houses.Length) >= houseQuantity) {
			ScoreSaveLoadTitle ();
		}
	}

	public void ScoreSaveLoadTitle () {
		//ただし重複して動作することを防ぐため、1回のみ動作する
		//houseExistenceの初期値を1としてデクリメントすることで、最初の1回(0のとき)のみ動作する
		houseExistence--;
		if (houseExistence == 0) {
			GameObject.Find ("/Save").GetComponent<SaveController> ().SaveScoreRanking (GameObject.Find ("/WeaponGen").GetComponent<WeaponGenerator> ().Score);

			if (GameObject.Find ("/CanvasUI") != null) {
				GameObject.Find ("/CanvasUI").SetActive (false);
			}
			GameObject gameOverCanvas = Instantiate (GameOverCanvas) as GameObject;

			Invoke ("LoadRankingScene", 2.0f);
		}
	}



	public void LoadRankingScene () {
		Time.timeScale = 1.0f;
		SceneManager.LoadScene ("TitleRankingScene");
	}

}
