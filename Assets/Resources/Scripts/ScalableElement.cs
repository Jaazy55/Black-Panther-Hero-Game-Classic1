using System;
using UnityEngine;

public class ScalableElement : MonoBehaviour
{
	public bool SelectStatus
	{
		get
		{
			return this.m_selectStatus;
		}
	}

	public void SetSelectStatus(bool value)
	{
		if (this.m_selectStatus != value)
		{
			this.m_selectStatus = value;
			this.scaleChanging = true;
		}
	}

	protected void Awake()
	{
		this.ScaleSpeed = (this.m_SelectedScale - this.m_UnselecredScale) / this.ScaleTime;
		if (!this.scaleChanging)
		{
			this.m_selectStatus = this.m_StartedSelectStatus;
			this.scaleChanging = false;
		}
		base.transform.localScale = ((!this.m_selectStatus) ? this.m_UnselecredScale : this.m_SelectedScale);
	}

	private void Update()
	{
		if (this.scaleChanging)
		{
			Vector3 vector;
			vector.x = Mathf.MoveTowards(base.transform.localScale.x, (!this.m_selectStatus) ? this.m_UnselecredScale.x : this.m_SelectedScale.x, this.ScaleSpeed.x * Time.unscaledDeltaTime);
			vector.y = Mathf.MoveTowards(base.transform.localScale.y, (!this.m_selectStatus) ? this.m_UnselecredScale.y : this.m_SelectedScale.y, this.ScaleSpeed.y * Time.unscaledDeltaTime);
			vector.z = Mathf.MoveTowards(base.transform.localScale.z, (!this.m_selectStatus) ? this.m_UnselecredScale.z : this.m_SelectedScale.z, this.ScaleSpeed.z * Time.unscaledDeltaTime);
			base.transform.localScale = vector;
			if (vector == ((!this.m_selectStatus) ? this.m_UnselecredScale : this.m_SelectedScale))
			{
				this.scaleChanging = false;
			}
		}
	}

	[SerializeField]
	private bool m_StartedSelectStatus;

	[SerializeField]
	private Vector3 m_SelectedScale;

	[SerializeField]
	private Vector3 m_UnselecredScale;

	[SerializeField]
	private float ScaleTime;

	private Vector3 ScaleSpeed;

	private bool scaleChanging;

	private bool m_selectStatus;
}
