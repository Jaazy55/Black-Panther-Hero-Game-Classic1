using System;
using Game.Items;
using UnityEngine;

public class StuffHelper : MonoBehaviour
{
	public Transform GetPlaceholder(SkinSlot slot)
	{
		Transform result = null;
		switch (slot)
		{
		case SkinSlot.Hat:
			result = this.HatPlaceholder;
			break;
		case SkinSlot.Glass:
			result = this.GlassPlaceholder;
			break;
		case SkinSlot.Mask:
			result = this.MaskPlaceholder;
			break;
		case SkinSlot.LeftBracelet:
			result = this.LeftBraceletPlaceholder;
			break;
		case SkinSlot.RightBracelet:
			result = this.RightBraceletPlaceholder;
			break;
		case SkinSlot.LeftHuckle:
			result = this.LeftHucklePlaceholder;
			break;
		case SkinSlot.RightHuckle:
			result = this.RightHucklePlaceholder;
			break;
		case SkinSlot.LeftPalm:
			result = this.LeftPalmPlaceholder;
			break;
		case SkinSlot.RightPalm:
			result = this.RightPalmPlaceholder;
			break;
		case SkinSlot.LeftToe:
			result = this.LeftToePlaceholder;
			break;
		case SkinSlot.RightToe:
			result = this.RightToePlaceholder;
			break;
		}
		return result;
	}

	public Transform HatPlaceholder;

	public Transform GlassPlaceholder;

	public Transform MaskPlaceholder;

	[Space(5f)]
	public Transform LeftBraceletPlaceholder;

	public Transform RightBraceletPlaceholder;

	[Space(5f)]
	public Transform LeftHucklePlaceholder;

	public Transform RightHucklePlaceholder;

	[Space(5f)]
	public Transform LeftPalmPlaceholder;

	public Transform RightPalmPlaceholder;

	[Space(5f)]
	public Transform LeftToePlaceholder;

	public Transform RightToePlaceholder;

	[Space(10f)]
	public SlotRenderer SlotRenderers;

	[Space(10f)]
	public GameItemClothes[] DefaultClotheses;

	[Separator("For moto specific")]
	public Transform LeftUpperArm;

	public Transform LeftForeArm;

	public Transform RightUpperArm;

	public Transform RightForeArm;

	public Transform LeftHand;

	public Transform RightHand;
}
