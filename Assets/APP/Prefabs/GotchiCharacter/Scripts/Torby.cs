using System;
using System.Collections.Generic;
using UnityEngine;
using Polycular.Utilities;
using Polycular;

namespace EcoGotchi
{
	public enum IdleState
	{
		//NEUTRAL,
		HAPPY,
		SAD,
		EXHAUSTED,
	}

	public enum AnimType
	{
		JUMP,
		WIN,
		LOSE,
		POINT,
		WAVE,
		YAWN,
		SPAWN,
		JUMP_FAST,
		THINKING,
		ANGRY
	}

	public class Torby : MonoBehaviour
	{
#if UNITY_EDITOR
		public AnimType DbgAnim;

		[Button(true, "DBGStartAnim", "Start Animation")]
		public bool m_dbgAnimStart;
		void DBGStartAnim()
		{
			StartAnimation(DbgAnim);
		}

		public IdleState DbgIdle;

		[Button(true, "DBGSetIdle", "Set Idle State")]
		public bool m_dbgIdleSet;
		void DBGSetIdle()
		{
			SetIdleState(DbgIdle);
		}
#endif

		CharacterController m_charCtrl;
		Animator m_animator;

		Dictionary<IdleState, int> m_idleToHash;
		Dictionary<AnimType, int> m_animToHash;

		Queue<AnimType> m_queuedAnimations;

		IdleState m_currentIdle;

		bool m_animationsEnabled;
		public bool AnimationsEnabled
		{
			get { return m_animationsEnabled; }
			set
			{
				m_animationsEnabled = value;

				if (m_animationsEnabled)
					PlayOutstandingAnimations();
			}
		}

		void Awake()
		{
			m_animator = GetComponent<Animator>();
			m_idleToHash = new Dictionary<IdleState, int>();
			m_animToHash = new Dictionary<AnimType, int>();
			m_queuedAnimations = new Queue<AnimType>();

			foreach (IdleState state in Enum.GetValues(typeof(IdleState)))
			{
				m_idleToHash.Add(state, Animator.StringToHash(state.ToString()));
			}

			foreach (AnimType anim in Enum.GetValues(typeof(AnimType)))
			{
				m_animToHash.Add(anim, Animator.StringToHash(anim.ToString()));
			}

			Eventbus.Instance.AddListener<IdleStateEvent>(ev => {
				SetIdleState((ev as IdleStateEvent).IdleState);
			}, this);

			Eventbus.Instance.AddListener<RequestAnimationEvent>(ev => {
				m_queuedAnimations.Enqueue((ev as RequestAnimationEvent).Animation);
				PlayOutstandingAnimations();
			}, this);
		}

		void Start()
		{
			
		}

		void OnEnable()
		{
			SetIdleState(m_currentIdle);
		}

		void OnDisable()
		{
			AnimationsEnabled = false;
		}

		void OnDestroy()
		{
			Eventbus.Instance.RemoveListener(this);
		}

		public void SetIdleState(IdleState state)
		{
			foreach (var kvp in m_idleToHash)
			{
				if (kvp.Key == state)
					m_animator.SetBool(kvp.Value, true);
				else
					m_animator.SetBool(kvp.Value, false);
			}

			m_currentIdle = state;
		}

		public void StartAnimation(AnimType type)
		{
			m_animator.SetTrigger(m_animToHash[type]);
		}

		public void PlayOutstandingAnimations()
		{
			if (!AnimationsEnabled)
				return;

			while (m_queuedAnimations.Count > 0)
			{
				var anim = m_queuedAnimations.Dequeue();
				StartAnimation(anim);
			}
		}
	}
}
