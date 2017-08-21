using EcoGotchi.Models;
using Noesis;
using Polycular;
using UnityEngine;

namespace EcoGotchi.UI
{
	public class HeaderbarView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "Headerbar.xaml"; }
		}

		public override bool IsOverlay
		{
			get { return true; }
		}

		TextBlock m_score;
		public TextBlock Score
		{
			get
			{
				if (m_score == null)
					m_score = NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Score");

				return m_score;
			}
		}

		StackPanel m_countdownBox;
		public StackPanel CountdownBox
		{
			get
			{
				if (m_countdownBox == null)
					m_countdownBox = NoesisUtil.FetchElement<StackPanel>(Root, "Sp_Countdown");

				return m_countdownBox;
			}
		}

		Grid m_statsBox;
		public Grid StatsBox
		{
			get
			{
				if (m_statsBox == null)
					m_statsBox = NoesisUtil.FetchElement<Grid>(Root, "Gr_StatBoxes");

				return m_statsBox;
			}
		}

		Button m_btnSW;
		public Button SandwichButton
		{
			get
			{
				if (m_btnSW == null)
					m_btnSW = NoesisUtil.FetchElement<Button>(Root, "Btn_Sandwich");

				return m_btnSW;
			}
		}

		Button m_btnContextReset;
		Button SwContextResetBtn
		{
			get
			{
				if (m_btnContextReset == null)
					m_btnContextReset = NoesisUtil.FetchElement<Button>(Root, "Btn_Context_Reset");

				return m_btnContextReset;
			}
		}

		Button m_btnContextExit;
		Button SwContextExitBtn
		{
			get
			{
				if (m_btnContextExit == null)
					m_btnContextExit = NoesisUtil.FetchElement<Button>(Root, "Btn_Context_Exit");

				return m_btnContextExit;
			}
		}

		Grid m_swContextGrid;
		public Grid SwContextGrid
		{
			get
			{
				if (m_swContextGrid == null)
					m_swContextGrid = NoesisUtil.FetchElement<Grid>(Root, "Gr_SandwichContent");

				return m_swContextGrid;
			}
		}

		Button m_swBg;
		public Button SwBg
		{
			get
			{
				if (m_swBg == null)
					m_swBg = NoesisUtil.FetchElement<Button>(Root, "Btn_Bg");

				return m_swBg;
			}
		}

		TextBlock m_remainingTime;
		public TextBlock RemainingTime
		{
			get { return m_remainingTime ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_TimeRemaining"); }
		}

		TextBlock m_gamePoints;
		public TextBlock GamePoints
		{
			get
			{
				if (m_gamePoints == null)
					m_gamePoints = NoesisUtil.FetchElement<TextBlock>(Root, "Tb_GamePoints");

				return m_gamePoints;
			}
		}

		StatBox m_comfortBox;
		StatBox ComfortBox
		{
			get { return m_comfortBox ?? NoesisUtil.FetchElement<StatBox>(Root, "StBox_Comfort"); }
		}

		StatBox m_healthBox;
		StatBox HealthBox
		{
			get { return m_healthBox ?? NoesisUtil.FetchElement<StatBox>(Root, "StBox_Health"); }
		}

		StatBox m_creditsBox;
		StatBox CreditsBox
		{
			get { return m_creditsBox ?? NoesisUtil.FetchElement<StatBox>(Root, "StBox_Credits"); }
		}

		public Character CharacterModel { private get; set; }

		public DataHub Datahub;
		public MinigameInfo MinigameInfo;


		void Awake()
		{
			Eventbus.Instance.AddListener<GameLoadedEvent>(ev =>
			{
				FetchFromCharacterModel();
			}, this);
		}

		void Start()
		{
			MainGrid.Width = NoesisUtil.GetWidth();

			ComfortBox.MaxValue = 10f;
			HealthBox.MaxValue = 10f;
			CreditsBox.MaxValue = 10f;

			SwBg.Visibility = NoesisUtil.BoolToVis(false);

			// simulates focus loss using filling bg button
			SwBg.Click += (sender, args) =>
			{
				SetSandwichMenuActive(false);
			};

			SwContextResetBtn.Click += (sender, args) =>
			{
				Eventbus.Instance.FireEvent<ResetGameEvent>(new ResetGameEvent());
			};

			SwContextExitBtn.Click += (sender, args) =>
			{
				SetSandwichMenuActive(false);
				Application.Quit();
			};

			SandwichButton.Click += (sender, args) =>
			{
				SetSandwichMenuActive(true);
			};
		}

		public override void EstablishBindings()
		{
			// Score
			Binding bScore = new Binding();
			bScore.Source = Datahub;
			bScore.Path = new PropertyPath("Score");
			bScore.Mode = BindingMode.OneWay;
			bScore.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			Score.SetBinding(TextBlock.TextProperty, bScore);


			// Time
			Binding bTime = new Binding();
			bTime.Source = MinigameInfo;
			bTime.Path = new PropertyPath("Time");
			bTime.Mode = BindingMode.OneWay;
			bTime.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			RemainingTime.SetBinding(TextBlock.TextProperty, bTime);

			// Minigame Score
			Binding bMinigameScore = new Binding();
			bMinigameScore.Source = MinigameInfo;
			bMinigameScore.Path = new PropertyPath("Score");
			bMinigameScore.Mode = BindingMode.OneWay;
			bMinigameScore.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			GamePoints.SetBinding(TextBlock.TextProperty, bMinigameScore);
		}

		public void SetActive(bool active)
		{
			base.MainGrid.Visibility = NoesisUtil.BoolToVis(active);
		}

		public void SetTimeContainerActive(bool active)
		{
			CountdownBox.Visibility = NoesisUtil.BoolToVis(active);
			StatsBox.Visibility = NoesisUtil.BoolToVis(!active);
		}

		public void SetStatBoxesActive(bool active)
		{
			StatsBox.Visibility = NoesisUtil.BoolToVis(active);
		}

		public void UpdateStatBoxes(float comfortChange, float healthChange, float creditsChange)
		{
			ComfortBox.Value += comfortChange;
			HealthBox.Value += healthChange;
			CreditsBox.Value += creditsChange;

			UpdateCharacterModel();
		}

		void UpdateCharacterModel()
		{
			if (CharacterModel == null)
			{
				Debug.LogWarning("Character in Headerbar is null");
				return;
			}

			CharacterModel.Comfort = (int)ComfortBox.Value;
			CharacterModel.Health = (int)HealthBox.Value;
			CharacterModel.Credits = (int)CreditsBox.Value;

			DataHub.Instance.Save(CharacterModel);
			UpdateComfortIdle();
		}

		void FetchFromCharacterModel()
		{
			if (CharacterModel == null)
			{
				Debug.LogWarning("Character in Headerbar is null");
				return;
			}

			ComfortBox.Value = CharacterModel.Comfort;
			HealthBox.Value = CharacterModel.Health;
			CreditsBox.Value = CharacterModel.Credits;

			UpdateComfortIdle();
		}

		void UpdateComfortIdle()
		{
			if (CharacterModel.Comfort < 5)
				Eventbus.Instance.FireEvent<IdleStateEvent>(new IdleStateEvent(IdleState.SAD));
			else
				Eventbus.Instance.FireEvent<IdleStateEvent>(new IdleStateEvent(IdleState.HAPPY));
		}

		public void GetStats(out float comfort, out float health, out float credits)
		{
			comfort = ComfortBox.Value;
			health = HealthBox.Value;
			credits = CreditsBox.Value;
		}

		void SetSandwichMenuActive(bool active)
		{
			SwContextGrid.Visibility = NoesisUtil.BoolToVis(active);
			SwBg.Visibility = NoesisUtil.BoolToVis(active);
		}
	}
}