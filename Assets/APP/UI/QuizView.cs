using System.Collections.Generic;
using Noesis;
using Polycular.Utilities;

namespace EcoGotchi.UI
{
	public class QuizView : NoesisViewBase
	{
		public override string XamlFileName
		{
			get { return "Quiz.xaml"; }
		}

		public const string BaseNameQuitButton = "Uc_Answer_";

		[Button(true, "AnimatePoints", "Anim")]
		public bool Skip;


		TextBlock m_question;
		public TextBlock Question
		{
			get { return m_question ?? NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Question"); }
		}

		TextBlock m_progress;
		public TextBlock Progress
		{
			get
			{
				if (m_progress == null)
					m_progress = NoesisUtil.FetchElement<TextBlock>(Root, "Tb_Progress");

				return m_progress;
			}
		}

		Grid m_confirm;
		public Grid Confirm
		{
			get
			{
				if (m_confirm == null)
					m_confirm = NoesisUtil.FetchElement<Grid>(Root, "Gr_Confirm");

				return m_confirm;
			}
		}

		TextBlock m_scoreAnim;
		public TextBlock ScoreAnim
		{
			get
			{
				if (m_scoreAnim == null)
					m_scoreAnim = NoesisUtil.FetchElement<TextBlock>(Root, "Tb_ScoreAnim");

				return m_scoreAnim;
			}
		}

		Grid m_storyboardTargetGrid;
		public Grid StoryboardTargetGrid
		{
			get
			{
				if (m_storyboardTargetGrid == null)
					m_storyboardTargetGrid = NoesisUtil.FetchElement<Grid>(Root, "Gr_ScoreElements");

				return m_storyboardTargetGrid;
			}
		}

		Storyboard m_scoreStoryboard;
		public Storyboard ScoreStoryboard
		{
			get
			{
				if (m_scoreStoryboard == null)
					m_scoreStoryboard = NoesisUtil.FetchResource<Storyboard>(Root, "SbPoints");

				return m_scoreStoryboard;
			}
		}

		List<QuizButton> m_quizButtons;


		void Start()
		{
			MainGrid.Width = NoesisUtil.GetWidth();
		}

		public override void FetchElements()
		{
			m_quizButtons = new List<QuizButton>();

			int nrQuizAnswers = 5;
			string baseNameQaFormat = "{0}{1}";

			for (int i = 0; i < nrQuizAnswers; i++)
			{
				string nameQuizAnswer = string.Format(baseNameQaFormat, BaseNameQuitButton, i);
				QuizButton quizAnswer = NoesisUtil.FetchElement<QuizButton>(Root, nameQuizAnswer);
				m_quizButtons.Add(quizAnswer);
			}
		}

		public void SetAnswerButtonTexts(List<string> texts)
		{
			ResetAnswers();

			for (int i = 0; i < m_quizButtons.Count; i++)
			{
				if (i > texts.Count - 1)
					return;

				m_quizButtons[i].SetText(texts[i]);
			}
		}

		public void AnimatePoints(string txt)
		{
			ScoreAnim.Text = txt;
			StoryboardTargetGrid.Visibility = Visibility.Visible;
			ScoreStoryboard.Begin();
		}

		// currently replaced by the nr. of correct answers
		public void SetProgress(int currentQuestion, int maxQuestions)
		{
			string format = "{0}/{1}";
			Progress.Text = string.Format(format, currentQuestion, maxQuestions);
		}

		public void SetCorrectAnswersCount(int nrCorrect)
		{
			string format = "{0}/{1}";
			Progress.Text = string.Format(format, nrCorrect, m_quizButtons.Count);
		}

		public void Solve(List<int> correctIndices)
		{
			SetAnswerButtonsClickable(false);
			SetConfirmActive(false);

			for (int i = 0; i < m_quizButtons.Count; i++)
			{
				if (correctIndices.Contains(i))
					m_quizButtons[i].SetRight();
				else
					m_quizButtons[i].SetWrong();
			}
		}

		public void SetAnswerSelectedState(int idx, bool sel)
		{
			m_quizButtons[idx].SetSelectedState(sel);
		}

		public void SetConfirmActive(bool active)
		{
			Confirm.Visibility = NoesisUtil.BoolToVis(active);
		}

		public void SetSelIndicatorActive(List<int> sel)
		{
			sel.ForEach(idx => m_quizButtons[idx].SetSelectionIndicatorActive(true));
		}

		void SetAnswerButtonsClickable(bool active)
		{
			m_quizButtons.ForEach(btn => btn.IsHitTestVisible = active);
		}

		void ResetAnswers()
		{
			m_quizButtons.ForEach(btn => btn.Reset());
			SetAnswerButtonsClickable(true);
		}
	}
}