using System;
using Game.Shop;

namespace Game.GlobalComponent.Qwest
{
	public class ComboTask : BaseTask
	{
		public override void TaskStart()
		{
			base.TaskStart();
			if (this.WeaponDependent)
			{
				ComboManager instance = ComboManager.Instance;
				instance.WeaponComboEvent = (ComboManager.ComboDelegate)Delegate.Combine(instance.WeaponComboEvent, new ComboManager.ComboDelegate(this.CheckForComplite));
			}
			else
			{
				ComboManager instance2 = ComboManager.Instance;
				instance2.OverallComboEvent = (ComboManager.ComboDelegate)Delegate.Combine(instance2.OverallComboEvent, new ComboManager.ComboDelegate(this.CheckForComplite));
			}
		}

		public void CheckForComplite(ComboManager.ComboInfo comboInfo)
		{
			if (!this.WeaponDependent || this.RequiredWeapon == WeaponNameList.Any || comboInfo.WeaponNameInList == this.RequiredWeapon)
			{
				ComboManager.Instance.UpdateComboMeter(comboInfo.ComboMultiplier, comboInfo.WeaponNameInList.ToString(), false);
				if (comboInfo.ComboMultiplier > this.maxComboAchieved)
				{
					this.maxComboAchieved = comboInfo.ComboMultiplier;
					if (this.maxComboAchieved == this.RequiredComboCount)
					{
						this.CurrentQwest.MoveToTask(this.NextTask);
					}
				}
			}
			else
			{
				ComboManager.Instance.UpdateComboMeter(0, string.Empty, true);
			}
		}

		public override void Finished()
		{
			base.Finished();
			if (this.WeaponDependent)
			{
				ComboManager instance = ComboManager.Instance;
				instance.WeaponComboEvent = (ComboManager.ComboDelegate)Delegate.Remove(instance.WeaponComboEvent, new ComboManager.ComboDelegate(this.CheckForComplite));
			}
			else
			{
				ComboManager instance2 = ComboManager.Instance;
				instance2.OverallComboEvent = (ComboManager.ComboDelegate)Delegate.Remove(instance2.OverallComboEvent, new ComboManager.ComboDelegate(this.CheckForComplite));
			}
			ComboManager.Instance.UpdateComboMeter(0, string.Empty, true);
		}

		public override string TaskStatus()
		{
			return string.Concat(new object[]
			{
				base.TaskStatus(),
				"\nMax combo achieved with ",
				this.RequiredWeapon,
				": X",
				this.maxComboAchieved,
				"/",
				this.RequiredComboCount
			});
		}

		public bool WeaponDependent = true;

		public WeaponNameList RequiredWeapon = WeaponNameList.Any;

		public int RequiredComboCount = 10;

		private int maxComboAchieved;
	}
}
