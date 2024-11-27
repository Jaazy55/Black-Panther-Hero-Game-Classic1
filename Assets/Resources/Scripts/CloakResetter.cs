using System;
using UnityEngine;

public class CloakResetter : MonoBehaviour
{
	private void Start()
	{
		if (this.cloak == null)
		{
			this.cloak = base.GetComponent<Cloth>();
		}
	}

	private void OnEnable()
	{
		if (this.cloak == null)
		{
			return;
		}
		this.cloak.enabled = false;
		this.framesToWait = this.Frames;
		this.reseted = false;
	}

	private void Update()
	{
		if (!this.reseted)
		{
			if (this.framesToWait <= 0)
			{
				this.cloak.enabled = true;
				this.reseted = true;
				if (this.DebugLog)
				{
					MonoBehaviour.print("Cloak reseted");
				}
			}
			else
			{
				this.framesToWait--;
			}
		}
	}

	public bool DebugLog;

	[Space(10f)]
	public Cloth cloak;

	public int Frames = 1;

	private int framesToWait;

	private bool reseted;
}
