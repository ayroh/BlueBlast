using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : Singleton<AdManager>
{
    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize(initStatus => { });
    }

}
