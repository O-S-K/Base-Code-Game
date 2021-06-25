using UnityEngine;
using System.Collections;
using OSK;

public class ChangeColorBackgroundByTime : Singleton<ChangeColorBackgroundByTime>
{
    [HideInInspector]
    public bool isDark;
    public Color[] listColorChange;
    int countColor = 0;
    float timeChangeColor = 10;

    void Start()
    {
        StartCoroutine(Update_Fade(Color.white, 1));
    }

    public void CheckComboChangeColor(bool _isComboPlus)
    {
        if (_isComboPlus)
        {
            isDark = true;
            countColor++;
            if (countColor >= 5)
                countColor = 5;

            switch (countColor)
            {
                case 1: StartCoroutine(Update_Fade(listColorChange[0], 0.1F)); break;
                case 2: StartCoroutine(Update_Fade(listColorChange[1], 0.1F)); break;
                case 3: StartCoroutine(Update_Fade(listColorChange[2], 0.1F)); break;
                case 4: StartCoroutine(Update_Fade(listColorChange[3], 0.1F)); break;
                case 5: StartCoroutine(Update_Fade(listColorChange[4], 0.1F)); break;
            }
        }
        else
        {
            isDark = false;
            countColor = 0;
            StartCoroutine(Update_Fade(Color.white, 1));
        }
    }

    IEnumerator Update_Fade(Color _color, float _intensity)
    {
        float timer = 0.0f;
        while (timer <= 1)
        {
            timer += Time.deltaTime;
            float lerp_Percentage = timer / (1 / timeChangeColor);
            Camera.main.backgroundColor = _color == Color.black ? new Color(0.1647059f, 0.1098039f, 0.254902f, 1) : new Color(0.2941177f, 0.6f, 0.937255f, 1);
            RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, _color, lerp_Percentage);
            RenderSettings.reflectionIntensity = Mathf.Lerp(RenderSettings.reflectionIntensity, _intensity, lerp_Percentage);
            yield return null;
        }
    }
}
