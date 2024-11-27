using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette
{
	public class RoulettePanel : MonoBehaviour
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnSpin;

		public RouletteController Controller
		{
			get
			{
				return this.m_Controller;
			}
			set
			{
				this.m_Controller = value;
				this.m_PrizesDistribution.Clear();
				if (this.m_Controller != null)
				{
					this.m_PrizesDistribution.AddRange(this.m_Controller.Distribution.Probabilities);
					this.GenerateLuckUpButtons(this.m_Controller.LuckPrizes);
					this.Luck = false;
				}
			}
		}

		public List<LuckUpButton> LuckUpButtons
		{
			get
			{
				if (this.m_LuckUpButtons == null)
				{
					this.m_LuckUpButtons = new List<LuckUpButton>();
					this.m_LuckUpButtons.AddRange(this.LuckUpContainer.GetComponentsInChildren<LuckUpButton>());
				}
				return this.m_LuckUpButtons;
			}
		}

		public void MouseDown()
		{
			this.Fail = (this.m_Slider.value > 0.2f || !this.m_Slider.interactable);
			if (this.m_Slider.interactable)
			{
				this.m_Slider.enabled = !this.Fail;
			}
			if (this.Fail && this.m_Slider.interactable)
			{
				this.m_Slider.value = 0f;
			}
		}

		public void MouseUp()
		{
			if (this.Fail)
			{
				this.Fail = false;
				this.m_Slider.enabled = true;
			}
		}

		public void BeginDrag()
		{
			this.SupportFinger.SetActive(false);
			this.FailDrag = this.Fail;
			if (this.Fail)
			{
				return;
			}
			this.FailDrag = (this.m_Slider.value > 0.2f);
			if (this.FailDrag)
			{
				if (this.m_Slider.interactable)
				{
					this.m_Slider.value = 0f;
				}
				this.m_Slider.enabled = false;
			}
			else
			{
				this.m_Slider.enabled = true;
				this.m_LuckButton.gameObject.SetActive(false);
				this.m_BackButton.gameObject.SetActive(false);
			}
		}

		public void EndDrag()
		{
			if (!this.FailDrag)
			{
				this.CheckSliderValue();
			}
			this.MouseUp();
		}

		public void CheckSliderValue()
		{
			if (this.m_Slider.interactable)
			{
				if (this.OnSpin != null)
				{
					this.OnSpin();
				}
				this.NeedPrize = this.m_PrizesDistribution.GetRandomPrize();
				UnityEngine.Debug.Log("You must get " + this.NeedPrize.name);
				this.Controller.Spins--;
				this.m_Description.text = string.Format("You have {0} spins", this.Controller.Spins);
				float num = (!(this.SelectedCell != null)) ? 0f : (this.SelectedCell.rectTransform.localPosition.x - this.SelectOffset + this.MovableTransform.anchoredPosition.x);
				this.RouletteCount = Mathf.FloorToInt(this.m_TotalLenghtPerSlider.Evaluate(this.m_Slider.value) / this.CellSpace);
				this.RouletteTotalLenght = ((float)this.RouletteCount + 1f + UnityEngine.Random.Range(-0.3f, 0.3f)) * this.CellSpace + num;
				this.RouletteResidualLenght = this.RouletteTotalLenght;
				this.m_Slider.interactable = false;
				int num2 = (!this.SelectedCell) ? -1 : this.MovableCells.IndexOf(this.SelectedCell);
				this.RouletteCount -= this.MovableCells.Count - num2 - 1;
				this.m_MusicSource.Play();
			}
		}

		[ContextMenu("Generate Cells")]
		private void GenerateCells()
		{
			this.MovableTransform = (this.m_MovableContainer.transform as RectTransform);
			while (this.MovableTransform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(this.MovableTransform.GetChild(0).gameObject);
			}
			for (int i = 0; i < this.CellPoolCount; i++)
			{
				UnityEngine.Object.Instantiate<RouletteCell>(this.CellPrefab, this.MovableTransform);
			}
		}

		[ContextMenu("Generate LuckUpButtons")]
		private void GenerateLuckUp()
		{
			while (this.LuckUpContainer.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(this.LuckUpContainer.GetChild(0).gameObject);
			}
			for (int i = 0; i < this.LuckUpPoolCount; i++)
			{
				UnityEngine.Object.Instantiate<LuckUpButton>(this.LuckUpPrefab, this.LuckUpContainer);
			}
		}

		private void OnEnable()
		{
			this.RouletteResidualLenght = 0f;
			this.ResetSlider();
			this.GenerateRandomCells();
			this.m_Description.text = string.Format("You have {0} spins", this.Controller.Spins);
			this.SupportFinger.SetActive(true);
			this.LuckIndicator.gameObject.SetActive(false);
		}

		private void Awake()
		{
			this.MovableTransform = (this.m_MovableContainer.transform as RectTransform);
			int childCount = this.MovableTransform.childCount;
			this.MovableCells = new List<RouletteCell>();
			for (int i = 0; i < childCount; i++)
			{
				RouletteCell component = this.MovableTransform.GetChild(i).GetComponent<RouletteCell>();
				this.MovableCells.Add(component);
			}
		}

		private void Start()
		{
			this.leftBorder = (this.MovableTransform.parent as RectTransform).rect.xMin;
			this.rightBorder = (this.MovableTransform.parent as RectTransform).rect.xMax;
			this.SelectOffset = this.m_Mask.GetComponent<RectTransform>().rect.width * this.SelectPositionPercent;
			if (this.MovableCells.Count > 0)
			{
				this.CellSpace = this.MovableCells[0].rectTransform.rect.width;
			}
		}

		private void Update()
		{
			this.CheckSelectedCell();
			if (this.RouletteResidualLenght > 0f)
			{
				float delta = Time.unscaledDeltaTime * this.m_SpeedPerLenght.Evaluate(this.RouletteResidualLenght);
				this.MoveContainer(delta);
				if (this.RouletteResidualLenght <= 0f)
				{
					this.MoveFinish();
					this.RouletteResidualLenght = 0f;
					this.ResetSlider();
				}
			}
		}

		private void MoveContainer(float delta)
		{
			delta = Mathf.Min(this.RouletteResidualLenght, delta);
			Vector3 localPosition = this.MovableTransform.localPosition;
			localPosition.x -= delta;
			this.MovableTransform.localPosition = localPosition;
			this.RouletteResidualLenght -= delta;
			int num = this.MovableCells.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				RouletteCell rouletteCell = this.MovableCells[i];
				float num2 = rouletteCell.rectTransform.localPosition.x + rouletteCell.rectTransform.rect.xMax + this.MovableTransform.localPosition.x;
				if (num2 < this.leftBorder)
				{
					localPosition.x += rouletteCell.rectTransform.rect.width;
					rouletteCell.transform.SetSiblingIndex(this.MovableTransform.childCount - 1);
					if (this.RouletteCount != 0)
					{
						rouletteCell.SetPrize(this.GenerateNextCell());
					}
					else
					{
						rouletteCell.SetPrize(this.NeedPrize);
					}
					this.MovableCells.RemoveAt(i);
					this.MovableCells.Add(rouletteCell);
					i--;
					num--;
					this.RouletteCount--;
				}
			}
			this.MovableTransform.localPosition = localPosition;
		}

		private void GenerateLuckUpButtons(LuckPrize[] luckPrizes)
		{
			int count = this.LuckUpButtons.Count;
			int num = luckPrizes.Length;
			for (int i = 0; i < count; i++)
			{
				bool flag = i < num && luckPrizes[i].Prize.CanBeGiven;
				this.LuckUpButtons[i].gameObject.SetActive(flag);
				this.LuckUpButtons[i].Button.onClick.RemoveAllListeners();
				if (flag)
				{
					int index = this.m_PrizesDistribution.IndexOf(luckPrizes[i].Prize);
					if (index > 0)
					{
						int prizeIndex = i;
						this.LuckUpButtons[i].SetLuckPrize(this.m_PrizesDistribution[index].Prize, this.m_PrizesDistribution[index].Probability, luckPrizes[i].LuckDistribution);
						this.LuckUpButtons[i].Button.onClick.AddListener(delegate()
						{
						//	AdsManager.ShowRewardAd(delegate(AdsResult result)
						//	{
								//MainThreadExecuter.Instance.Run(delegate
							//	{
									//if (result == AdsResult.Finished)
									//{
										this.m_PrizesDistribution[index] = new PrizeProbability(this.m_PrizesDistribution[index].Prize, this.m_PrizesDistribution[index].Probability + luckPrizes[prizeIndex].LuckDistribution);
										this.m_PrizesDistribution.Normalize();
										this.LuckIndicator.sprite = this.m_PrizesDistribution[index].Prize.Icon;
										this.LuckIndicator.gameObject.SetActive(true);
										this.Luck = true;
									//}
							//	});
							//});
							this.m_LuckPanel.SetActive(false);
							this.m_LuckButton.gameObject.SetActive(false);
						});
					}
				}
			}
		}

		private void GenerateRandomCells()
		{
			this.LastGeneratedPrize = null;
			int count = this.m_PrizesDistribution.Count;
			for (int i = 0; i < this.MovableCells.Count; i++)
			{
				Prize prize;
				do
				{
					prize = this.m_PrizesDistribution[UnityEngine.Random.Range(0, count)].Prize;
				}
				while (prize == this.LastGeneratedPrize);
				this.LastGeneratedPrize = prize;
				this.MovableCells[i].SetPrize(prize);
			}
		}

		private Prize GenerateNextCell()
		{
			int count = this.m_PrizesDistribution.Count;
			Prize prize;
			do
			{
				prize = this.m_PrizesDistribution[UnityEngine.Random.Range(0, count)].Prize;
			}
			while (prize == this.LastGeneratedPrize || prize == this.NeedPrize || !prize.CanBeGiven);
			this.LastGeneratedPrize = prize;
			return prize;
		}

		private void ResetSlider()
		{
			this.m_Slider.value = 0f;
			this.m_Slider.interactable = (this.Controller != null && this.Controller.Spins > 0);
			if (this.Controller.Spins > 0)
			{
				this.m_LuckButton.gameObject.SetActive(this.CanLuckUp());
			}
			this.m_BackButton.gameObject.SetActive(true);
		}

		private void CheckSelectedCell()
		{
			this.SelectedCell = null;
			float num = -1f;
			float x = this.MovableTransform.anchoredPosition.x;
			foreach (RouletteCell rouletteCell in this.MovableCells)
			{
				float num2 = Mathf.Abs(this.SelectOffset - rouletteCell.rectTransform.localPosition.x - x);
				if (num < 0f || num2 < num)
				{
					this.SelectedCell = rouletteCell;
					num = num2;
				}
			}
			foreach (RouletteCell rouletteCell2 in this.MovableCells)
			{
				rouletteCell2.SetSelectState(rouletteCell2 == this.SelectedCell);
			}
		}

		private void MoveFinish()
		{
			if (this.SelectedCell != null && this.SelectedCell.Prize != null)
			{
				this.SelectedCell.Prize.WillBeGiven();
				this.m_GetPrizePanel.Show(this.SelectedCell.Prize);
			}
			if (this.Luck)
			{
				this.LuckIndicator.gameObject.SetActive(false);
				this.Luck = false;
				this.m_PrizesDistribution.Clear();
				if (this.m_Controller)
				{
					this.m_PrizesDistribution.AddRange(this.m_Controller.Distribution.Probabilities);
				}
			}
		}

		private bool CanLuckUp()
		{
			//	if (this.Controller != null && AdsManager.IsRewardReady())
			if (this.Controller != null)
			{
				foreach (LuckPrize luckPrize in this.Controller.LuckPrizes)
				{
					if (luckPrize.Prize.CanBeGiven)
					{
						return true;
					}
				}
			}
			return false;
		}

		private const float koefForce = 5f;

		private const string DescriptionText = "You have {0} spins";

		[SerializeField]
		private Text m_Description;

		[SerializeField]
		private GameObject SupportFinger;

		[SerializeField]
		private Slider m_Slider;

		[SerializeField]
		private Button m_LuckButton;

		[SerializeField]
		private Button m_BackButton;

		[SerializeField]
		private GetPrizePanel m_GetPrizePanel;

		[SerializeField]
		private GameObject m_LuckPanel;

		[SerializeField]
		private Transform LuckUpContainer;

		[SerializeField]
		private Image LuckIndicator;

		[SerializeField]
		private Mask m_Mask;

		[SerializeField]
		private HorizontalLayoutGroup m_MovableContainer;

		[SerializeField]
		private AudioSource m_MusicSource;

		[SerializeField]
		[Tooltip("Занимаемая позиция выделения ячейеи относительно левого края ко всему видимому пространству")]
		[Range(0f, 1f)]
		private float SelectPositionPercent;

		[SerializeField]
		private AnimationCurve m_TotalLenghtPerSlider;

		[SerializeField]
		private AnimationCurve m_SpeedPerLenght;

		[SerializeField]
		private RouletteCell CellPrefab;

		[SerializeField]
		private int CellPoolCount = 6;

		[SerializeField]
		private LuckUpButton LuckUpPrefab;

		[SerializeField]
		private int LuckUpPoolCount = 3;

		private Prize NeedPrize;

		private Prize LastGeneratedPrize;

		private float CellSpace;

		private float SelectOffset;

		private List<PrizeProbability> m_PrizesDistribution = new List<PrizeProbability>();

		private int RouletteCount;

		private float RouletteLenght;

		private float RouletteResidualLenght;

		private float RouletteTotalLenght;

		private RectTransform MovableTransform;

		private float leftBorder;

		private float rightBorder;

		private List<RouletteCell> MovableCells;

		private RouletteCell SelectedCell;

		private List<LuckUpButton> m_LuckUpButtons;

		private bool Luck;

		private bool Fail;

		private bool FailDrag;

		private RouletteController m_Controller;
	}
}
