using EcoGotchi.UI;
using Polycular.Clustar.Behaviours;
using UnityEngine;

namespace EcoGotchi.Behaviours
{
	public class EndBehaviour : BehaviourBase
	{
		public NoesisGuiController GuiController;
		public EndView View;
		public HeaderbarView HeaderView;
		public Camera IntroCamera;
		public Torby Torby;
		public float TeleportWaitMultiplier;
		public GameObject TeleporterObj;


		void OnEnable()
		{
			HeaderView.SetStatBoxesActive(false);

			View.FetchFromCharacterModel();
			View.SetScore(DataHub.Instance.Score);

			IntroCamera.gameObject.SetActive(true);
			Torby.gameObject.SetActive(true);
			TeleporterObj.SetActive(true);
			Torby.StartAnimation(AnimType.JUMP);

			GuiController.SetViewVisible(View);
		}
	}
}