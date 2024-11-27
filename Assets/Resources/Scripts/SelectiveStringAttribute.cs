using System;
using UnityEngine;

public class SelectiveStringAttribute : PropertyAttribute
{
	public SelectiveStringAttribute(string tagsContainerName)
	{
		this.ValidTagsContainerName = tagsContainerName;
	}

	public readonly string ValidTagsContainerName;
}
