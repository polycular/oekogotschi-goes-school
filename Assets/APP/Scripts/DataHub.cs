using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using EcoGotchi.Models;
using EcoGotchi.UI;
using JSONApi.Models;
using JSONApi.Services;
using Polycular;
using Polycular.Clustar;
using Polycular.Logging;
using Polycular.Persistance;
using Polycular.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Component = EcoGotchi.Models.Component;

namespace EcoGotchi
{
	public class ResetGameEvent : EventBase
	{
	}

	public class GameLoadedEvent : EventBase
	{
	}

	public class DataHub : MonoBehaviour, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public static DataHub Instance;
		public GraphController GraphController;

		public int MinPointsPerAction = 1;
		public int MaxPointsPerAction = 5;
		public static readonly int ScoreMultiplier = 100;

		Store m_store;
		Campaign m_campaign;

		float m_saveTimerMS;
		const float m_saveIntervalMS = 5000f;

		IPolycularLog m_logger;
		
		public int Score
		{
			get
			{
				// null check necessary for data binding
				if (m_campaign != null)
					return m_campaign.Score;
				else
				{
					return 0;
				}
			}
			set
			{
				int diff = value - m_campaign.Score;
				AddToScore(diff);
				OnPropertyChanged("Score");
			}
		}

		void Awake()
		{
			Instance = this;

			Eventbus.Instance.AddListener<ComponentCompletedEvent>(ComponentCompletedHandler, this);
			Eventbus.Instance.AddListener<ScoreAchievedEvent>(ev =>
			{
				Score += ((int)(ev as ScoreAchievedEvent).MappedScore);
			}, this);
			Eventbus.Instance.AddListener<ResetGameEvent>(ev => ResetGame(), this);
		}

		void Start()
		{	
			SetupLogging();

			m_store = new Store("EcoGotchi.Models");

			string relPath = @"CONTENT/new_content/main.json"; //@"CONTENT/Campaign.json";
			string path = StreamingAssetsUtil.GetPlatformPath(relPath);

			var www = new WWW(path);
			while (!www.isDone) ;
			string json = www.text;

			Storage.Load(m_store);

			if (m_store.Empty())
			{
				m_store.LoadFromJSON(json)
					.Then(models =>
					{
						m_campaign = models.Single(model => model is Campaign) as Campaign;
						GraphController.Init(m_campaign);
						PostInit();
					})
					.Catch((ex) => Debug.LogError(ex));
			}
			else
			{
				m_campaign = m_store.FindLocal("66891fc3-15e4-497b-b393-733a0e3be986") as Campaign;

				Restore();
				GraphController.Init(m_campaign);
				PostInit();
			}
		}

		void PostInit()
		{
			var character = m_store.FindLocal("5612c1f3-cee9-4a09-8214-8e3570199726") as Character;
			FindObjectOfType<HeaderbarView>().CharacterModel = character;
			FindObjectOfType<EndView>().CharacterModel = character;
			Eventbus.Instance.FireEvent<GameLoadedEvent>(new GameLoadedEvent());
		}

		void SetupLogging()
		{
			var fileLog = new FileLog();

			var filterLog = new TraceFilterLog(fileLog);
			filterLog.AddFilter("Vuforia");

			var consoleLog = new ConsoleLog(filterLog);
			(consoleLog as ConsoleLog).LogConstraint.EnableLogTypes(LogType.Log, LogType.Warning, LogType.Error);

			PolycularLog.AttachToUnity(consoleLog);
		}

		void OnDestroy()
		{
			PolycularLog.DetachFromUnity();
		}

		public void Save(IModel resource)
		{
			m_store.UpdateResource(resource);
			Storage.Save(m_store);
		}

		void Save()
		{
			if (m_store != null)
				Storage.Save(m_store);
		}

		public void AddToScore(int points)
		{
			if (points > 0)
			{
				m_campaign.Score += points * ScoreMultiplier;
				m_store.UpdateResource(m_campaign);

				Debug.LogFormat("{0}: new score: {1}", GetType().Name, m_campaign.Score);

				//if (points <= MaxPointsPerAction * 0.33)
				//{
				//	Eventbus.Instance.FireEvent<RequestAnimationEvent>(new RequestAnimationEvent(AnimType.WIN));
				//}
				//else if (points > MaxPointsPerAction * 0.33 && points <= MaxPointsPerAction * 0.66)
				//{
				//	Eventbus.Instance.FireEvent<RequestAnimationEvent>(new RequestAnimationEvent(AnimType.THINKING));
				//}
				//else if (points > MaxPointsPerAction * 0.66)
				//{
				//	Eventbus.Instance.FireEvent<RequestAnimationEvent>(new RequestAnimationEvent(AnimType.LOSE));
				//}
			}

			Save();
		}

		void Update()
		{
			m_saveTimerMS += (Time.deltaTime * 1000);

			if (m_saveTimerMS >= m_saveIntervalMS)
			{
				m_saveTimerMS = 0f;
				Save();
			}
		}

		void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		void Restore()
		{
			Score = m_campaign.Score;
		}

		void ComponentCompletedHandler(EventBase ev)
		{
			var cce = ev as ComponentCompletedEvent;

			var rawComponent = cce.Component as Component;
			rawComponent.Completed = DateTime.Now.ToUniversalTime().ToString("o");
			m_store.UpdateResource(rawComponent);

			var currentNode = m_campaign.Current;
			currentNode.CurrentOrderId = cce.Component.OrderId + 1;
			m_store.UpdateResource(currentNode);

			Save();
		}

		void ResetGame()
		{
			m_store.Reset();
			var savedStorePath = Path.Combine(Application.persistentDataPath, m_store.SavePath);
			File.Delete(savedStorePath);
			Eventbus.ClearBus();
			SceneManager.LoadScene("main");
		}
	}
}


