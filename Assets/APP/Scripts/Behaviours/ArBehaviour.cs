using EcoGotchi.Models;
using EcoGotchi.UI;
using Noesis;
using Polycular;
using Polycular.Clustar.Behaviours;
using Polycular.Utilities;
using UnityEngine;
using Vuforia;
using Component = EcoGotchi.Models.Component;

namespace EcoGotchi.Behaviours
{
	public class ArBehaviour : BehaviourBase
	{
		[Button(true, "NotifyComponentFinished", "Skip")]
		public bool Skip;

		public GameObject TorbyObject;
		public GameObject TeleporterObject;
		public NoesisGuiController GuiController;
		public ArOverlayView View;
		public TutorialView TutorialView;
		public HeaderbarView HeaderView;

		public ARMediator ARMediator;
		public Camera NoesisCam;

		int m_textIdx;
		Component m_rawComponent
		{
			get
			{
				return Component as Component;
			}
		}

		Consequence m_bufferedConsequences;
		Torby m_torby;


		void Awake()
		{
			Eventbus.Instance.AddListener<DecisionMadeEvent>((ev) =>
			{
				var c = (ev as DecisionMadeEvent).ChosenConsequence;
				m_bufferedConsequences = c as Consequence;

			}, this);

			Eventbus.Instance.AddListener<CamReadyEvent>((ev) =>
			{
				View.Loading.Visibility = Visibility.Hidden;
			}, this);

			m_torby = TorbyObject.GetComponent<Torby>();
		}

		void OnEnable()
		{
			ObjectTracker objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			objTracker.Start();

			var storedConsequence = m_rawComponent.ChosenConsequence;

			// restore / save
			if (m_bufferedConsequences == null && storedConsequence != null)
			{
				m_bufferedConsequences = storedConsequence;
			}
			else
			{
				m_rawComponent.ChosenConsequence = m_bufferedConsequences;
				DataHub.Instance.Save(m_rawComponent);
			}

			View.ScanPromptText.Text = m_rawComponent.PageNumber.ToString();
			View.Init();
			GuiController.SetViewVisible(View);

			Eventbus.Instance.AddListener<ARTrackableStateChangedEvent>(OnArTrackableStateChangedHandler, this);
			Eventbus.Instance.AddListener<ButtonTappedEvent>(OnButtonTappedHandler, this);

			ARMediator.SetActiveImageTarget(m_rawComponent.Markername, true);
		}

		void OnDisable()
		{
			ObjectTracker objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			objTracker.Stop();

			TorbyObject.SetActive(false);
			TeleporterObject.SetActive(false);

			View.OnHelpButtonClick -= EnableHelpOverlay;

			Eventbus.Instance.RemoveListener<ARTrackableStateChangedEvent>(this);
			Eventbus.Instance.RemoveListener<ButtonTappedEvent>(this);

			ARMediator.SetActiveImageTarget(m_rawComponent.Markername, false);
		}

		void NotifyComponentFinished()
		{
			base.Notify(this);
		}

		void ProcessConsequences()
		{
			if (m_bufferedConsequences == null)
				return;

			View.HeaderView.UpdateStatBoxes(m_bufferedConsequences.Comfort, m_bufferedConsequences.Health, m_bufferedConsequences.Credits);

			// set text from buffered consequence
			View.SetDecisionFeedback(m_bufferedConsequences.Header, m_bufferedConsequences.Texts[0]);

			int comfort = m_bufferedConsequences.Comfort;
			View.DescrComfortChange.Text = comfort > 0 ? "+" + comfort.ToString() : comfort.ToString();

			int health = m_bufferedConsequences.Health;
			View.DescrHealthChange.Text = health > 0 ? "+" + health.ToString() : health.ToString();

			int credits = m_bufferedConsequences.Credits;
			View.DescrCreditstChange.Text = credits > 0 ? "+" + credits.ToString() : credits.ToString();

			// reset applied consequence
			m_bufferedConsequences = null;
			m_rawComponent.ChosenConsequence = null;
			DataHub.Instance.Save(m_rawComponent);
		}

		void OnArTrackableStateChangedHandler(EventBase ev)
		{
			var evnt = (ev as ARTrackableStateChangedEvent);

			bool isTracked = evnt.IsTracked;
			string markerName = evnt.MarkerName;

			bool valid = isTracked && m_rawComponent.Markername == markerName;

			TeleporterObject.SetActive(valid);
			TorbyObject.SetActive(valid);
			m_torby.AnimationsEnabled = valid;


			if (!valid)
				return;

			View.SetOverlayActive(View.Outcome);

			View.OnHelpButtonClick -= EnableHelpOverlay;
			View.OnHelpButtonClick += EnableHelpOverlay;

			m_textIdx = -1;

			ProcessConsequences();

			// disabled until used - atm only consequence texts are used in the AR view
			//UpdateSbText();
		}

		void EnableHelpOverlay()
		{
			GuiController.SetViewVisible(TutorialView);
			TutorialView.SetExitButtonActive(true);

			TutorialView.ExitButtonClicked -= DisableHelpOverlay;
			TutorialView.ExitButtonClicked += DisableHelpOverlay;
		}

		void DisableHelpOverlay()
		{
			TutorialView.SetExitButtonActive(false);
			TutorialView.ExitButtonClicked -= DisableHelpOverlay;
			GuiController.SetViewVisible(View);
		}

		void OnButtonTappedHandler(EventBase ev)
		{
			var btnType = (ev as ButtonTappedEvent).BtnType;

			if (btnType == ButtonTappedEvent.ButtonType.TEXT)
			{
				UpdateSbText();
			}
			else if (btnType == ButtonTappedEvent.ButtonType.CONTINUE)
			{
				Eventbus.Instance.RemoveListener<ButtonTappedEvent>(this);
				NotifyComponentFinished();
			}
		}

		void UpdateSbText()
		{
			if (m_rawComponent.Texts.Count > ++m_textIdx)
			{
				View.SpeechText.Text = m_rawComponent.Texts[m_textIdx];
			}
			else
			{
				Eventbus.Instance.RemoveListener<ButtonTappedEvent>(this);
				NotifyComponentFinished();
			}
		}
	}
}