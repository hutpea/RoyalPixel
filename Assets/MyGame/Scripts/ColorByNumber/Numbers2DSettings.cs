using UnityEngine;

public class Numbers2DSettings
{
	public Color[][] NumberPixels = new Color[10][];

	public int CellSize
	{
		get;
		set;
	}

	public ShortVector2 NumberSize
	{
		get;
		set;
	}

	public string GridCellTextureName
	{
		get;
		set;
	}

	public string NumberResourcesSuffix
	{
		get;
		set;
	}

	public void LoadNumbers()
	{
		NumberPixels[0] = ((Texture2D)Resources.Load("Grid/number_zero" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[1] = ((Texture2D)Resources.Load("Grid/number_one" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[2] = ((Texture2D)Resources.Load("Grid/number_two" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[3] = ((Texture2D)Resources.Load("Grid/number_three" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[4] = ((Texture2D)Resources.Load("Grid/number_four" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[5] = ((Texture2D)Resources.Load("Grid/number_five" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[6] = ((Texture2D)Resources.Load("Grid/number_six" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[7] = ((Texture2D)Resources.Load("Grid/number_seven" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[8] = ((Texture2D)Resources.Load("Grid/number_eight" + NumberResourcesSuffix)).GetPixels();
		NumberPixels[9] = ((Texture2D)Resources.Load("Grid/number_nine" + NumberResourcesSuffix)).GetPixels();
	}

	public static Numbers2DSettings GetSettingsByQuality(AppQuality quality)
	{
		Numbers2DSettings numbers2DSettings = null;

		switch (quality)
		{
			case AppQuality.Low:
				{
					Numbers2DSettings numbers2DSettings2 = new Numbers2DSettings();
					//First cell size : 30
					numbers2DSettings2.CellSize = 30;
					//Where to fix
					//First : 10, 12
					numbers2DSettings2.NumberSize = new ShortVector2(10, 12);
					numbers2DSettings2.GridCellTextureName = "grid_cell_30";
					numbers2DSettings2.NumberResourcesSuffix = "_12";
					numbers2DSettings = numbers2DSettings2;
					break;
				}
			case AppQuality.High:
				{
					Numbers2DSettings numbers2DSettings2 = new Numbers2DSettings();
					// First : 75
					numbers2DSettings2.CellSize = 75;
					//Where to fix
					//First : 25, 30
					numbers2DSettings2.NumberSize = new ShortVector2(25, 30);
					numbers2DSettings2.GridCellTextureName = "grid_cell_75";
					numbers2DSettings2.NumberResourcesSuffix = "_30";
					numbers2DSettings = numbers2DSettings2;
					break;
				}
			default:
				{
					Numbers2DSettings numbers2DSettings2 = new Numbers2DSettings();
					//First cell size : 50
					numbers2DSettings2.CellSize = 50;
					// Where to fix
					// First : 16, 20
					numbers2DSettings2.NumberSize = new ShortVector2(16, 20);
					numbers2DSettings2.GridCellTextureName = "grid_cell_50";
					numbers2DSettings2.NumberResourcesSuffix = string.Empty;
					numbers2DSettings = numbers2DSettings2;
					break;
				}
		}

        numbers2DSettings.LoadNumbers();
		return numbers2DSettings;
	}
}
