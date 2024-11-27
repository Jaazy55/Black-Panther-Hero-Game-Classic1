using System;
using System.Collections.Generic;

namespace Game.GlobalComponent.Qwest
{
	public class QwestProfile
	{
		public static Dictionary<string, bool> QwestStatus
		{
			get
			{
				return BaseProfile.ResolveValue<Dictionary<string, bool>>(QwestProfileList.QwestStatus.ToString(), new Dictionary<string, bool>());
			}
			set
			{
				BaseProfile.StoreValue<Dictionary<string, bool>>(value, QwestProfileList.QwestStatus.ToString());
			}
		}

		public static bool QwestArrow
		{
			get
			{
				return BaseProfile.ResolveValue<bool>(QwestProfileList.QwestArrow.ToString(), true);
			}
			set
			{
				BaseProfile.StoreValue<bool>(value, QwestProfileList.QwestArrow.ToString());
			}
		}
	}
}
