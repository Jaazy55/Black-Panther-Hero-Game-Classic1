using System;
using UnityEngine;

public class ClearPrefsButton : MonoBehaviour
{
	public void Click()
	{
		BaseProfile.ClearProfile();
	}
}
