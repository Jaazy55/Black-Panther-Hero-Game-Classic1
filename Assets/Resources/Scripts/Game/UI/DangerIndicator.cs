using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class DangerIndicator : MonoBehaviour
	{
		public static DangerIndicator Instance
		{
			get
			{
				if (DangerIndicator.instance == null)
				{
					DangerIndicator.instance = UnityEngine.Object.FindObjectOfType<DangerIndicator>();
				}
				return DangerIndicator.instance;
			}
		}

		private void Awake()
		{
			if (!DangerIndicator.instance)
			{
				DangerIndicator.instance = this;
			}
			this.panelImage = this.Panel.GetComponent<Image>();
			if (!this.panelImage)
			{
				UnityEngine.Debug.LogError("Set panel for danger indicator and check that panel have component 'Image'");
			}
			this.Deactivate();
		}

		private void Update()
		{
			if (!this.IsActive)
			{
				return;
			}
			if (this.panelImage.canvasRenderer.GetAlpha() >= this.MaxAlpha && this.increase)
			{
				this.increase = false;
				this.panelImage.CrossFadeAlpha(this.MinAlpha, this.Duration, false);
			}
			else if (this.panelImage.canvasRenderer.GetAlpha() <= this.MinAlpha && !this.increase)
			{
				this.increase = true;
				this.panelImage.CrossFadeAlpha(this.MaxAlpha, this.Duration, false);
			}
		}

		public void Activate(string message)
		{
			if (this.IsActive)
			{
				return;
			}
			this.Text.text = message;
			this.Panel.SetActive(true);
			this.IsActive = true;
		}

		public void Deactivate()
		{
			if (!this.IsActive)
			{
				return;
			}
			this.Panel.SetActive(false);
			this.IsActive = false;
		}

		private static DangerIndicator instance;

		public GameObject Panel;

		public Text Text;

		public float Duration = 1f;

		[Range(0f, 1f)]
		public float MaxAlpha = 1f;

		[Range(0f, 1f)]
		public float MinAlpha;

		[HideInInspector]
		public bool IsActive;

		private Image panelImage;

		private bool increase = true;
	}
}
