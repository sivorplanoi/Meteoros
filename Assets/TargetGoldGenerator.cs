using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGoldGenerator : MonoBehaviour {

	//ターゲットGoldを入れる
	public GameObject TargetGold;

	//ゲーム開始からの時間を測定
	private float time = 0.02f;

	//ターゲット生成範囲を入れる
	private int targetRangeFrontZ = 0, targetRangeRightX = 0, targetRangeBackZ = 0, targetRangeLeftX = 0;


	// Use this for initialization
	void Start () {

		//ターゲット生成範囲を取得
		targetRangeFrontZ = (int)GameObject.Find ("/TargetRange/TargetRange1").transform.position.z;
		targetRangeRightX = (int)GameObject.Find ("/TargetRange/TargetRange2").transform.position.x;
		targetRangeBackZ = (int)GameObject.Find ("/TargetRange/TargetRange3").transform.position.z;
		targetRangeLeftX = (int)GameObject.Find ("/TargetRange/TargetRange4").transform.position.x;

		TargetGoldInstantiate (20);

		StartCoroutine (targetGoldGenLoop ());

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator targetGoldGenLoop () {
		while (true) {
			if (Time.timeScale != 0) {
				yield return new WaitForSeconds (30.0f);
				TargetGoldInstantiate (20);
			}
		}
	}

	void TargetGoldInstantiate (int density) {
		
		for (int z = targetRangeBackZ + 1; z < targetRangeFrontZ; z = z + 5) {

			for (int x = targetRangeLeftX + 1; x < targetRangeRightX; x = x + 5) {
				//密度density％の確率でInstantiate
				if (Random.Range (1, 101) <= density) {
					//ランダムな高さ
					int y = Random.Range (2, 13);
					//プレハブ配列の中からランダムに生成
					GameObject targetGold = Instantiate (TargetGold, new Vector3 (x, y, z),Quaternion.identity);
					//パラメータセット
				}
			}
		}
	}
}
