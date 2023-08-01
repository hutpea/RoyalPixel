using UnityEngine;

public class MySystemInfo
{
	private static bool? s_IsTablet;

	public static bool IsTablet
	{
		get
		{
			bool? flag = s_IsTablet;
			if (!flag.HasValue)
			{
				s_IsTablet = (DeviceDiagonalSizeInInches() > 6.5f);
			}
			bool? flag2 = s_IsTablet;
			return flag2.Value;
		}
	}

	public static AppQuality AppTextureQuality
	{
		get
		{
			int systemMemorySize = SystemInfo.systemMemorySize;
			UnityEngine.Debug.Log("Memory: " + SystemInfo.systemMemorySize);
			if (systemMemorySize > 0)
			{
				if (systemMemorySize < 1000)
				{
					return AppQuality.Low;
				}
				if (systemMemorySize < 2000)
				{
					return AppQuality.Mid;
				}
				return AppQuality.High;
			}
			return AppQuality.Mid;
		}
	}

	private static float DeviceDiagonalSizeInInches()
	{
		float f = (float)Screen.width / Screen.dpi;
		float f2 = (float)Screen.height / Screen.dpi;
		return Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
	}
}
