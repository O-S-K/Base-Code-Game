/*
So the resolution is pixels (ex: 1080x1920) 
*/

using UnityEngine;
using System.Collections;
using System.IO;

public class TakeScreenShot : MonoBehaviour
{
    public KeyCode keyCodeTakeScreenShot = KeyCode.Space;

    // defaul = 1
    public float startX;
    public float startY;

    // defaul = 1
    public int valueX;
    public int valueY;

    bool isProcessing;

    private void Update()
    {
        if (Input.GetKeyDown(keyCodeTakeScreenShot))
        {
            StartCoroutine(CaptureScreenshot());
        }
    }

    public void ShareScreenshot()
    {
        if (!isProcessing)
            StartCoroutine(CaptureScreenshot());
    }

    public IEnumerator CaptureScreenshot()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();

        //var screenTexture = new Texture2D(Screen.width * valueX / 10000, Screen.height * valueY / 10000, TextureFormat.RGB24, true);
        var screenTexture = new Texture2D(Screen.width, Screen.height);

        // put buffer into texture
        //screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);

        //create a Rect object as per your needs.
        //screenTexture.ReadPixels(new Rect(Screen.width * startX, (Screen.height * startY), Screen.width * valueX / 10000, Screen.height * valueY / 10000), 0, 0);
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO

        //byte[] dataToSave = Resources.Load<TextAsset>("everton").bytes;
        byte[] dataToSave = screenTexture.EncodeToPNG();

        string path = Application.dataPath + "/ScreenShot";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string destination = Path.Combine(path, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        Debug.Log("Path " + path);
        File.WriteAllBytes(destination, dataToSave);


        if (!Application.isEditor)
        {
            // block to open the file and share it ------------START
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Download: " + GetStoreLink());
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Name game");
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            // option one WITHOUT chooser:
            currentActivity.Call("startActivity", intentObject);

            // block to open the file and share it ------------END

        }
        isProcessing = false;
    }
}
