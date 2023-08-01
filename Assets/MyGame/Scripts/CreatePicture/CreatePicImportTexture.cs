using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePicImportTexture : MonoBehaviour
{
    private class ImportedTexture
    {
        public string id;
        public int xPixels;
        public int yPixels;
        public Texture2D texture;
        public List<PaletteItem> palette;
        public Dictionary<string, int> paletteIndexMap;
    }

    #region Enums

    private enum ScaleMode
    {
        BoxSampling,
        CenterPixel
    }

    private enum AwardType
    {
        Manual,
        PerPixel
    }

    #endregion

    #region Member Variables

    private Texture2D importTexture;
    private int xPixels = 25;
    private int yPixels = 25;
    private ScaleMode scaleMode = ScaleMode.BoxSampling;
    private bool mergeSimilarColors = true;
    private float mergeThreshold = 1.5f;
    private string id = "";
    private int idArea = -1;

    private bool isLevelLocked = false;
    private int unlockAmount = 100;
    private bool awardOnCompletion = false;
    private AwardType awardType = AwardType.Manual;
    private float perPixelAmount = 0.05f;
    private int awardAmount = 100;
    private string fileName = "";
    private string outputFolder = "";

    private ImportedTexture importedTexture;
    private int blankItemIndex = -1;
    private float pictureScale = 1f;
    private bool showGridLines;
    private bool showNumbers;

    private Texture2D headerLineTexture;
    private Texture2D blackTexture;
    private List<Texture2D> paletteItemTextures;

    // Textures for icons
    private Dictionary<string, Texture2D> iconTextures;

    // List of infomation about the icon textures, first value is key second is icon file name
    private static readonly List<string[]> iconTextureInfo = new List<string[]>()
    {
        new string[] { "grid_on_white", "icon_grid_on_white" },
        new string[] { "grid_on_black", "icon_grid_on_black" },
        new string[] { "zoom_in_black", "icon_zoom_in" },
        new string[] { "zoom_out_black", "icon_zoom_out" },
        new string[] { "numbers_white", "icon_numbers_white" },
        new string[] { "numbers_black", "icon_numbers_black" }
    };

    #endregion



    #region Unity Methods

    private void OnDisable()
    {
        DestroyTexture(headerLineTexture);
        DestroyTexture(blackTexture);

        if (importedTexture != null)
        {
            DestroyTexture(importedTexture.texture);
        }

        if (iconTextures != null)
        {
            foreach (KeyValuePair<string, Texture2D> pair in iconTextures)
            {
                DestroyTexture(pair.Value);
            }
        }
    }


    private void ExportFilePicture()
    {
        if (string.IsNullOrEmpty(outputFolder))
        {
            if (GameData.isDrawPixel)
                outputFolder = GameData.DRAW_PATH;
            else
                outputFolder = GameData.CREATE_PATH;
            fileName = id;
        }
        GameData.content = TextureUtilities.ExportTextureToFile(
            importedTexture.id,
            importedTexture.xPixels,
            importedTexture.yPixels,
            importedTexture.palette,
            outputFolder,
            fileName,
            blankItemIndex,
            isLevelLocked,
            unlockAmount,
            awardOnCompletion,
            awardAmount
          );
        Debug.Log("content" + GameData.content);

        // Refresh the asset database so the new file shows up in the Project folder
    }

    #endregion

    public void ImportTexture(Texture2D importTexture)
    {
        Texture2D trimmedTexture = TextureUtilities.TrimAlpha(importTexture);

        bool firstItemBlank;

        xPixels = importTexture.width;
        yPixels = importTexture.height;

        Texture2D newTexture = new Texture2D(xPixels, yPixels, TextureFormat.ARGB32, false);
        newTexture.filterMode = FilterMode.Point;

        TextureUtilities.ScaleType scaleType = TextureUtilities.ScaleType.BoxSampling;
        List<PaletteItem> palette = TextureUtilities.Pixelize(trimmedTexture.GetPixels(), trimmedTexture.width, trimmedTexture.height, newTexture, xPixels, yPixels, scaleType, (mergeSimilarColors ? mergeThreshold : 0), out firstItemBlank);

        blankItemIndex = firstItemBlank ? 0 : -1;

        SortPalette(palette);

        importedTexture = new ImportedTexture();
        importedTexture.xPixels = xPixels;
        if (GameData.isDrawPixel)
        {
            importedTexture.id = "1000" + GameData.IdPicCreate.ToString();
        }
        else
            importedTexture.id = "0" + GameData.IdPicCreate.ToString();
        id = importedTexture.id;
        GameData.IdPicCreate++;
        importedTexture.yPixels = yPixels;
        importedTexture.texture = newTexture;
        importedTexture.palette = palette;
        importedTexture.paletteIndexMap = new Dictionary<string, int>();

        for (int i = 0; i < importedTexture.palette.Count; i++)
        {
            PaletteItem paletteItem = importedTexture.palette[i];

            for (int j = 0; j < paletteItem.xCoords.Count; j++)
            {
                int x = paletteItem.xCoords[j];
                int y = paletteItem.yCoords[j];

                importedTexture.paletteIndexMap[PaletteIndexKey(x, y)] = i;
            }
        }
        ExportFilePicture();
        // Create a new list of textures for each palette item
        DestroyTextures(paletteItemTextures);

        paletteItemTextures = new List<Texture2D>();

        for (int i = 0; i < importedTexture.palette.Count; i++)
        {
            paletteItemTextures.Add(TextureUtilities.CreateTexture(1, 1, importedTexture.palette[i].color));
        }
        Debug.LogFormat("Imported texture has {0} palette colors", palette.Count);
    }

    private void SortPalette(List<PaletteItem> palette)
    {
        if (palette.Count > 2)
        {
            List<PaletteItem> tempPalette = new List<PaletteItem>(palette);

            palette.Clear();

            PaletteItem paletteItem = tempPalette[0];

            while (true)
            {
                palette.Add(paletteItem);

                tempPalette.Remove(paletteItem);

                if (tempPalette.Count == 0)
                {
                    break;
                }

                float diff;

                paletteItem = TextureUtilities.GetClosestPaletteItem(paletteItem.color, tempPalette, out diff);
            }
        }
    }

    /// <summary>
    /// Gets the index key for the coords
    /// </summary>
    private string PaletteIndexKey(int x, int y)
    {
        return string.Format("{0}_{1}", x, y);
    }

    /// <summary>
    /// Destroys the given texture if it's not null
    /// </summary>
    private void DestroyTexture(Texture2D texture)
    {
        //if (texture != null)
        //{
        //    DestroyImmediate(texture);
        //}
    }

    /// <summary>
    /// Destroies the textures.
    /// </summary>
    private void DestroyTextures(List<Texture2D> textures)
    {
        if (textures != null)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                DestroyTexture(textures[i]);
            }
        }
    }
}
