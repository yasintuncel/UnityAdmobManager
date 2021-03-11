using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void DelegateOnRewardAdLoaded();
public delegate void DelegateOnRewardAdClosed();
public delegate void DelegateOnRewardAdReward();

[Serializable]
public class AdPackage
{
    public string bannerId;
    public string interstitialId;
    public string rewardedId;
}

[Serializable]
public class AdPlatformPackage
{
    public string       appId;
    public AdPackage    adPackage;
}

public class AdmobManager : MonoBehaviour
{
    [Header("Android Ads Conf.")]
    [SerializeField]
    AdPlatformPackage androidAd;

    //
    [Header("iOS Ads Conf.")]
    [SerializeField]
    AdPlatformPackage iosAd;
    //
    AdPackage androidTestAds = new AdPackage {
        bannerId = "ca-app-pub-3940256099942544/6300978111",
        interstitialId = "ca-app-pub-3940256099942544/1033173712",
        rewardedId = "ca-app-pub-3940256099942544/5224354917"
    };
    //
    AdPackage iosTestAds = new AdPackage
    {
        bannerId = "ca-app-pub-3940256099942544/2934735716",
        interstitialId = "ca-app-pub-3940256099942544/4411468910",
        rewardedId = "ca-app-pub-3940256099942544/1712485313"
    };

    [Header("General Conf.")]
    public bool isShowBanner = true;
    public bool isBannerTest = true;
    [Space]
    public bool isShowInterstitial = true;
    public bool isInterstitialTest = true;
    [Space]
    public bool isShowRewarded = true;
    public bool isRewardedTest = true;
    //
    AdPlatformPackage currentPlatform;
    //
    BannerView bannerView;
    InterstitialAd interstitial;
    RewardBasedVideoAd rewardBasedVideo;
    //
    private int adCounter = 0;
    private static bool isAdObjectCreated = false;
    List<DelegateOnRewardAdLoaded> rewardOnLoadDelegates;
    List<DelegateOnRewardAdReward> rewardOnRewardDelegates;
    List<DelegateOnRewardAdClosed> rewardOnClosedDelegates;
    void Awake()
    {
        rewardOnLoadDelegates = new List<DelegateOnRewardAdLoaded>();
        rewardOnRewardDelegates = new List<DelegateOnRewardAdReward>();
        rewardOnClosedDelegates = new List<DelegateOnRewardAdClosed>();
        //
        if (!isAdObjectCreated) //if GameManagerexcistst is not true --> this action will happen.
        {
            isAdObjectCreated = true;
            DontDestroyOnLoad(transform.gameObject); /// taken from this Tutorial https://youtu.be/x9lguwc0Pyk
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        //
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                currentPlatform = androidAd;
                if (isBannerTest)
                    currentPlatform.adPackage.bannerId = androidTestAds.bannerId;
                if (isInterstitialTest)
                    currentPlatform.adPackage.interstitialId = androidTestAds.interstitialId;
                if (isRewardedTest)
                    currentPlatform.adPackage.rewardedId = androidTestAds.rewardedId;
                break;
            case RuntimePlatform.IPhonePlayer:
                currentPlatform = iosAd;
                if (isBannerTest)
                    currentPlatform.adPackage.bannerId = iosTestAds.bannerId;
                if (isInterstitialTest)
                    currentPlatform.adPackage.interstitialId = iosTestAds.interstitialId;
                if (isRewardedTest)
                    currentPlatform.adPackage.rewardedId = iosTestAds.rewardedId;
                break;
        }

    }
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        //
        SetupBannerAd();
        //
        SetupInterstitialAd();
        //
        SetupRewarded();
    }

    public void AddRewardedLoaded(DelegateOnRewardAdLoaded delegateLoaded)
    {
        rewardOnLoadDelegates.Add(delegateLoaded);
    }
    public void AddRewardedReward(DelegateOnRewardAdReward delegateLoaded)
    {
        rewardOnRewardDelegates.Add(delegateLoaded);
    }
    public void AddRewardedClosed(DelegateOnRewardAdClosed delegateLoaded)
    {
        rewardOnClosedDelegates.Add(delegateLoaded);
    }

    #region Banner Logics
    private void SetupBannerAd()
    {
        if (isShowBanner)
        {
            bannerView = new BannerView(currentPlatform.adPackage.bannerId, AdSize.Banner, AdPosition.Bottom);
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the banner with the request.
            bannerView.LoadAd(request);
            bannerView.Show();
        }
    }
    #endregion

    #region Interstitial Logics
    private void SetupInterstitialAd()
    {
        if (isShowInterstitial)
        {
            interstitial = new InterstitialAd(currentPlatform.adPackage.interstitialId);
            // Called when an ad request has successfully loaded.
            //interstitial.OnAdLoaded += HandleOnAdLoaded;
            // Called when an ad request failed to load.
            //interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            // Called when an ad is shown.
            //interstitial.OnAdOpening += HandleOnAdOpened;
            // Called when the ad is closed.
            interstitial.OnAdClosed += OnInterstitialAdClosed;
            // interstitial.OnAdLoaded += OnInterstitialAdLoaded;
            // Called when the ad click caused the user to leave the application.
            //interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
            LoadInterstitialAd();
        }
    }
    public void OnInterstitialAdClosed(object sender, EventArgs args)
    {
        SetupInterstitialAd();
        //interstitial = new InterstitialAd(intestitialId);
        //interstitial.OnAdClosed += OnAddCloseFunction;
        //LoadInterstitialAd();
    }
    public void LoadInterstitialAd()
    {
        AdRequest req = new AdRequest.Builder().Build();
        //AdRequest req = new AdRequest.Builder().AddTestDevice(testdeviceID).TagForChildDirectedTreatment(true).Build();
        //AdRequest.Builder().addTestDevice("72E3A8B57A31BC1DBC8DBB46149BDCED").build();
        interstitial.LoadAd(req);
    }
    public void ShowInterstitialAd(bool isDirectShow)
    {

        if (isDirectShow)
        {
            StartCoroutine(DelayForInterstitialLoad());
            //if (!interstitial.IsLoaded())
            //{
            //    interstitial.Show();
            //}
        }
        else
        {
            if (adCounter % 3 == 0)
            {
                StartCoroutine(DelayForInterstitialLoad());
                //if (!interstitial.IsLoaded())
                //{
                //    interstitial.Show();
                //}
            }
            adCounter++;
        }
    }
    public IEnumerator DelayForInterstitialLoad()
    {
        while (!interstitial.IsLoaded())
        {
            yield return new WaitForSeconds(.2f);
        }
        interstitial.Show();
    }
    #endregion

    #region Rewarded Logics
    private void SetupRewarded()
    {
        if (isShowRewarded)
        {
            rewardBasedVideo = RewardBasedVideoAd.Instance;

            AdRequest request = new AdRequest.Builder().Build();
            rewardBasedVideo.LoadAd(request, currentPlatform.adPackage.rewardedId);
            rewardBasedVideo.OnAdLoaded     += OnRewardedAdLoaded;
            rewardBasedVideo.OnAdRewarded   += OnRewarded;
            rewardBasedVideo.OnAdClosed     += OnRewardedAdClosed;
        }
    }

    private void OnRewarded(object sender, Reward e)
    {
        isShowRewarded = false;
        // hide button or add gold amount
        foreach (DelegateOnRewardAdReward rd in rewardOnRewardDelegates)
        {
            rd.Invoke();
        }
    }

    private void OnRewardedAdLoaded(object sender, EventArgs e)
    {
        // show button
        foreach(DelegateOnRewardAdLoaded rd in rewardOnLoadDelegates)
        {
            rd.Invoke();
        }
    }
    private void OnRewardedAdClosed(object sender, EventArgs e)
    {
        // hide button / re-load ads
        foreach (DelegateOnRewardAdClosed rd in rewardOnClosedDelegates)
        {
            rd.Invoke();
        }
        SetupRewarded();
    }
    public void ShowRewardedAdd()
    {
        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
        }
    }
    #endregion

}
