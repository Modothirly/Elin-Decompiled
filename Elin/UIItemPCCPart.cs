using System;
using UnityEngine;
using UnityEngine.UI;

public class UIItemPCCPart : EMono
{
	public ButtonGeneral button;

	public Outline outline;

	[NonSerialized]
	public Sprite[] sprites = new Sprite[16];

	private bool _init;

	public void SetSpriteIndex(int dir, int frame)
	{
		if (_init)
		{
			button.icon.sprite = sprites[dir * 4 + frame];
		}
	}

	public void SetSprites(Texture2D texture)
	{
		int num = texture.width / 4;
		int num2 = texture.height / 4;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int num3 = j * num;
				int num4 = (3 - i) * num2;
				Rect rect = new Rect(num3, num4, num, num2);
				sprites[i * 4 + j] = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
			}
		}
		_init = true;
	}
}
