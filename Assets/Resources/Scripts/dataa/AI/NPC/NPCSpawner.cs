using System;
using Game.GlobalComponent;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	public class NPCSpawner : MonoBehaviour
	{
		private void Awake()
		{
			PoolManager.Instance.GetFromPool(this.m_NPCPrefab.gameObject, base.transform.position, base.transform.rotation);
		}

		private void OnDrawGizmosSelected()
		{
			if (this.renderers == null)
			{
				if (this.m_NPCPrefab != null)
				{
					this.renderers = this.m_NPCPrefab.GetComponentsInChildren<Renderer>();
				}
				else
				{
					this.renderers = new Renderer[0];
				}
			}
			Gizmos.color = Color.red;
			Vector3 a = base.transform.position - this.m_NPCPrefab.transform.position;
			Quaternion rotation = base.transform.rotation * Quaternion.Inverse(this.m_NPCPrefab.transform.rotation);
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer is SkinnedMeshRenderer)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
					{
						Gizmos.DrawMesh(skinnedMeshRenderer.sharedMesh, a + skinnedMeshRenderer.transform.position, skinnedMeshRenderer.transform.rotation, rotation * skinnedMeshRenderer.transform.lossyScale);
					}
				}
			}
		}

		[SerializeField]
		private NPCBehaviour m_NPCPrefab;

		private Renderer[] renderers;
	}
}
