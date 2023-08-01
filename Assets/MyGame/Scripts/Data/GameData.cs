using BizzyBeeGames.ColorByNumbers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameData
{
    public static PictureInformation picChoice;
    public static Texture2D curGrayTexture, CurColorTexture, grayScale, areasColor, whiteColor, peletteColor;
    public static Vector2 vtDefault = new Vector2(-1000, -1000);
    private static Dictionary<string, History> histories = new Dictionary<string, History>();
    public static int totalPicInAreas;
    internal static string HISTORY_PATH = Path.Combine(Application.persistentDataPath, "histories");
    internal static string ORIGINAL_PATH = Path.Combine(Application.persistentDataPath, "pictures", "original");
    internal static string CURRENT_PATH = Path.Combine(Application.persistentDataPath, "pictures", "current");
    internal static string CREATE_PATH = Path.Combine(Application.persistentDataPath, "create");
    internal static string DRAW_PATH = Path.Combine(Application.persistentDataPath, "draw");
    public static List<PictureInformation> pictures = new List<PictureInformation>();
    public static readonly DateTime zeroTime = new DateTime(0);
    public static string typeCreate;
    public static string content = "";
    public static int IdAreaChoice = 0;
    public static bool isPicBig = false;
    public static List<string> picSinglePainted = new List<string>();
    public static bool finish = true;
    public static bool isCreateCard = false;
    public static bool isDrawPixel;
    public static bool isEdit;
    public static bool choicePicEvent;
    public static bool isReciveGemPig;
    public static bool isSelectInEventFarm;
    public static bool isInViewEventFarm;
    public static bool isBackFromEventFarmGameplay;
    public static int farmPicTimeLimit;
    public static float gameplayProgress;
    public static float aspect;
    public static int pianoCurrent;
    public static void InprogressPic(string id)
    {
        string progress = PlayerPrefs.GetString(StringConstants.KEY.SAVE_INPROGRESS);
        if (ConstainExitsId(id))
            return;
        progress += id + ",";
        PlayerPrefs.SetString(StringConstants.KEY.SAVE_INPROGRESS, progress);
    }
    public static void SetShowAdsPic(string id, bool value)
    {
        PlayerPrefs.SetInt(StringConstants.KEY.SHOW_ADS_PIC + id, value ? 1 : 0);
    }
    public static bool GetShowAdsPic(string id)
    {
        return PlayerPrefs.GetInt(StringConstants.KEY.SHOW_ADS_PIC + id) == 1;
    }
    public static bool ConstainExitsId(string id)
    {
        string progress = PlayerPrefs.GetString(StringConstants.KEY.SAVE_INPROGRESS);
        if (!string.IsNullOrEmpty(progress))
        {
            string[] ids = progress.Split(',');
            for (int i = 0; i < ids.Length; i++)
            {
                if (id == ids[i])
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool CompleteArea
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.COMPLETE_AREAS) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.COMPLETE_AREAS, value ? 1 : 0);
        }
    }
    public static void SetUnlockAreas(int id)
    {
        PlayerPrefs.SetInt(StringConstants.KEY.UNLOCK_AREAS + id, 1);
    }
    public static bool GetUnlockArea(int id)
    {
        return PlayerPrefs.GetInt(StringConstants.KEY.UNLOCK_AREAS + id, 0) == 1;

    }
    public static DateTime LuckySpinLastTimeFreeSpined
    {
        get
        {
            return GetDateTime("last_time_spin", DateTime.MinValue);
        }
        set
        {
            SetDateTime("last_time_spin", value);
        }
    }
    public static int CountPicCompleteShowIAP
    {
        get
        {
            return PlayerPrefs.GetInt("pic_complete", 0);
        }
        set
        {
            PlayerPrefs.SetInt("pic_complete", value);
        }
    }
    public static int CountPicComplete
    {
        get
        {
            return PlayerPrefs.GetInt("pic_win", 0);
        }
        set
        {
            PlayerPrefs.SetInt("pic_win", value);
        }
    }

    public static float CountGemPig
    {
        get
        {
            return PlayerPrefs.GetFloat("gem_pig", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("gem_pig", Math.Min(1000, value));
        }
    }
    public static int CountDailyPicture
    {
        get
        {
            return PlayerPrefs.GetInt("pic_daily", -1);
        }
        set
        {
            PlayerPrefs.SetInt("pic_daily", value);
        }
    }
    public static int Gem
    {
        get
        {
            return PlayerPrefs.GetInt("Gem", 0);
        }
        set
        {
            PlayerPrefs.SetInt("Gem", value);
            PopupEventFarmBuilding.instance?.UpdateUI();
        }
    }
    public static bool ShowEvent
    {
        get
        {
            return PlayerPrefs.GetInt("show_event") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("show_event", value ? 1 : 0);
        }

    }

    public static bool ShowFarmEvent
    {
        get
        {
            return PlayerPrefs.GetInt("show_farm_event") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("show_farm_event", value ? 1 : 0);
        }

    }

    public static bool BuyPackIAP(string pack)
    {
        return PlayerPrefs.GetInt("buy_pack_" + pack) == 1;
    }
    public static void SetPackIAP(string pack)
    {
        PlayerPrefs.SetInt("buy_pack_" + pack, 1);
    }
    public static DateTime GetDateTimeLastFreeItem(string item)
    {
        return GetDateTime("date_time_free_item_" + item, DateTime.Now);
    }
    public static void SetDateTimeLastFreeItem(string item, DateTime value)
    {
        SetDateTime("date_time_free_item_" + item, value);
    }
    public static int PaintingAreas
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.PAINTING_AREAS, 1);
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.PAINTING_AREAS, value);
        }
    }
    public static int GetNumberAds(string ID)
    {
        return PlayerPrefs.GetInt("number_ads_" + ID, 0);
    }
    public static void SetNumberAds(string ID, int value)
    {
        PlayerPrefs.SetInt("number_ads_" + ID, value);
    }
    public static int PaintingCateAreas
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.PAINTING_CATE_AREAS, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.PAINTING_CATE_AREAS, value);
        }
    }
    public static void SetStatusPic(string idPic, Status statusPic)
    {
        PlayerPrefs.SetInt(StringConstants.KEY.PIC_CURRENT + idPic, (int)statusPic);
    }
    public static Status GetStatusPic(string idPic)
    {
        return (Status)PlayerPrefs.GetInt(StringConstants.KEY.PIC_CURRENT, 0);
    }
    public static History HistoryForPictureId(string id)
    {
        if (histories.ContainsKey(id))
        {
            return histories[id];
        }
        else
        {
            History history = new History(id);
            histories[id] = history;
            return history;
        }
    }
    public static DateTime GetDateTime(string key, DateTime defaultValue)
    {
        string @string = PlayerPrefs.GetString(key);
        DateTime result = defaultValue;
        if (!string.IsNullOrEmpty(@string))
        {
            long dateData = Convert.ToInt64(@string);
            result = DateTime.FromBinary(dateData);
        }
        return result;
    }
    public static void SaveCratePic(string id)
    {
        string path = Path.Combine(CREATE_PATH, id + ".txt");
        if (!Directory.Exists(CREATE_PATH))
        {
            Directory.CreateDirectory(CREATE_PATH);
        }
        System.IO.File.WriteAllText(path, content);
    }
    public static void SetDateTime(string key, DateTime val)
    {
        PlayerPrefs.SetString(key, val.ToBinary().ToString());
    }
    public static void SetDateTimeReciveGift(DateTime value)
    {
        PlayerPrefs.SetString(StringConstants.KEY.DATE_RECIVE_GIFT_VIP, value.ToBinary().ToString());
    }
    public static DateTime GetDateTimeReciveGift()
    {
        return GetDateTime(StringConstants.KEY.DATE_RECIVE_GIFT_VIP, DateTime.MinValue);
    }

    public static void SetDateTimeReciveDailyGift(DateTime value)
    {
        PlayerPrefs.SetString(StringConstants.KEY.DATE_RECIVE_GIFT_DAILY, value.ToBinary().ToString());
    }
    public static DateTime GetDateTimeReciveDailyGift()
    {
        return GetDateTime(StringConstants.KEY.DATE_RECIVE_GIFT_DAILY, DateTime.MinValue);
    }

    public static string OriginalPathForId(string pictureId)
    {
        return Path.Combine(ORIGINAL_PATH, pictureId + ".png");
    }

    public static string CurrentPathForId(string pictureId)
    {
        return Path.Combine(CURRENT_PATH, pictureId + ".png");
    }
    public static string HistoryPathForId(string pictureId)
    {
        return Path.Combine(HISTORY_PATH, pictureId + ".bin");
    }
    public static DateTime GetPictureCreatedAt(string pictureId)
    {
        return GetDateTime(StringConstants.KEY.PREF_LOCAL_PICTURE_CREATED_AT_PREFIX + pictureId, zeroTime);
    }

    public static void UpdateLocalPictureCreatedAt(string pictureId)
    {
        SetDateTime(StringConstants.KEY.PREF_LOCAL_PICTURE_CREATED_AT_PREFIX + pictureId, UnbiasedTime.Instance.Now());
    }
    public static int ItemStar
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.ITEM_STAR, 3);
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.ITEM_STAR, value);
        }
    }
    public static int ItemPen
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.ITEM_PEN, 2);
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.ITEM_PEN, value);
        }
    }
    public static int GetTotalTimeItem(string item)
    {
        return PlayerPrefs.GetInt("time_item_" + item, 0);
    }
    public static void SetTotalTimeItem(string item, int value)
    {
        PlayerPrefs.SetInt("time_item_" + item, value);
    }
    public static int ItemFind
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.ITEM_FIND, 10);
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.ITEM_FIND, value);
        }
    }
    public static bool isShowHeadPhone
    {
        get
        {
            return PlayerPrefs.GetInt("head_phone", 0) == 0;
        }
        set
        {
            PlayerPrefs.SetInt("head_phone", value ? 0 : 1);
        }
    }
    public static string UserName
    {
        get
        {
            return PlayerPrefs.GetString("user_name", "Perrty");
        }
        set
        {
            PlayerPrefs.SetString("user_name", value);
        }
    }
    public static int ItemBomb
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.ITEM_BOMB, 3);
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.ITEM_BOMB, value);
        }
    }
    public static PictureInformation GetPictureInforById(string id)
    {
        foreach (CategoryInfo category in GameController.Instance.dataPic.CategoryInfos.Values)
        {
            foreach (PictureInformation pic in category.PictureInfos)
            {
                if (pic.Id == id)
                {
                    return pic;
                }
            }
        }
        return null;
    }
    public static PictureInformation[] GetAreaById(string idArea)
    {
        foreach (var data in GameController.Instance.dataAreas.cateItems)
        {
            foreach (CategoryInfo category in data.dataPic.CategoryInfos.Values)
            {
                if (category.displayName == idArea)
                {
                    return category.PictureInfos.ToArray();
                }
            }
        }
        return null;
    }
    public static CategoryInfo GetCateById(string idArea)
    {
        foreach (var data in GameController.Instance.dataAreas.cateItems)
        {
            foreach (CategoryInfo category in data.dataPic.CategoryInfos.Values)
            {
                if (category.displayName == idArea)
                {
                    return category;
                }
            }
        }
        return null;
    }
    public static void SetCurrentPicInAreas(int idAreas, int value)
    {
        if (value == totalPicInAreas && !picChoice.SinglePicNoel)
        {
            SetUnlockAreas(idAreas);
            CompleteArea = true;
        }
        PlayerPrefs.SetInt(StringConstants.KEY.CURRENT_PIC_IN_AREAS + idAreas, value);
    }
    public static int GetCurrentPicInAreas(int idAreas)
    {
        return PlayerPrefs.GetInt(StringConstants.KEY.CURRENT_PIC_IN_AREAS + idAreas);
    }
    public static string PicPainting
    {
        get
        {
            return PlayerPrefs.GetString(StringConstants.KEY.PIC_PROGRESS, "");
        }
        set
        {
            PlayerPrefs.SetString(StringConstants.KEY.PIC_PROGRESS, value);
        }
    }
    public static void SetReciveGift(int id, bool value)
    {
        PlayerPrefs.SetInt(StringConstants.KEY.RECIVE_GIFT + id, value ? 1 : 0);
    }
    public static bool GetReciveGift(int id)
    {
        return PlayerPrefs.GetInt(StringConstants.KEY.RECIVE_GIFT + id) == 1;
    }
    public static void SetReciveGiftInGame(string id, int value)
    {
        PlayerPrefs.SetInt(StringConstants.KEY.RECIVE_GIFT_IN_GAME + id, value);
    }
    public static int GetReciveGiftInGame(string id)
    {
        return PlayerPrefs.GetInt(StringConstants.KEY.RECIVE_GIFT_IN_GAME + id);
    }

    public static int IdPicCreate
    {
        get
        {
            return PlayerPrefs.GetInt(StringConstants.KEY.ID_CREATE_PIC, -10000);
        }
        set
        {
            PlayerPrefs.SetInt(StringConstants.KEY.ID_CREATE_PIC, value);
        }
    }

    public static int NumberDailyQuest
    {
        get
        {
            return PlayerPrefs.GetInt("daily_number_quest", -1);
        }
        set
        {
            PlayerPrefs.SetInt("daily_number_quest", value);
        }
    }
    public static int NumberDailyReturnApp
    {
        get
        {
            return PlayerPrefs.GetInt("daily_number_return_app", 1);
        }
        set
        {
            PlayerPrefs.SetInt("daily_number_return_app", value);
        }
    }
    public static void SetDateTimeTopBanner(DateTime value)
    {
        PlayerPrefs.SetString(StringConstants.KEY.DATE_TOPBANNER, value.ToBinary().ToString());
    }
    public static DateTime GetDateTimeTopBanner()
    {
        return GetDateTime(StringConstants.KEY.DATE_TOPBANNER, DateTime.MinValue);
    }
    public static void SetDateTimeDailyQuest(DateTime value)
    {
        PlayerPrefs.SetString(StringConstants.KEY.DATE_DAILY_QUEST, value.ToBinary().ToString());
    }
    public static DateTime GetDateTimeDailyQuest()
    {
        return GetDateTime(StringConstants.KEY.DATE_DAILY_QUEST, DateTime.MinValue);
    }
    public static bool NewUpdate
    {
        get
        {
            return PlayerPrefs.GetInt("new_update") == 0;
        }
        set
        {
            PlayerPrefs.SetInt("new_update", value ? 0 : 1);
        }
    }

    public static EventFarmBuilding_ProgressData EventFarmBuilding_ProgressData
    {
        get
        {
            EventFarmBuilding_ProgressData defaultData = new EventFarmBuilding_ProgressData();
            defaultData.zone = 0;
            defaultData.currentPic = 0;
            var data = PlayerPrefs.GetString(StringHelper.EVENT_FARM_BUILDING_PROGRESS_DATA, JsonUtility.ToJson(defaultData));
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return JsonUtility.FromJson<EventFarmBuilding_ProgressData>(data);
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.EVENT_FARM_BUILDING_PROGRESS_DATA, JsonUtility.ToJson(value));
            PlayerPrefs.Save();
        }
    }

    public static bool EventFarmRewardClaimable
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.EVENT_FARM_REWARD_CLAIMABLE, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.EVENT_FARM_REWARD_CLAIMABLE, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool EventFarmZone1Claimed
    {
        get
        {
            return PlayerPrefs.GetInt("EVENT_FARM_ZONE_1_CLAIMED", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("EVENT_FARM_ZONE_1_CLAIMED", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool EventFarmZone2Claimed
    {
        get
        {
            return PlayerPrefs.GetInt("EVENT_FARM_ZONE_2_CLAIMED", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("EVENT_FARM_ZONE_2_CLAIMED", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool EventFarmZone3Claimed
    {
        get
        {
            return PlayerPrefs.GetInt("EVENT_FARM_ZONE_3_CLAIMED", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("EVENT_FARM_ZONE_3_CLAIMED", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool EventFarmZone4Claimed
    {
        get
        {
            return PlayerPrefs.GetInt("EVENT_FARM_ZONE_4_CLAIMED", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("EVENT_FARM_ZONE_4_CLAIMED", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool EventFarmZone5Claimed
    {
        get
        {
            return PlayerPrefs.GetInt("EVENT_FARM_ZONE_5_CLAIMED", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("EVENT_FARM_ZONE_5_CLAIMED", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool EventFarmTutorial_1_Done
    {
        get
        {
            return PlayerPrefs.GetInt("EVENT_FARM_TUTORIAL_1_DONE", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("EVENT_FARM_TUTORIAL_1_DONE", value ? 1 : 0);
        }
    }

    public static bool EventFarmTutorial_2_Done
    {
        get
        {
            return PlayerPrefs.GetInt("EVENT_FARM_TUTORIAL_2_DONE", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("EVENT_FARM_TUTORIAL_2_DONE", value ? 1 : 0);
        }
    }

    public static float FarmTreasureFillAmount
    {
        get
        {
            return PlayerPrefs.GetFloat("Farm_Treasure_Fill_Amount", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("Farm_Treasure_Fill_Amount", value);
        }
    }
    
    public static bool DisableHandInFarm
    {
        get
        {
            return PlayerPrefs.GetInt("DisableHandInFarm", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("DisableHandInFarm", value ? 1 : 0);
        }
    }

    /*public static EventFarmBuilding_ZoneClaimedData EventFarmBuilding_ZoneClaimedData
    {
        get
        {
            EventFarmBuilding_ZoneClaimedData defaultData = new EventFarmBuilding_ZoneClaimedData();
            defaultData.zone1Claimed = 0;
            defaultData.zone2Claimed = 0;
            defaultData.zone3Claimed = 0;
            defaultData.zone4Claimed = 0;
            defaultData.zone5Claimed = 0;
            
            var data = PlayerPrefs.GetString(StringHelper.EVENT_FARM_ZONE_CLAIMED_DATA, JsonUtility.ToJson(defaultData));
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            return JsonUtility.FromJson<EventFarmBuilding_ZoneClaimedData>(data);
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.EVENT_FARM_ZONE_CLAIMED_DATA, JsonUtility.ToJson(value));
            PlayerPrefs.Save();
        }
    }*/
}
public enum Status
{
    Lock = 0,
    UnLock = 1,
    Complete = 2
}

public enum TypeQuest
{
    FinishPic,
    SavePic,
    SaveArea,
    UsePicCamera,
    UseItem,
    UseItemBomb,
    UseItemStar,
    UseItemFind,
    PaintedPixel,
    UnlockPic,
    FinishArea,
    DailyWorking,
    Artist,
    Pixels,
    BoomPainter,
    PixelFinder,
    StarPainter,
    PenPainter,
    Creator,
    Food,
    Fantasy,
    Popular,
    Animal,
    Cute,
    Festival,
    Fashion,
    Unicorn
}

