using UnityEngine;

public class ChangeNoesisClearType : MonoBehaviour
{
	Camera m_noesisCam;

	void Start()
	{
		var noesisPanel = FindObjectOfType<NoesisGUIPanel>().gameObject;
		m_noesisCam = noesisPanel.gameObject.GetComponent<Camera>();
		m_noesisCam.clearFlags = CameraClearFlags.Depth;	
	}
	
	void OnDestroy()
	{
		m_noesisCam.clearFlags = CameraClearFlags.SolidColor;
	}
}
