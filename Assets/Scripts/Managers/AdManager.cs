using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;


public class AdManager : Singleton<AdManager>, IUnityAdsShowListener, IUnityAdsInitializationListener, IUnityAdsLoadListener
{
    private string gameId = "5523099";
    private bool testMode = true;

    private string bannerId = "Banner_Android";
    private string rewardedId = "Rewarded_Android";
    private string interstitialId = "Interstitial_Android";

    private IEnumerator bannerCoroutine;


    void Start()
    {
        if(!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            ShowBannerAd();
        if (Input.GetKeyDown(KeyCode.S))
            ShowInterstitialAd();
        if (Input.GetKeyDown(KeyCode.D))
            ShowRewardedAd();
    }


    #region Interstitial

    private void LoadInterstitialAd()
    {
        Advertisement.Load(interstitialId);
    }

    public void ShowInterstitialAd()
    {
        Advertisement.Show(interstitialId);
        GoalManager.instance.AddNumberOfMoves(5);
        LoadInterstitialAd();
    }

    #endregion



    #region Rewarded

    private void LoadRewardedAd()
    {
        Advertisement.Load(rewardedId);
    }

    public void ShowRewardedAd()
    {
        Advertisement.Show(rewardedId, this);
        GoalManager.instance.AddNumberOfMoves(10);
        LoadRewardedAd();
    }

    #endregion



    #region Banner

    private void LoadBannerAd()
    {
        if (!Advertisement.Banner.isLoaded)
            Advertisement.Banner.Load(bannerId);
    }

    public void ShowBannerAd()
    {
        if (bannerCoroutine != null)
            return;
        StartCoroutine(bannerCoroutine = BannerAd());
    }

    private IEnumerator BannerAd()
    {
        Advertisement.Banner.Show(bannerId);
        yield return new WaitForSeconds(4f);
        Advertisement.Banner.Hide();
        Advertisement.Banner.Load(bannerId);

        bannerCoroutine = null;
    }

    #endregion

    #region Show Listener

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
    }

    public void OnUnityAdsShowStart(string placementId)
    {
    }

    #endregion


    #region Initialize Listener

    public void OnInitializationComplete()
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        LoadBannerAd();
        LoadInterstitialAd();
        LoadRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
    }

    #endregion

}
