using System;
using Game.GlobalComponent;
using UnityEngine;

public class SearchProcessing : MonoBehaviour
{
	public void Init()
	{
		this.isInit = true;
		this.process.Initialize();
		this.updateProcess = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.process.Processing), 0.3f);
	}

	private void FixedUpdate()
	{
		if (this.isInit)
		{
			this.updateProcess.ProceedOnFixedUpdate();
		}
	}

	private void OnDestroy()
	{
		if (this.isInit)
		{
			this.process.Release();
		}
	}

	public ISeachProcess process;

	private SlowUpdateProc updateProcess;

	private bool isInit;
}
