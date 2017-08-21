using System.Collections.Generic;
using System.Linq;
using Noesis;
using UnityEngine;

namespace EcoGotchi.UI
{
	public class NoesisGuiController : MonoBehaviour
	{
		public GameObject ViewGameObject;
		public string XamlFolderPath;

		Page m_root;
		Grid m_contentContainer;

		List<INoesisView> m_viewInstances;

		// this is necessary because of the distinct xaml namescopes!
		Dictionary<string, FrameworkElement> m_loadedFiles;


		void Awake()
		{
			m_root = NoesisUtil.GetRootNode<Page>();
			m_contentContainer = m_root.FindName("Content_Container") as Grid;

			m_viewInstances = ViewGameObject.GetComponents<INoesisView>().ToList();

			m_loadedFiles = new Dictionary<string, FrameworkElement>();

			Load();
			// Inject each loaded FrameworkElement in the corresponding view class
			PrepareViews();
			SetInitVisibility();
		}

		public void SetViewVisible(INoesisView view, bool overlay = false)
		{
			string filename = view.XamlFileName;

			if (!overlay)
				CollapseAllContent();

			m_loadedFiles[filename].Visibility = Visibility.Visible;
		}

		public void SetViewVisible<T>(bool overlay = false) where T : INoesisView
		{
			T res = GetViewInstance<T>();
			string filename = (res as INoesisView).XamlFileName;

			if (!overlay)
				CollapseAllContent();

			m_loadedFiles[filename].Visibility = Visibility.Visible;
		}

		#region XAML file loading
		void Load()
		{
			m_contentContainer.Children.Clear();

			// order as on the gameobject -> bottom-most view gets rendered on top
			// assumption based on https://forum.unity3d.com/threads/getcomponentsinchildren.4582/#post-33983
			m_viewInstances.ForEach(view => LoadFrameworkElement(view.XamlFileName));
		}

		void PrepareViews()
		{
			m_viewInstances.ForEach(view =>
			{
				// inject root element in views
				view.Root = GetFrameworkElement(view.XamlFileName);

				// fetch elements which can't be fetched on demand (lists of elements e.g. quiz answers)
				view.FetchElements();
				view.RegisterEventHandler();
				view.EstablishBindings();
			});
		}

		void SetInitVisibility()
		{
			// overlays are visible from the beginning
			m_viewInstances.ForEach(view =>
			{
				if (view.IsOverlay)
					SetViewVisible(view);
			});
		}

		void LoadFrameworkElement(string path, bool collapsed = true)
		{
			FrameworkElement element = NoesisGUISystem.LoadXaml(GetXamlPath(path)) as FrameworkElement;
			Debug.Assert(element != null, string.Format("{0}: FrameworkElement '{1}' failed to load ...", GetType().Name, element.Name));

			if (collapsed)
				element.Visibility = Visibility.Hidden;

			//Debug.LogFormat("{0}: Loaded {1} from path {2}", GetType().Name, element.Name, path);
			m_loadedFiles.Add(path, element);
			m_contentContainer.Children.Add(element);
		}

		FrameworkElement GetFrameworkElement(string xName)
		{
			FrameworkElement element;
			m_loadedFiles.TryGetValue(xName, out element);
			return element;
		}

		string GetXamlPath(string fileName)
		{
			return System.IO.Path.Combine(XamlFolderPath, fileName);
		}
		#endregion

		T GetViewInstance<T>() where T : INoesisView
		{
			var res = m_viewInstances.Find(x => x.GetType() == typeof(T));
			return (T)res;
		}

		void CollapseAllContent()
		{
			foreach (KeyValuePair<string, FrameworkElement> e in m_loadedFiles)
			{
				var correspondingViewElement = m_viewInstances.Where(view => view.XamlFileName.Equals(e.Key)).FirstOrDefault();
				if (correspondingViewElement.IsOverlay)
					continue;

				e.Value.Visibility = Visibility.Hidden;
			}
		}
	}
}