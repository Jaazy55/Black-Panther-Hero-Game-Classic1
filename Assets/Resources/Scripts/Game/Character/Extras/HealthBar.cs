using System;
using UnityEngine;

namespace Game.Character.Extras
{
	public class HealthBar : MonoBehaviour
	{
		private void Awake()
		{
			HealthBar.Instance = this;
		}

		public void SetHealth(float newHealth)
		{
			this.health = newHealth;
			if (this.health < 0f)
			{
				this.health = 0.1f;
			}
		}

		private void OnGUI()
		{
			float x = this.PosX * (float)Screen.width;
			float num = this.PosY * (float)Screen.height;
			float num2 = this.health / 100f * 200f;
			float num3 = 200f - num2;
			float num4 = 0f;
			if (this.health > 0.1f)
			{
				num4 = Mathf.Clamp01(1f - this.health / 100f) * 5f;
			}
			GUI.DrawTexture(new Rect(x, num, 50f, 200f), this.HealthBarEmpty, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(x, num + num3, 50f, 200f - num3 - num4), this.HealthBarFull, ScaleMode.StretchToFill);
		}

		public static HealthBar Instance;

		public float PosX;

		public float PosY;

		public Texture2D HealthBarFull;

		public Texture2D HealthBarEmpty;

		private float health;
	}
}
