public class TraitMerchantTravel : TraitMerchant
{
	public override int ShopLv
	{
		get
		{
			if (!base.owner.IsPCFactionOrMinion)
			{
				return base.owner.LV;
			}
			return base.ShopLv;
		}
	}

	public override ShopType ShopType => ShopType.TravelMerchant;

	public override bool AllowCriminal => true;

	public override int CostRerollShop => 10;

	public override int RestockDay => 360;
}
