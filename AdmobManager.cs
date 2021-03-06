using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;

public class AdmobManager : MonoBehaviour
{
    [Header("App ID")]
    public string appID;

    [Header("Remove ADs")]
    public bool noAds = false;
    // gecis
    [Header("Insterstitial Ad")]
    public string interstitialAdId;
    public bool isInterstitialTest = true;
    private readonly string interstitialTestId = "ca-app-pub-3940256099942544/1033173712";
    string intestitialId;
    private InterstitialAd interstitial;
    public bool isShowInterstitial = true;


    [Header("Banner Ad")]
    public string bannerAdId;
    public bool isBannerTest = true;
    private string bannerTestId = "ca-app-pub-3940256099942544/6300978111";
    string bannerId;
    private BannerView bannerView;
    public bool isShowBanner = true;


    private int adCounter = 0;

    private static bool isAdObjectCreated = false;

    void Awake()
    {
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
    }

    void Start()
    {

        MobileAds.Initialize(initStatus => { });
        bannerId = GetBannerID();
        intestitialId = GetInterstitialID();
        if (!noAds)
        {
            if (isShowBanner)
            {
                ShowBannerAd();
            }

            if (isShowInterstitial)
            {
                SetupInterstitialAd();
            }
        }
    }

    private string GetBannerID()
    {
        return isBannerTest ? bannerTestId : bannerAdId;
    }
    private string GetInterstitialID()
    {
        return isInterstitialTest ? interstitialTestId : interstitialAdId;
    }
    private void ShowBannerAd()
    {
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Top);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
        bannerView.Show();
    }
    private void SetupInterstitialAd()
    {
        interstitial = new InterstitialAd(intestitialId);
        // Called when an ad request has successfully loaded.
        //interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        //interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        //interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        interstitial.OnAdClosed += OnAddCloseFunction;
        // Called when the ad click caused the user to leave the application.
        //interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        LoadInterstitialAd();
    }

    public void OnAddCloseFunction(object sender, EventArgs args)
    {
        SetupInterstitialAd();
        //interstitial = new InterstitialAd(intestitialId);
        //interstitial.OnAdClosed += OnAddCloseFunction;
        //LoadInterstitialAd();
    }

    private void LoadInterstitialAd()
    {
        AdRequest req = new AdRequest.Builder().Build();
        //AdRequest req = new AdRequest.Builder().AddTestDevice(testdeviceID).TagForChildDirectedTreatment(true).Build();
        //AdRequest.Builder().addTestDevice("72E3A8B57A31BC1DBC8DBB46149BDCED").build();
        interstitial.LoadAd(req);
    }

    public void ShowInterstitialAd()
    {
        adCounter++;
        if (adCounter % 4 == 1 )
        {
            StartCoroutine(DelayForLoad());
        }
    }
    public void DirectShowInterstitialAd()
    {
        StartCoroutine(DelayForLoad());
    }
    public IEnumerator DelayForLoad()
    {
        while (!interstitial.IsLoaded())
        {
            yield return new WaitForSeconds(.2f);
        }
        interstitial.Show();
    }
}
