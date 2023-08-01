using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringHelper
{
    public const string ONOFF_SOUND = "ONOFF_SOUND";
    public const string ONOFF_MUSIC = "ONOFF_MUSIC";
    public const string ONOFF_VIBRATION = "ONOFF_VIBRATION";
    public const string FIRST_TIME_INSTALL = "FIRST_TIME_INSTALL";

    public const string VERSION_FIRST_INSTALL = "VERSION_FIRST_INSTALL";
    public const string REMOVE_ADS = "REMOVE_ADS";
    public const string CURRENT_LEVEL = "CURRENT_LEVEL";
    public const string CURRENT_LEVEL_PLAY = "CURRENT_LEVEL_PLAY";
    public const string PATH_CONFIG_LEVEL = "Levels/Level_{0}";
    public const string PATH_CONFIG_LEVEL_TEST = "LevelsTest/Level_{0}";

    public const string SALE_IAP = "_sale";

    public const string RETENTION_D = "retent_type";
    public const string DAYS_PLAYED = "days_played";
    public const string PAYING_TYPE = "retent_type";
    public const string LEVEL = "level";

    public const string LAST_TIME_OPEN_GAME = "LAST_TIME_OPEN_GAME";
    public const string FIRST_TIME_OPEN_GAME = "FIRST_TIME_OPEN_GAME";

    public const string CAN_SHOW_RATE = "CAN_SHOW_RATE";

    public const string IS_TUTED_RETURN = "IS_TUTED_RETURN";
    public const string CURRENT_NUM_RETURN = "CURRENT_NUM_RETURN";
    public const string CURRENT_NUM_ADD_STAND = "CURRENT_NUM_ADD_STAND";
    public const string CURRENT_NUM_REMOVE_BOMB = "CURRENT_NUM_REMOVE_BOMB";
    public const string CURRENT_NUM_REMOVE_CAGE = "CURRENT_NUM_REMOVE_CAGE";
    public const string CURRENT_NUM_REMOVE_EGG = "CURRENT_NUM_REMOVE_EGG";
    public const string CURRENT_NUM_REMOVE_SLEEP = "CURRENT_NUM_REMOVE_SLEEP";
    public const string CURRENT_NUM_REMOVE_JAIL = "CURRENT_NUM_REMOVE_JAIL";


    public const string IS_TUTED_BUY_STAND = "IS_TUTED_BUY_STAND";
    public const string ACCUMULATION_REWARD = "ACCUMULATION_REWARD";
    public const string CURRENT_BIRD_SKIN = "CURRENT_BIRD_SKIN";
    public const string CURRENT_BRANCH_SKIN = "CURRENT_BRANCH_SKIN";
    public const string CURRENT_THEME = "CURRENT_THEME";
    public const string OWNED_BIRD_SKIN = "OWNED_BIRD_SKIN";
    public const string OWNED_BRANCH_SKIN = "OWNED_BRANCH_SKIN";
    public const string OWNED_THEME = "OWNED_THEME";
    public const string RANDOM_BIRD_SKIN_IN_SHOP = "RANDOM_BIRD_SKIN_IN_SHOP";
    public const string RANDOM_BRANCH_IN_SHOP = "RANDOM_BRANCH_IN_SHOP";
    public const string RANDOM_THEME_IN_SHOP = "RANDOM_THEME_IN_SHOP";

    public const string RANDOM_BIRD_SKIN_SALE_WEEKEND_1 = "RANDOM_BIRD_SKIN_SALE_WEEKEND_1";

    public const string CURRENT_RANDOM_BIRD_SKIN = "CURRENT_RANDOM_BIRD_SKIN";
    public const string CURRENT_RANDOM_BRANCH_SKIN = "CURRENT_RANDOM_BRANCH_SKIN";
    public const string CURRENT_RANDOM_THEME = "CURRENT_RANDOM_THEME";


    public const string NUM_SHOWED_ACCUMULATION_REWARD_RANDOM = "NUM_SHOWED_ACCUMULATION_REWARD_RANDOM";

    public const string NUMBER_OF_ADS_IN_DAY = "NUMBER_OF_ADS_IN_DAY";
    public const string NUMBER_OF_ADS_IN_PLAY = "NUMBER_OF_ADS_IN_PLAY";

    public const string IS_PACK_PURCHASED_ = "IS_PACK_PURCHASED_";
    public const string IS_PACK_ACTIVATED_ = "IS_PACK_ACTIVATED_";
    public const string LAST_TIME_PACK_ACTIVE_ = "LAST_TIME_PACK_ACTIVE_";
    public const string LAST_TIME_PACK_SHOW_HOME_ = "LAST_TIME_PACK_SHOW_HOME_";
    public const string STARTER_PACK_IS_COMPLETED = "STARTER_PACK_IS_COMPLETED";

    public const string LAST_TIME_RESET_SALE_PACK_SHOP = "LAST_TIME_RESET_SALE_PACK_SHOP";



    public const string LAST_TIME_ONLINE = "LAST_TIME_ONLINE";
    public const string NEW_USER = "new_user";
    public const string CURRENT_ID_MINI_GAME = "current_id_mini_game";

    public const string IS_TRACKED_PREMISSION = "is_tracked_premission";
    public const string IS_ACCEPT_TRACKED_PREMISSION = "is_accept_tracked_premission";

    public const string EVENT_FARM_BUILDING_PROGRESS_DATA = "EVENT_FARM_BUILDING_PROGRESS_DATA";
    public const string EVENT_FARM_REWARD_CLAIMABLE = "EVENT_FARM_REWARD_CLAIMABLE";
    public const string EVENT_FARM_ZONE_CLAIMED_DATA = "EVENT_FARM_ZONE_CLAIMED_DATA";
}

public class PathPrefabs
{
    public const string POPUP_REWARD_BASE = "UI/Popups/PopupRewardBase";
    public const string CONFIRM_POPUP = "UI/Popups/ConfirmBox";
    public const string WAITING_BOX = "UI/Popups/WaitingBox";
    public const string WIN_BOX = "UI/Popups/WinBox";
    public const string REWARD_IAP_BOX = "UI/Popups/RewardIAPBox";
    public const string SHOP_BOX = "UI/ShopBox";
    public const string RATE_GAME_BOX = "UI/Popups/RateGameBox";
    public const string SETTING_BOX = "UI/Popups/SettingBox";
    public const string LOSE_BOX = "UI/Popups/LoseBox";
    public const string REWARD_CONGRATULATION_BOX = "UI/Popups/RewardCongratulationBox";
    public const string SHOP_GAME_BOX = "UI/Popups/ShopBox";
    public const string CONGRATULATION_BOX = "UI/Popups/CongratulationBox";
    public const string POPUP_SUGGEST_ITEM_BOMB = "UI/Popups/PopupSuggestItemBomb";
    public const string STARTER_PACK_BOX = "UI/Popups/PackBoxes/StarterPackBox";
    public const string THREE_SKIN_BIRD_PACK_BOX = "UI/Popups/PackBoxes/ThreeSkinBirdPackBox";
    public const string BRANCH_AND_THEME_PACK_BOX = "UI/Popups/PackBoxes/BranchAndThemePackBox";
    public const string POPUP_SUGGEST_ITEM_WANDS = "UI/Popups/PopupSuggestItemWand";
    public const string POPUP_NO_INTERNET = "UI/Popups/PopupInternet";
    public const string BIG_REMOVE_ADS_PACK_BOX = "UI/Popups/PackBoxes/BigRemoveAdsPackBox";
    public const string SALE_WEEKEND_1_PACK_BOX = "UI/Popups/PackBoxes/SaleWeekend1PackBox";
    public const string MINI_GAME_CONNECT_BIRD_BOX = "UI/Popups/ConnectBirdMGBox";
    public const string CONNECT_BIRD_MINI_GAME_SHOP_BOX = "UI/Popups/ConnectBirdMiniGameShop";
    public const string REWARD_CONNECT_BIRD_MN_BOX = "UI/Popups/RewardConnectBirdMNBox";
    public const string POPUP_VIP = "UI/PopupVipSub";
    public const string POPUP_CLAIM_VIP = "UI/PopupClaimVip";
    public const string PANEL_CAMERA = "UI/PanelCamera";
    public const string POPUP_PERMISSION = "UI/PopupPermission";
    public const string POPUP_DAILY_REWARD = "UI/PopupDailyReward";
    public const string POPUP_GET_MORE_BOMB = "UI/Popups/PopupSuggestGetMoreBomb";
    public const string POPUP_GET_MORE_WAND = "UI/Popups/PopupSuggestGetMoreWand";
    public const string POPUP_DAILY_QUEST = "UI/PopupDailyQuest";
    public const string NOTI_CLAIM_DAILY_QUEST = "UI/NotiClaimQuestDaily";
    public const string POPUP_BACK_HOME = "UI/PopupBackHome";
    public const string POPUP_REMOVE_LOGO = "UI/PopupShowRemoveLogo";
    public const string POPUP_EVENT_NOEL = "UI/PopupEventNoel";
    public const string POPUP_NOTI_EVENT = "UI/PopupNotiEventNoel";
    public const string TRACKING_BOX = "UI/TrackingBox";
    public const string POPUP_TUT_PEN = "UI/PopupTutorial";
    public const string LUCKY_SPIN = "UI/LuckySpin";
    public const string POPUP_REWARD_SPIN = "UI/PopupRewardSpin";
    public const string POPUP_PACK_SALE = "UI/PopupPackSale";
    public const string POPUP_DRAW_PIXEL = "UI/PopupSelectCanvasDrawPixel";
    public const string POPUP_CONTINUE_DRAW = "UI/PopupContinuePicture";
    public const string PANEL_SHOP = "UI/PanelShop";
    public const string POPUP_RENAME = "UI/PopupRename";
    public const string POPUP_EVENT_JIGSAW = "UI/PopupEventJigsaw";
    public const string POPUP_EVENT_FARMBUILDING = "UI/PopupEventFarmBuilding";
    public const string POPUP_PIG_GEM = "UI/PopupPigGem";
    public const string POPUP_SUGGEST_EVENT = "UI/Popups/PopupSuggestEvent";
    public const string POPUP_SUGGEST_FARM_EVENT = "UI/Popups/PopupSuggestFarmEvent";
    public const string POPUP_LICENSE_BOX = "UI/Popups/LicenseBox";
    public const string POPUP_GAMETIMEOUT = "UI/Popups/PopupGameTimeout";
    public const string POPUP_REWARD_FARM = "UI/PopupRewardFarm";
    public const string POPUP_COMPLETELEVEL = "UI/PopupComplete";
    public const string POPUP_HEAD_PHONE = "UI/PopupHeadPhone";
    public const string POPUP_DELAY_INTER = "UI/PopupDelayInter";
    public const string POPUP_REMOVE_ADS_PLUS = "UI/PopupRemoveAdsPlus";

}

public class SceneName
{
    public const string LOADING_SCENE = "Loading";
    public const string HOME_SCENE = "Home";
    public const string GAME_PLAY = "GamePlay";
}

public class AudioName
{
    public const string bgMainHome = "Music_BG_MainHome";
    public const string bgGamePlay = "Music_BG_GamePlay";

    //Ingame music
    public const string winMusic = "winMusic";
    public const string spawnerPlayerMusic = "spawnerPlayer";

    //Action Player music
    public const string jumpMusic = "jump";
    public const string jumpEndMusic = "jumpEnd";
    public const string swapMusic = "swap";
    public const string pushRockMusic = "pushRock";
    public const string dieMusic = "die";
    public const string reviveMusic = "revive";
    public const string flyMusic = "fly";

    //Collect music
    public const string collectCoinMusic = "collectCoin";
    public const string collectKeyMusic = "collectKey";
    public const string collectItemSound = "collectItem";

    //Level music
    public const string jumpOnWaterMusic = "jumpOnWater";
    public const string collisionDoorMusic = "collisionDoor";
    public const string doorOpenMusic = "doorOpen";
    public const string doorCloseMusic = "doorClose";
    public const string springMusic = "spring";
    public const string touchSwitchMusic = "touchSwitch";
    public const string bridgeMoveMusic = "bridgeMove";
    public const string bridgeMoveEndMusic = "bridgeMoveEnd";
    public const string iceDropFall = "rock1";
    public const string iceDropExplosion = "bigrock";
    public const string activeDiamond = "crystalactive";
    public const string releaseDiamond = "crystalrelease";
    //UI Music
    public const string buttonClick = "buttonClick";
}

public class KeyPref
{
    public const string SERVER_INDEX = "SERVER_INDEX";

}
public class PackIAP
{
    public const string SALE = "sale";
    public const string REMOVE_ADS = "remove_ads";
    public const string SUBSCRIPTION = "subscription";
}
public static class CategoryConst
{
    public const int POPULAR = 10002;
    public const int FASHION = 10006;
    public const int FANTASY = 10005;
    public const int CUTE = 10007;
    public const int FESTIVAL = 10003;
    public const int FOOD = 10001;
    public const int UNICORN = 10000;
    public const int ANIMAL = 10004;
    public const int DAILY = 10008;
    public const int FARM = 20001;

}
public class FirebaseConfig
{

    public const string DELAY_SHOW_INITSTIALL = "delay_show_initi_ads";//Thời gian giữa 2 lần show inital 30
    public const string LEVEL_START_SHOW_INITSTIALL = "level_start_show_initstiall";//Level bắt đầu show initial//3
    public const string SHOW_INTER_IN_GAME = "show_inter_in_game";
    public const string NUMBER_PIC_SHOW_NATIVE_ADS = "number_pic_show_native_ads";

    public const string LEVEL_START_SHOW_RATE = "level_start_show_rate";//Level bắt đầu show popuprate

    public const string DEFAULT_NUM_ADD_BRANCH = "default_num_add_branch";//2
    public const string DEFAULT_NUM_REMOVE_BOMB = "default_num_remove_bomb";//0
    public const string DEFAULT_NUM_REMOVE_EGG = "default_num_remove_egg";//0
    public const string DEFAULT_NUM_REMOVE_JAIL = "default_num_remove_jail";//0
    public const string DEFAULT_NUM_REMOVE_SLEEP = "default_num_remove_sleep";//0
    public const string DEFAULT_NUM_REMOVE_CAGE = "default_num_remove_cage";//0

    public const string DEFAULT_NUM_RETURN = "default_num_return";//2
    public const string NUM_RETURN_CLAIM_VIDEO_REWARD = "num_return_claim_video_reward";//3

    public const string LEVEL_START_TUT_RETURN = "level_start_tut_return";//4
    public const string LEVEL_START_TUT_BUY_STAND = "level_start_tut_buy_stand";//5

    public const string ON_OFF_REMOVE_ADS = "on_off_remove_ads_2";//5
    public const string MAX_LEVEL_SHOW_RATE = "max_level_show_rate";//30

    public const string TEST_LEVEL_CAGE_BOOM = "test_level_cage_boom";//30

    public const string ON_OFF_ACCUMULATION_REWARD_LEVEL_START = "on_off_accumulation_reward_level_start";//true
    public const string ACCUMULATION_REWARD_LEVEL_START = "accumulation_reward_level_start";//6
    public const string ACCUMULATION_REWARD_END_LEVEL = "accumulation_reward_end_level_{0}";//
    public const string ACCUMULATION_REWARD_TIME_SHOW_NEXT_BUTTON = "accumulation_reward_time_show_next_button";//1.5
    public const string ACCUMULATION_REWARD_END_LEVEL_RANDOM = "accumulation_reward_end_level_random";//10
    public const string MAX_TURN_ACCUMULATION_REWARD_END_LEVEL_RANDOM = "max_turn_accumulation_reward_end_level_random";//150

    public const string ON_OFF_SALE_INAPP = "on_off_sale_inapp";//true

    public const string LEVEL_UNLOCK_SALE_PACK = "level_unlock_sale_pack"; //11
    public const string LEVEL_UNLOCK_PREMIUM_PACK = "level_unlock_premium_pack"; //25
    public const string TIME_LIFE_STARTER_PACK = "time_life_starter_pack"; // 3DAY
    public const string TIME_LIFE_PREMIUM_PACK = "time_life_premium_pack"; // 2DAY
    public const string TIME_LIFE_SALE_PACK = "time_life_premium_pack"; // 1DAY
    public const string TIME_LIFE_BIG_REMOVE_ADS_PACK = "time_life_big_remove_ads_pack"; // 3h

    public const string NUMBER_OF_ADS_IN_DAY_TO_SHOW_PACK = "number_of_ads_in_day_to_show_pack"; //5ADS
    public const string NUMBER_OF_ADS_IN_PLAY_TO_SHOW_PACK = "number_of_ads_in_play_to_show_pack"; //3ADS
    public const string TIME_DELAY_SHOW_POPUP_SALE_PACK_ = "time_delay_show_popup_sale_pack_"; // 6H
    public const string TIME_DELAY_ACTIVE_SALE_PACK = "time_delay_active_sale_pack_"; // 6H
    public const string NUMBER_PAINTED_SHOW_INTER_IN_GAME = "number_painted_show_inter_in_game";
    public const string REVIEW_IAP_VERSION = "review_iap_version"; // 6H
    public const string TIME_EVENT = "time_event";
    public const string CAN_SHOW_POPUP = "can_show_popup";
    public const string TOTAL_PIXEL_SHOW_INTER_1 = "total_pixel_show_inter_1";
    public const string TOTAL_PIXEL_SHOW_INTER_2 = "total_pixel_show_inter_2";
}

