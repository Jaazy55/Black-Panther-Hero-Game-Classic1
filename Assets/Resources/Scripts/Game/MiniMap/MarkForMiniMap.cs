using System;
using System.Collections;
using UnityEngine;

namespace Game.MiniMap
{
	public class MarkForMiniMap : MonoBehaviour
	{
		private void Awake()
		{
			this.Init();
		}

		private void Init()
		{
			this.miniMap = MiniMap.Instance;
			this.parent = this.miniMap.WorldSpace;
			this.visibleRange = this.miniMap.MiniMapOrthographicSize - 13f;
			this.iconY = this.parent.position.y + 1f + this.SortLayer;

			this.DrawIcon();
		}

		protected virtual void DrawIcon()
		{
			if (this.drawedIcon)
			{
				return;
			}
			GameObject gameObject = new GameObject("MapIcon");
			gameObject.transform.position = new Vector3(base.gameObject.transform.position.x, this.iconY, base.gameObject.transform.position.z);
			gameObject.transform.eulerAngles = new Vector3(90f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(this.IconScale, this.IconScale, this.IconScale);
			gameObject.layer = 21;
			gameObject.transform.parent = this.parent;
			this.drawedIconSprite = gameObject.AddComponent<SpriteRenderer>();
			this.drawedIconSprite.sprite = this.IconImage;
			this.drawedIconSprite.color = this.Color;
			this.drawedIcon = gameObject;
			this.drawing = true;
		}

		private void DrawArrow(Vector3 position, bool draw)
		{
			if (!this.drawing)
			{
				return;
			}
			if (!this.drawedArrow)
			{
				GameObject gameObject = new GameObject("ArrowIcon");
				gameObject.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(this.ArrowScale, this.ArrowScale, this.ArrowScale);
				gameObject.layer = 21;
				gameObject.transform.parent = this.parent;
				this.drawedArrowSprite = gameObject.AddComponent<SpriteRenderer>();
				this.drawedArrowSprite.sprite = this.ArrowImage;
				this.drawedArrowSprite.color = this.Color;
				this.drawedArrow = gameObject;
			}
			if (draw)
			{
				this.drawedArrow.transform.position = new Vector3(position.x, this.iconY, position.z);
				if (this.RotateArrow)
				{
					Quaternion rotation = Quaternion.LookRotation(new Vector3(base.transform.position.x, this.iconY, base.transform.position.z) - this.drawedArrow.transform.position);
					this.drawedArrow.transform.rotation = rotation;
				}
				this.drawedArrow.transform.eulerAngles = new Vector3(90f, this.drawedArrow.transform.eulerAngles.y, this.drawedArrow.transform.eulerAngles.z);
			}
			this.drawedArrow.SetActive(draw);
			this.drawedIcon.SetActive(!draw);
		}

		public void ShowIcon()
		{
			this.drawing = true;
			if (this.drawedIcon)
			{
				this.drawedIconSprite.color = new Color(this.Color.r, this.Color.g, this.Color.b, 1f);
			}
			if (this.drawedArrow)
			{
				this.drawedArrowSprite.color = new Color(this.Color.r, this.Color.g, this.Color.b, 1f);
			}
		}

		public void HideIcon()
		{
			this.drawing = false;
			if (this.drawedIcon)
			{
				this.drawedIconSprite.color = new Color(this.Color.r, this.Color.g, this.Color.b, 0f);
			}
			if (this.drawedArrow)
			{
				this.drawedArrowSprite.color = new Color(this.Color.r, this.Color.g, this.Color.b, 0f);
			}
		}

		public void RotateIcon(Vector3 eulerRotation)
		{
			if (!this.drawedIcon)
			{
				this.Init();
			}
			this.drawedIcon.transform.localEulerAngles = new Vector3(0f, 180f, eulerRotation.y);
		}

		public virtual void MarckOnClick()
		{
		}

		private void OnDestroy()
		{
			if (this.drawedIcon)
			{
				UnityEngine.Object.Destroy(this.drawedIcon);
			}
		}

		private void Update()
		{
			if (this.drawing)
			{
				this.drawedIcon.transform.position = new Vector3(base.transform.position.x, this.iconY, base.transform.position.z);
			}
			this.ScaleControll();
		}

		private IEnumerator CheckVisibility()
		{
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			for (;;)
			{
				this.target = this.miniMap.Target;
				if (this.target)
				{
					Vector3 vector = new Vector3(this.target.transform.position.x, this.iconY, this.target.transform.position.z);
					Vector3 vector2 = new Vector3(base.transform.position.x, this.iconY, base.transform.position.z);
					float num = Vector3.Distance(vector, vector2);
					if (num > this.visibleRange && !this.miniMap.IsFullScreen)
					{
						if (this.FullMapArrow)
						{
							Vector3 a = (vector2 - vector).normalized * this.visibleRange;
							this.DrawArrow(a + vector, true);
						}
						else if (num < this.NotFullMapVisibleRange)
						{
							Vector3 a2 = (vector2 - vector).normalized * this.visibleRange;
							this.DrawArrow(a2 + vector, true);
						}
						else
						{
							this.DrawArrow(default(Vector3), false);
						}
					}
					else
					{
						this.DrawArrow(default(Vector3), false);
					}
				}
				yield return waitForEndOfFrame;
			}
			yield break;
		}

		private void ScaleControll()
		{
			float num = (!this.miniMap.IsFullScreen) ? 0f : (this.miniMap.MiniMapCamera.orthographicSize / 8f);
			this.drawedIcon.transform.localScale = new Vector3(this.IconScale + num, this.IconScale + num, this.IconScale + num);
		}

		private void OnDisable()
		{
			this.HideIcon();
			base.StopAllCoroutines();
		}

		private void OnEnable()
		{
			if (this.ArrowImage)
			{
				base.StartCoroutine(this.CheckVisibility());
			}
			this.ShowIcon();
		}

		private const int MMLayer = 21;

		private const int MarkVisibleOffset = 13;

		private const int ScaleCounterReduction = 8;

		public Sprite IconImage;

		public Sprite ArrowImage;

		public bool RotateArrow = true;

		public bool FullMapArrow = true;

		public float NotFullMapVisibleRange;

		public Color Color = Color.white;

		public float IconScale = 20f;

		public float ArrowScale = 90f;

		public float SortLayer;

		private GameObject drawedIcon;

		private GameObject drawedArrow;

		[HideInInspector]
		public SpriteRenderer drawedIconSprite;

		private SpriteRenderer drawedArrowSprite;

		private bool drawing;

		private float iconY;

		private GameObject target;

		private Transform parent;

		private float visibleRange;

		private MiniMap miniMap;
	}
}
