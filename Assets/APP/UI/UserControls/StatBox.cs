using Noesis;
using UnityEngine;

namespace EcoGotchi.UI
{
	[UserControlSource("Assets/APP/UI/UserControls/StatBox.xaml")]
	public class StatBox: UserControl
	{

#pragma warning disable 0414
		static DependencyProperty FillBrushProperty = DependencyProperty.Register("FillBrush", typeof(Brush), typeof(StatBox));

		static DependencyProperty FillIconProperty = DependencyProperty.Register("FillIcon", typeof(Brush), typeof(StatBox));
#pragma warning restore 0414

		public enum OrientationType
		{
			HORIZONTAL,
			VERTICAL
		}

		float m_maxWidth;
		float m_maxHeight;

		double m_animDuration = 5000;

		Rectangle m_fillRect;

		OrientationType _orientation;
		public OrientationType Orientation
		{
			get { return _orientation; }
			set
			{
				if (value == _orientation)
					return;

				_orientation = value;
				ChangeOrientation();
			}
		}

		float _value = 0f;
		public float Value
		{
			get { return _value; }
			set
			{
				if (value < 0)
					_value = 0;
				else if (value > MaxValue)
					_value = MaxValue;
				else
					_value = value;

				Adjust();
			}
		}

		float _maxValue = 0f;
		public float MaxValue
		{
			get { return _maxValue; }
			set
			{
				_maxValue = value;
				Value = _maxValue;
			}
		}

		public StatBox()
		{
		}

		public void OnPostInit()
		{
			m_fillRect = FindName("Re_Fill") as Rectangle;
			m_maxHeight = m_fillRect.Height;
			m_maxWidth = m_fillRect.Width;
			Orientation = OrientationType.VERTICAL;
		}

		void ChangeOrientation()
		{
			if (_maxValue == 0)
				return;

			var curValue = _value / _maxValue;

			// From HORIZONTAL to VERTICAl.
			if (Orientation == OrientationType.VERTICAL)
			{
				m_fillRect.Width = m_maxWidth;
				m_fillRect.Height = m_maxHeight * curValue;
			}
			else // From VERTICAL to HORIZONTAL
			{
				m_fillRect.Height = m_maxHeight;
				m_fillRect.Width = m_maxWidth * curValue;
			}
		}

		void Adjust()
		{
			if (m_fillRect == null)
				return;

			float curValue = Value / MaxValue;

			if (Orientation == OrientationType.HORIZONTAL)
			{
				var oldSize = m_fillRect.Width;
				var newSize = m_maxWidth * curValue;

				if (float.IsNaN(oldSize) || float.IsNaN(newSize))
					return;

				Animate(oldSize, newSize);
			}
			else
			{
				var oldSize = m_fillRect.Height;
				var newSize = m_maxHeight * curValue;

				if (float.IsNaN(oldSize) || float.IsNaN(newSize))
					return;

				Animate(oldSize, newSize);
			}
		}

		void Animate(float from, float to)
		{
			if (from == to)
				return;

			DoubleAnimation anim = new DoubleAnimation();
			Storyboard sb = new Storyboard();
			Storyboard.SetTarget(anim, m_fillRect);

			anim.From = from;
			anim.To = to;

			if (Orientation == OrientationType.HORIZONTAL)
			{
				anim.Duration = new Duration(System.TimeSpan.FromMilliseconds(m_animDuration * (Mathf.Abs(to - from) / m_maxWidth)));
				Storyboard.SetTargetProperty(anim, new PropertyPath(Rectangle.WidthProperty));
			}
			else
			{
				anim.Duration = new Duration(System.TimeSpan.FromMilliseconds(m_animDuration * (Mathf.Abs(to - from) / m_maxHeight)));
				Storyboard.SetTargetProperty(anim, new PropertyPath(Rectangle.HeightProperty));
			}

			sb.Children.Add(anim);
			sb.Begin(m_fillRect);
		}
	}
}