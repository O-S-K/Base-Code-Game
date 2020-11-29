using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width;
        int h = Screen.height;

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = (h * 2) / 75;
        style.normal.textColor = Color.white;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} FPS)", msec, fps);

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        GUI.Label(rect, text, style);
    }
}
