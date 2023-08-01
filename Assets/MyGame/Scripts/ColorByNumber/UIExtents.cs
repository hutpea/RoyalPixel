using UnityEngine;
using UnityEngine.UI;

public static class UIExtents
{
	public static void SetAlpha(this MaskableGraphic graphic, float alpha)
	{
		Color color = graphic.color;
		color.a = alpha;
		graphic.color = color;
	}

	public static bool IsDark(this Color color)
	{
		double num = 1.0 - (0.299 * (double)color.r + 0.587 * (double)color.g + 0.114 * (double)color.b);
		return num >= 0.5;
	}

	public static bool EqualsColor32(this Color32 color1, Color32 color2)
	{
		return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b;
	}
}
