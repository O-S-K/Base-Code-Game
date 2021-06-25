using UnityEngine;
using UnityEngine.UI;


public class FPSDisplay : MonoBehaviour
{
    private Canvas fpsCanvas;
    private Text fpsText;


    private void Start()
    {
        int w = Screen.width;
        int h = Screen.height;

        // Init canvas
        fpsCanvas = gameObject.AddComponent<Canvas>();
        var tmp = gameObject.AddComponent<CanvasScaler>();
        tmp.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        tmp.referenceResolution = new Vector2(w, h);

        gameObject.AddComponent<GraphicRaycaster>();
        fpsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Init text
        GameObject temp = new GameObject();
        temp.transform.parent = transform;
        fpsText = temp.AddComponent<Text>();
        fpsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        fpsText.fontSize = (h * 2) / 65;
        fpsText.alignment = TextAnchor.UpperRight;
        fpsText.resizeTextForBestFit = true;
        fpsText.resizeTextMaxSize = 100;

        RectTransform rect = fpsText.GetComponent<RectTransform>();

        rect.sizeDelta = new Vector2(w, h / 25f);
        rect.localPosition = new Vector3(0, (h - rect.sizeDelta.y) / 2f, 0f);
    }


    private void Update()
    {
        int msec = (int)(Time.deltaTime * 1000f);
        int fps = (int)(1f / Time.deltaTime);
        fpsText.text = msec + " msec " + fps + " fps";
    }
}