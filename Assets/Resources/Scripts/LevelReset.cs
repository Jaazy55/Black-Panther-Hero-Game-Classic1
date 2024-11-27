using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelReset : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData data)
	{
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
	}

	private void Update()
	{
	}
}
