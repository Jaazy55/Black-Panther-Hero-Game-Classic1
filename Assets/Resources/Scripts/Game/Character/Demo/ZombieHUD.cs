using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Demo
{
	public class ZombieHUD : MonoBehaviour
	{
		public static ZombieHUD Instance { get; private set; }

		public void ZombieKilled()
		{
			this.zombies++;
		}

		public void WormKilled()
		{
			this.worms++;
		}

		private void Awake()
		{
			ZombieHUD.Instance = this;
			if (!this.Text)
			{
				this.Text = base.gameObject.GetComponent<Text>();
			}
		}

		private void OnGUI()
		{
			float x = (float)Screen.width * this.Position.x;
			float y = (float)Screen.height * this.Position.y;
			float width = (float)Screen.width * this.Size.x;
			float height = (float)Screen.height * this.Size.y;
			GUI.DrawTexture(new Rect(x, y, width, height), this.HUD, ScaleMode.ScaleToFit);
			x = (float)Screen.width * this.Pos2.x;
			y = (float)Screen.height * this.Pos2.y;
			float num = (float)Screen.height * this.Size2;
			//this.Text.pixelOffset = new Vector2(x, y);
			this.Text.fontSize = (int)num;
			this.Text.text = this.worms.ToString() + "\n\n" + this.zombies.ToString();
		}

		public Texture HUD;

		public Vector2 Position;

		public Vector2 Size;

		public Vector2 Pos2;

		public float Size2;

		public Text Text;

		private int zombies;

		private int worms;
	}
}
