using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleRankingController : MonoBehaviour {

	private int[] ranking = new int[10];
	private string[] rankingPrefsKey = new string[10];

	private float delta = 0.00f;
	private float flashDelta = 0.5f;

	private int rankInRank = 100;

	public GameObject MainCameraForAudio;

	public GameObject TitleCanvas;
	public GameObject RankingCanvas;
	public GameObject ConfirmCanvas;
	public GameObject ConfirmQuitCanvas;
	public GameObject HowToPlayCanvas;
	public GameObject PleaseQuitCanvas;


	// Use this for initialization
	void Start () {

		for (int i = 0; i < 10; i++) {

			rankingPrefsKey [i] = "scoreRankingPrefsKey" + i;

		}

		if (FindObjectOfType<SaveController> ()) {
			rankInRank = FindObjectOfType<SaveController> ().RankIn;
		}

		DisplayScoreRanking ();


		//RankInしている場合
		if (rankInRank >= 0 && rankInRank <= 9) {
			//ランクインスコアを赤色
			GameObject.Find ("/RankingCanvas/RankingPanel/Ranking" + rankInRank + "Text").GetComponent<Text> ().color = Color.red;
		}

		//初期状態としてタイトル画面
		RankingCanvas.SetActive (false);
		ConfirmCanvas.SetActive (false);
		ConfirmQuitCanvas.SetActive (false);


		//ゲームオーバー後であればランキング表示
		if (FindObjectOfType<SaveController> ()) {
			if (FindObjectOfType<SaveController> ().AfterGameOver) {
				TitleCanvas.SetActive (false);
				RankingCanvas.SetActive (true);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				DisplayConfirmQuitCanvas ();
			}
		}
	}

	// ゲームスタート(GameScene読み込み)
	public void GameStart () {
		MainCameraForAudio.GetComponent<AudioSource> ().Play ();
		GameObject.Find ("/TitleCanvas/Panel/StartButton/StartButtonText").GetComponent<Text> ().text = "Now Loading  ・・・";
		SceneManager.LoadScene ("GameScene");
	}

	// ゲーム終了
	public void GameQuit () {
		Application.Quit ();
	}

	// ゲーム終了確認ダイアログ表示
	public void DisplayConfirmQuitCanvas () {
		MainCameraForAudio.GetComponent<AudioSource> ().Play ();
		ConfirmCanvas.SetActive (false);
		ConfirmQuitCanvas.SetActive (true);
	}

	// ゲーム終了確認ダイアログキャンセル
	public void ConfimrQuitCancel () {
		MainCameraForAudio.GetComponent<AudioSource> ().Play ();
		ConfirmQuitCanvas.SetActive (false);
	}


	// TitleCanvasを非表示、RankingCanvasを表示
	public void RankingView () {
		MainCameraForAudio.GetComponent<AudioSource> ().Play ();
		TitleCanvas.SetActive (false);
		ConfirmCanvas.SetActive (false);
		ConfirmQuitCanvas.SetActive (false);
		RankingCanvas.SetActive (true);
	}

	// RankingCanvasを非表示、TitleCanvasを表示
	public void BackToTitle () {
		MainCameraForAudio.GetComponent<AudioSource> ().Play ();
		RankingCanvas.SetActive (false);
		ConfirmCanvas.SetActive (false);
		ConfirmQuitCanvas.SetActive (false);

		TitleCanvas.SetActive (true);
		if (FindObjectOfType<SaveController> ()) {
			FindObjectOfType<SaveController> ().RankIn = 100;
			FindObjectOfType<SaveController> ().AfterGameOver = false;
		}
	}

	// ランキング表示
	void DisplayScoreRanking() {
		//ランキングPrefs表示
		for (int i = 0; i < 10; i++) {
			GameObject.Find ("/RankingCanvas/RankingPanel/Ranking" + i + "Text").GetComponent<Text> ().text = (i + 1) + ". " + PlayerPrefs.GetInt (rankingPrefsKey [i]) + "pt";
		}
		//Newスコアを明示
		if (rankInRank >= 0 && rankInRank <= 9) {
			GameObject.Find ("/RankingCanvas/RankingPanel/Ranking" + rankInRank + "Text").GetComponent<Text> ().text = (rankInRank + 1) + ". " + PlayerPrefs.GetInt (rankingPrefsKey [rankInRank]) + "pt (NEW!!)";
		}
	}

	// 削除確認画面表示
	public void DisplayConfirmCanvas () {
		RankingCanvas.SetActive (false);
		ConfirmCanvas.SetActive (false);
		ConfirmQuitCanvas.SetActive (false);
		ConfirmCanvas.SetActive (true);
		ConfirmCanvas.GetComponent<AudioSource> ().Play ();
	}

	// 削除キャンセル
	public void ConfirmCancel () {
		MainCameraForAudio.GetComponent<AudioSource> ().Play ();
		ConfirmCanvas.SetActive (false);
		ConfirmQuitCanvas.SetActive (false);
		RankingCanvas.SetActive (true);	
	}

	public void HowToPlayDisplay () {
		MainCameraForAudio.GetComponent<AudioSource> ().Play ();
		TitleCanvas.SetActive (false);
		ConfirmCanvas.SetActive (false);
		ConfirmQuitCanvas.SetActive (false);
		RankingCanvas.SetActive (false);
		HowToPlayCanvas.SetActive (true);
	}

	public void HowToPlayBack () {
		MainCameraForAudio.GetComponent<AudioSource> ().Play ();
		HowToPlayCanvas.SetActive (false);
		TitleCanvas.SetActive (true);
	}


	// スコアを削除(0ptに上書き)
	public void DeleteScoreRanking () {

		RankingCanvas.SetActive (true);	

		for (int i = 0; i < 10; i++) {
			PlayerPrefs.SetInt (rankingPrefsKey [i], 0);
		}
		PlayerPrefs.Save ();

		for (int i = 0; i < 10; i++) {
			//ランクインスコアを元の色に戻す
			GameObject.Find ("/RankingCanvas/RankingPanel/Ranking" + i + "Text").GetComponent<Text> ().color = new Color (50f / 255f, 50f / 255f, 50f / 255f, 255f / 255f);
		}
		if (FindObjectOfType<SaveController> ()) {
			FindObjectOfType<SaveController> ().RankIn = 100;
			rankInRank = FindObjectOfType<SaveController> ().RankIn;
			FindObjectOfType<SaveController> ().AfterGameOver = false;
		}

		DisplayScoreRanking ();


		RankingCanvas.SetActive (false);	

		ConfirmCanvas.SetActive (false);

		PleaseQuitCanvas.SetActive (true);

	}


}
