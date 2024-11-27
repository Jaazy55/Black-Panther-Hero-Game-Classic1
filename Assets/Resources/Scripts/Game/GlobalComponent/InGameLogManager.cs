using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class InGameLogManager : MonoBehaviour
	{
		public static InGameLogManager Instance
		{
			get
			{
				if (InGameLogManager.instance == null)
				{
					throw new Exception("InGameLogManager is not initialized");
				}
				return InGameLogManager.instance;
			}
		}

		public bool LogFree
		{
			get
			{
				return Time.time > this.lastMessageShowedTime + this.StartedShowTime;
			}
		}

		private void Awake()
		{
			InGameLogManager.instance = this;
		}

		public void RegisterNewMessage(MessageType messageType, string specificString)
		{
			string format = this.messagePresets[messageType];
			Color color = this.colorPresets[messageType];
			GameObject fromPool = PoolManager.Instance.GetFromPool(this.TextSample.gameObject);
			RectTransform rectTransform = fromPool.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(this.InGameLogPanel.transform, false);
			}
			else
			{
				fromPool.transform.parent = this.InGameLogPanel.transform;
			}
			fromPool.transform.localScale = Vector3.one;
			Text component = fromPool.GetComponent<Text>();
			component.text = string.Format(format, specificString);
			component.color = color;
			fromPool.SetActive(false);
			this.messageStack.Add(fromPool);
		}

		private void Update()
		{
			if (!this.StartTextPosition.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.messageStack.Count > 0 && this.LogFree)
			{
				GameObject gameObject = this.messageStack[0];
				gameObject.GetComponent<RectTransform>().anchoredPosition = this.StartTextPosition.anchoredPosition;
				gameObject.SetActive(true);
				this.messageStack.Remove(gameObject);
				this.showedMessage.Add(gameObject);
				Coroutine value = base.StartCoroutine(this.RemoveMessageFromLog(gameObject, this.MessageLifeTime));
				if (this.coroutineList.ContainsKey(gameObject))
				{
					UnityEngine.Debug.LogWarning("InGameLogManager: Unexpected logic, skipping message: " + gameObject.GetComponent<Text>().text);
				}
				else
				{
					this.coroutineList.Add(gameObject, value);
					this.lastMessageShowedTime = Time.time;
				}
			}
			if (this.showedMessage.Count >= this.MaxMessageCount)
			{
				base.StopCoroutine(this.coroutineList[this.showedMessage[0]]);
				this.RemoveMessageImmediate(this.showedMessage[0]);
			}
		}

		private IEnumerator RemoveMessageFromLog(GameObject message, float delay)
		{
			if (!this.showedMessage.Contains(message))
			{
				yield break;
			}
			yield return new WaitForSeconds(delay);
			this.RemoveMessageImmediate(message);
			yield break;
		}

		private void RemoveMessageImmediate(GameObject message)
		{
			this.showedMessage.Remove(message);
			this.coroutineList.Remove(message);
			PoolManager.Instance.ReturnToPool(message);
		}

		private static InGameLogManager instance;

		public Text TextSample;

		public GameObject InGameLogPanel;

		public RectTransform StartTextPosition;

		public int MaxMessageCount = 4;

		public float StartedShowTime;

		public float MessageLifeTime;

		private float lastMessageShowedTime;

		private IDictionary<GameObject, Coroutine> coroutineList = new Dictionary<GameObject, Coroutine>();

		private readonly List<GameObject> messageStack = new List<GameObject>();

		private readonly List<GameObject> showedMessage = new List<GameObject>();

		private readonly IDictionary<MessageType, string> messagePresets = new Dictionary<MessageType, string>
		{
			{
				MessageType.Money,
				"{0} money"
			},
			{
				MessageType.HealthPack,
				"+{0}HP"
			},
			{
				MessageType.Item,
				"{0}"
			},
			{
				MessageType.Bullets,
				"{0} ammo"
			},
			{
				MessageType.QuestItem,
				"{0}"
			},
			{
				MessageType.QuestStart,
				"'{0}' starting."
			},
			{
				MessageType.QuestFinished,
				"'{0}' complete!"
			},
			{
				MessageType.QuestFailed,
				"'{0}' failed!"
			},
			{
				MessageType.SitInCar,
				"{0}"
			},
			{
				MessageType.AddQuestTime,
				"+{0} seconds"
			},
			{
				MessageType.RadioName,
				"{0}"
			},
			{
				MessageType.Enegry,
				"+{0} enegry"
			},
			{
				MessageType.Experience,
				"+{0} EXP"
			},
			{
				MessageType.Gems,
				"+{0} Gems"
			},
			{
				MessageType.Collect,
				"+{0}"
			},
			{
				MessageType.NegativeMoney,
				"{0} money"
			}
		};

		private readonly IDictionary<MessageType, Color> colorPresets = new Dictionary<MessageType, Color>
		{
			{
				MessageType.Money,
				new Color(0.8f, 1f, 0.8f)
			},
			{
				MessageType.HealthPack,
				new Color(1f, 0.8f, 0.8f)
			},
			{
				MessageType.Item,
				new Color(0.7f, 0.4f, 0.7f)
			},
			{
				MessageType.Bullets,
				new Color(0.8f, 0.8f, 0.8f)
			},
			{
				MessageType.QuestItem,
				new Color(0.8f, 0.8f, 1f)
			},
			{
				MessageType.QuestStart,
				new Color(1f, 1f, 0.8f)
			},
			{
				MessageType.QuestFinished,
				new Color(1f, 1f, 0.8f)
			},
			{
				MessageType.QuestFailed,
				new Color(1f, 0f, 0f)
			},
			{
				MessageType.SitInCar,
				new Color(1f, 0.8f, 0.1f)
			},
			{
				MessageType.AddQuestTime,
				new Color(1f, 0.5f, 0.5f)
			},
			{
				MessageType.RadioName,
				new Color(0f, 0.5f, 1f)
			},
			{
				MessageType.Enegry,
				new Color(1f, 0.5f, 1f)
			},
			{
				MessageType.Experience,
				new Color(0f, 0.8f, 0f)
			},
			{
				MessageType.Gems,
				new Color(1f, 1f, 1f)
			},
			{
				MessageType.Collect,
				new Color(1f, 1f, 1f)
			},
			{
				MessageType.NegativeMoney,
				new Color(1f, 0f, 0f)
			}
		};
	}
}
