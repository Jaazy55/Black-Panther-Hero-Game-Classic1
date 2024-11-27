using System;
using UnityEngine;

public class CombosPlaceholder : MonoBehaviour
{
	private void Start()
	{
		if (ComboManager.Instance != null)
		{
			base.transform.parent = ComboManager.Instance.transform;
		}
	}
}
