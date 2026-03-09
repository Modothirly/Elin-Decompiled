public class ActKiss : Ability
{
	public override bool CanPressRepeat => true;

	public override bool CanPerform()
	{
		if (Act.TC == null || !Act.TC.isChara)
		{
			return false;
		}
		if (Act.TC.IsPC && Act.TC.Chara.parasite == null && Act.TC.Chara.ride == null)
		{
			return false;
		}
		return true;
	}

	public override bool Perform()
	{
		if (Act.CC.IsPC && Act.TC.IsPC)
		{
			Act.TC = ((EClass.rnd(2) == 0 || EClass.pc.parasite == null) ? EClass.pc.ride : EClass.pc.parasite);
			if (Act.TC == null)
			{
				Act.TC = EClass.pc;
			}
		}
		Act.CC.Kiss(Act.TC.Chara);
		return true;
	}
}
