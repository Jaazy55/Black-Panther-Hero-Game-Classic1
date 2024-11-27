using System;
using System.Collections.Generic;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

public class PreviewRotator : MonoBehaviour
{
	private void Start()
	{
		foreach (Transform transform in this.RotatableTransforms)
		{
			this.defaultRotations.Add(transform.localRotation);
		}
	}

	private void Update()
	{
		this.drug = -CrossPlatformInputManager.GetVirtualOnlyAxis("Horizontal", false);
		if (this.drug != 0f)
		{
			this.rotationSpeed = this.drug * 300f;
			this.x = 0f;
			this.startRotationSpeed = this.rotationSpeed;
		}
		else if (this.rotationSpeed != this.normalRotationSpeed)
		{
			this.x += 0.01f;
			this.rotationSpeed = Mathf.Lerp(this.startRotationSpeed, this.normalRotationSpeed, this.x);
		}
		foreach (Transform transform in this.RotatableTransforms)
		{
			transform.Rotate(Vector3.up, this.rotationSpeed);
		}
	}

	public void ResetRotators()
	{
		this.rotationSpeed = this.normalRotationSpeed;
		for (int i = 0; i < this.RotatableTransforms.Length; i++)
		{
			this.RotatableTransforms[i].localRotation = this.defaultRotations[i];
		}
	}

	public Transform[] RotatableTransforms;

	public float normalRotationSpeed = 0.1f;

	private float drug;

	private float startRotationSpeed;

	private float rotationSpeed = 1f;

	private float x;

	private List<Quaternion> defaultRotations = new List<Quaternion>();
}
