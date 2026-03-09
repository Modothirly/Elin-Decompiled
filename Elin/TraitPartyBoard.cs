public class TraitPartyBoard : TraitBoard
{
	public override bool IsHomeItem => true;

	public override void TrySetAct(ActPlan p)
	{
		if (EClass._zone.IsPCFaction)
		{
			p.TrySetAct("LayerPeople", () => LayerPeople.CreateParty(), owner);
		}
	}
}
