using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpSphereController: MonoBehaviour {

	public GameObject ExplosionParticlePrefab;

	public GameObject ExpSphere;

	private float chainExpTime = 0.2f;

	public float ExpSphExpansionRate;

	private float time;

	// Use this for initialization
	void Start () {

		Invoke ("ScaleUp", chainExpTime);

	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		if (time > chainExpTime + 0.1f) {
			Destroy (this.gameObject);
		}

	}

	//sphereの衝突判定
	void OnTriggerStay(Collider other) {

		if (time > chainExpTime) {
			//sphere衝突相手がターゲットなら連鎖する
			if (other.gameObject.tag == "Target") {
				//衝突相手の地点にsphere生成(連鎖)
				GameObject expSphere = Instantiate (ExpSphere, other.gameObject.transform.position, Quaternion.identity) as GameObject;
				//sphereの大きさ(10の半分)にpara.scaleを掛けておく
				expSphere.transform.localScale = ExpSphere.transform.localScale * other.gameObject.GetComponent<Para> ().Scale;
				expSphere.GetComponent<ExpSphereController> ().ExpSphExpansionRate = this.ExpSphExpansionRate;

				//衝突相手爆発エフェクト
				//GameObject hitExplosion = Instantiate (ExplosionParticlePrefab, other.gameObject.transform.position, Quaternion.identity) as GameObject;
				//hitExplosion.GetComponent<ParticleSystem> ().Play ();


				GameObject.Find ("/ExplosionPlay").GetComponent<ExplosionPlayer> ().ExpParticleRange = ExpSphExpansionRate;
				GameObject.Find ("/ExplosionPlay").GetComponent<ExplosionPlayer> ().HitExplosionPlay (other.gameObject);

				GameObject.Find ("/WeaponGen").GetComponent<WeaponGenerator> ().Score += other.gameObject.GetComponent<Para> ().MaxHP * 2;
				GameObject.Find("/CanvasScore/ScoreText").GetComponent<Text> ().text = "Score : " + GameObject.Find ("/WeaponGen").GetComponent<WeaponGenerator> ().Score + "pt";

				//衝突相手を破棄
				DestroyObject (other.gameObject);
			}
		}
	}

	//遅延でsphere拡大
	void ScaleUp () {

		this.gameObject.transform.localScale = this.gameObject.transform.localScale * 10000 * ExpSphExpansionRate;

	}


}
