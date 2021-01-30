using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsController : MonoBehaviour
{
    [Header("Ads Banner")]
    public RectTransform bgbanner;
    private static bool isFirstLoad = true;
    private static bool isPauseGame = true;

    void Start() => InitAds();

    #region Ads
    void InitAds()
    {
#if UNITY_ANDROID
        ShowbannerAndroidOnTop();
#else
        ShowBanneriOSOnTop();
#endif
        // if (AdsHelper.isRemoveAds == 1)
        // {
        //     bgbanner.gameObject.SetActive(false);
        // }
        // else
        // {
        //     StartCoroutine(ShowBanner());
        // }
        if (isFirstLoad)
        {
            isFirstLoad = false;
        }
    }
    IEnumerator ShowBanner()
    {
        yield return new WaitForSeconds(1.0f);
#if !UNITY_EDITOR
        //AdsHelper.Instance.showBanner(0, 0);
#endif
    }

    void ShowbannerAndroidOnTop()
    {
        float safeYMin = Screen.height - Screen.safeArea.yMax;
        float hbn = 50.0f * Screen.dpi / 160.0f;
        //Debug.Log("mysdk: ymin=" + Screen.safeArea.yMin + ", ymax=" + Screen.safeArea.yMax + ", 
        //                  hs=" + Screen.height + ", 50dpi=" + hbn);
        safeYMin = (hbn + Screen.safeArea.yMin) / (float)Screen.height;
        bgbanner.anchorMin = new Vector2(0, 1 - safeYMin);
        bgbanner.anchorMax = new Vector2(1, 1);
        bgbanner.sizeDelta = new Vector2(0, 0);
        bgbanner.anchoredPosition = new Vector2(0, 0);
    }

    void ShowBanneriOSOnTop()
    {
        //#if UNITY_IOS
        float safeYMin = Screen.height - Screen.safeArea.yMax;
        float hbn = 50.0f * Screen.dpi / 160.0f;
        //Debug.Log("mysdk: ymin=" + Screen.safeArea.yMin + ", ymax=" + Screen.safeArea.yMax + ", 
        //                  hs=" + Screen.height + ", 50dpi=" + hbn);
        safeYMin = (hbn + safeYMin) / (float)Screen.height;
        bgbanner.anchorMin = new Vector2(0, 1 - safeYMin);
        bgbanner.anchorMax = new Vector2(1, 1);
        bgbanner.sizeDelta = new Vector2(0, 0);
        bgbanner.anchoredPosition = new Vector2(0, 0);
        //#endif
    }
    #endregion
}
