using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestMaker : MonoBehaviour
	{
		public void Rebuild()
		{
			GameEventManager componentInParent = base.GetComponentInParent<GameEventManager>();
			if (componentInParent == null)
			{
				UnityEngine.Debug.LogError("GameEventManager not found");
				return;
			}
			List<Qwest> list = new List<Qwest>();
			Transform child = base.transform.GetChild(0);
			if (child == null)
			{
				UnityEngine.Debug.LogError("QwestContainer not found");
				return;
			}
			for (int i = 0; i < child.childCount; i++)
			{
				Transform child2 = child.GetChild(i);
				if (child2.gameObject.activeInHierarchy)
				{
					QwestNode component = child2.GetComponent<QwestNode>();
					if (component != null)
					{
						Qwest qwest = component.ToPo();
						qwest.Name = component.name;
						list.Add(qwest);
						this.QwestNodeBypass(list, component, qwest);
					}
				}
			}
		}

		private void QwestNodeBypass(List<Qwest> qwests, QwestNode qwestNode, Qwest qwest)
		{
			for (int i = 0; i < qwestNode.transform.childCount; i++)
			{
				Transform child = qwestNode.transform.GetChild(i);
				if (child.gameObject.activeInHierarchy)
				{
					QwestNode component = child.GetComponent<QwestNode>();
					if (component != null)
					{
						Qwest qwest2 = component.ToPo();
						qwests.Add(qwest2);
						qwest2.Name = component.name;
						qwest.QwestTree.Add(qwest2);
						qwest2.ParentQwest = qwest;
						this.QwestNodeBypass(qwests, component, qwest2);
					}
				}
			}
		}
	}
}
