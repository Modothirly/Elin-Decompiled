public class TraitGarokkHammer : TraitItem
{
	public override bool OnUse(Chara c)
	{
		ActEffect.Proc(EffectId.ChangeRarity, EClass.pc, null, 100, new ActRef
		{
			n1 = owner.material.alias,
			refThing = owner.Thing
		});
		return true;
	}
}
