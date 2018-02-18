using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

		// 追加
		private Vector3 initTouchPos;

		private float yRot = 0;
		private float xRot = 0;

		//moveUIとの境目(y軸)
		private float moveUIyPosTo = (1f / 4f) * (float)Screen.height;

		private bool controllCamera = false;


		private bool tapStart = false;

		private float fromTapTime = 0.00f;



        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }


        public void LookRotation(Transform character, Transform camera)
        {

			foreach (Touch t in Input.touches) {
				
				for (int i = 0; i < Input.touchCount; i++) {
					
					if (t.position.y > moveUIyPosTo) {
						// moveUIより上に触れている場合カメラ回転
	
						controllCamera = true;

						if (t.phase == TouchPhase.Began) {
							tapStart = true;
						}

						if (tapStart) {
							fromTapTime += Time.deltaTime;

							if (t.phase == TouchPhase.Ended) {
								fromTapTime = 0.00f;
								tapStart = false;
							}



							if (fromTapTime >= 0.1f) {
								// 指を動かしている間カメラ回転
								if (Time.timeScale != 0) {
									if (t.phase == TouchPhase.Moved) {
										//                                 ↓sensitivity
										xRot = t.deltaPosition.y * 0.1f * 1.0f;
										yRot = t.deltaPosition.x * 0.1f * 1.0f;

									}
								}
							}
						}
					

						if (t.phase != TouchPhase.Moved) {
							xRot = 0;
							yRot = 0;
						}
					}

					//ポーズ時カメラ回転を止める
					if (Time.timeScale == 0) {
						xRot = 0;
						yRot = 0;
					}

					//1本指で操作している時、カメラ操作から移動UIに指が移動してきたらカメラ回転を止める
					if (controllCamera && Input.touchCount == 1) {
						if (t.position.y < moveUIyPosTo) {
							xRot = 0;
							yRot = 0;
							controllCamera = false;
						}
					}

				}
			}

            m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

            if(smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
