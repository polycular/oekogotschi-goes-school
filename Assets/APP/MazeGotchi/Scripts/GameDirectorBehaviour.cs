using System;
using EcoGotchi;
using Polycular;
using SharedAssets.GuiMain;
using UnityEngine;
using InteractionType = SharedAssets.GuiMain.InteractionSpriteListBehaviour.InteractionType;

namespace MazeGotchi
{
	public class GameDirectorBehaviour : MonoBehaviour, IGameDirector
	{
		public GameObject[] MazeLevelPrefabs;
		public GameObject MazeParent;
		public GameObject ItemParent;
		public GameObject EcoGotchiPrefab;
		public UILabel ItemDescriptionLabel;
		public int CurrentLevel = 1;
		public bool ShowTutorial = true, ShowSwitchGroupInfo = true;

		public SharedAssets.GuiMain.DialogueManagerBehaviour DialogueManager;
		public string IntroQuestDialogueName = "IntroQuestD";
		public string MoveTonsHint = "Tippe auf die Steinhände";
		public InteractionType MoveTonsHintIcon = InteractionType.TAP;
		public string TrapDoorHint = "Tippe auf Minti, das EcoGotchi";
		public InteractionType TrapDoorHintIcon = InteractionType.TAP;
		public string FindMarkerHint = "Halte die Kamera wieder auf den Marker";
		public InteractionType FindMarkerHintIcon = InteractionType.FIND_LOST_MARKER;
		public string FinishedTutorialDialogueName = "FinishedTutorialD";
		public string FinishedTutorialWrongBinDialogueName = "FinishedTutorialWrongBinD";
		public string FinishedLevelGoodDialogueName = "FinishedLevelGoodD";
		public string FinishedLevelBadDialogueName = "FinishedLevelBadD";
		public string NextLevelDialogueName = "NextLevelD";
		public string ReadyStartLevelDialogueName = "ReadyStartLevelD";
		public string SwitchGroupDialogueName = "SwitchGroupD";
		public string TryAgainDialogueName = "TryAgainD";
		public string OutroDialogueName = "OutroD";

		public string TopBarName = "TopBar";
		public int PlayTimeInSeconds = 60;
		public int MaxLevelsToPlay = 3;
		public float TrapDoorOneWayOpeningTimeInSeconds = 0.2f;
		public int MinTonMovementCountForTutorial = 1;
		public int PointsForCorrectItem = 70;
		public int PointsForIncorrectItem = -20;
		public int GoodLevelPointThreshold = 70;
		public ImageTargetTracker MazeImageTarget;
		public GuiInputHandlerBehaviour InputHandler;
		public BinManagerBehaviour BinManager;
		public TrashSpawnDecisorBehaviour SpawnDecisor;

		public event Action GameCompleted;

		bool mIsFirstTimePlaying = true, mLastDialogDecision;
		MazeBehaviour mCurrentMaze;
		GameObject mEcoGotchi;
		BinBehaviour[] mBins;
		TrashItemBehaviour mCurrentItem;
		SharedAssets.GuiMain.TopBarBehaviour mTopBar;
		int mPlaySecondsLeft;
		int mScore = 0;
		enum GameState { INITIALIZED, PREPARED, IN_TUTORIAL, IN_TUTORIAL_PART2, PLAYING, PAUSED, FINISHED };
		GameState mGameState = GameState.INITIALIZED;
		int mTutorialTonMovementCount = 0;
		bool mItemIsFalling = false;
		string mCurrentHint;
		InteractionType mCurrentHintIcon;
		int mLevelsPlayed = 0;

		MinigameInfo mMinigameInfo;

		public ImageTargetTracker ImgTargetTracker
		{
			get { return MazeImageTarget; }
			set { MazeImageTarget = value; }
		}

		void Start()
		{
			MazeImageTarget.onTrackableStateChange -= new ImageTargetTracker.TrackableStateChangeHandler(onTrackableStateChange);
			InputHandler.onLeftHandClick -= new GuiInputHandlerBehaviour.OnLeftHandClickHandler(onLeftHandClick);
			InputHandler.onRightHandClick -= new GuiInputHandlerBehaviour.OnRightHandClickHandler(onRightHandClick);
			InputHandler.onTrapDoorTrigger -= new GuiInputHandlerBehaviour.OnTrapDoorTriggerHandler(onTrapDoorTrigger);

			InputHandler.onLeftHandClick += new GuiInputHandlerBehaviour.OnLeftHandClickHandler(onLeftHandClick);
			InputHandler.onRightHandClick += new GuiInputHandlerBehaviour.OnRightHandClickHandler(onRightHandClick);
			InputHandler.onTrapDoorTrigger += new GuiInputHandlerBehaviour.OnTrapDoorTriggerHandler(onTrapDoorTrigger);
			InputHandler.setActive(false);

			GUIManagerBehaviour.Instance.afterDialogueHide -= afterDialogHide;
			GUIManagerBehaviour.Instance.onDialogueDecision -= onDialogueDecision;
			GUIManagerBehaviour.Instance.onGuiPause -= Pause;
			GUIManagerBehaviour.Instance.onGuiContinue -= resume;
			GUIManagerBehaviour.Instance.afterDialogueHide += afterDialogHide;
			GUIManagerBehaviour.Instance.onDialogueDecision += onDialogueDecision;
			GUIManagerBehaviour.Instance.onGuiPause += Pause;
			GUIManagerBehaviour.Instance.onGuiContinue += resume;

			mTopBar = DialogueManager.getTopBarController(TopBarName);

			mGameState = GameState.INITIALIZED;

			mMinigameInfo = FindObjectOfType<MinigameInfo>();

			Eventbus.Instance.FireEvent<MinigameReadyEvent>(new MinigameReadyEvent(this));
			MazeImageTarget.onTrackableStateChange += new ImageTargetTracker.TrackableStateChangeHandler(onTrackableStateChange);
		}

		void OnDestroy()
		{
			MazeImageTarget.onTrackableStateChange -= new ImageTargetTracker.TrackableStateChangeHandler(onTrackableStateChange);
			InputHandler.onLeftHandClick -= new GuiInputHandlerBehaviour.OnLeftHandClickHandler(onLeftHandClick);
			InputHandler.onRightHandClick -= new GuiInputHandlerBehaviour.OnRightHandClickHandler(onRightHandClick);
			InputHandler.onTrapDoorTrigger -= new GuiInputHandlerBehaviour.OnTrapDoorTriggerHandler(onTrapDoorTrigger);

			if (GUIManagerBehaviour.Instance != null)
			{
				GUIManagerBehaviour.Instance.afterDialogueHide -= afterDialogHide;
				GUIManagerBehaviour.Instance.onDialogueDecision -= onDialogueDecision;
				GUIManagerBehaviour.Instance.onGuiPause -= Pause;
				GUIManagerBehaviour.Instance.onGuiContinue -= resume;
			}
		}

		void Update()
		{
			if (mGameState == GameState.IN_TUTORIAL && (mTutorialTonMovementCount >= MinTonMovementCountForTutorial))
			{
				// First part of tutorial was finished.
				storeCurrentHint(TrapDoorHint, TrapDoorHintIcon);
				mGameState = GameState.IN_TUTORIAL_PART2;
				ShowTutorial = false;
			}
		}

		void prepare()
		{
			// Reset if this should be a valid session
			// MetaGame.Instance.Behaviour.CurrentTrial.CurrentSession.IsSuccessful = false;
			// Init score and time in topbar
			mPlaySecondsLeft = PlayTimeInSeconds;
			SetTopBarTime(mPlaySecondsLeft);
			SetTopBarScore(mScore);

			if (mCurrentMaze != null)
			{
				Destroy(mCurrentMaze.transform.parent.gameObject);
			}
			GameObject currentMaze = Instantiate(MazeLevelPrefabs[CurrentLevel - 1]) as GameObject;
			currentMaze.transform.parent = MazeParent.transform;
			currentMaze.name = "Maze_Level" + CurrentLevel.ToString();
			mCurrentMaze = currentMaze.GetComponentsInChildren<MazeBehaviour>(true)[0];
			mCurrentMaze.TrapDoorOpenTimeInSeconds = TrapDoorOneWayOpeningTimeInSeconds;
			mCurrentMaze.afterTrapDoorClosed += new MazeBehaviour.AfterTrapDoorClosedHandler(afterTrapDoorClosed);

			if (mEcoGotchi == null)
			{
				mEcoGotchi = Instantiate(EcoGotchiPrefab) as GameObject;
				mEcoGotchi.transform.parent = MazeParent.transform;
				mEcoGotchi.SetActive(true);
				InputHandler.TrapDoorTriggers.Add(mEcoGotchi);
			}

			if (mBins != null)
			{
				//Destroy old bins
				foreach (BinBehaviour bin in mBins)
				{
					GameObject.Destroy(bin.gameObject);
				}
			}
			mBins = BinManager.spawnBins();
			foreach (BinBehaviour bin in mBins)
			{
				bin.onCorrectItemCatched += new BinBehaviour.OnCorrectItemCatchedHandler(onCorrectItemCatched);
				bin.onIncorrectItemCatched += new BinBehaviour.OnIncorrectItemCatchedHandler(onIncorrectItemCatched);
			}

			if (mCurrentItem != null)
				GameObject.Destroy(mCurrentItem.gameObject);


			MazeParent.transform.localScale = new Vector3(2, 2, 2);

			spawnItem();
			mPlaySecondsLeft = PlayTimeInSeconds;
			mGameState = GameState.PREPARED;

			if (mIsFirstTimePlaying)
				DialogueManager.getDialogue(IntroQuestDialogueName).show();
			else if (CurrentLevel > 1 && ShowSwitchGroupInfo)
				DialogueManager.getDialogue(SwitchGroupDialogueName).show();
			else
				DialogueManager.getDialogue(ReadyStartLevelDialogueName).show();
		}

		void decreaseCountdown()
		{
			if (mGameState != GameState.PLAYING)
				return;

			if (--mPlaySecondsLeft == 0)
			{
				CancelInvoke("decreaseCountdown");
				mGameState = GameState.FINISHED;
				//the game ends when the currently falling item (if any) has fallen into a bin
				if (!mItemIsFalling)
					EndGame();
			}
			SetTopBarTime(mPlaySecondsLeft);
		}

		void StartGame()
		{
			mGameState = GameState.PLAYING;

			if (mCurrentItem != null)
				GameObject.Destroy(mCurrentItem.gameObject);

			spawnItem();

			if (MazeImageTarget.IsTracked)
				InvokeRepeating("decreaseCountdown", 1.0f, 1.0f);
			else
				Pause();
		}

		void EndGame()
		{
			mGameState = GameState.FINISHED;

			MazeImageTarget.onTrackableStateChange -= onTrackableStateChange;

			DialogueManager.getDialogue(OutroDialogueName).show();
			return;
		}

		void SetTopBarTime(int timeInSecs)
		{
			int minutes = (int)(timeInSecs / 60);
			int seconds = (int)(timeInSecs % 60);
			string secString = "";
			if (seconds < 10)
				secString = "0";
			secString += seconds.ToString();
			//mTopBar.LabelRight = minutes + ":" + secString;
			//mTopBar.update();

			if (mMinigameInfo != null)
			{
				mMinigameInfo.Time = minutes + ":" + secString;
			}
		}

		void SetTopBarScore(int score)
		{
			if (mMinigameInfo != null)
			{
				mMinigameInfo.Score = score.ToString();
			}

			//mTopBar.LabelLeft = score.ToString();
			//mTopBar.update();
		}

		void Pause()
		{
			if (mGameState == GameState.PLAYING)
			{
				mGameState = GameState.PAUSED;
				CancelInvoke("decreaseCountdown");
			}
		}

		void resume()
		{
			if (mGameState == GameState.PAUSED)
			{
				mGameState = GameState.PLAYING;
				InvokeRepeating("decreaseCountdown", 1.0f, 1.0f);
			}
		}

		TrashItemBehaviour spawnItem()
		{
			TrashItemBehaviour itemToSpawn = SpawnDecisor.takeNext();
			GameObject spawnedItem = GameObject.Instantiate(itemToSpawn.gameObject, ItemParent.transform.position, Quaternion.identity) as GameObject;
			spawnedItem.transform.parent = ItemParent.transform;

			ItemDescriptionLabel.transform.parent.gameObject.SetActive(true);
			ItemDescriptionLabel.text = itemToSpawn.ItemDescription;
			mCurrentItem = spawnedItem.GetComponent<TrashItemBehaviour>();
			mCurrentItem.onSelfDestroy += onItemSelfDestroy;
			InputHandler.TrapDoorTriggers.Add(spawnedItem);

			return mCurrentItem;
		}

		void onTrackableStateChange(ImageTargetTracker trackable)
		{

			if (trackable.IsTracked)
			{
				//is now tracking
				GetComponentsInChildren<Transform>(true)[1].gameObject.SetActive(true);

				//restore any active hint
				restoreCurrentHint();

				if (mGameState == GameState.INITIALIZED)
				{

					prepare();
				}
				else if (mGameState == GameState.PAUSED)
				{
					resume();
				}
			}
			else
			{
				//stopped tracking
				GetComponentsInChildren<Transform>(true)[1].gameObject.SetActive(false);

				//set hint for finding the marker. This hint IS NOT STORED since an active hint that is now dismissed for this hint should be restored if the marker is tracked again.
				GUIManagerBehaviour.Instance.Hints.setCombinedHint(FindMarkerHint, FindMarkerHintIcon);
				GUIManagerBehaviour.Instance.Hints.setCombinedHintAsWarning();

				if (mGameState == GameState.PLAYING)
				{
					Pause();
				}
			}
		}

		void onLeftHandClick()
		{
			if (mGameState == GameState.IN_TUTORIAL)
				mTutorialTonMovementCount++;
			BinManager.moveLeft();
		}

		void onRightHandClick()
		{
			if (mGameState == GameState.IN_TUTORIAL)
				mTutorialTonMovementCount++;
			BinManager.moveRight();
		}

		void onTrapDoorTrigger()
		{
			if (mGameState == GameState.IN_TUTORIAL)
			{
				return;
			}
			if (!mItemIsFalling && (mGameState == GameState.PLAYING || mGameState == GameState.IN_TUTORIAL_PART2))
			{
				InputHandler.setActive(false);
				mCurrentItem.gameObject.GetComponent<Rigidbody>().isKinematic = false;
				mCurrentItem.gameObject.GetComponent<Rigidbody>().WakeUp();
				mCurrentMaze.openTrapDoor();
				mItemIsFalling = true;
			}
		}
		void onCorrectItemCatched(TrashItemBehaviour item)
		{
			mScore = Mathf.Max(0, (mScore + PointsForCorrectItem));
			GUIManagerBehaviour.Instance.LiveScores.addScore(item.transform.position, PointsForCorrectItem);
			afterCatched(true);
		}

		void onIncorrectItemCatched(BinBehaviour bin, TrashItemBehaviour item)
		{
			mScore = Mathf.Max(0, (mScore + PointsForIncorrectItem));
			GUIManagerBehaviour.Instance.LiveScores.addScore(item.transform.position, PointsForIncorrectItem);
			afterCatched(false);
		}

		void afterCatched(bool correct)
		{
			mItemIsFalling = false;
			if (mGameState == GameState.PLAYING || mGameState == GameState.PAUSED)
				InputHandler.setActive(true);

			//mTopBar.LabelLeft = mScore.ToString();
			//mTopBar.update();

			if (mMinigameInfo != null)
			{
				mMinigameInfo.Score = mScore.ToString();
			}

			if (mGameState == GameState.IN_TUTORIAL_PART2)
			{
				//tutorial was finished
				InputHandler.setActive(false);
				removeCurrentHint();

				if (correct)
					DialogueManager.getDialogue(FinishedTutorialDialogueName).show();
				else
					DialogueManager.getDialogue(FinishedTutorialWrongBinDialogueName).show();

			}
			else if (mGameState == GameState.FINISHED)
			{
				//the playtimer ran out and the last item fell through - end game
				EndGame();
			}
		}

		void onItemSelfDestroy(TrashItemBehaviour item)
		{
			mItemIsFalling = false;
			InputHandler.setActive(true);

			InputHandler.TrapDoorTriggers.Remove(item.gameObject);

			if (mGameState == GameState.IN_TUTORIAL_PART2)
				spawnItem();
		}

		void afterDialogHide(SharedAssets.GuiMain.DialogueBehaviour dialog)
		{
			//HOTFIX
			if (dialog == null)
				return;

			if (dialog.name == IntroQuestDialogueName)
			{
				InputHandler.setActive(true);
				mGameState = GameState.IN_TUTORIAL;

				if (ShowTutorial)
					//Show Hint for interacting with the stone hands
					storeCurrentHint(MoveTonsHint, MoveTonsHintIcon);
				else
				{
					StartGame();
				}
			}
			else if (dialog.name == FinishedTutorialDialogueName || dialog.name == FinishedTutorialWrongBinDialogueName || (dialog.name == ReadyStartLevelDialogueName))
			{
				InputHandler.setActive(true);
				StartGame();
			}
			else if (dialog.name == SwitchGroupDialogueName)
			{
				ShowSwitchGroupInfo = false;
				DialogueManager.getDialogue(ReadyStartLevelDialogueName).show();
			}
			else if (dialog.name == FinishedLevelGoodDialogueName)
			{
				if (CurrentLevel < MazeLevelPrefabs.Length)
				{
					//there is a next level, ask player if he wants to proceed or leave the game
					DialogueManager.getDialogue(NextLevelDialogueName).show();
				}
				else
					DialogueManager.getDialogue(OutroDialogueName).show();
			}
			else if (dialog.name == FinishedLevelBadDialogueName)
			{
				DialogueManager.getDialogue(TryAgainDialogueName).show();
			}
			else if (dialog.name == OutroDialogueName)
			{
				exit();
			}
			else if (dialog.name == TryAgainDialogueName)
			{
				if (mLastDialogDecision == true)
				{
					mIsFirstTimePlaying = false;
					prepare();
				}
				else
				{
					DialogueManager.getDialogue(OutroDialogueName).show();
				}
			}
			else if (dialog.name == NextLevelDialogueName)
			{
				if (mLastDialogDecision == true)
				{
					mIsFirstTimePlaying = false;
					CurrentLevel++;
					prepare();
				}
				else
				{
					DialogueManager.getDialogue(OutroDialogueName).show();
				}
			}
		}

		void onDialogueDecision(SharedAssets.GuiMain.DialogueBehaviour dialog, bool decision)
		{
			mLastDialogDecision = decision;
		}
		void afterTrapDoorClosed()
		{
			ItemDescriptionLabel.parent.gameObject.SetActive(false);
			ItemDescriptionLabel.text = "";

			//spawn new item if game is currently running and not in tutorial mode
			if (mGameState == GameState.PLAYING || mGameState == GameState.PAUSED)
				spawnItem();
		}

		/// <summary>
		/// Sets an interaction hint and stores it in case it needs to be restored e.g. if another hint is temporarily shown instead of the current one (e.g. marker lost)
		/// </summary>
		/// <param name="text"></param>
		/// <param name="icon"></param>
		/// <param name="visibility"></param>
		/// <param name="pulse"></param>
		void storeCurrentHint(string text, InteractionType icon, bool visibility = true, bool pulse = true)
		{
			mCurrentHint = text;
			mCurrentHintIcon = icon;
			GUIManagerBehaviour.Instance.Hints.setCombinedHint(text, icon, visibility, pulse);
		}

		/// <summary>
		/// Restores the last stored hint (if it's not an empty string) to be visible again. 
		/// </summary>
		void restoreCurrentHint(bool visibility = true, bool pulse = true)
		{
			if (String.IsNullOrEmpty(mCurrentHint))
				GUIManagerBehaviour.Instance.Hints.setCombinedHint("", null, false);
			else
				GUIManagerBehaviour.Instance.Hints.setCombinedHint(mCurrentHint, mCurrentHintIcon, visibility, pulse);
		}

		/// <summary>
		/// Removes any current hint. It cannot be restored with restoreCurrentHint() anymore. 
		/// </summary>
		void removeCurrentHint()
		{
			mCurrentHint = "";
			GUIManagerBehaviour.Instance.Hints.setCombinedHint("", null, false);

		}

		void exit()
		{
			Eventbus.Instance.FireEvent<ScoreAchievedEvent>(new ScoreAchievedEvent(mScore, 150, 600));

			if (GameCompleted != null)
				GameCompleted();
		}
	}
}