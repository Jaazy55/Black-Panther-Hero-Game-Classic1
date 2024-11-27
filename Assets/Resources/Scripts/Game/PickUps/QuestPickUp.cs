using System;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;

namespace Game.PickUps
{
	public class QuestPickUp : PickUp
	{
		public void DeInit()
		{
			this.RelatedTask = null;
		}

		protected override void TakePickUp()
		{
			InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestItem, this.type.ToString());
			GameEventManager.Instance.Event.PickedQwestItemEvent(base.transform.position, this.type, this.RelatedTask);
			base.TakePickUp();
		}

		public const float QuestItemSpawnY = 1f;

		public QwestPickupType type;

		public BaseTask RelatedTask;
	}
}
