public class TraitMerchantTravel : TraitMerchant
{
	public bool ShouldOpenShop
	{
		get
		{
			if (!base.owner.IsPCFactionOrMinion)
			{
				return !EClass._zone.IsPCFactionOrTent;
			}
			return false;
		}
	}

	public override int ShopLv
	{
		get
		{
			if (!ShouldOpenShop)
			{
				return base.ShopLv;
			}
			return EClass.pc.FameLv + EClass.pc.FameLv / 2 + 10;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			if (!ShouldOpenShop)
			{
				return ShopType.None;
			}
			return ShopType.TravelMerchant;
		}
	}

	public override bool CanInvest => false;

	public override bool AllowCriminal => true;

	public override int CostRerollShop => 0;

	public override int RestockDay => -1;
}
