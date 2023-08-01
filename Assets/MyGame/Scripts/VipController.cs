using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.Purchasing.MiniJSON;
using UnityEngine.Purchasing.Security;
using EventDispatcher;
//using com.adjust.sdk;

public class VipController : IAPListener
{
    public static VipController Instance;
    public const string SUBS_VIP_MONTHLY_PRODUCT_ID = "royal.pixel.monthly1";
    //public const string IAP_VIP_ONE_TIME_PRODUCT_ID = "colorball_iap_vip_one_time";

    public const int VIP_DAILY_REWARD_LIVES = 5;
    public const int VIP_DAILY_REWARD_COIN = 1000;
    public const int VIP_DAILY_REWARD_GEM = 10;
    public DataGift giftVip;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        while (!CodelessIAPStoreListener.initializationComplete)
        {
            yield return null;
        }
        CheckValidVip();
    }

    public void CheckValidVip()
    {
        if (UseProfile.IsVip)
        {
            bool foundExpired = false;
            bool foundValid = false;

            Result isSubscriptionExpired = IsExpired(SUBS_VIP_MONTHLY_PRODUCT_ID);
            if (isSubscriptionExpired == Result.True)
            {
                foundExpired = true;
            }
            else if (isSubscriptionExpired == Result.False)
            {
                foundValid = true;
            }
            if (foundExpired && !foundValid)
            {
                UseProfile.IsVip = false;
            }
        }
        else
        {
            Product product = CodelessIAPStoreListener.Instance.GetProduct(SUBS_VIP_MONTHLY_PRODUCT_ID);
            if (product != null && product.hasReceipt)
            {
                ProcessVipPurchase(product);
            }
        }
    }

    public void ProcessVipPurchase(Product product)
    {
        if (!UseProfile.IsVip)
        {
#if UNITY_EDITOR
            bool validPurchase = true;
#else
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);
            var validationResult = validator.Validate(product.receipt);
            IPurchaseReceipt purchaseReceipt = null;
            bool validPurchase = false;
            //Debug.Log("Receipt is valid: " + product.transactionID);
            foreach (IPurchaseReceipt productReceipt in validationResult)
            {
                //Debug.Log("productId: " + productReceipt.productID);
                //Debug.Log("purchaseDate: " + productReceipt.purchaseDate);
                //Debug.Log("transactionId: " + productReceipt.transactionID);
#if UNITY_IOS
                AppleInAppPurchaseReceipt appleReceipt = productReceipt as AppleInAppPurchaseReceipt;
                //Debug.Log("original Transaction Id: " + appleReceipt.originalTransactionIdentifier);
                if ((product.definition.type == ProductType.Subscription && appleReceipt.subscriptionExpirationDate <= UnbiasedTime.Instance.Now())
                    || (appleReceipt.productType == (int)ProductType.NonConsumable && product.definition.type == ProductType.NonConsumable))
                {
                    purchaseReceipt = productReceipt;
                    validPurchase = true;
                    break;
                }
#else
                GooglePlayReceipt googlePlayReceipt = productReceipt as GooglePlayReceipt;                
                if (googlePlayReceipt.purchaseState == GooglePurchaseState.Purchased) {
                    purchaseReceipt = productReceipt;
                    validPurchase = true;
                    break;
                }    
#endif
        }
#endif

            if (validPurchase && IsExpired(product.definition.id) == Result.False && product == CodelessIAPStoreListener.Instance.GetProduct(SUBS_VIP_MONTHLY_PRODUCT_ID))
            {
                UseProfile.IsVip = true;
                //GameData.ClaimResourceItems(resourceItemColl.items);
                //GameAnalytics.LogEarnVirtualCurrency("shop_iap_" + product.definition.id, resourceItemColl.items);
                //GameData.ItemBomb += giftVip.gifts[0].itemBomb;
                //GameData.ItemStar += giftVip.gifts[0].itemStar;
                //GameData.ItemFind += giftVip.gifts[0].itemFind;
                //GameAnalytics.LogBuyIAP(product.definition.id);
                HomeController.instance.ActiveBtnNoAds();
                GameController.Instance.admobAds.DestroyBanner();
                StartCoroutine(Helper.StartAction(() => { PopupVip.Setup().Close(); }, 1));
                ClaimAllVip();
                this.PostEvent(EventID.BUY_VIP_SUB);
                if (product.definition.type == ProductType.Subscription)
                {
                    if (product.hasReceipt)
                    {
#if !UNITY_EDITOR
#if UNITY_IOS

                        //            AdjustAppStoreSubscription subscription = new AdjustAppStoreSubscription(
                        //product.metadata.localizedPrice.ToString(),
                        //product.metadata.isoCurrencyCode,
                        //product.transactionID,
                        //product.receipt);
                        //            subscription.transactionDate = ((DateTimeOffset)purchaseReceipt.purchaseDate).ToUnixTimeMilliseconds().ToString();
                        //            Adjust.trackAppStoreSubscription(subscription);
#else
                                        GooglePlayReceipt googleReceipt = purchaseReceipt as GooglePlayReceipt;
                                        var dict = Json.Deserialize(product.receipt) as Dictionary<string, object>;
                                        var googleJsonDict = Json.Deserialize((string)dict["Payload"]) as Dictionary<string, object>;
                                        string signature = (string)googleJsonDict["signature"];
                                        long transactionDate = ((DateTimeOffset)purchaseReceipt.purchaseDate).ToUnixTimeMilliseconds();
                            //            AdjustPlayStoreSubscription subscription = new AdjustPlayStoreSubscription(
                            //product.metadata.localizedPrice.ToString(),
                            //product.metadata.isoCurrencyCode,
                            //product.definition.id,
                            //product.transactionID,
                            //signature,
                            //googleReceipt.purchaseToken);
                                       // subscription.setPurchaseTime(transactionDate.ToString());
                                       // Adjust.trackPlayStoreSubscription(subscription);
#endif
#endif
                    }
                }
                else
                {
                    //GameAnalytics.LogInAppPurchase(product);
                }
            }
        }

    }

    private IEnumerator IEShowClaimVipPanel()
    {
        while (SceneManager.GetActiveScene().buildIndex == 0)
        {
            yield return new WaitForEndOfFrame();
        }
        PopupClaimVip.Setup().Show();
    }

    public Result IsExpired(string productId)
    {
        var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
        if (product != null && product.availableToPurchase && product.hasReceipt)
        {
            if (product.definition.type == ProductType.NonConsumable)
            {
                return Result.False;
            }
            IAppleExtensions m_AppleExtensions = CodelessIAPStoreListener.Instance.GetStoreExtensions<IAppleExtensions>();
            Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

            string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(product.definition.storeSpecificId)) ? null : introductory_info_dict[product.definition.storeSpecificId];
#if UNITY_EDITOR
            return Result.False;
#else
            SubscriptionManager p = new SubscriptionManager(product, intro_json);
            SubscriptionInfo info = p.getSubscriptionInfo();
            //Debug.Log("expired date = " + info.getExpireDate().ToLongDateString());
            return info.isExpired();
#endif
        }
        return Result.Unsupported;
    }

    private void ClaimAllVip()
    {
        PopupClaimVip.Setup().Show();
    }
}
