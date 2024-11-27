using System;
using Game.Shop;

namespace Game.GlobalComponent.Qwest
{
	public class ComboTaskNode : TaskNode
	{
		public override BaseTask ToPo()
		{
			ComboTask comboTask = new ComboTask();
			comboTask.WeaponDependent = this.WeaponDependent;
			comboTask.RequiredWeapon = this.RequiredWeapon;
			comboTask.RequiredComboCount = this.RequiredComboCount;
			base.ToPoBase(comboTask);
			return comboTask;
		}

		[Separator("Specific")]
		public bool WeaponDependent = true;

		public WeaponNameList RequiredWeapon = WeaponNameList.Any;

		public int RequiredComboCount = 10;
	}
}
