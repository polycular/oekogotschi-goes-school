using Noesis;
using UnityEngine;

namespace EcoGotchi.UI
{
	public class NoesisViewBase : MonoBehaviour, INoesisView
	{
		protected string m_xamlFileName;
		public virtual string XamlFileName
		{
			get { return m_xamlFileName; }
		}

		protected bool m_isOverlay;
		public virtual bool IsOverlay
		{
			get { return m_isOverlay; }
		}

		FrameworkElement m_root;
		public FrameworkElement Root
		{
			get { return m_root; }
			set
			{
				if (m_root == null)
					m_root = value;
			}
		}

		Grid m_mainGrid;
		public Grid MainGrid
		{
			get
			{
				if (m_mainGrid == null)
					m_mainGrid = NoesisUtil.FetchElement<Grid>(Root, "Gr_Main");

				return m_mainGrid;
			}
		}

		public virtual void FetchElements() { }
		public virtual void RegisterEventHandler() { }
		public virtual void EstablishBindings() { }
	}
}