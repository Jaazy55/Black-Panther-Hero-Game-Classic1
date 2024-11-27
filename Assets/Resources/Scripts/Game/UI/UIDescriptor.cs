using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class UIDescriptor : MonoBehaviour
	{
		public void ShowInfo(RectTransform showedRectTransform, string description, float showedTime)
		{
			this.Container.gameObject.SetActive(true);
			base.transform.parent = showedRectTransform;
			base.transform.localPosition = Vector3.zero;
			Vector3 a = (base.transform.position.y <= (float)Screen.height / 2f) ? Vector3.up : Vector3.down;
			Vector3 a2 = (base.transform.position.x <= (float)Screen.width / 2f) ? Vector3.right : Vector3.left;
			base.transform.localPosition += a * (this.myRectTransform.rect.height / 2f + showedRectTransform.rect.height / 2f);
			base.transform.localPosition += a2 * (this.myRectTransform.rect.width / 2f + showedRectTransform.rect.width / 2f);
			base.transform.localScale = Vector3.one;
			this.DescriptionText.text = description;
			base.StopAllCoroutines();
			base.StartCoroutine(this.Timer(showedTime, new Action(this.HideInfo)));
		}

		public void HideInfo()
		{
			this.Container.gameObject.SetActive(false);
			base.StopAllCoroutines();
		}

		private void Awake()
		{
			UIDescriptor.Instance = this;
			this.myRectTransform = (base.transform as RectTransform);
		}

		private void OnDisable()
		{
			this.HideInfo();
		}

		private IEnumerator Timer(float time, Action afterAction)
		{
			yield return new WaitForSecondsRealtime(time);
			if (afterAction != null)
			{
				afterAction();
			}
			yield break;
		}

		public static UIDescriptor Instance;

		public RectTransform Container;

		public Text DescriptionText;

		private RectTransform myRectTransform;
	}
}
