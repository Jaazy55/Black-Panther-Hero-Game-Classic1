using System;
using System.Collections;
using System.Collections.Generic;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.UI;

namespace Game.DialogSystem
{
	public class DialogManager : MonoBehaviour
	{
		public static DialogManager Instance
		{
			get
			{
				if (DialogManager.instance == null)
				{
					throw new Exception("DialogController is not initialized");
				}
				return DialogManager.instance;
			}
		}

		private void Awake()
		{
			DialogManager.instance = this;
		}

		public void StartDialog(string jsonFileText)
		{
			Dialog dialog = MiamiSerializier.JSONDeserialize<Dialog>(jsonFileText);
			this.StartDialog(dialog);
		}

		public void StartDialog(int dialogIndexInList)
		{
			this.StartDialog(this.Dialogs[dialogIndexInList]);
		}

		public void StartDialog(Dialog dialog)
		{
			if (this.dialogsStash.Count == 0)
			{
				this.dialogsStash.Add(dialog);
				base.StartCoroutine(this.TypeDialog(dialog));
			}
			else if (this.dialogsStash.Find((Dialog x) => x.DialogName == dialog.DialogName) == null)
			{
				this.dialogsStash.Add(dialog);
			}
		}

		public void Continue()
		{
			if (this.replicaFinished)
			{
				this.nextLeprica = true;
			}
			else
			{
				this.finishTyping = true;
			}
		}

		public void ExitFromDialog()
		{
			this.MenuManager.OpenPanel(this.panelBeforeDialog);
			this.indexLeprica = 0;
			this.replicaFinished = false;
			this.finishTyping = false;
			Time.timeScale = 1f;
			base.StopAllCoroutines();
			this.dialogsStash.Remove(this.currentDialog);
			if (this.dialogsStash.Count > 0)
			{
				base.StartCoroutine(this.TypeDialog(this.dialogsStash[0]));
			}
		}

		public void ExportDialog()
		{
			if (this.ExportedDialog.Length > 0)
			{
				Dialog newDialog = MiamiSerializier.JSONDeserialize<Dialog>(this.ExportedDialog);
				if (this.Dialogs.Find((Dialog x) => x.DialogName == newDialog.DialogName) == null)
				{
					this.Dialogs.Add(newDialog);
					UnityEngine.Debug.Log(newDialog.DialogName + " added in list.");
				}
				else
				{
					UnityEngine.Debug.Log("Dialog " + newDialog.DialogName + " already exists in list.");
				}
				this.ExportedDialog = string.Empty;
			}
		}

		public void SaveCheckedDialogsAsJson()
		{
			List<Dialog> list = new List<Dialog>();
			foreach (Dialog dialog in this.Dialogs)
			{
				if (dialog.SaveDialog)
				{
					dialog.SaveDialog = false;
					DialogManager.SaveDialog(dialog, dialog.DialogName);
					list.Add(dialog);
				}
			}
			foreach (Dialog item in list)
			{
				this.Dialogs.Remove(item);
			}
			list.Clear();
		}

		public static void SaveDialog(Dialog dialog, string dialogName)
		{
		}

		private IEnumerator TypeDialog(Dialog dialog)
		{
			Time.timeScale = 1f;
			this.panelBeforeDialog = this.MenuManager.GetCurrentPanel();
			this.MenuManager.OpenPanel(this.DialogPanel.GetComponent<Animator>());
			this.DialogText.text = string.Empty;
			this.currentDialog = dialog;
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			for (;;)
			{
				if (!this.replicaFinished)
				{
					this.ActorText.text = dialog.Replics[this.indexLeprica].Actor;
					for (int z = 0; z <= dialog.Replics[this.indexLeprica].Replica.Length - 1; z++)
					{
						Text dialogText = this.DialogText;
						dialogText.text += dialog.Replics[this.indexLeprica].Replica[z];
						this.replicaFinished = (this.DialogText.text.Length == dialog.Replics[this.indexLeprica].Replica.Length);
						if (this.finishTyping)
						{
							this.DialogText.text = dialog.Replics[this.indexLeprica].Replica;
							this.replicaFinished = true;
							this.finishTyping = false;
							break;
						}
						yield return waitForEndOfFrame;
					}
				}
				if (this.nextLeprica)
				{
					this.replicaFinished = false;
					this.DialogText.text = string.Empty;
					this.nextLeprica = false;
					if (this.indexLeprica.Equals(dialog.Replics.Length - 1))
					{
						break;
					}
					this.indexLeprica++;
				}
				yield return waitForEndOfFrame;
			}
			this.ExitFromDialog();
			yield break;
		}

		public List<Dialog> Dialogs = new List<Dialog>();

		[InspectorButton("SaveCheckedDialogsAsJson")]
		public string SaveCheckedDialogs = string.Empty;

		[Space(5f)]
		public Text DialogText;

		public Text ActorText;

		public GameObject DialogPanel;

		public MenuPanelManager MenuManager;

		[TextArea]
		public string ExportedDialog;

		[InspectorButton("ExportDialog")]
		public string ExportDialogInList;

		private bool finishTyping;

		private bool nextLeprica;

		private bool replicaFinished;

		private int indexLeprica;

		private Animator panelBeforeDialog;

		private readonly List<Dialog> dialogsStash = new List<Dialog>();

		private Dialog currentDialog;

		private static DialogManager instance;
	}
}
