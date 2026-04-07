using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ConStrife : BaseBuff
{
	[JsonProperty]
	private ElementContainer ec = new ElementContainerCondition();

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int lv;

	[JsonProperty]
	public int turn;

	public override string TextDuration => "Lv." + lv;

	public int ExpToNext => (lv + 1) * (lv + 1);

	public override bool ShouldOverride(Condition c)
	{
		return false;
	}

	public override bool CanStack(Condition c)
	{
		return true;
	}

	public override ElementContainer GetElementContainer()
	{
		return ec;
	}

	public void AddKill(Chara c)
	{
		if (!owner.IsPC)
		{
			return;
		}
		if (c.IsPCFactionOrMinion)
		{
			if (c.IsMinion)
			{
				exp += 2;
			}
			else
			{
				exp += 30;
			}
		}
		else
		{
			exp++;
		}
		while (exp >= ExpToNext)
		{
			if (lv >= 5)
			{
				exp = ExpToNext;
				break;
			}
			exp -= ExpToNext;
			lv++;
			ec.SetBase(964, (lv >= 3) ? ((lv - 2) * 5) : 0);
			ec.SetBase(662, lv * 10);
			TryApplyParty();
		}
		SetTurn();
	}

	public void SetTurn()
	{
		turn = Mathf.Max(100 - lv * 10, 10);
	}

	public override void Tick()
	{
		if (!owner.IsPC)
		{
			SyncPC();
			return;
		}
		turn--;
		if (turn < 0)
		{
			lv--;
			if (lv >= 1)
			{
				SetTurn();
				exp = ExpToNext / 2;
			}
			else
			{
				Kill();
			}
		}
		else
		{
			TryApplyParty();
		}
	}

	public void TryApplyParty()
	{
		if (lv < 2 || !owner.IsPC)
		{
			return;
		}
		foreach (Chara member in EClass.pc.party.members)
		{
			if (!member.HasCondition<ConStrife>())
			{
				(member.AddCondition<ConStrife>() as ConStrife)?.SyncPC();
			}
		}
	}

	public void SyncPC()
	{
		ConStrife condition = EClass.pc.GetCondition<ConStrife>();
		if (condition == null || condition.lv < 2)
		{
			turn = 0;
			Kill();
			return;
		}
		turn = condition.turn;
		lv = condition.lv;
		exp = condition.exp;
		ec.SetBase(964, condition.ec.GetOrCreateElement(964).vBase);
		ec.SetBase(662, condition.ec.GetOrCreateElement(662).vBase);
	}

	public override void OnWriteNote(List<string> list)
	{
		list.Add("hintStrife".lang(lv.ToString() ?? "", exp + "/" + ExpToNext));
		list.Add("hintStrife2".lang((lv * 10).ToString() ?? "", (lv * 5).ToString() ?? "").TagColorGoodBad(() => true));
		foreach (Element e in ec.dict.Values)
		{
			if (e.Value == 0)
			{
				continue;
			}
			if (e.IsFlag)
			{
				list.Add(e.Name.TagColorGoodBad(() => e.Value >= 0));
			}
			else if (elements != null && elements.Has(e.id) && elements.Value(e.id) != e.Value)
			{
				list.Add("modValue".lang(e.Name, ((e.Value < 0) ? "" : "+") + elements.Value(e.id) + (e.source.tag.Contains("ratio") ? "%" : "") + " (" + e.Value + ")").TagColor(() => e.Value >= 0));
			}
			else
			{
				list.Add("modValue".lang(e.Name, ((e.Value < 0) ? "" : "+") + e.Value + (e.source.tag.Contains("ratio") ? "%" : "")).TagColorGoodBad(() => e.Value >= 0));
			}
		}
		if (lv >= 2)
		{
			list.Add("hintStrife3".lang().TagColorGoodBad(() => true));
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		ec.SetParent(owner);
	}

	public override void OnRemoved()
	{
		ec.SetParent();
	}
}
