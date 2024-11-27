using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.MiniMap
{
	public class MetroMarkForMiniMap : MarkForMiniMap
	{
		protected override void DrawIcon()
		{
			base.DrawIcon();
			BoxCollider boxCollider = this.drawedIconSprite.gameObject.AddComponent<BoxCollider>();
			boxCollider.size = new Vector3(boxCollider.size.x * 2f, boxCollider.size.y * 2f, 0.01f);
			this.drawedIconSprite.gameObject.AddComponent<MarkButton>().Init(this);
		}

		public void Select()
		{
			this.drawedIconSprite.color = Color.green;
		}

		public void SetCurrent()
		{
			this.drawedIconSprite.color = Color.yellow + Color.red;
		}

		public void DisableSelected()
		{
			this.drawedIconSprite.color = Color.white;
		}

		public void SetNormalSprite(Sprite sprite)
		{
			this.drawedIconSprite.sprite = this.IconImage;
			this.drawedIconSprite.transform.localScale = new Vector3(this.IconScale, this.IconScale, this.IconScale);
			this.DisableSelected();
		}

		public void SetMetroSprite(Sprite sprite)
		{
			this.drawedIconSprite.sprite = sprite;
		}

		public override void MarckOnClick()
		{
			if (!MetroManager.Instance.InMetro)
			{
				return;
			}
			if (this.MainMetro.Equals(MetroManager.Instance.TerminusMetro))
			{
				MetroManager.Instance.TakeTheSubway();
			}
			else
			{
				MetroManager.Instance.SetTerminus(this.MainMetro);
			}
			if (MetroPanel.Instance)
			{
				MetroPanel.Instance.CheckSelected();
			}
		}

		private const float ColliderScaleRate = 2f;

		public Metro MainMetro;
	}
}
