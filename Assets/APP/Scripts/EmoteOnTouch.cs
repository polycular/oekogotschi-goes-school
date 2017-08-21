using Polycular;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace EcoGotchi
{
	/// <summary> Plays a random animation from a selection when the GameObject is touched.
	/// Does not work currently. </summary>
	public class EmoteOnTouch : MonoBehaviour
	{
		public GameObject ParticleObject;

		Camera m_arCamera;
		float m_cooldownMS;
		List<AnimType> m_anims = new List<AnimType>();
		ParticleSystem m_particles;
	
		void Start()
		{
			m_anims.Add(AnimType.JUMP);
			m_anims.Add(AnimType.WIN);
			m_anims.Add(AnimType.WAVE);
			m_anims.Add(AnimType.POINT);

			m_particles = ParticleObject.GetComponentInChildren<ParticleSystem>();
			m_particles.playOnAwake = false;
			ParticleObject.SetActive(true);
		}

		void Update()
		{
			if (m_cooldownMS > 0)
			{
				m_cooldownMS -= Time.deltaTime;

				if (m_cooldownMS < 0)
					m_cooldownMS = 0;

				return;
			}

			if (m_arCamera == null)
			{
				var vuf = FindObjectOfType<VuforiaBehaviour>();

				if (vuf == null)
					return;

				m_arCamera = vuf.GetComponentInChildren<Camera>();
			}

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			for (var i = 0; i < Input.touchCount; ++i)
			{
				if (Input.GetTouch(i).phase == TouchPhase.Began)
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
#elif UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE

			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = m_arCamera.ScreenPointToRay(Input.mousePosition);
#endif
					RaycastHit hitInfo;

					if (Physics.Raycast(ray, out hitInfo))
					{
						if (hitInfo.collider.gameObject == gameObject)
							Emote();
					}
				}
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			}
#endif
		}

		void Emote()
		{
			Eventbus.Instance.FireEvent<RequestAnimationEvent>(new RequestAnimationEvent(m_anims[Random.Range(0, m_anims.Count)]));
			m_particles.Play();
			m_cooldownMS = Random.Range(1f, 3f);
		}

		
	}
}
