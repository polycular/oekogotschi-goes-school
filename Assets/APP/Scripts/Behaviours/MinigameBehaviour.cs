using EcoGotchi.UI;
using Noesis;
using Polycular;
using Polycular.Clustar.Behaviours;
using Polycular.Utilities;
using UnityEngine.SceneManagement;
using Vuforia;
using Component = EcoGotchi.Models.Component;

namespace EcoGotchi.Behaviours
{
	[System.Serializable]
	public class MinigameBehaviour : BehaviourBase, IBehaviour
	{
		[Button(true, "NotifyComponentFinished", "Skip")]
		public bool Skip;

		public NoesisGuiController GuiController;
		public MinigameView View;
		public MinigameEndscreenView EndView;
		public HeaderbarView HeaderView;

		public ARMediator ARMediator;

		Component m_rawComponent
		{
			get
			{
				return Component as Component;
			}
		}

		IGameDirector m_currentGameDirector;


		void Awake()
		{
			Eventbus.Instance.AddListener<CamReadyEvent>((ev) =>
			{
				View.Loading.Visibility = Visibility.Hidden;
			}, this);
		}

		void OnEnable()
		{
			ObjectTracker objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			objTracker.Start();

			Eventbus.Instance.AddListener<MinigameReadyEvent>(ev =>
			{
				m_currentGameDirector = (ev as MinigameReadyEvent).GameDirector;

				m_currentGameDirector.GameCompleted -= OnGameCompleted;
				m_currentGameDirector.GameCompleted += OnGameCompleted;

				if (m_rawComponent.Markername != null)
				{
					// get the minigame target marker from the component
					string targetMarkername = m_rawComponent.Markername;

					// fetch the trackable game object
					var trackable = ARMediator.GetImageTargetGameObject(targetMarkername);

					// add the "ImageTargetTracker" which is used for the minigames (could be refactored)
					m_currentGameDirector.ImgTargetTracker = trackable.AddComponent<ImageTargetTracker>();

					// enable the image target
					ARMediator.SetActiveImageTarget(targetMarkername, true);
				}

			}, this);

			Eventbus.Instance.AddListener<ScoreAchievedEvent>(ev => EndView.EcoPoints.Text = ((ev as ScoreAchievedEvent).MappedScore * DataHub.ScoreMultiplier).ToString(), this);

			Eventbus.Instance.AddListener<ButtonTappedEvent>(ev =>
			{
				var btnType = (ev as ButtonTappedEvent).BtnType;
				if (btnType == ButtonTappedEvent.ButtonType.CONTINUE)
					NotifyComponentFinished();

			}, this);

			GuiController.SetViewVisible(View);

			HeaderView.SetTimeContainerActive(true);

			SceneManager.LoadScene((Component as Models.Component).Name, LoadSceneMode.Additive);
		}

		void OnDisable()
		{
			ObjectTracker objTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			objTracker.Stop();

			HeaderView.SetTimeContainerActive(false);

			m_currentGameDirector.GameCompleted -= OnGameCompleted;
			m_currentGameDirector = null;

			SceneManager.UnloadScene((Component as Models.Component).Name);

			Eventbus.Instance.RemoveListener(this);
		}

		void OnGameCompleted()
		{
			if (m_rawComponent.Markername != null)
			{
				// remove the ImageTargetTracker script attached for the minigame to work
				var trackable = ARMediator.GetImageTargetGameObject(m_rawComponent.Markername);
				var imgTargetTracker = trackable.GetComponent<ImageTargetTracker>();
				Destroy(imgTargetTracker);

				ARMediator.SetActiveImageTarget(m_rawComponent.Markername, false);
			}

			GuiController.SetViewVisible(EndView);
		}

		void NotifyComponentFinished()
		{
			base.Notify(this);
		}
	}
}