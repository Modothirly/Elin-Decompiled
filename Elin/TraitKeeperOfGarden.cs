public class TraitKeeperOfGarden : TraitUniqueChara
{
	public override bool CanInvite => false;

	public override bool CanChangeAffinity => false;

	public override ShopType ShopType
	{
		get
		{
			if (!(EClass._zone.id == "startVillage2") && !EClass.game.IsSurvival)
			{
				return ShopType.None;
			}
			return ShopType.KeeperOfGarden;
		}
	}

	public override CurrencyType CurrencyType => CurrencyType.Influence;
}
