public class Zone_Town : Zone_Civilized
{
	public override bool IsTown => true;

	public override bool IsExplorable => false;

	public override bool CanDigUnderground => false;

	public override bool CanSpawnAdv => base.lv == 0;

	public override bool AllowCriminal => false;

	public override void OnRegenerate()
	{
		if (EClass.rnd(5) == 0)
		{
			Add("mad_rich");
		}
		if (EClass.rnd(EClass.debug.enable ? 1 : 8) == 0)
		{
			Add((EClass.rnd(EClass.debug.enable ? 1 : 4) == 0) ? "unseenhand" : "murderer");
		}
		void Add(string id)
		{
			Chara chara = CharaGen.Create(id);
			chara.isSubsetCard = true;
			EClass._zone.AddCard(chara, GetSpawnPos(chara));
		}
	}
}
