using System;
using UnityEngine;

public class ColliderValueGizmo : MonoBehaviour
{
	private void OnDrawGizmosSelected()
	{
		if (this.collider == null || this.mesh == null)
		{
			return;
		}
		Gizmos.color = this.Color;
		BoxCollider boxCollider = this.collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.DrawMesh(this.mesh, this.collider.bounds.center, this.collider.transform.rotation, boxCollider.size);
			return;
		}
		MeshCollider x = this.collider as MeshCollider;
		if (x != null)
		{
			Gizmos.DrawMesh(this.mesh, this.collider.bounds.center, this.collider.transform.rotation);
			return;
		}
	}

	public Mesh mesh;

	public Collider collider;

	public Color Color = Color.white;
}
