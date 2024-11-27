using System;
using Game.Items;

public class GameItemPack : GameItem
{
	public override bool SameParametrWithOther(object[] parametrs)
	{
		return this.PackedItems == (GameItem[])parametrs[0];
	}

	public GameItem[] PackedItems;
}
