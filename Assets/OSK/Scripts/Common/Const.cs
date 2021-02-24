using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Const
{
    #if UNITY_IOS
    public const string APP_URL = "http://itunes.apple.com/app/";
#else
    public const string APP_URL = "https://play.google.com/store/apps/details?id=";
#endif

#if DEBUG_ADMOB

#if UNITY_EDITOR || UNITY_STANDALONE
    public const string ADMOB_APP_ID = "";
    public const string ADMOB_BANNER_ID = "";
    public const string ADMOB_VIDEO_ID = "";
    public const string ADMOB_CENTER_INTERSTITIAL_ID = "";
#elif UNITY_IOS
    public const string ADMOB_APP_ID = "";
    public const string ADMOB_BANNER_ID = "";
    public const string ADMOB_VIDEO_ID = "";
    public const string ADMOB_CENTER_INTERSTITIAL_ID = "";
#elif UNITY_ANDROID
    public const string ADMOB_APP_ID = "";
    public const string ADMOB_BANNER_ID = "x";
    public const string ADMOB_VIDEO_ID = "";
    public const string ADMOB_CENTER_INTERSTITIAL_ID = "x";
#endif

#else

#if UNITY_EDITOR || UNITY_STANDALONE
    public const string ADMOB_APP_ID = "";
    public const string ADMOB_BANNER_ID = "";
    public const string ADMOB_VIDEO_ID = "";
    public const string ADMOB_CENTER_INTERSTITIAL_ID = "";
#elif UNITY_IOS
    public const string ADMOB_APP_ID = "";
    public const string ADMOB_BANNER_ID = "";
    public const string ADMOB_VIDEO_ID = "";
    public const string ADMOB_CENTER_INTERSTITIAL_ID = "";
#elif UNITY_ANDROID
    public const string ADMOB_APP_ID = "";
    public const string ADMOB_BANNER_ID = "";
    public const string ADMOB_VIDEO_ID = "";
    public const string ADMOB_CENTER_INTERSTITIAL_ID = "";
#endif

#endif
    public const int NUM_TOTAL_STAGE = 100;
    public const int START_ADS = 3;
    public const float TIME_BETWEEN_ADS = 60;
}
