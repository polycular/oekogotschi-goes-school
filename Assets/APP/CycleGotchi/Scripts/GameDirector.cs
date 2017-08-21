using System;
using System.Collections.Generic;
using EcoGotchi;
using Polycular;
using SharedAssets;
using SharedAssets.GuiMain;
using UnityEngine;

namespace CycleGotchi
{
	public class GameDirector : MonoBehaviour, IGameDirector
	{
		enum GameState
		{
			PRE_INTRO,
			INTRO,
			RULES,
			INTRO_WAIT,
			PLAY_CONNECT,
			WAIT,
			PLAY_CONSTRUCTION,
			OUTRO,
			END
		}

		public ImageTargetTracker StationTrackable;
		public DialogueManagerBehaviour DialogueManager;

		public SpawnCarBehaviour CarSpawner;
		public StreetGridBehaviour StreetGrid;
		public BikeLaneBuilderBehaviour BikeLaneBuilder;
		public ResolveTilesBehaviour ResolveTilesController;
		public TileGroupBlenderBehaviour TileGroupBlender;
		public NextLevelVisualizationBehaviour NextLevelEffect;
		public EggTimerBehaviour GameTimer;
		public TopBarBehaviour TopBar;
		public int AmountSpawnedCars = 5;
		public int AmountSpawnedCarsIncreasePerLevel = 1;

		[Tooltip("'Dead End' is the SpecialCase where some cars are left on the last tile, and it needs to be automatically solved")]
		public bool GivePointsForDeadEnd = true;

		public event Action GameCompleted;

		public int Score = 0;
		public int ScorePoints
		{
			get
			{
				return Score;
			}
			set
			{
				Score = value;
			}
		}

		List<CarBehaviour> mCars = new List<CarBehaviour>();
		TileGroupBlenderBehaviour.TileGroup mCurrentPreviewGroup = null;
		TileBehaviour[,] mLatestValidAffectedTiles;

		bool mBuildingCanceled = false;
		bool mIsCamRdy = true;

		private GameState mCurrentGameState = GameState.PRE_INTRO;
		private GameState mLastNotWaitState = GameState.PRE_INTRO;
		private bool mIsActionWait = false;
		private bool mIsMarkerHintShowing = false;
		private bool mIsGameFinished = false;

		private bool mIsTutorialRound = true;

		public bool IsTutorialRound
		{
			get
			{
				return mIsTutorialRound;
			}
			set
			{
				if (!(bool)value)
				{
					mIsTutorialRound = false;
				}
			}
		}

		public ImageTargetTracker ImgTargetTracker
		{
			get { return StationTrackable; }
			set { StationTrackable = value; }
		}

		MinigameInfo mMinigameInfo;


		public void DestroyCars(List<CarBehaviour> cars)
		{
			foreach (CarBehaviour car in cars)
			{
				mCars.Remove(car);
				Destroy(car.gameObject);
			}
		}


		private void afterDialogueHide(DialogueBehaviour dialogue)
		{
			if (dialogue == null)
				return;

			//after the intro dialogue show the rules for cyclegotchi
			if (dialogue.name == DialogueManager.getDialogue("D1-Intro").name)
			{
				mCurrentGameState = GameState.RULES;
			}

			//show the hint when rules dialogue is done
			if (dialogue.name == DialogueManager.getDialogue("D1-Rules").name)
			{
				mCurrentGameState = mLastNotWaitState = GameState.PLAY_CONNECT;
				GameTimer.unpause();
			}

			//end the game after the outro dialogue gets dismissed
			if (dialogue.name == DialogueManager.getDialogue("D2-Outro").name)
			{
				GUIManagerBehaviour.Instance.afterDialogueHide -= afterDialogueHide;
				//MetaGame.Instance.Behaviour.CurrentTrial.quitSession();
				//SharedAssets.GuiMain.GUIManagerBehaviour.Instance.clearTopBar();

				Eventbus.Instance.FireEvent<ScoreAchievedEvent>(new ScoreAchievedEvent(Score, 450, 1000));

				if (GameCompleted != null)
					GameCompleted();
			}
		}

		void recreateGrid()
		{
			TileGroupBlender.releaseAllGroups();
			StreetGrid.recreateGrid();
		}

		void spawnCars()
		{
			if (mIsTutorialRound)
			{
				//to demonstrate how to play, spawn only one car at this time
				mCars = CarSpawner.spawnCars(1, StreetGrid);
			}
			else
			{
				mCars = CarSpawner.spawnCars(AmountSpawnedCars, StreetGrid);
			}

		}

		bool hasNewAffectedTiles(TileBehaviour[,] affected)
		{
			if (mLatestValidAffectedTiles == null)
				return true;

			if (affected.GetLength(0) != mLatestValidAffectedTiles.GetLength(0) || affected.GetLength(1) != mLatestValidAffectedTiles.GetLength(1))
				return true;

			for (int x = 0; x < affected.GetLength(0); x++)
			{
				for (int y = 0; y < affected.GetLength(1); y++)
				{
					if (mLatestValidAffectedTiles[x, y] != affected[x, y])
						return true;
				}
			}
			return false;
		}

		List<TileBehaviour> listBadTiles(TileBehaviour[,] tiles)
		{
			List<TileBehaviour> targetTiles = new List<TileBehaviour>();
			foreach (TileBehaviour tile in tiles)
			{
				if (tile.IsBad)
				{
					targetTiles.Add(tile);
				}
			}
			return targetTiles;
		}

		void resolveGreenArea(TileBehaviour[,] tiles, bool givePoints)
		{
			List<TileBehaviour> targetTiles = listBadTiles(tiles);
			List<CarBehaviour> destroyedCars = ResolveTilesController.resolveGreenArea(targetTiles);

			setPreviewGroup(null);
			TileGroupBlender.defineNewGroup(targetTiles).CurrentEffect = TileGroupBlenderBehaviour.TileGroup.Effect.BLENDING;

			if (givePoints)
			{
				int newPoints = resolvePoints(StreetGrid.Tiles.GetLength(0), StreetGrid.Tiles.GetLength(0), targetTiles.Count, destroyedCars.Count);
				ScorePoints += newPoints;

				//Show the points in the GUI too

				//Show a Bonus Counter for each Car
				int bonusCounter = 3;
				foreach (CarBehaviour car in destroyedCars)
				{
					GUIManagerBehaviour.Instance.LiveScores.addScore(car.transform.position, "BONUS x" + bonusCounter);
					bonusCounter++;
				}

				//Find the middle of the given tiles
				Vector3 center = Vector3.zero;
				foreach (TileBehaviour tile in targetTiles)
				{
					center += tile.transform.position;
				}
				center /= targetTiles.Count;

				GUIManagerBehaviour.Instance.LiveScores.addScore(center, newPoints);
			}

			DestroyCars(destroyedCars);
		}

		/// <summary>
		/// Call this to set a preview for Tiles to be turned
		/// </summary>
		/// <param name="tiles"></param>
		public void setPreviewGroup(List<TileBehaviour> badTiles)
		{
			if (mCurrentPreviewGroup != null)
				TileGroupBlender.releaseGroup(mCurrentPreviewGroup);
			if (badTiles != null)
			{
				mCurrentPreviewGroup = TileGroupBlender.defineNewGroup(badTiles);
				mCurrentPreviewGroup.CurrentEffect = TileGroupBlenderBehaviour.TileGroup.Effect.BLINKING;
			}
		}

		void pause()
		{
			GameTimer.pause();
		}

		void resume()
		{
			GameTimer.unpause();
		}

		void Awake()
		{
			Eventbus.Instance.AddListener<CamReadyEvent>(ev => mIsCamRdy = true, this);
		}

		void Start()
		{
			SharedAssets.GuiMain.GUIManagerBehaviour.Instance.onGuiPause += pause;
			SharedAssets.GuiMain.GUIManagerBehaviour.Instance.onGuiContinue += resume;

			GUIManagerBehaviour.Instance.afterDialogueHide -= afterDialogueHide;
			GUIManagerBehaviour.Instance.afterDialogueHide += afterDialogueHide;

			if (GameTimer == null)
				GameTimer = GetComponent<SharedAssets.EggTimerBehaviour>();

			GameTimer.pause();

			if (StreetGrid == null)
				StreetGrid = GetComponentInChildren<StreetGridBehaviour>();

			StreetGrid.recreateIfNecessary();
			spawnCars();

			mMinigameInfo = FindObjectOfType<MinigameInfo>();

			Eventbus.Instance.FireEvent<MinigameReadyEvent>(new MinigameReadyEvent(this));
		}

		// Update is called once per frame
		void Update()
		{
			if (!mIsCamRdy || StationTrackable == null)
				return;

			//update score and time in topbar
			//TopBar.LabelLeft = GameTimer.CountdownStringP1;
			//TopBar.LabelRight = ScorePoints.ToString();
			//TopBar.update();

			if (mMinigameInfo != null)
			{
				mMinigameInfo.Time = GameTimer.CountdownStringP1;
				mMinigameInfo.Score = ScorePoints.ToString();
			}

			if (StationTrackable.IsTracked)
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					transform.GetChild(i).gameObject.SetActive(true);
				}



				if (mIsMarkerHintShowing)
				{
					mIsMarkerHintShowing = false;

					//hide the hint while the dialogues are shown
					if (mCurrentGameState == GameState.PRE_INTRO)
					{
						GUIManagerBehaviour.Instance.Hints.setCombinedHint(string.Empty, InteractionSpriteListBehaviour.InteractionType.NONE, false, false);
					}

					mCurrentGameState = mLastNotWaitState;
					mIsActionWait = false;
				}

				if (BikeLaneBuilder.IsBuilding && !mIsActionWait)
				{
					mCurrentGameState = mLastNotWaitState = GameState.PLAY_CONSTRUCTION;
					mIsActionWait = true;
				}

				else if (BikeLaneBuilder.HasBuilt)
				{
					mCurrentGameState = mLastNotWaitState = GameState.PLAY_CONNECT;
					mIsActionWait = false;
				}

				else if (mBuildingCanceled)
				{
					mCurrentGameState = mLastNotWaitState = GameState.PLAY_CONNECT;
					mBuildingCanceled = false;
				}


				if (GameTimer.IsFinished && !BikeLaneBuilder.IsBuilding && mCurrentGameState != GameState.END)
				{
					mCurrentGameState = mLastNotWaitState = GameState.OUTRO;
				}

				//manage dialogues
				switch (mCurrentGameState)
				{
					case GameState.PRE_INTRO:

						mCurrentGameState = GameState.INTRO;
						break;

					case GameState.INTRO:
						DialogueManager.getDialogue("D1-Intro").show();

						mCurrentGameState = GameState.INTRO_WAIT;
						break;

					case GameState.RULES:

						if (StationTrackable.IsTracked)
						{
							DialogueManager.getDialogue("D1-Rules").show();
						}

						mCurrentGameState = GameState.INTRO_WAIT;
						break;

					case GameState.PLAY_CONNECT:

						GUIManagerBehaviour.Instance.Hints.setCombinedHint("Verbinde zwei Kanten", InteractionSpriteListBehaviour.InteractionType.DRAW_LINE, true, true);
						mCurrentGameState = GameState.WAIT;
						break;

					case GameState.PLAY_CONSTRUCTION:

						GUIManagerBehaviour.Instance.Hints.setCombinedHint("Warte auf den Bau", InteractionSpriteListBehaviour.InteractionType.WAIT, true, true);
						mCurrentGameState = GameState.WAIT;
						break;

					case GameState.OUTRO:

						GUIManagerBehaviour.Instance.Hints.setCombinedHint("", InteractionSpriteListBehaviour.InteractionType.NONE, false, false);
						//MetaGame.Instance.Behaviour.CurrentTrial.CurrentSession.IsSuccessful = true;
						DialogueManager.getDialogue("D2-Outro").show();
						mCurrentGameState = GameState.END;
						break;
				}
			}
			else
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					transform.GetChild(i).gameObject.SetActive(false);
				}
				//station trackable not tracked
				if (mCurrentGameState == GameState.PRE_INTRO
				|| mCurrentGameState == GameState.PLAY_CONNECT
				|| mCurrentGameState == GameState.PLAY_CONSTRUCTION
				|| mCurrentGameState == GameState.WAIT)
				{
					mIsMarkerHintShowing = true;
					GUIManagerBehaviour.Instance.Hints.setCombinedHint("Halte die Kamera wieder auf den Marker", InteractionSpriteListBehaviour.InteractionType.FIND_LOST_MARKER, true, false);
					GUIManagerBehaviour.Instance.Hints.setCombinedHintAsWarning();
				}
			}

			if (mIsGameFinished)
				return;

			if (mCars.Count == 0 && StreetGrid.Tiles.Length > 0)
			{
				/**
                 * Turn all Tiles green/good once no cars are left.
                 * */
				resolveGreenArea(StreetGrid.Tiles, false);
			}
			else if (mCars.Count > 0 && listBadTiles(StreetGrid.Tiles).Count == 1)
			{
				/**
                 * (Special Case) once only 1 Tile & Car is left (Dead-End)
                 * Give points for that sad situation
                 * */
				resolveGreenArea(StreetGrid.Tiles, GivePointsForDeadEnd);
			}
			else if (BikeLaneBuilder.HasBuilt)
			{
				resolveGreenArea(BikeLaneBuilder.AffectedTiles, true);
			}
			else if (BikeLaneBuilder.IsBuilding)
			{
				List<CarBehaviour> destroyedCars = new List<CarBehaviour>();

				bool buildingCanceled = false;
				mBuildingCanceled = false;

				Ray ray = new Ray(BikeLaneBuilder.BuildStart, BikeLaneBuilder.BuildTarget - BikeLaneBuilder.BuildStart);
				float length = (BikeLaneBuilder.BuildCurrentHead - BikeLaneBuilder.BuildStart).magnitude;
				foreach (CarBehaviour car in mCars)
				{
					RaycastHit hit;
					if (car.GetComponent<Collider>().Raycast(ray, out hit, length))
					{
						destroyedCars.Add(car);
						buildingCanceled = true;
						mBuildingCanceled = true;
					}
				}
				if (buildingCanceled)
				{
					BikeLaneBuilder.resetBuild();
				}

				DestroyCars(destroyedCars);
			}
			/**
             * Previewing affected tiles by current building process
             * */
			else if (BikeLaneBuilder.InputIsInProgress && BikeLaneBuilder.InputHasValidStart && BikeLaneBuilder.InputHasValidEnd)
			{
				if (hasNewAffectedTiles(BikeLaneBuilder.AffectedTiles))
				{
					//A new set of tiles, a new preview is needed
					mLatestValidAffectedTiles = BikeLaneBuilder.AffectedTiles;
					setPreviewGroup(listBadTiles(BikeLaneBuilder.AffectedTiles));
				}
			}
			else
			{
				mLatestValidAffectedTiles = null;
				setPreviewGroup(null);
			}

			//check if all tiles are flipped green already
			int badTilesLeft = 0;
			foreach (TileBehaviour tile in StreetGrid.Tiles)
			{
				if (!tile.IsTotallyGood)
				{
					badTilesLeft++;
				}
			}

			if (badTilesLeft == 0)
			{
				NextLevelEffect.gameObject.SetActive(false);
				NextLevelEffect.gameObject.SetActive(true);
				//recreate grid for next level if all tiles were flipped
				StreetGrid.recreateGrid();
				//spawn level-increased car amount
				mCars = CarSpawner.spawnCars(AmountSpawnedCars + AmountSpawnedCarsIncreasePerLevel, StreetGrid);
			}

			if (GameTimer.IsFinished && !BikeLaneBuilder.IsBuilding)
				mIsGameFinished = true;
		}

		/// <summary>
		/// Resolve the maximum points possible defined in Confluence for the given StreetGrid dimensions
		/// </summary>
		/// <param name="rows">Nr of StreetGrid rows</param>
		/// <param name="columns">Nr of StreetGrid columns</param>
		/// <returns>The maximum possible points for the given dimensions</returns>
		private int resolveMaxPoints(int rows, int columns)
		{

			int dim1 = ((rows / 2) * columns) + 1;
			int dim2 = ((columns / 2) * rows) + 1;

			if (dim1 >= dim2)
				return dim1;
			else
				return dim2;
		}

		/// <summary>
		/// Calculates the maximum points for the given StreetGrid dimensions, Nr of resolved tiles and cars
		/// </summary>
		/// <param name="rows">Nr of StreetGrid rows</param>
		/// <param name="columns">Nr of StreetGrid columns</param>
		/// <param name="nrOfResolvedTiles">Nr of resolved tiles</param>
		/// <param name="nrOfResolvedCars">Nr of resolved cars</param>
		/// <returns>Points for one ResolveGreenArea call</returns>
		private int resolvePoints(int rows, int columns, int nrOfResolvedTiles, int nrOfResolvedCars)
		{
			int points = Mathf.Max(0, ((resolveMaxPoints(rows, columns) - nrOfResolvedTiles) * 2) * Mathf.Max(1, (2 * nrOfResolvedCars)));

			//multiply with fixed value to increase points gained
			return points * 2;
		}
	}
}