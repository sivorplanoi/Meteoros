using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WeaponGenerator : MonoBehaviour {

	//武器Prefab配列
	public GameObject[] Weapon;
	//現在武器の配列番号
	private int selectWeapon = 0;

	//ヒットエフェクトPrefab
	public GameObject HandgunHitEffectPrefab;

	//銃口flashエフェクトオブジェクトを入れる
	private GameObject flashEffect;

	//現在武器オブジェクトを保持する
	private GameObject currentWeapon;
	private GameObject currentMuzzle;


	//fpsカメラ
	private GameObject fpsCamera;

	//moveUIエリア閾値
	private float moveUIyPosTo = (1f / 4f) * (float)Screen.height;
	private float moveUIxPosFrom = (1f / 5f) * (float)Screen.width;

	private bool shootPermission = true; //射撃許可
	private float fromTapTime = 0.00f; //タップ開始からの累積時間
	private bool tapStart = false;
	private float shootWaitTime = 0.00f;

	//武器パラメータ
	private int weaponPower;
	private float weaponWait;
	private int weaponRange;
	private float weaponExRange;
	//強化後一時的武器パラメータ
	private int tempweaponPower;
	private float tempweaponWait;
	private int tempweaponRange;
	private float tempweaponExRange;
	//ハンドガン
	private int handgunPower = 10;
	private float handgunWait = 0.5f;
	private int handgunRange = 150;
	private float handgunExRange = 2.5f;
	//ライフル
	private int riflePower = 30;
	private float rifleWait = 0.9f;
	private int rifleRange = 300;
	private float rifleExRange = 1.5f;


	private int waitInt;
	private int ExRangeInt;

	//スコア
	private GameObject scoreText;
	public int Score = 0;

	public int GoldScore = 0;

	//爆発sphere
	public GameObject ExplosionSphere;

	//can't reach Canvas
	public GameObject CanvasCantReach;

	//表示/非表示するUI
	public GameObject CanvasScore;
	public GameObject CanvasUI;
	public GameObject CanvasPowerUp;
	public GameObject CanvasPowerUpDialog;
	public GameObject CanvasDontHaveDialog;
	public GameObject CanvasTitleBackDialog;


	public GameObject[] DisplayWeapon;
	private GameObject currentDisplayWeapon;


	private bool alignmentHitbool = false;

	// GameOverしてtitleScene読み込みに入っていた場合true
	private bool alreadyGameOver = false;




	// Use this for initialization
	void Start () {
		//fpsカメラ取得
		fpsCamera = GameObject.Find ("FirstPersonCharacter");
		//初期武器を装備
		WeaponPreparation ();

		//スコアテキスト取得
		scoreText = GameObject.Find("ScoreText");
			
	}
	
	// Update is called once per frame
	void Update () {

		//Gameover後のtitlescene読み込み中は何もさせない
		if (!alreadyGameOver) {

			if (Application.platform == RuntimePlatform.Android) {
				if (Input.GetKeyDown (KeyCode.Escape)) {
					DisplayTitleBackDialog ();
				}
			}

			if (Time.timeScale != 0) {
			
				//照準ray作成
				Ray alignmentRay = new Ray (fpsCamera.transform.position, fpsCamera.transform.forward);
				//照準raycasthitを入れる
				RaycastHit alignmentHit;

				// 照準Rayが衝突したら
				if (Physics.Raycast (alignmentRay, out alignmentHit, 2000)) {
					GameObject alignmentHitObject = alignmentHit.collider.gameObject;
					if (alignmentHitObject.tag == "Target") {
						alignmentHitbool = true;
					} else {
						alignmentHitbool = false;
					}
				} else {
					alignmentHitbool = false;
				}

				//タッチで操作
				foreach (Touch t in Input.touches) {

					//武器強化画面
					if (t.position.y < moveUIyPosTo && t.position.x < moveUIxPosFrom) {
						if (t.phase == TouchPhase.Began) {
							// scoreとUIを非表示
							//CanvasScore.SetActive (false);
							CanvasUI.SetActive (false);
							//現在武器を破棄
							Destroy (currentWeapon);
							//powerUp画面を表示
							CanvasPowerUp.SetActive (true);
							//現在武器のパラメータを反映
							CanvasPowerUpParaSet ();

							//display武器を表示
							DisplayWeaponPreparation ();
							//time.timescaleを0に
							Time.timeScale = 0;


						}
					}

					//武器切り替え
					if (t.position.y < moveUIyPosTo && t.position.x > (Screen.width - moveUIxPosFrom)) {
						if (t.phase == TouchPhase.Began) {
							//現在武器を破棄
							Destroy (currentWeapon);
							//次の武器を選択、最後の武器の場合は最初の武器に戻る
							if (selectWeapon < Weapon.Length - 1) {
								selectWeapon += 1;
							} else {
								selectWeapon = 0;
							}
							//選択武器を装備
							WeaponPreparation ();
						}
					}

					if (t.position.y > moveUIyPosTo) {
						if (tapStart) {
							fromTapTime += Time.deltaTime;
							if (fromTapTime < 0.2f && shootWaitTime > weaponWait) {
								if (t.phase == TouchPhase.Began) {
									WeaponShoot ();
									shootWaitTime = 0.00f;
									tapStart = false;
									fromTapTime = 0.00f;
								}
							} else {
								// reset
								tapStart = false;
								fromTapTime = 0.00f;
							}
						} else {
							if (t.phase == TouchPhase.Began) {
								tapStart = true;
							}
						}
					}
				}	

				shootWaitTime += Time.deltaTime;

			}
		}
	}

	void CanvasPowerUpParaSet () {
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/WeaponNameText").GetComponent<Text> ().text = currentWeapon.tag;
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/AttackPowerText").GetComponent<Text> ().text = "Attack Power : " + weaponPower + " damages";
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/WaitTimeText").GetComponent<Text> ().text = "Wait Time : " + weaponWait + " seconds";
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/ReachDistanceText").GetComponent<Text> ().text = "Reach Distance : " + weaponRange + " meters";
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/ExplosionRangeText").GetComponent<Text> ().text = "Explosion Range : " + weaponExRange + " times";
	}



	//powerUp画面Back
	public void PowerUpBack () {
		//powerUp画面を非表示
		CanvasPowerUp.SetActive (false);
		//現在display武器を破棄
		Destroy (currentDisplayWeapon);
		// scoreとUIを表示
		//CanvasScore.SetActive (true);
		//don't haveテキストを非表示
		CanvasDontHaveDialog.SetActive(false);
		CanvasPowerUpDialog.SetActive (false);
		CanvasTitleBackDialog.SetActive (false);
		CanvasUI.SetActive (true);
		//現在武器を装備
		WeaponPreparation ();
		//time.timescaleを1に
		Time.timeScale = 1.0f;
	}


	//武器装備
	void WeaponPreparation() {
		//武器生成
		GameObject weapon = Instantiate (Weapon [selectWeapon]) as GameObject;
		//生成した武器をfpsカメラの子オブジェクトにして装備
		weapon.transform.SetParent (fpsCamera.transform, false);
		//現在武器を保持
		currentWeapon = weapon;
		//現在銃口を保持
		currentMuzzle = GameObject.Find ("/FPSController/FirstPersonCharacter/" + currentWeapon.name + "/Shell");
		//現在武器のflashエフェクトを取得
		flashEffect = GameObject.Find ("/FPSController/FirstPersonCharacter/" + currentWeapon.name + "/FlashEffect");
		//武器情報取得
		WeaponSetting(weapon.tag);
		GameObject.Find ("/CanvasWeaponSwapAudio").GetComponent<AudioSource> ().Play ();
	}

	//display武器表示
	void DisplayWeaponPreparation() {
		//display武器生成
		GameObject displayWeapon = Instantiate (DisplayWeapon [selectWeapon]) as GameObject;
		//生成した武器をfpsカメラの子オブジェクトにして配置
		displayWeapon.transform.SetParent (fpsCamera.transform, false);
		//現在display武器を保持
		currentDisplayWeapon = displayWeapon;
	}



	//射撃
	void WeaponShoot() {
		//現在武器の射撃Animationを取得、再生
		//currentWeapon.GetComponent <Animation> ().Play ();
		//現在武器のflashエフェクトのパーティクルを取得、再生
		flashEffect.GetComponent <ParticleSystem> ().Play ();
		//現在武器の射撃音を取得、再生
		currentWeapon.GetComponent<AudioSource> ().Play();

		//銃弾ray作成
		Ray bulletRay = new Ray (fpsCamera.transform.position, fpsCamera.transform.forward);
		//銃弾raycasthitを入れる
		RaycastHit bulletHit;

		// 銃弾Rayが衝突したら
		if (Physics.Raycast (bulletRay, out bulletHit, weaponRange)) {
			// Rayが当たった地点にhitエフェクトを発生させる
			GameObject hitEffect = Instantiate (HandgunHitEffectPrefab, bulletHit.point, Quaternion.identity) as GameObject;
			hitEffect.GetComponent<ParticleSystem> ().Play ();

			// Rayの衝突相手
			GameObject hitObject = bulletHit.collider.gameObject;

			// 銃弾がターゲットに当たった時の処理
			if (hitObject.tag == "Target") {
				//weaponPowerの分だけターゲットHPを減らす
				hitObject.GetComponent<Para> ().HP -= weaponPower;
				//HP残量をHPバーに反映
				hitObject.transform.Find ("HPgagePrefab/Panel/Slider").GetComponent<Slider> ().value = (hitObject.GetComponent<Para> ().HP * 100) / hitObject.GetComponent<Para> ().MaxHP;

				//ターゲットを破壊した(HPを0にした)時の処理
				if (hitObject.GetComponent<Para> ().HP < 0) {
					//破壊したターゲットの位置に爆発パーティクルを発生させる
					GameObject.Find("/ExplosionPlay").GetComponent<ExplosionPlayer> ().ExpParticleRange = weaponExRange;
					GameObject.Find ("/ExplosionPlay").GetComponent<ExplosionPlayer> ().HitExplosionPlay (hitObject);

					//sphere生成
					GameObject expSphere = Instantiate (ExplosionSphere, hitObject.transform.position, Quaternion.identity) as GameObject;
					//sphereの大きさ(10の1/10000)にpara.scaleを掛けておく
					expSphere.transform.localScale = expSphere.transform.localScale * hitObject.GetComponent<Para> ().Scale;
					//sphere内の爆発規模の変数に武器のExRangeを代入
					expSphere.GetComponent<ExpSphereController> ().ExpSphExpansionRate = weaponExRange;

					//破壊したターゲットのMaXHP分スコアを加算し、現在スコアの表示を更新
					Score += hitObject.GetComponent<Para> ().MaxHP;
					scoreText.GetComponent<Text> ().text = "Score : " + Score + "pt";

					//破壊したターゲットオブジェクトを破棄
					Destroy (hitObject);
				}
			}
		} else if (alignmentHitbool) {
			//銃弾は届いてないが照準はターゲットにあったってる場合、届いてない旨を表示する
			CanvasCantReach.SetActive (true);
			Invoke ("DelayCanvasCantReachOff", 1);
		}
	}

	void DelayCanvasCantReachOff () {
		CanvasCantReach.SetActive(false);
	}


	void WeaponSetting(string tag) {
		//タグによって現在武器のパラメータをセッティング
		switch (tag) {
		case "Handgun":
			weaponPower = handgunPower;	
			weaponWait = handgunWait;
			weaponRange = handgunRange;
			weaponExRange = handgunExRange;
			break;
		case "Rifle":
			weaponPower = riflePower;	
			weaponWait = rifleWait;
			weaponRange = rifleRange;
			weaponExRange = rifleExRange;
			break;
		default:
			break;
		}
	}

	//ランダムで武器パラメータアップ
	public void WeaponRandomUp() {

		if (GameObject.Find ("/FPSController").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().GoldScore < 10) {
			CanvasDontHaveDialog.SetActive (true);
		} else {

			GameObject.Find ("/FPSController").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().GoldScore -= 10;
			GameObject.Find ("/CanvasScore/GoldScoreText").GetComponent<Text> ().text = "Gold : " + GameObject.Find ("/FPSController").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().GoldScore + "G";

			switch (currentDisplayWeapon.tag) {
			case "Handgun":
				tempweaponPower = handgunPower;
				tempweaponWait = handgunWait;
				tempweaponRange = handgunRange;
				tempweaponExRange = handgunExRange;

				handgunPower += Random.Range (1, 5) * 2;
				if (handgunWait >= 0.2f) {
					waitInt = Mathf.RoundToInt (handgunWait * 10);
					waitInt -= Random.Range (0, 2) * 2;
					handgunWait = ((float)waitInt) / 10.0f;
				}
				handgunRange += Random.Range (1, 5) * 1;
				ExRangeInt = Mathf.RoundToInt (handgunExRange * 10);
				ExRangeInt += Random.Range (1, 3) * 2;
				handgunExRange = ((float)ExRangeInt) / 10.0f;
				break;
			case "Rifle":
				tempweaponPower = riflePower;
				tempweaponWait = rifleWait;
				tempweaponRange = rifleRange;
				tempweaponExRange = rifleExRange;

				riflePower += Random.Range (5, 11) * 2;
				if (rifleWait >= 0.2f) {
					waitInt = Mathf.RoundToInt(rifleWait * 10);
					waitInt -= Random.Range (0, 2) * 2;
					rifleWait = ((float)waitInt) / 10.0f;
				}
				rifleRange += Random.Range (1, 5) * 3;
				ExRangeInt = Mathf.RoundToInt (rifleExRange * 10);
				ExRangeInt += Random.Range (1, 3);
				rifleExRange = ((float)ExRangeInt) / 10.0f;
				break;
			default:
				break;
			}

			WeaponSetting (currentDisplayWeapon.tag);

			CanvasPowerUpAfterTextSet ();

			CanvasPowerUp.GetComponent<AudioSource> ().Play ();
	
		}

		CanvasPowerUpDialog.SetActive (false);

	}

	//武器強化後のtext表示
	void CanvasPowerUpAfterTextSet () {
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/AttackPowerText").GetComponent<Text> ().text = "Attack Power : " + tempweaponPower + " >> " + weaponPower +" damages";
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/WaitTimeText").GetComponent<Text> ().text = "Wait Time : " + tempweaponWait + " >> " + weaponWait + " seconds";
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/ReachDistanceText").GetComponent<Text> ().text = "Reach Distance : " + tempweaponRange + " >> " + weaponRange + " meters";
		GameObject.Find ("/CanvasPowerUp/PowerUpPanel/ExplosionRangeText").GetComponent<Text> ().text = "Explosion Range : " + tempweaponExRange + " >> " + weaponExRange + " times";
	}

	//強化確認ダイアログ表示
	public void DisplayPowerUpDialog () {
		CanvasPowerUpDialog.SetActive (true);
		CanvasDontHaveDialog.SetActive (false);
		CanvasTitleBackDialog.SetActive (false);
	}

	//強化確認ダイアログキャンセル
	public void DisplayPowerUpDialogCancel () {
		CanvasPowerUpDialog.SetActive (false);
		CanvasDontHaveDialog.SetActive (false);
		CanvasTitleBackDialog.SetActive (false);
	}

	public void DontHaveDialogOK () {
		CanvasDontHaveDialog.SetActive (false);
	}

	public void DisplayTitleBackDialog() {
		CanvasTitleBackDialog.SetActive (true);
	}

	public void DisplayTitleBackDialogCancel () {
		CanvasTitleBackDialog.SetActive (false);
	}

	public void TitleBackLoadScene () {
		CanvasTitleBackDialog.SetActive (false);
		CanvasPowerUpDialog.SetActive (false);
		CanvasDontHaveDialog.SetActive (false);
		CanvasPowerUp.SetActive (false);
		Destroy (currentDisplayWeapon);
		Time.timeScale = 1.0f;
		GameObject.Find ("/ExplosionPlay").GetComponent<ExplosionPlayer> ().ScoreSaveLoadTitle ();
		alreadyGameOver = true;
	}


}
