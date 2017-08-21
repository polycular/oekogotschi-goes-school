using EcoGotchi.UI;
using Polycular;
using Polycular.Clustar.Behaviours;
using Polycular.Utilities;
using UnityEngine;
using Vuforia;
using Component = EcoGotchi.Models.Component;

namespace EcoGotchi.Behaviours
{
	public class IntroBehaviour : BehaviourBase
	{
		[Button(true, "NotifyComponentFinished", "Skip")]
		public bool Skip;

		public NoesisGuiController GuiController;
		public IntroView View;
		public Camera IntroCamera;
		public Torby Torby;
		public float TeleportWaitMultiplier;
		public GameObject TeleporterObj;

		TeleportEffect m_teleport;

		int m_textIdx;
		Component m_rawComponent
		{
			get
			{
				return Component as Component;
			}
		}


		void Awake()
		{
			m_teleport = Torby.GetComponent<TeleportEffect>();
		}

		void OnEnable()
		{
			var objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			objTracker.Stop();

			TeleporterObj.SetActive(true);
			m_textIdx = -1;
			UpdateText();
			Eventbus.Instance.AddListener<ButtonTappedEvent>(OnTappedHandler, this);

			GuiController.SetViewVisible(View);

			IntroCamera.gameObject.SetActive(true);
			Torby.gameObject.SetActive(true);
			Torby.StartAnimation(AnimType.WAVE);

			if (!string.IsNullOrEmpty(m_rawComponent.Topic))
				View.TopicTxtBlock.Text = m_rawComponent.Topic;
		}

		void OnDisable()
		{
			TeleporterObj.SetActive(false);

			Eventbus.Instance.RemoveListener<ButtonTappedEvent>(this);

			IntroCamera.gameObject.SetActive(false);
			Torby.gameObject.SetActive(false);

			View.TopicTxtBlock.Text = "";
		}
		void NotifyComponentFinished()
		{
			base.Notify(this);
		}

		void OnTappedHandler(EventBase ev)
		{
			var btnType = (ev as ButtonTappedEvent).BtnType;

			if (btnType == ButtonTappedEvent.ButtonType.TEXT || btnType == ButtonTappedEvent.ButtonType.CONTINUE)
				UpdateText();
		}

		void UpdateText()
		{
			if (m_rawComponent.Texts.Count > ++m_textIdx)
			{
				View.IntroTxtBlock.Text = m_rawComponent.Texts[m_textIdx];
			}
			else
			{
				Eventbus.Instance.RemoveListener<ButtonTappedEvent>(this);
				m_teleport.StartEffect();
				Invoke("NotifyComponentFinished", m_teleport.AnimTime * TeleportWaitMultiplier);
			}
		}
	}
}
