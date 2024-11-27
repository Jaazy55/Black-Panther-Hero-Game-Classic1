using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using Game.Items;
using Game.UI;
using Game.Vehicle;
using UnityEngine;
using UnityEngine.UI;

public abstract class Achievement : MonoBehaviour, IEvent
{
	public virtual void Init()
	{
		this.achiveParams = new Achievement.SaveLoadAchievmentStruct(false, 0, 0);
	}

	public virtual void SaveAchiev()
	{
		BaseProfile.StoreValue<Achievement.SaveLoadAchievmentStruct>(this.achiveParams, this.achievementName);
	}

	public virtual void LoadAchiev()
	{
		try
		{
			this.achiveParams = BaseProfile.ResolveValue<Achievement.SaveLoadAchievmentStruct>(this.achievementName, this.achiveParams);
		}
		catch (Exception)
		{
			this.Init();
			this.SaveAchiev();
		}
	}

	public virtual void AchievComplite()
	{
		this.achiveParams.isDone = true;
		GameEventManager.Instance.activeAchievements.Remove(this);
		this.SaveAchiev();
		Transform transform = UIGame.Instance.achievmentUI.transform;
		transform.gameObject.SetActive(true);
		if (this.achievementPicture != null)
		{
			transform.Find("Picture").GetComponent<Image>().sprite = this.achievementPicture;
		}
		else
		{
			transform.Find("Picture").gameObject.SetActive(false);
		}
		transform.Find("AchevmentNameText").GetComponent<Text>().text = this.achievementName;
		PointSoundManager.Instance.PlaySoundAtPoint(PlayerManager.Instance.Player.transform.position, TypeOfSound.GetAchievment);
		this.Rewards.GiveReward();
		QwestCompletePanel.Instance.ShowCompletedQwestInfo("Achievment unlocked", this.Rewards);
	}

	public virtual void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
	{
	}

	public virtual void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
	{
	}

	public virtual void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask)
	{
	}

	public virtual void PointReachedEvent(Vector3 position, BaseTask task)
	{
	}

	public virtual void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle)
	{
	}

	public virtual void GetIntoVehicleEvent(DrivableVehicle vehicle)
	{
	}

	public virtual void GetOutVehicleEvent(DrivableVehicle vehicle)
	{
	}

	public virtual void PickUpCollectionEvent(string collectionName)
	{
	}

	public virtual void GetLevelEvent(int level)
	{
	}

	public virtual void GetShopEvent()
	{
	}

	public virtual void VehicleDrawingEvent(DrivableVehicle vehicle)
	{
	}

	public void BuyItemEvent(GameItem item)
	{
	}

	public string achievementName = "NoNameAchievment";

	public string achievementDiscription = "NoNameAchievment";

	public Sprite achievementPicture;

	public Achievement.SaveLoadAchievmentStruct achiveParams;

	public UniversalReward Rewards;

	[Serializable]
	public struct SaveLoadAchievmentStruct
	{
		public SaveLoadAchievmentStruct(bool complite, int counter, int target)
		{
			this.achiveCounter = counter;
			this.achiveTarget = target;
			this.isDone = complite;
		}

		public bool isDone;

		public int achiveCounter;

		public int achiveTarget;
	}
}
