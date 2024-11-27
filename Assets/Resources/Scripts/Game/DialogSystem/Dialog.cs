using System;

namespace Game.DialogSystem
{
	[Serializable]
	public class Dialog
	{
		public string DialogName;

		public bool SaveDialog;

		public Dialog.DialogReplica[] Replics = new Dialog.DialogReplica[0];

		[Serializable]
		public class DialogReplica
		{
			public string Actor;

			public string Replica;
		}
	}
}
