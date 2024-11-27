using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : MonoBehaviour
{
	public void LoadScene(string scene)
	{
		this.loading = true;
	//	this.SetTexturesFromAtlas();
		base.InvokeRepeating("Inscreace", 0.5f, 0.1f);
		base.StartCoroutine(this.LoadNewScene(scene));
	}

	private void OnGUI()
	{
		if (this.loading)
		{
			this.ShowElements();
		}
	}

	private void SetTexturesFromAtlas()
	{
		this.spinner = new Texture2D((int)this.Spinner.rect.width, (int)this.Spinner.rect.height);
		Color[] pixels = this.Spinner.texture.GetPixels((int)this.Spinner.rect.x, (int)this.Spinner.rect.y, (int)this.Spinner.rect.width, (int)this.Spinner.rect.height);
		this.spinner.SetPixels(pixels);
		this.spinner.Apply();
		this.back = new Texture2D((int)this.Back.rect.width, (int)this.Back.rect.height);
		pixels = this.Back.texture.GetPixels((int)this.Back.rect.x, (int)this.Back.rect.y, (int)this.Back.rect.width, (int)this.Back.rect.height);
		this.back.SetPixels(pixels);
		this.back.Apply();
		this.bar = new Texture2D((int)this.Bar.rect.width, (int)this.Bar.rect.height);
		pixels = this.Bar.texture.GetPixels((int)this.Bar.rect.x, (int)this.Bar.rect.y, (int)this.Bar.rect.width, (int)this.Bar.rect.height);
		this.bar.SetPixels(pixels);
		this.bar.Apply();
	}

	private void ShowElements()
	{
		Rect position = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		Rect position2 = new Rect(position.xMin + position.width * 0.1f, position.yMax - position.height * 0.2f, position.xMax - position.width * 0.2f, position.height * 0.05f);
		Rect position3 = new Rect(position.xMin + position.width * 0.1f, position.yMax - position.height * 0.2f, (position.xMax - position.width * 0.2f) * this.progress, position.height * 0.05f);
		Rect position4 = new Rect((float)(Screen.width / 2 - Screen.width / 20), (float)(Screen.height / 6), (float)(Screen.width / 10), (float)(Screen.width / 10));
		GUI.BeginGroup(position);
		GUI.DrawTexture(position2, this.back, ScaleMode.StretchToFill);
		GUI.DrawTexture(position3, this.bar, ScaleMode.StretchToFill);
		GUI.EndGroup();
		Vector2 pivotPoint = new Vector2(position4.xMin + position4.width * 0.5f, position4.yMin + position4.height * 0.5f);
		GUIUtility.RotateAroundPivot(this.angle, pivotPoint);
		GUI.DrawTexture(position4, this.spinner);
	}

	private IEnumerator LoadNewScene(string scene)
	{
		yield return new WaitForSeconds(0.5f);
		AsyncOperation async = SceneManager.LoadSceneAsync(scene);
		while (!async.isDone)
		{
			this.progress = async.progress;
			yield return null;
		}
		this.loading = false;
		yield break;
	}

	private void Inscreace()
	{
		this.angle -= 5f;
	}

	public Sprite Spinner;

	public Sprite Back;

	public Sprite Bar;

	private Texture2D spinner;

	private Texture2D back;

	private Texture2D bar;

	private float progress;

	private bool loading;

	private float angle;
}
