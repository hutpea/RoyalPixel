using BizzyBeeGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum PictureType
{
    Free,
    Ads,
    Vip,
    Gem
}
public class PictureInformation
{
    #region Member Variables

    private string fileContents;
    private bool isFileLoaded;
    private bool isIdLoaded;
    private bool isFileTotalPixel;
    private bool isLoadInfo;
    private string id;
    private int idArea;
    private int xCells;
    private int yCells;
    private List<List<int>> colorNumbers;
    private List<Color> colors;
    private bool hasBlankCells;
    private bool isLocked;
    private int unlockAmount;
    private bool awardOnComplete;
    private int awardAmount = 1;
    private int totalPixel;
    private bool isNewPic;
    // Saved matrix of color numbers, -1 means it's colored in, number >= 0 means it still needs to be colored
    private List<List<int>> progress;
    private List<int> colorsLeft;
    private bool unlocked;
    private bool completed;
    private PictureType pictureType;
    public bool modify;
    public bool IsNew;
    private int totalAds;
    public int CurrentAds
    {
        get
        {
            if (pictureType == PictureType.Ads)
            {
                return PlayerPrefs.GetInt("number_" + Id, awardAmount);
            }
            return PlayerPrefs.GetInt("number_" + Id, 0);
        }
        set
        {
            PlayerPrefs.SetInt("number_" + Id, value);
        }
    }
    #endregion

    #region Properties

    /// <summary>
    /// If true then the grayscale for the menu screen needs to be re-loaded and not use the one in the cache
    /// </summary>
    public bool ReloadGrayscale { get; set; }

    /// <summary>
    /// Gets the unique id of the picture
    /// </summary>
    public string Id
    {
        get
        {
            //if (!isIdLoaded)
            //{
            LoadIdFromPictureFile();
            //}

            return id;
        }
    }
    public int IdArea
    {
        get
        {
            string strId = Id.Split('-')[0];
            idArea = int.Parse(strId);
            return idArea;
        }
    }
    public int IdCateAre = 0;
    public bool SinglePic
    {
        get
        {
            if (IdArea >= 10000 || IdArea == 100 || IdArea == 101 || IdArea == 102 || IdArea == 103)
            {
                return true;
            }
            return false;
        }
    }
    public bool SinglePicNoel
    {
        get
        {
            if (IdArea == 100 || IdArea == 101 || IdArea == 102 || IdArea == 103)
            {
                return true;
            }
            return false;
        }
    }
    public bool CreatePic
    {
        get
        {
            if (IdArea == 0)
            {
                return true;
            }
            return false;
        }
    }
    public bool DrawPic
    {
        get
        {
            if (IdArea == 1000)
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// Gets the number of X cells in the picture
    /// </summary>
    /// <value>The X cells.</value>
    public int XCells
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }

            return xCells;
        }
    }
    public PictureType PictureType
    {
        get
        {
            if (!isLoadInfo)
            {
                LoadInfoPicture();
            }
            return pictureType;
        }
    }


    /// <summary>
    /// Gets the number of Y cells in the picture
    /// </summary>
    /// <value>The Y cells.</value>
    public int YCells
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();

            }

            return yCells;
        }
    }

    public int TotalPixel
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }
            return totalPixel;
        }
    }
    public int TotalPixelPainted
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.TOTAL_PIXEL_PAINTED + Id, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.TOTAL_PIXEL_PAINTED + Id, value);
        }
    }
    /// <summary>
    /// Gets a matrix of each cell and the color number for the cell
    /// </summary>
    public List<List<int>> ColorNumbers
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }

            return colorNumbers;
        }
    }

    /// <summary>
    /// Gets a list of all the colors in the picture, the index of the color is it's number
    /// </summary>
    public List<Color> Colors
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }

            return colors;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has blank cells.
    /// </summary>
    public bool HasBlankCells
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }

            return hasBlankCells;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is locked until purchased with in-game currency
    /// </summary>
    public bool IsLocked
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }

            return isLocked && !unlocked;
        }
    }

    /// <summary>
    /// Gets the unlock amount
    /// </summary>
    public int UnlockAmount
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }

            return unlockAmount;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance awards in-game currency when completed
    /// </summary>
    public bool AwardOnComplete
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }

            return awardOnComplete;
        }
    }

    /// <summary>
    /// Gets the award amount
    /// </summary>
    public int AwardAmount
    {
        get
        {
            if (!isFileLoaded)
            {
                LoadPictureFile();
            }

            return awardAmount;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has saved progress
    /// </summary>
    public bool HasProgress
    {
        get
        {
            return progress != null;
        }
    }

    public List<List<int>> Progress
    {
        get
        {
            if (progress == null)
            {
                InitProgress();
            }

            return progress;
        }
    }

    public List<int> ColorsLeft
    {
        get
        {
            if (colorsLeft == null)
            {
                InitProgress();
            }

            return colorsLeft;
        }
    }

    public bool Completed
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.PIC_COMPLETE + Id) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.PIC_COMPLETE + Id, value ? 1 : 0);
        }
    }

    #endregion

    #region Public Methods

    public PictureInformation(string content)
    {
        fileContents = content.Replace("\r", "");
    }

    /// <summary>
    /// Unlocks this PictureInformation instance so the user can play any number of times
    /// </summary>
    public void SetUnlocked()
    {
        unlocked = true;
    }

    /// <summary>
    /// Sets this PictureInformation instance completed
    /// </summary>
    public void SetCompleted(bool isComplete = true)
    {
        completed = isComplete;
    }

    /// <summary>
    /// Clears any progress, makes it so this instance was never started
    /// </summary>
    public void ClearProgress()
    {
        progress = null;
        colorsLeft = null;
        ReloadGrayscale = true;
    }

    /// <summary>
    /// Checks if a given color is complete (Has all it's pixels colored in)
    /// </summary>
    public bool IsColorComplete(int colorIndex)
    {
        return HasProgress && colorsLeft[colorIndex] == 0;
    }

    /// <summary>
    /// Checks if this PictureInformation has all of it's pixels colored in
    /// </summary>
    public bool IsLevelComplete()
    {
        // Check if the level has any progress, it cant be complete if it has even been started yet
        if (HasProgress)
        {
            bool allColorsComplete = true;

            // Check if each of the colors are complete
            for (int i = 0; i < colors.Count; i++)
            {
                if (!IsColorComplete(i))
                {
                    allColorsComplete = false;

                    break;
                }
            }

            return allColorsComplete;
        }

        return false;
    }

    public Dictionary<string, object> GetSaveData()
    {
        Dictionary<string, object> saveData = new Dictionary<string, object>();

        saveData["has_progress"] = HasProgress;

        if (HasProgress)
        {
            saveData["progress"] = Progress;
            saveData["colors_left"] = ColorsLeft;
        }

        saveData["id"] = Id;
        saveData["completed"] = completed;
        saveData["unlocked"] = unlocked;

        return saveData;
    }

    public void LoadSaveData(JSONNode saveData)
    {
        if (saveData["has_progress"].AsBool)
        {
            progress = new List<List<int>>();
            colorsLeft = new List<int>();

            foreach (JSONArray list in saveData["progress"].AsArray)
            {
                List<int> temp = new List<int>();

                foreach (JSONNode item in list)
                {
                    temp.Add(item.AsInt);
                }

                progress.Add(temp);
            }

            foreach (JSONNode item in saveData["colors_left"].AsArray)
            {
                colorsLeft.Add(item.AsInt);
            }
        }

        completed = saveData["completed"].AsBool;
        unlocked = saveData["unlocked"].AsBool;
    }

    public void InitProgress()
    {
        if (!isFileLoaded)
        {
            LoadPictureFile();
        }

        progress = new List<List<int>>();
        colorsLeft = new List<int>();

        // Create the dictionary that keeps track of how many color cells are left for each color
        for (int i = 0; i < colors.Count; i++)
        {
            colorsLeft.Add(0);
        }

        // Copy the colorNumbers matrix
        for (int i = 0; i < colorNumbers.Count; i++)
        {
            progress.Add(new List<int>(colorNumbers[i]));

            // Increase the colorsLeft count for each of the colors
            for (int j = 0; j < colorNumbers[i].Count; j++)
            {
                int colorIndex = colorNumbers[i][j];

                if (colorIndex != -1)
                {
                    colorsLeft[colorIndex]++;
                }
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads just the levels id from the file
    /// </summary>
    private void LoadIdFromPictureFile()
    {
        int secondLineStartIndex = fileContents.IndexOf('\n') + 1;
        int secondNewlineIndex = fileContents.IndexOf('\n', secondLineStartIndex);
        int length = secondNewlineIndex - secondLineStartIndex;
        id = fileContents.Substring(secondLineStartIndex, length);
    }
    private void LoadInfoPicture()
    {
        if (IdArea == 0 || IdArea == 1000)
        {
            pictureType = PictureType.Free;
            isNewPic = false;
        }
        else if (TotalPixelPainted != 0)
            pictureType = PictureType.Free;
        else
        {
            string[] sub = fileContents.Split('\n');
            string strPictureType = sub[3];
            if (System.Enum.TryParse(sub[3], out PictureType picture))
            {
                pictureType = picture;
            }
            else
            {
                string[] str = strPictureType.Split(',');
                pictureType = (PictureType)System.Enum.Parse(typeof(PictureType), str[0]);
                awardAmount = System.Int32.Parse(str[1]);
            }
            isNewPic = sub[2].Split(',').Length > 1 ? true : false;
            isLoadInfo = true;
        }
    }
    /// <summary>
    /// Parses the picture file.
    /// </summary>
    private void LoadPictureFile()
    {
        List<string[]> lines = ParseCSVLines(fileContents);

        if (lines.Count == 0)
        {
            Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, there are no lines in the file.");

            return;
        }

        int index = 0;

        //formatVersion = lines[index][0];
        index++;

        //id = lines[index][0];
        //idArea = int.Parse(lines[index][11]);
        index++;

        // Get the level lock info
        //if (!ParseBool(lines[index], 0, out isLocked) || !ParseInt(lines[index], 1, out unlockAmount))
        //{
        //    Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, could not parse level lock information.");

        //    //return;
        //}

        index++;

        // Get the award info
        if (!ParseBool(lines[index], 0, out awardOnComplete) || !ParseInt(lines[index], 1, out awardAmount))
        {
            //Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, could not parse level lock information.");

            //return;
        }

        index++;

        //Get the number of x and y cells in the picture
        if (!ParseInt(lines[index], 0, out xCells) || !ParseInt(lines[index], 1, out yCells))
        {
            Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, could not parse xCells and/or yCells.");

            return;
        }

        index++;

        // Get a list of integers that represent what colors each pixel are
        colorNumbers = new List<List<int>>();

        for (int i = index; i < yCells + index; i++)
        {
            if (i >= lines.Count)
            {
                Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, no enough lines when parse color numbers.");

                return;
            }

            colorNumbers.Add(new List<int>());

            for (int j = 0; j < xCells; j++)
            {
                int number;

                if (!ParseInt(lines[i], j, out number))
                {
                    Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, could not parse color number.");

                    return;
                }

                if (number == -1)
                {
                    hasBlankCells = true;
                }
                else
                {
                    totalPixel++;
                }
                colorNumbers[i - index].Add(number);
            }
        }

        index += yCells;
        // Get the list of colors in the picture
        colors = new List<Color>();

        for (int i = index; i < lines.Count; i++)
        {
            float r, g, b;

            if (!ParseFloat(lines[i], 0, out r) ||
                !ParseFloat(lines[i], 1, out g) ||
                !ParseFloat(lines[i], 2, out b))
            {
                Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, could not parse color information. " + lines[i]);

                return;
            }

            colors.Add(new Color(r, g, b, 1f));
        }

        isIdLoaded = true;
        isFileLoaded = true;
    }
    private int LoadTotalPixel()
    {
        List<string[]> lines = ParseCSVLines(fileContents);
        if (lines.Count == 0)
        {
            Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, there are no lines in the file.");

            return 0;
        }

        int index = 0;
        index++;
        index++;
        index++;
        index++;
        //Get the number of x and y cells in the picture
        index++;
        for (int i = index; i < yCells + index; i++)
        {
            for (int j = 0; j < xCells; j++)
            {
                int number;

                if (!ParseInt(lines[i], j, out number))
                {
                    Debug.LogError("[PictureInformation] ParsePictureFile: Malformed file contents, could not parse color number.");

                    return 0;
                }

                if (number == -1)
                {
                    hasBlankCells = true;
                }
                else
                {
                    totalPixel++;
                }
            }
        }
        Debug.Log("total" + totalPixel);
        return totalPixel;
    }

    /// <summary>
    /// Parses the CSV file and seperate the lines
    /// </summary>
    private List<string[]> ParseCSVLines(string csv)
    {
        List<string[]> lines = new List<string[]>();
        string[] csvLines = csv.Split('\n');

        for (int i = 0; i < csvLines.Length; i++)
        {
            string line = csvLines[i].Replace("\r", "").Trim();

            if (!string.IsNullOrEmpty(line))
            {
                lines.Add(line.Split(','));
            }
        }

        return lines;
    }

    /// <summary>
    /// Helper method that converts a string at the given index into an integer, returns false if it fails
    /// </summary>
    private bool ParseInt(string[] line, int index, out int value)
    {
        value = 0;

        if (index >= line.Length)
        {
            return false;
        }

        if (!int.TryParse(line[index], out value))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Helper method that converts a string at the given index into an float, returns false if it fails
    /// </summary>
    private bool ParseFloat(string[] line, int index, out float value)
    {
        value = 0;

        if (index >= line.Length)
        {
            return false;
        }

        if (!float.TryParse(line[index], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Helper method that converts a string at the given index into a boolean, returns false if it fails
    /// </summary>
    private bool ParseBool(string[] line, int index, out bool value)
    {
        value = false;

        if (index >= line.Length)
        {
            return false;
        }

        if (!bool.TryParse(line[index], out value))
        {
            return false;
        }

        return true;
    }
    #endregion
    public void SetTotalColorPixelPainted(int numberColor, int value)
    {
        PlayerPrefs.SetInt(StringConstants.KEY.COLOR_PAINTED + Id + numberColor, value);
    }
    public int GetTotalColorPixelPainted(int numberColor)
    {
        return PlayerPrefs.GetInt(StringConstants.KEY.COLOR_PAINTED + Id + numberColor);
    }
    public void SaveCurrentDataAsync(byte[] data)
    {
        Helper.WriteBytes(GameData.CurrentPathForId(Id), data);
        GameData.UpdateLocalPictureCreatedAt(id);

    }
    public void DeletePictureData()
    {
        string originalPath = GameData.OriginalPathForId(this.id);
        if (File.Exists(originalPath))
        {
            File.Delete(originalPath);
        }
        string currentPath = GameData.CurrentPathForId(this.id);
        if (File.Exists(currentPath))
        {
            File.Delete(currentPath);
        }
        //DataContain.Instance.ClearPictureHistory(this.id);
        //DataContain.Instance.ClearReceivedGift(this.id);
    }
    public DateTime CreatedAt
    {
        get
        {
            return GameData.GetPictureCreatedAt(this.id);
        }
    }
}
public class PictureInfoCreatedAtComparer : IComparer<PictureInformation>
{
    public int Compare(PictureInformation x, PictureInformation y)
    {
        return -x.CreatedAt.CompareTo(y.CreatedAt);
    }

}

