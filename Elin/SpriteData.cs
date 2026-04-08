using System;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;
using UnityEngine;

public class SpriteData
{
	public string id;

	public string path;

	public Texture2D tex;

	public Sprite[] sprites;

	public DateTime date;

	public int frame = 1;

	public int scale = 50;

	public float time = 0.2f;

	public SourcePref pref;

	public void Init()
	{
		try
		{
			id = Path.GetFileNameWithoutExtension(path);
			LoadAnimationIni();
			LoadPref();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			Debug.LogError("#sprite failed to init '" + id + "' at " + path);
		}
	}

	public void LoadPref()
	{
		pref = null;
		if (File.Exists(path + ".pref"))
		{
			pref = SourcePref.ReadFromIni(path + ".pref");
		}
	}

	public void LoadAnimationIni()
	{
		if (!File.Exists(path + ".ini"))
		{
			return;
		}
		try
		{
			IniData iniData = new FileIniDataParser().ReadFile(path + ".ini", Encoding.UTF8);
			if (!int.TryParse(iniData.GetKey("frame") ?? "1", out frame))
			{
				frame = 1;
			}
			if (!int.TryParse(iniData.GetKey("scale") ?? "50", out scale))
			{
				scale = 50;
			}
			if (!float.TryParse(iniData.GetKey("time") ?? "0.2", out time))
			{
				time = 0.2f;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			frame = 1;
			scale = 50;
			time = 0.2f;
		}
	}

	public Sprite[] GetSprites()
	{
		Validate();
		return sprites;
	}

	public Sprite GetSprite()
	{
		Validate();
		return sprites[0];
	}

	public void Validate()
	{
		DateTime lastWriteTime = File.GetLastWriteTime(path + ".png");
		if (date.Year == 1 || !(date == lastWriteTime))
		{
			Load();
			date = lastWriteTime;
		}
	}

	public void Load()
	{
		Texture2D texture2D = IO.LoadPNG(path + ".png");
		Debug.Log(texture2D.width + "/" + texture2D.height + "/" + path);
		if ((bool)tex)
		{
			if (texture2D.width != tex.width || texture2D.height != tex.height)
			{
				tex.Reinitialize(texture2D.width, texture2D.height);
			}
			tex.SetPixels32(texture2D.GetPixels32());
			tex.Apply();
			UnityEngine.Object.Destroy(texture2D);
		}
		else
		{
			tex = texture2D;
			int num = tex.width / frame;
			int height = tex.height;
			sprites = new Sprite[frame];
			Vector2 pivot = new Vector2(0.5f, 0.5f);
			for (int i = 0; i < frame; i++)
			{
				sprites[i] = Sprite.Create(tex, new Rect(i * num, 0f, num, height), pivot, 100f, 0u, SpriteMeshType.FullRect);
			}
		}
		LoadPref();
	}
}
