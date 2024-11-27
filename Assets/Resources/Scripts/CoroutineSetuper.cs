using System;
using System.Collections;
using UnityEngine;

public class CoroutineSetuper
{
	public CoroutineSetuper(CoroutineConverter converter, CoroutineAction breaker)
	{
		this.Converter = converter;
		this.Breaker = breaker;
	}

	public CoroutineSetuper(MonoBehaviour behaviour) : this(new CoroutineConverter(behaviour.StartCoroutine), new CoroutineAction(behaviour.StopCoroutine))
	{
	}

	public Coroutine CurrentCoroutine
	{
		get
		{
			return this._currentCoroutine;
		}
		set
		{
			this.SetCoroutine(value);
		}
	}

	public IEnumerator CurrentEnumerator
	{
		get
		{
			return this._currentEnumerator;
		}
	}

	public EnumeratorCreater CurrentCreater
	{
		get
		{
			return this._currentCreater;
		}
	}

	public void SetCoroutine(IEnumerator enumerator, bool start = true)
	{
		if (this._currentEnumerator != enumerator)
		{
			this.Reset();
			this._currentEnumerator = enumerator;
			if (start)
			{
				this._currentCoroutine = this.Converter(this._currentEnumerator);
			}
		}
	}

	public void SetCoroutine(Coroutine coroutine)
	{
		if (this._currentCoroutine != coroutine)
		{
			this.Reset();
			this._currentCoroutine = coroutine;
		}
	}

	public void SetCoroutine(EnumeratorCreater creater, bool start = true)
	{
		if (this._currentCreater != creater || this._currentCoroutine == null)
		{
			this.Reset();
			this._currentCreater = creater;
			if (start)
			{
				this._currentEnumerator = creater();
				this._currentCoroutine = this.Converter(this._currentEnumerator);
			}
		}
	}

	public void Reset()
	{
		this._currentCreater = null;
		this._currentEnumerator = null;
		if (this._currentCoroutine != null)
		{
			this.Breaker(this._currentCoroutine);
			this._currentCoroutine = null;
		}
	}

	public void Stop()
	{
		if (this._currentCoroutine != null)
		{
			this.Breaker(this._currentCoroutine);
			this._currentCoroutine = null;
		}
	}

	public void Continue()
	{
		if (this._currentEnumerator != null && this._currentCoroutine == null)
		{
			this._currentCoroutine = this.Converter(this._currentEnumerator);
		}
	}

	private readonly CoroutineConverter Converter;

	private readonly CoroutineAction Breaker;

	private EnumeratorCreater _currentCreater;

	private IEnumerator _currentEnumerator;

	private Coroutine _currentCoroutine;
}
