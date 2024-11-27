using System;
using System.Collections.Generic;
using Game.Character;
using Game.MiniMap;
using Game.Tools;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class UIMarkManager : MonoBehaviour
	{
		public static UIMarkManager Instance
		{
			get
			{
				return UIMarkManager.instance;
			}
		}

		public static bool InstanceExist
		{
			get
			{
				return !(UIMarkManager.instance == null);
			}
		}

		private bool IsCanViewMark
		{
			get
			{
				return !(this.m_DialogWindow != null) || !this.m_DialogWindow.activeSelf;
			}
		}

		public Transform TargetStaticMark
		{
			get
			{
				return this.m_StaticMark.Target;
			}
			set
			{
				this.m_StaticMark.Target = value;
			}
		}

		public void ActivateStaticMark(bool value)
		{
			if (value != this.m_StaticMark.mark.gameObject.activeSelf)
			{
				this.m_StaticMark.mark.gameObject.SetActive(value);
			}
		}

		private void HideMark(MarkContainer markContainer, bool value)
		{
			markContainer.mark.Hide(value);
		}

		private void UpdateMarkDistanceLabel(MarkContainer markContainer, float dist)
		{
			UIMarkView uimarkView = markContainer.mark as UIMarkView;
			if (uimarkView != null)
			{
				uimarkView.UpdateDistanceLabel(dist);
			}
		}

		private void UpdateMarkPosition(MarkContainer markContainer)
		{
			if (markContainer.Target == null)
			{
				return;
			}
			this.viewportPos = this.m_Camera.WorldToViewportPoint(markContainer.Target.position + markContainer.offset);
			bool flag = this.viewportPos.z < 0f;
			bool flag2 = FastMath.PointInRect(this.viewportPos, this.m_RectView);
			if (!flag && flag2)
			{
				this.viewportPos.z = 0f;
				markContainer.mark.transform.position = this.m_Camera.ViewportToScreenPoint(this.viewportPos);
				return;
			}
			if (flag)
			{
				Vector3 b = this.m_Camera.WorldToViewportPoint(markContainer.Target.position);
				b.z = 0f;
				Vector3 vector = this.CenterViewPort - b;
				vector = FastMath.SetVectorLength(vector, 20f);
				this.viewportPos = this.CenterViewPort + vector;
			}
			this.viewportPos = FastMath.LineIntersectionRect(this.CenterViewPort, this.viewportPos, this.m_RectView);
			this.viewportPos.z = 0f;
			markContainer.mark.transform.position = this.m_Camera.ViewportToScreenPoint(this.viewportPos);
		}

		private float GetDistance(MarkContainer markContainer)
		{
			if (markContainer.Target == null)
			{
				return 0f;
			}
			if (PlayerInteractionsManager.Instance == null)
			{
				return Vector3.Distance(this.m_Camera.ViewportToWorldPoint(Vector3.one * 0.5f), markContainer.Target.position);
			}
			return Vector3.Distance(PlayerInteractionsManager.Instance.GetPlayerPosition(), markContainer.Target.position);
		}

		public MarkContainer AddDinamicMark(Transform target, string typeMark)
		{
			MarkDetails markByType = this.m_MarksData.GetMarkByType(typeMark);
			UIMarkViewBase markView = UnityEngine.Object.Instantiate<UIMarkViewBase>(this.m_UIMarkSimplePrefab, this.m_MarkParent, false);
			MarkContainer markContainer = new MarkContainer(markView, target, markByType);
			this.m_DinamicMarks.Add(markContainer);
			return markContainer;
		}

		public void RemoveDinamicMarks(MarkContainer mark)
		{
			if (mark != null && this.m_DinamicMarks != null && this.m_DinamicMarks.Count > 0)
			{
				int num = this.m_DinamicMarks.IndexOf(mark);
				if (num >= 0)
				{
					mark.FreeResources();
					this.m_DinamicMarks[num] = null;
				}
			}
		}

		private void RemoveNullMarks()
		{
			if (this.m_DinamicMarks != null)
			{
				this.m_DinamicMarks.RemoveAll((MarkContainer x) => x == null);
			}
		}

		private void UpdateStaticMark()
		{
			if (this.IsCanViewMark)
			{
				float distance = this.GetDistance(this.m_StaticMark);
				if (distance < this.m_StaticMark.MinDistanceView && this.m_StaticMark.MinDistanceView > 0f)
				{
					this.HideMark(this.m_StaticMark, true);
					return;
				}
				this.HideMark(this.m_StaticMark, false);
				this.UpdateMarkPosition(this.m_StaticMark);
				this.UpdateMarkDistanceLabel(this.m_StaticMark, distance);
			}
			else
			{
				this.HideMark(this.m_StaticMark, true);
			}
		}

		private void UpdateDinamicMarks()
		{
			int count = this.m_DinamicMarks.Count;
			for (int i = 0; i < count; i++)
			{
				MarkContainer markContainer = this.m_DinamicMarks[i];
				if (markContainer != null)
				{
					if (this.IsCanViewMark && markContainer.Target != null && markContainer.Target.gameObject.activeInHierarchy)
					{
						float distance = this.GetDistance(markContainer);
						if (distance < markContainer.MinDistanceView && markContainer.MinDistanceView > 0f)
						{
							this.HideMark(markContainer, true);
						}
						else
						{
							this.HideMark(markContainer, false);
							this.UpdateMarkPosition(markContainer);
						}
					}
					else
					{
						this.HideMark(markContainer, true);
					}
				}
			}
		}

		private void ChangeSignalHandler(bool hasSignal)
		{
			base.enabled = hasSignal;
		}

		private void Awake()
		{
			UIMarkManager.instance = this;
		}

		private void Start()
		{
			Game.MiniMap.MiniMap.Instance.OnChangeSignal += this.ChangeSignalHandler;
		}

		private void OnDestroy()
		{
			if (Game.MiniMap.MiniMap.HasInstance)
			{
				Game.MiniMap.MiniMap.Instance.OnChangeSignal -= this.ChangeSignalHandler;
			}
		}

		private void OnEnable()
		{
			this.m_MarkParent.gameObject.SetActive(true);
		}

		private void OnDisable()
		{
			this.m_MarkParent.gameObject.SetActive(false);
		}

		private void Update()
		{
			this.UpdateStaticMark();
			this.UpdateDinamicMarks();
			this.RemoveNullMarks();
		}

		private static UIMarkManager instance;

		private List<MarkContainer> m_DinamicMarks = new List<MarkContainer>();

		[Separator("Settings")]
		public Camera m_Camera;

		[SerializeField]
		private MarkContainer m_StaticMark;

		public Rect m_RectView = new Rect(0.1f, 0.1f, 0.8f, 0.8f);

		[Separator("Blocked windows")]
		public GameObject m_DialogWindow;

		private Vector3 viewportPos;

		private readonly Vector3 CenterViewPort = new Vector3(0.5f, 0.5f, 0f);

		[SerializeField]
		private MarkListBase m_MarksData;

		[SerializeField]
		private Transform m_MarkParent;

		[SerializeField]
		private UIMarkViewBase m_UIMarkSimplePrefab;
	}
}
