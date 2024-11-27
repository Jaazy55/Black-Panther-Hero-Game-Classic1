using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.MiniMap;
using Game.UI;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class MetroPanel : MonoBehaviour
	{
		private MetroManager metroManager
		{
			get
			{
				if (this.manager)
				{
					return this.manager;
				}
				this.manager = MetroManager.Instance;
				return this.manager;
			}
		}

		private void Awake()
		{
			MetroPanel.Instance = this;
		}

		private void Start()
		{
			this.player = PlayerInteractionsManager.Instance.Player;
		}

		public void Open()
		{
			if (!this.player)
			{
				this.player = PlayerInteractionsManager.Instance.Player;
			}
			if (this.player.IsDead)
			{
				return;
			}
			this.MenuManager.OpenPanel(this.MetroPaneleAnimator);
		}

		private void OnEnable()
		{
			UIGame.Instance.Pause();
			foreach (Metro metro in this.metroManager.Metros)
			{
				if (metro.MetroMark)
				{
					metro.MetroMark.SetMetroSprite(this.MetroMapSprite);
				}
			}
			this.CheckSelected();
		}

		private void OnDisable()
		{
			if (!this.manager)
			{
				return;
			}
			UIGame.Instance.Resume();
			foreach (Metro metro in this.metroManager.Metros)
			{
				if (metro.MetroMark)
				{
					metro.MetroMark.SetNormalSprite(this.MenuMapSprite);
				}
			}
			Game.MiniMap.MiniMap.Instance.ChangeMapSize(false);
		}

		private void Update()
		{
			this.AnimateMetro();
		}

		private void AnimateMetro()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			if (!this.metroManager.TerminusMetro)
			{
				return;
			}
			if (!this.animatedMetro)
			{
				this.animatedMetro = this.metroManager.TerminusMetro;
			}
			if (this.animatedMetro.Equals(this.metroManager.TerminusMetro))
			{
				this.timer += Time.fixedDeltaTime;
				if (this.AnimationSettings.timePerFrame / 2f > this.timer)
				{
					this.animatedMetro.MetroMark.drawedIconSprite.color = Color.Lerp(this.animatedMetro.MetroMark.drawedIconSprite.color, this.AnimationSettings.StartColor, Time.fixedDeltaTime * 8f);
					this.animatedMetro.MetroMark.IconScale = Mathf.Lerp(this.animatedMetro.MetroMark.IconScale, this.NormalScale * this.AnimationSettings.Scale, Time.fixedDeltaTime * 2f);
				}
				else
				{
					this.animatedMetro.MetroMark.drawedIconSprite.color = Color.Lerp(this.animatedMetro.MetroMark.drawedIconSprite.color, this.AnimationSettings.EndColor, Time.fixedDeltaTime * 8f);
					this.animatedMetro.MetroMark.IconScale = Mathf.Lerp(this.animatedMetro.MetroMark.IconScale, this.NormalScale, Time.fixedDeltaTime * 2f);
				}
				if (this.AnimationSettings.timePerFrame < this.timer)
				{
					this.timer = 0f;
				}
			}
			else
			{
				this.animatedMetro.MetroMark.IconScale = this.NormalScale;
				this.animatedMetro = null;
			}
		}

		public void CheckSelected()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			foreach (Metro metro in MetroManager.Instance.Metros)
			{
				if (metro.Equals(this.metroManager.CurrentMetro))
				{
					metro.MetroMark.SetCurrent();
				}
				else if (metro.Equals(this.metroManager.TerminusMetro))
				{
					metro.MetroMark.Select();
				}
				else
				{
//					metro.MetroMark.DisableSelected();
				}
			}
		}

		public static MetroPanel Instance;

		public Sprite MenuMapSprite;

		public Sprite MetroMapSprite;

		public float NormalScale;

		public Animator MetroPaneleAnimator;

		public MenuPanelManager MenuManager;

		public MetroPanel.AnimationSetup AnimationSettings;

		private MetroManager manager;

		private Metro animatedMetro;

		private float timer;

		private Player player;

		[Serializable]
		public class AnimationSetup
		{
			public float Scale = 2f;

			public Color StartColor;

			public Color EndColor;

			public float timePerFrame = 2f;
		}
	}
}
