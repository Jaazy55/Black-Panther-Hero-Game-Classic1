using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Stats
{
	public class StatBar : CharacterStatDisplay
	{
		private Slider Slider
		{
			get
			{
				if (this.slider == null)
				{
					this.slider = base.GetComponent<Slider>();
					if (this.slider == null)
					{
						UnityEngine.Debug.LogError("Can't find Slider");
						base.enabled = false;
					}
				}
				return this.slider;
			}
		}

		protected override void Awake()
		{
			if (this.DoSetColor)
			{
				if (this.Slider != null && this.Slider.fillRect != null)
				{
					this.fillImage = this.Slider.fillRect.GetComponent<Image>();
					if (this.fillImage == null)
					{
						UnityEngine.Debug.LogError("Can't set the color because can't find Image");
					}
					else
					{
						this.fillImage.color = this.BarColor;
					}
				}
				else
				{
					UnityEngine.Debug.LogError("Can't set the color because fillRect is null");
				}
			}
			if (this.BlinkingAnimator == null)
			{
				this.BlinkingAnimator = base.GetComponentInChildren<Animator>();
				if (this.BlinkingAnimator == null)
				{
					UnityEngine.Debug.LogError("Can't find blinkingAnimator");
				}
				else
				{
					this.blinkingImage = this.BlinkingAnimator.GetComponent<Image>();
					if (this.blinkingImage == null)
					{
						UnityEngine.Debug.LogError("Can't find blinkingImage");
					}
				}
			}
			else
			{
				this.blinkingImage = this.BlinkingAnimator.GetComponent<Image>();
				if (this.blinkingImage == null)
				{
					UnityEngine.Debug.LogError("Can't find blinkingImage");
				}
			}
			base.Awake();
		}

		protected override void OnEnable()
		{
			if (this.delayedBlink)
			{
				this.Blink(this.blinkingImage.color);
				this.delayedBlink = false;
			}
		}

		public void Blink(Color blinkingColor)
		{
			if (this.BlinkingAnimator == null || this.blinkingImage == null)
			{
				return;
			}
			blinkingColor.a = 0f;
			if (this.DoSetColor)
			{
				this.blinkingImage.color = blinkingColor;
			}
			if (base.gameObject.activeInHierarchy && base.enabled)
			{
				this.BlinkingAnimator.SetTrigger("Blink");
			}
			else
			{
				this.delayedBlink = true;
			}
		}

		public void BlinkRed()
		{
			this.Blink(Color.red);
		}

		public void BlinkGreen()
		{
			this.Blink(Color.green);
		}

		protected override void UpdateDisplayValue()
		{
			this.dx = Mathf.Clamp01(this.current / this.max);
			if (this.Slider != null)
			{
				this.Slider.value = this.dx * this.fillAmountPercent;
			}
		}

		public override void OnChanged(float amount)
		{
			if (!this.DoBlink)
			{
				return;
			}
			this.Blink(this.BarColor);
		}

		[Tooltip("Если текстура белая/бесцветная и хотим её красить в нужный цвет")]
		public bool DoSetColor;

		public Color BarColor = Color.white;

		public float fillAmountPercent = 1f;

		private Image fillImage;

		[SerializeField]
		private Slider slider;

		public bool DoBlink = true;

		public Animator BlinkingAnimator;

		private Image blinkingImage;

		private bool delayedBlink;

		private float dx;
	}
}
