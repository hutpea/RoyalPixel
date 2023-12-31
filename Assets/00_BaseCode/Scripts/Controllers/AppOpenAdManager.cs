// Copyright 2021 Google LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class AppOpenAdManager
{
#if UNITY_ANDROID
    private const string AD_UNIT_ID = "ca-app-pub-8467610367562059/6622029302";
#elif UNITY_IOS
    private const string AD_UNIT_ID = "ca-app-pub-8467610367562059/9089260604";
#else
    private const string AD_UNIT_ID = "unexpected_platform";
#endif

    private static AppOpenAdManager instance;

    private AppOpenAd ad;

    private bool isShowingAd = false;

    // COMPLETE: Add loadTime field
    private DateTime loadTime;

    public static AppOpenAdManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AppOpenAdManager();
            }

            return instance;
        }
    }

    private bool IsAdAvailable
    {
        get
        {
            // COMPLETE: Consider ad expiration
            return ad != null && (System.DateTime.UtcNow - loadTime).TotalHours < 4;
        }
    }

    public void LoadAd(UnityAction actionLoadDone = null)
    {
        var request = new AdRequest.Builder().Build();

        // Load an app open ad for portrait orientation
        AppOpenAd.LoadAd(AD_UNIT_ID, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }

            // App open ad is loaded
            ad = appOpenAd;
            Debug.Log("App open ad loaded");

            // COMPLETE: Keep track of time when the ad is loaded.
            loadTime = DateTime.UtcNow;

            actionLoadDone?.Invoke();
        }));
    }

    public void ShowAdIfAvailable()
    {
        if (UseProfile.IsRemoveAds || UseProfile.IsVip)
            return;
        if (!IsAdAvailable || isShowingAd)
        {
            LoadAd();
            return;
        }

        ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        ad.OnPaidEvent += HandlePaidEvent;

        ad.Show();
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        isShowingAd = false;

        LoadAd();
        //GameController.Instance.admobAds.ShowBanner();
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
        // GameController.Instance.admobAds.DestroyBanner();
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
            args.AdValue.CurrencyCode, args.AdValue.Value);
    }
}