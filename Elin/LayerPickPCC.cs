using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LayerPickPCC : ELayer
{
	public UIItemPCCPart mold;

	public LayoutGroup layoutParts;

	private UIItemPCC _uiItemPcc;

	private UIPCC _uiPcc;

	private List<UIItemPCCPart> _items = new List<UIItemPCCPart>();

	private List<Outline> _outlines = new List<Outline>();

	private static readonly Dictionary<PCC.Part, BaseModPackage> _cached = new Dictionary<PCC.Part, BaseModPackage>();

	private static readonly Regex _idRegex = new Regex("(\\d+)|(\\D+)", RegexOptions.Compiled);

	public void Activate(UIPCC uiPcc, UIItemPCC uiItemPcc)
	{
		_uiPcc = uiPcc;
		_uiItemPcc = uiItemPcc;
		PopulateButtons();
	}

	private void Update()
	{
		SpriteProvider provider = _uiPcc.actor.provider;
		int frame = (provider.currentFrame + 2) % 4;
		foreach (UIItemPCCPart item in _items)
		{
			item.SetSpriteIndex(provider.currentDir, frame);
		}
	}

	public void PopulateButtons()
	{
		List<PCC.Part> list = _uiItemPcc.parts.ToList();
		list.Sort(PartSorter);
		foreach (PCC.Part part in list)
		{
			UIItemPCCPart item = Util.Instantiate(mold, layoutParts);
			if (part == _uiItemPcc.part)
			{
				SetOutline(item.outline);
			}
			if (part == null)
			{
				item.button.onClick.AddListener(delegate
				{
					_uiItemPcc.slider.value = 0f;
					SetOutline(item.outline);
				});
				item.button.tooltip.lang = "empty";
			}
			else
			{
				ModItem<Texture2D> modItem = part.modTextures.TryGetValue("walk");
				if (modItem == null)
				{
					UnityEngine.Object.Destroy(item.gameObject);
					continue;
				}
				item.button.tooltip.lang = GetPartProviderString(part);
				item.button.onClick.AddListener(delegate
				{
					_uiItemPcc.slider.value = _uiItemPcc.parts.IndexOf(part);
					SetOutline(item.outline);
				});
				item.SetSprites(IO.LoadPNG(modItem.fileInfo.FullName));
				Color color = _uiPcc.pcc.data.GetColor(_uiItemPcc.idPartSet);
				item.button.icon.color = _uiPcc.pccm.ApplyColorMod(color);
			}
			_items.Add(item);
		}
		void SetOutline(Outline outline)
		{
			_outlines.Add(outline);
			foreach (Outline outline in _outlines)
			{
				outline.enabled = false;
			}
			outline.enabled = true;
		}
	}

	public static string GetPartProviderString(PCC.Part part)
	{
		return "ID: " + part.id + "\n<i>" + GetPartProvider(part).title.TagColor(Color.cyan) + "</i>\n" + part.dir.ShortPath();
	}

	public static BaseModPackage GetPartProvider(PCC.Part part)
	{
		if (part == null)
		{
			return null;
		}
		if (_cached.TryGetValue(part, out var value))
		{
			return value;
		}
		string dir = part.dir;
		value = BaseModManager.Instance.packages.FirstOrDefault((BaseModPackage p) => dir.StartsWith(p.dirInfo.FullName)) ?? new BaseModPackage
		{
			title = "unknownAuthor".lang()
		};
		return _cached[part] = value;
	}

	public static int PartSorter(PCC.Part lhs, PCC.Part rhs)
	{
		if (lhs == null)
		{
			if (rhs != null)
			{
				return -1;
			}
			return 0;
		}
		if (rhs == null)
		{
			return 1;
		}
		int num = GetPartSortOrder(lhs);
		int num2 = GetPartSortOrder(rhs);
		if (num != num2)
		{
			return num - num2;
		}
		string id = lhs.id;
		string id2 = rhs.id;
		if (id == id2)
		{
			return 0;
		}
		int result;
		bool flag = int.TryParse(id, out result);
		int result2;
		bool flag2 = int.TryParse(id2, out result2);
		if (flag && flag2)
		{
			return result.CompareTo(result2);
		}
		if (flag)
		{
			return -1;
		}
		if (flag2)
		{
			return 1;
		}
		MatchCollection matchCollection = _idRegex.Matches(id);
		MatchCollection matchCollection2 = _idRegex.Matches(id2);
		int num3 = Math.Min(matchCollection.Count, matchCollection2.Count);
		for (int i = 0; i < num3; i++)
		{
			string value = matchCollection[i].Value;
			string value2 = matchCollection2[i].Value;
			int result3;
			bool num4 = int.TryParse(value, out result3);
			int result4;
			bool flag3 = int.TryParse(value2, out result4);
			int num5 = ((!(num4 && flag3)) ? string.Compare(value, value2, StringComparison.OrdinalIgnoreCase) : result3.CompareTo(result4));
			if (num5 != 0)
			{
				return num5;
			}
		}
		return matchCollection.Count - matchCollection2.Count;
		static int GetPartSortOrder(PCC.Part part)
		{
			int num6 = BaseModManager.Instance.packages.IndexOf(GetPartProvider(part));
			if (num6 < 0)
			{
				return int.MaxValue;
			}
			return num6;
		}
	}
}
