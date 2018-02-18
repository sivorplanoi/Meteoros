using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGenerator : MonoBehaviour {

	//ターゲットプレハブを入れる
	public GameObject[] TargetPrefab1;
	public GameObject[] TargetPrefab2;
	public GameObject[] TargetPrefab3;
	public GameObject[] TargetPrefab4;

	public Material[] Skybox;

	//ゲーム開始からの時間を測定
	private float time = 0.02f;

	//ターゲット生成範囲を入れる
	private int targetRangeFrontZ = 0, targetRangeRightX = 0, targetRangeBackZ = 0, targetRangeLeftX = 0;

	private int offset = 300;


	// Use this for initialization
	void Start () {
		//skyboxをランダムに設定
		int ranNumForSky = Random.Range (1, 101);
		if (ranNumForSky <= 70) {
			GameObject.Find ("/Directional Light").GetComponent<Light> ().intensity = 1.0f;
			RenderSettings.skybox = Skybox [0];
		} else if (ranNumForSky <= 85) {
			GameObject.Find ("/Directional Light").GetComponent<Light> ().intensity = 0.5f;
			RenderSettings.skybox = Skybox [1];
		} else if (ranNumForSky <= 95) {
			GameObject.Find ("/Directional Light").GetComponent<Light> ().intensity = 0.1f;
			RenderSettings.skybox = Skybox [2];
		} else {
			GameObject.Find ("/Directional Light").GetComponent<Light> ().intensity = 0.0f;
			RenderSettings.skybox = Skybox [3];
		}

		//ターゲット生成範囲を取得
		targetRangeFrontZ = (int)GameObject.Find ("/TargetRange/TargetRange1").transform.position.z;
		targetRangeRightX = (int)GameObject.Find ("/TargetRange/TargetRange2").transform.position.x;
		targetRangeBackZ = (int)GameObject.Find ("/TargetRange/TargetRange3").transform.position.z;
		targetRangeLeftX = (int)GameObject.Find ("/TargetRange/TargetRange4").transform.position.x;

		//TargetInstantiate(prefabの名前,prefabのRank,密度%,発生方角)
		TargetInstantiate (TargetPrefab1, 1, 2, 1);
		StartCoroutine (targetGenLoop ());

	}
	
	// Update is called once per frame
	void Update () {		
		if (Time.timeScale != 0) {
			time += Time.deltaTime;
		}
	}

	IEnumerator targetGenLoop () {
		while (true) {
			if (Time.timeScale != 0) {
				if (time < 120) {
					yield return new WaitForSeconds (30.0f);
					TargetInstantiate (TargetPrefab1, 1, 2 * Mathf.CeilToInt (time / 60.0f), Random.Range (1, 5));
				}
				else if (time >= 120 && time < 240) {
					yield return new WaitForSeconds (25.0f);
					TargetInstantiate (TargetPrefab2, 2, 2 * Mathf.CeilToInt (time / 60.0f), Random.Range (1, 5));
				}
				else if (time >= 240 && time < 360) {
					yield return new WaitForSeconds (20.0f);
					TargetInstantiate (TargetPrefab3, 3, 2 * Mathf.CeilToInt (time / 60.0f), Random.Range (1, 5));
				}
				else if  (time >= 360 && time < 480)  {
					yield return new WaitForSeconds (15.0f);
					TargetInstantiate (TargetPrefab3, 3, 2 * Mathf.CeilToInt (time / 60.0f), Random.Range (1, 5));
				}
				else if  (time >= 480)  {
					yield return new WaitForSeconds (10.0f);
					TargetInstantiate (TargetPrefab4, 4, 2 * Mathf.CeilToInt (time / 60.0f), Random.Range (1, 5));
				}

			}
		}
	}

	//ターゲット生成関数
	void TargetInstantiate (GameObject[] targetPrefab, int targetPrefabRank, int density, int directionNum) {

		int frontZ = targetRangeFrontZ, backZ = targetRangeBackZ, rightX = targetRangeRightX, leftX = targetRangeLeftX;

		switch (directionNum) {
		case 1:
			frontZ += offset;
			backZ += offset;
			rightX += offset;
			leftX -= offset;
			break;
		case 2:
			frontZ += offset;
			backZ -= offset;
			rightX += offset;
			leftX += offset;
			break;
		case 3:
			frontZ -= offset;
			backZ -= offset;
			rightX += offset;
			leftX -= offset;
			break;
		case 4:
			frontZ += offset;
			backZ -= offset;
			rightX -= offset;
			leftX -= offset;
			break;
		default:
			break;
		}



		for (int z = backZ + 5; z < frontZ; z = z + 12) {

			for (int x = leftX + 7; x < rightX; x = x + 15) {
				//密度density％の確率でInstantiate
				if (Random.Range (1, 101) <= density) {
					//ランダムな高さ
					int y = Random.Range (250, 300);
					//プレハブ配列の中からランダムに生成
					int ArrayNum = Random.Range (0, targetPrefab.Length);
					GameObject target = Instantiate (targetPrefab [ArrayNum], new Vector3 (x, y, z),Quaternion.identity);
					//傾きランダム　ただし裏返らないよう±45度まで
					target.transform.Rotate(Random.Range(-135, -45), 0, Random.Range(-45,45));
					//パラメータセット
					target.AddComponent<Para> ();
					target.GetComponent<Para> ().ParaSet (targetPrefabRank, time);
					target.transform.localScale = target.transform.localScale * target.GetComponent<Para> ().Scale;
				}
			}
		}
	}




}

// ターゲットに個々にパラメータを持たせ、タグによって値をセット
public class Para : MonoBehaviour {
	public int MaxHP;
	public int HP;
	public float Scale;
	public float ExplosionSize;

	public void ParaSet (int rank, float time)
	{

		//確率用のランダムな数値生成
		int probability = Random.Range (1, 10);

		//ランクごとにのパラメータ設定
		if (rank <= 2) {
			//時間が経つほどターゲットのHPが増え、サイズが小さくなる
			//60%の確率
			if (probability <= 6) {
				MaxHP = Random.Range (10, 15) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
			//30%の確率
			else if (probability <= 9) {
				MaxHP = Random.Range (16, 20) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
			//10%の確率
			else {
				MaxHP = Random.Range (21, 25) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
		} else if (rank == 3) {
			//時間が経つほどターゲットのHPが増え、サイズが小さくなる
			//60%の確率
			if (probability <= 6) {
				MaxHP = Random.Range (10, 15) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
			//30%の確率
			else if (probability <= 9) {
				MaxHP = Random.Range (16, 20) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
			//10%の確率
			else {
				MaxHP = Random.Range (21, 25) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
		} else {
			//時間が経つほどターゲットのHPが増え、サイズが小さくなる
			//60%の確率
			if (probability <= 6) {
				MaxHP = Random.Range (15, 20) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
			//30%の確率
			else if (probability <= 9) {
				MaxHP = Random.Range (21, 25) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
			//10%の確率
			else {
				MaxHP = Random.Range (26, 30) * Mathf.CeilToInt (time / 30);
				HP = MaxHP;
				Scale = Mathf.Pow (Random.Range (0.9f, 0.8f), Mathf.Ceil (time / 150));
				ExplosionSize = 10.0f * Scale;
			}
		}




	}
}