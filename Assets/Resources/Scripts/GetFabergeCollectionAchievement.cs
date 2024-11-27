using System;
using Game.GlobalComponent;

public class GetFabergeCollectionAchievement : Achievement
{
	public override void Init()
	{
		base.Init();
		CollectionPickUpsManager instance = CollectionPickUpsManager.Instance;
		instance.OnManagerInitAction = (Action)Delegate.Combine(instance.OnManagerInitAction, new Action(delegate()
		{
			this.achiveParams.achiveTarget = CollectionPickUpsManager.Instance.totalAmmount[this.collectionType];
			this.achiveParams.achiveCounter = CollectionPickUpsManager.Instance.countAmmount[this.collectionType];
		}));
		CollectionPickUpsManager instance2 = CollectionPickUpsManager.Instance;
		instance2.OnElementTakenEvent = (CollectionPickUpsManager.OnElementTaken)Delegate.Combine(instance2.OnElementTakenEvent, new CollectionPickUpsManager.OnElementTaken(this.OnElementTaken));
	}

	public override void PickUpCollectionEvent(string collectionName)
	{
		if (this.collectionType.ToString() == collectionName)
		{
			this.AchievComplite();
		}
	}

	private void OnElementTaken(CollectionPickUpsManager.CollectionTypes type)
	{
		if (type == this.collectionType)
		{
			this.achiveParams.achiveCounter = this.achiveParams.achiveCounter + 1;
		}
	}

	private CollectionPickUpsManager.CollectionTypes collectionType = CollectionPickUpsManager.CollectionTypes.Hook;
}
