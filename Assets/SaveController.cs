using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SaveController : MonoBehaviour {

	private int[] ranking = new int[10];
	private string[] rankingPrefsKey = new string[10];

	public int RankIn = 100; //0～9以外で初期化

	public bool AfterGameOver = false;


	// Use this for initialization
	void Start () {

		for (int i = 0; i < 10; i++) {

			rankingPrefsKey [i] = "scoreRankingPrefsKey" + i;

			ranking [i] = PlayerPrefs.GetInt (rankingPrefsKey [i]);

		}


		DontDestroyOnLoad (this);


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//public void getScoreRanking () {
	//	string ranking = PlayerPrefs.GetString ("ScoreRanking");
	//	if 

	//スコアがトップ10に入る場合ランキングに登録
	public void SaveScoreRanking(int score) {
		//今回scoreとランキングスコアを比較、書き換え
		for (int i = 0; i < 10; i++) {

			// ランキングスコアと比較して大きければ書き換え
			if (ranking [i] <= score) {
				// 書き換え順位未満の順位を1つずつずらす(例:2位を3位に,1位を2位になど)
				for (int j = 9; j > i; j--) {
					ranking [j] = ranking [j - 1];
				}

				//今回scoreをランキングに入れ込む
				ranking [i] = score;

				//ランクイン
				RankIn = i;

				//for文を抜ける
				i = 10;
			}
		}
		//PlayerPrefsセット
		for (int i = 0; i < 10; i++) {
			PlayerPrefs.SetInt (rankingPrefsKey [i], ranking [i]);
		}
		//playerPrefsセーブ
		PlayerPrefs.Save ();

		//ゲームオーバーからの遷移であることを示す
		AfterGameOver = true;
	}

}
