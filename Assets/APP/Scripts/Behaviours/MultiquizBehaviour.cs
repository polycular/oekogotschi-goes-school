using System.Collections.Generic;
using System.Linq;
using EcoGotchi.Models;
using EcoGotchi.UI;
using Polycular;
using Polycular.Clustar.Behaviours;
using Polycular.Utilities;
using Component = EcoGotchi.Models.Component;

namespace EcoGotchi.Behaviours
{
	public class MultiquizBehaviour : BehaviourBase
	{
		[Button(true, "NotifyComponentFinished", "Skip")]
		public bool Skip;

		public NoesisGuiController GuiController;
		public QuizView View;

		float m_delayAfterQuiz;

		Component m_rawComponent
		{
			get { return Component as Component; }
		}

		Quiz m_currQuiz
		{
			get
			{
				if (m_rawComponent.CurrentQuizIdx <= m_rawComponent.Quizzes.Count - 1)
				{
					return m_rawComponent.Quizzes[m_rawComponent.CurrentQuizIdx];
				}
				else
					return null;
			}
		}

		List<int> m_selectedAnswers;
		int m_currentPoints;

		void Awake()
		{
			View.ScoreStoryboard.Completed += ((sender, args) =>
			{
				Eventbus.Instance.FireEvent<ScoreAchievedEvent>(new ScoreAchievedEvent(m_currentPoints));
			});
		}

		void OnEnable()
		{
			// duration after quiz is equally long as the animation
			m_delayAfterQuiz = 2.0f;

			m_selectedAnswers = new List<int>();

			if (m_currQuiz != null)
				SetupView();

			Eventbus.Instance.AddListener<ButtonTappedEvent>(ButtonTappedHandler, this);
			GuiController.SetViewVisible(View);
		}

		void OnDisable()
		{
			Eventbus.Instance.RemoveListener(this);
		}

		void ButtonTappedHandler(EventBase ev)
		{
			var tev = ev as ButtonTappedEvent;
			var btnType = tev.BtnType;

			if (btnType == ButtonTappedEvent.ButtonType.ANSWER)
			{
				int selectedIdx = int.Parse(tev.BtnName.Remove(0, QuizView.BaseNameQuitButton.Length));
				bool containsSelection = m_selectedAnswers.Contains(selectedIdx);
				int nrCorrectAnswers = m_currQuiz.Correct.Count;

				if (containsSelection)
				{
					View.SetAnswerSelectedState(selectedIdx, false);
					m_selectedAnswers.Remove(selectedIdx);
				}
				else if (!containsSelection && m_selectedAnswers.Count < nrCorrectAnswers)
				{
					View.SetAnswerSelectedState(selectedIdx, true);
					m_selectedAnswers.Add(selectedIdx);
				}
				else
				{
					int last = m_selectedAnswers.Last();

					View.SetAnswerSelectedState(last, false);
					m_selectedAnswers.Remove(last);

					View.SetAnswerSelectedState(selectedIdx, true);
					m_selectedAnswers.Add(selectedIdx);
				}

				// check if at least one answer is selected
				View.SetConfirmActive(m_selectedAnswers.Count > 0);
			}
			else if (btnType == ButtonTappedEvent.ButtonType.CONTINUE)
			{
				var correct = m_currQuiz.Correct;
				var chosen = m_selectedAnswers;

				m_currentPoints = chosen.Intersect(correct).ToArray().Length;
				View.AnimatePoints("+" + m_currentPoints * 100);

				View.SetSelIndicatorActive(m_selectedAnswers);
				View.Solve(m_currQuiz.Correct);
				m_selectedAnswers.Clear();

				m_rawComponent.CurrentQuizIdx++;
				DataHub.Instance.Save(m_rawComponent);

				if (m_currQuiz != null)
				{
					Invoke("SetupView", m_delayAfterQuiz);
				}
				else
				{
					Eventbus.Instance.RemoveListener<ButtonTappedEvent>(this);
					// Delay to display the correct answer and then end
					Invoke("NotifyComponentFinished", m_delayAfterQuiz);
				}
			}
		}

		void OnNextClickedHandler()
		{

		}

		void SetupView()
		{
			View.SetConfirmActive(false);
			View.Question.Text = m_currQuiz.Question;
			View.SetAnswerButtonTexts(m_currQuiz.Answers);
			View.SetCorrectAnswersCount(m_currQuiz.Correct.Count);
		}

		void NotifyComponentFinished()
		{
			base.Notify(this);
		}
	}
}