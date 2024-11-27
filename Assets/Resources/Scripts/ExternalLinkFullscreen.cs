using System;
using UnityEngine;

public class ExternalLinkFullscreen : MonoBehaviour
{
	public void Click()
	{
		Application.OpenURL(this.ExternalLink);
	}

	public string ExternalLink;
}
