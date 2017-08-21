using UnityEngine;
using Polycular.Utilities;
using System.Collections.Generic;
using System.Collections;

public class TeleportEffect : MonoBehaviour
{
	public Material TPMaterial;
	public GameObject Particles;
	public List<GameObject> ToSetMaterial;
	public float AnimTime;
	public float ParticleOnlyTime;

	Material m_stdMat;
	float m_totalTime;

	[Button(true,"StartEffect", "Start Effect")]
	public bool m_enabled;

	void Start()
	{
		m_stdMat = ToSetMaterial[0].GetComponent<Renderer>().material;
	}

	/// <summary> Enables the Teleport effect. Is automatically
	/// disabled after AnimTime. </summary>
	public void StartEffect()
	{
		StartCoroutine(StartEffectImpl());
	}

	IEnumerator StartEffectImpl()
	{
		TPMaterial.SetFloat("_Holo_Speed", AnimTime);
		Particles.SetActive(true);

		yield return new WaitForSeconds(ParticleOnlyTime);

		m_enabled = true;

		foreach (var go in ToSetMaterial)
		{
			go.GetComponent<Renderer>().material = TPMaterial;
		}

		yield break;
	}
	
	void EndEffect()
	{
		ResetEffect();
		gameObject.SetActive(false);
	}

	void ResetEffect()
	{
		m_enabled = false;
		m_totalTime = 0f;
		Particles.SetActive(false);

		foreach (var go in ToSetMaterial)
		{
			go.GetComponent<Renderer>().material = m_stdMat;
		}
	}

	void Update()
	{
		if (!m_enabled)
			return;

		m_totalTime += Time.deltaTime;

		TPMaterial.SetFloat("_Time_Total", m_totalTime);

		if (m_totalTime > AnimTime)
			EndEffect();
	}
}
