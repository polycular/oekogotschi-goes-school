using System.ComponentModel;
using UnityEngine;

namespace EcoGotchi
{
	public class MinigameInfo : MonoBehaviour, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		static string m_time;
		public string Time
		{
			get
			{
				if (m_time == null)
					return "00:00";
				else
					return m_time;
			}
			set
			{
				m_time = value;
				OnPropertyChanged("Time");
			}
		}

		static string m_score;
		public string Score
		{
			get
			{
				if (m_score == null)
					return "0";
				else
					return m_score;
			}
			set
			{
				m_score = value;
				OnPropertyChanged("Score");
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
	}
}
