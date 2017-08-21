using EcoGotchi.Models;
using Noesis;
using Polycular;
using UnityEngine;

namespace EcoGotchi.UI
{
	public class EndView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "End.xaml"; }
		}

		StatBox ComfortBox
		{
			get { return NoesisUtil.FetchElement<StatBox>(Root, "StBox_Comfort"); }
		}

		StatBox HealthBox
		{
			get { return NoesisUtil.FetchElement<StatBox>(Root, "StBox_Health"); }
		}

		StatBox CreditsBox
		{
			get { return NoesisUtil.FetchElement<StatBox>(Root, "StBox_Credits"); }
		}

		TextBlock Score
		{
			get { return NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Score"); }
		}

		public Character CharacterModel { private get; set; }


		void Awake()
		{
			Eventbus.Instance.AddListener<GameLoadedEvent>(ev =>
			{
				ComfortBox.MaxValue = 10;
				HealthBox.MaxValue = 10;
				CreditsBox.MaxValue = 10;

				FetchFromCharacterModel();

			}, this);
		}

		void Start()
		{
			MainGrid.Width = NoesisUtil.GetWidth();
		}

		public void FetchFromCharacterModel()
		{
			if (CharacterModel == null)
			{
				Debug.LogWarning("Character in EndView is null");
				return;
			}

			ComfortBox.Value = CharacterModel.Comfort;
			HealthBox.Value = CharacterModel.Health;
			CreditsBox.Value = CharacterModel.Credits;
		}

		public void SetScore(int score)
		{
			Score.Text = score.ToString();
		}
	}
}