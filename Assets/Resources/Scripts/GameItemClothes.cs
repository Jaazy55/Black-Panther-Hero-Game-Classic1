using System;
using Game.Items;
using UnityEngine;

public class GameItemClothes : GameItemSkin
{
	[Space(10f)]
	public Material[] SkinMaterials;

	public Mesh SkinMesh;

	public GameItemAccessory[] RelatedAccesories;

	public bool HideByDefault;
}
