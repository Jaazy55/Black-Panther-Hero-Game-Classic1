using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour
{
	private void OnEnable()
	{
		this.text.text = this.objects[this.currentActiveObject].name;
	}

	public void NextCamera()
	{
		int num = (this.currentActiveObject + 1 < this.objects.Length) ? (this.currentActiveObject + 1) : 0;
		for (int i = 0; i < this.objects.Length; i++)
		{
			this.objects[i].SetActive(i == num);
		}
		this.currentActiveObject = num;
		this.text.text = this.objects[this.currentActiveObject].name;
	}

	public GameObject[] objects;

	private int currentActiveObject;

	public Text text;
}
