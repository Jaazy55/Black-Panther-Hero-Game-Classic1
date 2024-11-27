using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Items;
using Game.Weapons;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
	public static AmmoManager Instance
	{
		get
		{
			if (AmmoManager.instance == null)
			{
				AmmoManager.instance = UnityEngine.Object.FindObjectOfType<AmmoManager>();
			}
			return AmmoManager.instance;
		}
	}

	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		JointAmmoContainer[] componentsInChildren = base.GetComponentsInChildren<JointAmmoContainer>();
		foreach (JointAmmoContainer jointAmmoContainer in componentsInChildren)
		{
			this.ammoContainers.Add(jointAmmoContainer);
			jointAmmoContainer.UpdateAmmo();
		}
		this.inited = true;
	}

	public void AddContainer(GameItemAmmo incAmmoItem)
	{
		if (!this.inited)
		{
			this.Init();
		}
		foreach (JointAmmoContainer jointAmmoContainer in this.ammoContainers)
		{
			if (jointAmmoContainer.GameItemAmmo.ID == incAmmoItem.ID)
			{
				jointAmmoContainer.AmmoCount += incAmmoItem.ShopVariables.PerStackAmount;
				return;
			}
		}
		this.CreateContainer(incAmmoItem);
	}

	public void CreateContainer(GameItemAmmo incAmmoItem)
	{
		if (this.GetContainer(incAmmoItem.AmmoType) != null)
		{
			return;
		}
		JointAmmoContainer jointAmmoContainer = new GameObject
		{
			name = incAmmoItem.AmmoType.ToString(),
			transform = 
			{
				parent = base.transform
			}
		}.AddComponent<JointAmmoContainer>();
		jointAmmoContainer.GameItemAmmo = incAmmoItem;
		jointAmmoContainer.AmmoCount = incAmmoItem.ShopVariables.PerStackAmount;
		this.ammoContainers.Add(jointAmmoContainer);
	}

	public void AddAmmo(AmmoTypes neededType)
	{
		if (!this.inited)
		{
			this.Init();
		}
		if (neededType == AmmoTypes.None)
		{
			return;
		}
		foreach (JointAmmoContainer jointAmmoContainer in this.ammoContainers)
		{
			if (jointAmmoContainer.GameItemAmmo.AmmoType == neededType)
			{
				jointAmmoContainer.AmmoCount += jointAmmoContainer.GameItemAmmo.ShopVariables.PerStackAmount;
				RangedWeapon rangedWeapon = PlayerManager.Instance.WeaponController.CurrentWeapon as RangedWeapon;
				if (rangedWeapon != null && rangedWeapon.AmmoType == neededType)
				{
					PlayerManager.Instance.WeaponController.UpdateAmmoText(rangedWeapon);
				}
				return;
			}
		}
		if (PlayerManager.Instance.WeaponController.CurrentWeapon.AmmoType == neededType)
		{
			PlayerManager.Instance.WeaponController.UpdateAmmoText();
		}
	}

	public void UpdateAmmo(AmmoTypes neededType)
	{
		if (!this.inited)
		{
			this.Init();
		}
		foreach (JointAmmoContainer jointAmmoContainer in this.ammoContainers)
		{
			if (jointAmmoContainer.GameItemAmmo.AmmoType == neededType)
			{
				jointAmmoContainer.UpdateAmmo();
				break;
			}
		}
	}

	public JointAmmoContainer GetContainer(AmmoTypes neededType)
	{
		if (!this.inited)
		{
			this.Init();
		}
		JointAmmoContainer result = null;
		foreach (JointAmmoContainer jointAmmoContainer in this.ammoContainers)
		{
			if (jointAmmoContainer.GameItemAmmo.AmmoType == neededType)
			{
				result = jointAmmoContainer;
				break;
			}
		}
		return result;
	}

	private static AmmoManager instance;

	private List<JointAmmoContainer> ammoContainers = new List<JointAmmoContainer>();

	private bool inited;
}
