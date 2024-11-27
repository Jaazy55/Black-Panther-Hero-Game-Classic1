using System;
using Game.Character;
using Game.GlobalComponent;

public class CutsceneResurrection : Cutscene
{
	public override void StartScene()
	{
		PlayerInteractionsManager.Instance.Player.Resurrect();
		this.EndScene(true);
	}
}
