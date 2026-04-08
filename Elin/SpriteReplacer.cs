using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteReplacer
{
	public static Dictionary<string, SpriteReplacer> dictSkins = new Dictionary<string, SpriteReplacer>();

	public static Dictionary<string, string> dictModItems = new Dictionary<string, string>();

	public SpriteData data;

	public Dictionary<string, SpriteData> suffixes = new Dictionary<string, SpriteData>();

	public Dictionary<string, bool> isChecked = new Dictionary<string, bool>();

	public static Dictionary<string, SpriteReplacer> ListSkins()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, SpriteReplacer> dictSkin in dictSkins)
		{
			if (!File.Exists(dictSkin.Value.data.path + ".png"))
			{
				list.Add(dictSkin.Key);
			}
		}
		foreach (string item in list)
		{
			dictSkins.Remove(item);
		}
		FileInfo[] files = new DirectoryInfo(CorePath.custom + "Skin").GetFiles("*.png");
		foreach (FileInfo fileInfo in files)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
			if (!dictSkins.ContainsKey(fileNameWithoutExtension))
			{
				SpriteReplacer spriteReplacer = new SpriteReplacer();
				spriteReplacer.data = new SpriteData
				{
					path = fileInfo.GetFullFileNameWithoutExtension()
				};
				spriteReplacer.data.Init();
				dictSkins.Add(fileNameWithoutExtension, spriteReplacer);
			}
		}
		return dictSkins;
	}

	public Sprite GetSprite(string suffix = "")
	{
		if (!suffixes.TryGetValue(suffix, out var value))
		{
			return null;
		}
		return value.GetSprite();
	}

	public void Validate()
	{
		data?.Validate();
		foreach (SpriteData value in suffixes.Values)
		{
			value?.Validate();
		}
	}

	public void Reload(string id, RenderData renderData = null)
	{
		data = null;
		suffixes.Clear();
		try
		{
			if (dictModItems.ContainsKey(id))
			{
				foreach (var (text3, path) in dictModItems)
				{
					if (text3.StartsWith(id))
					{
						string text4 = text3[id.Length..];
						SpriteData spriteData = new SpriteData
						{
							path = path
						};
						spriteData.Init();
						suffixes[text4] = spriteData;
						Debug.Log("#sprite replacer init '" + text4 + "' at " + path.ShortPath());
					}
				}
				data = suffixes.TryGetValue("");
			}
			else
			{
				string text5 = CorePath.packageCore + "Texture/Item/" + id;
				if (!File.Exists(text5 + ".png") && renderData != null)
				{
					text5 = CorePath.packageCore + "Texture/Item/" + renderData.name;
				}
				if (File.Exists(text5 + ".png"))
				{
					data = new SpriteData
					{
						path = text5
					};
					data.Init();
					suffixes[""] = data;
				}
			}
			if (data != null)
			{
				Debug.Log(id + ":" + data.path);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("#sprite error fetching sprite replacer:" + ex);
		}
		isChecked[id] = true;
	}

	public bool HasSprite(string id, RenderData renderData = null)
	{
		if (!isChecked.GetValueOrDefault(id) || (data != null && data.id != id))
		{
			Reload(id, renderData);
		}
		return data != null;
	}
}
