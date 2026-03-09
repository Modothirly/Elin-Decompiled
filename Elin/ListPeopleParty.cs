using System.Linq;

public class ListPeopleParty : BaseListPeople
{
	public override void OnCreate()
	{
		list.sorts = new UIList.SortMode[3]
		{
			UIList.SortMode.ByFeat,
			UIList.SortMode.ByJob,
			UIList.SortMode.ByRace
		};
		list.sortMode = UIList.SortMode.ByFeat;
	}

	public override void OnInstantiate(Chara c, ItemGeneral i)
	{
		Zone zone = (main ? c.currentZone : c.homeZone);
		i.SetSubText((zone == null) ? "???" : zone.Name, 240);
		if (!c.IsPC)
		{
			UIButton uIButton = i.AddSubButton(EClass.core.refs.icons.fav, delegate
			{
				SE.ClickGeneral();
				c.isFav = !c.isFav;
				RefreshAll();
			});
			uIButton.icon.SetAlpha(c.isFav ? 1f : 0.3f);
			uIButton.icon.SetNativeSize();
		}
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		if (main)
		{
			if (c.isDead || c.HasCondition<ConSuspend>() || c.currentZone == null)
			{
				SE.Beep();
				return;
			}
			if (c.currentZone != EClass._zone)
			{
				c.MoveZone(EClass._zone);
				c.MoveImmediate(EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false) ?? EClass.pc.pos);
			}
			EClass.pc.party.AddMemeber(c);
		}
		else
		{
			if (c.IsPC)
			{
				SE.Beep();
				return;
			}
			EClass.pc.party.RemoveMember(c);
			if (c.homeZone != EClass._zone)
			{
				c.MoveZone(c.homeZone);
			}
		}
		MoveToOther(c);
		base.Main.OnRefreshMenu();
	}

	public override void OnList()
	{
		if (main)
		{
			foreach (Chara value in EClass.game.cards.globalCharas.Values)
			{
				if (value.IsPCFaction && !value.IsPCParty && value.memberType == FactionMemberType.Default)
				{
					list.Add(value);
				}
			}
			return;
		}
		foreach (Chara member in EClass.pc.party.members)
		{
			list.Add(member);
		}
		list.sortMode = UIList.SortMode.ByNone;
	}

	public HireInfo GetInfo(Chara c)
	{
		return EClass.Home.listReserve.First((HireInfo a) => a.chara == c);
	}
}
