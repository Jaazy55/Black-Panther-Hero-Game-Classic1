using System;
using Game.Items;
using UnityEngine;

[Serializable]
public class SlotRenderer
{
	public SkinnedMeshRenderer GetRenderer(SkinSlot slot)
	{
		SkinnedMeshRenderer result;
		switch (slot)
		{
		case SkinSlot.Head:
			result = this.HeadRenderer;
			break;
		case SkinSlot.Face:
			result = this.FaceRenderer;
			break;
		case SkinSlot.Body:
			result = this.BodyRenderer;
			break;
		case SkinSlot.Arms:
			result = this.ArmsRenderer;
			break;
		case SkinSlot.Forearms:
			result = this.ForearmsRenderer;
			break;
		case SkinSlot.Hands:
			result = this.HandsRenderer;
			break;
		case SkinSlot.Thighs:
			result = this.ThighsRenderer;
			break;
		case SkinSlot.Shins:
			result = this.ShinsRenderer;
			break;
		case SkinSlot.Foots:
			result = this.FootsRenderer;
			break;
		case SkinSlot.ExternalBody:
			result = this.ExternalBodyRenderer;
			break;
		case SkinSlot.ExternalFoots:
			result = this.ExternalFootsRenderer;
			break;
		default:
			result = null;
			break;
		}
		return result;
	}

	[Space(10f)]
	public SkinnedMeshRenderer HeadRenderer;

	public SkinnedMeshRenderer FaceRenderer;

	[Space(10f)]
	public SkinnedMeshRenderer BodyRenderer;

	[Space(10f)]
	public SkinnedMeshRenderer ArmsRenderer;

	public SkinnedMeshRenderer ForearmsRenderer;

	public SkinnedMeshRenderer HandsRenderer;

	[Space(10f)]
	public SkinnedMeshRenderer ThighsRenderer;

	public SkinnedMeshRenderer ShinsRenderer;

	public SkinnedMeshRenderer FootsRenderer;

	public SkinnedMeshRenderer ExternalBodyRenderer;

	public SkinnedMeshRenderer ExternalFootsRenderer;
}
