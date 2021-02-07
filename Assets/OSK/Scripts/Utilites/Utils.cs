using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public static class Utils
{
    #region Random
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    static System.Random random = new System.Random();
    public static double GetRandomNumber(double minimum, double maximum)
    {
        return random.NextDouble() * (maximum - minimum) + minimum;
    }
    public static float GetRandomNumber(float minimum, float maximum)
    {
        return (float)random.NextDouble() * (maximum - minimum) + minimum;
    }
    static int lastRandomNumber;
    public static int GenerateRandomNumber(int min, int max)
    {
        int result = Random.Range(min, max);

        if (result == lastRandomNumber)
        {
            return GenerateRandomNumber(min, max);
        }

        lastRandomNumber = result;
        return result;
    }
    public static string ConvertMoneyToString(long money)
    {
        if (money >= 1000000000)
        {
            return (money / 1000000000).ToString() + "B";
        }
        if (money >= 1000000)
        {
            return (money / 1000000).ToString() + "M";
        }
        if (money >= 1000)
        {
            return (money / 1000).ToString() + "K";
        }
        return money.ToString();
    }

    public static string FormatSeconds(float elapsed)
    {
        //#1
        int d = (int)(elapsed * 100.0f);
        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;
        int hundredths = d % 100;
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);

        //#2
        //timer += Time.deltaTime;
        //string hours = Mathf.Floor(timer / 3600).ToString("00");
        //string minutes = Mathf.Floor(timer / 60).ToString("00");
        //string seconds = (timer % 60).ToString("00");
        //txtTimeSuvival.text = string.Format("{0}:{1}:{2}", hours, minutes, seconds);
    }

    public static int GetSecondElalsed(DateTime prevDate)
    {
        DateTime now = DateTime.Now;
        TimeSpan timeDiffernce = now.Subtract(prevDate);
        return timeDiffernce.Seconds;
    }

    public static string CutString(string text, int MaxLength)
    {
        if (text == null) return "";
        string[] SplitText = text.Split(' ');
        if (SplitText == null) return "";
        string newText = "";
        foreach (string c_text in SplitText)
        {
            if (newText.Length + c_text.Length > MaxLength) return newText;
            newText = newText + " " + c_text;
        }

        return newText;
    }

    public static string ConvertIntToTimeHH_MM_SS(int duration)
    {
        string time = "";
        for (int i = 2; i >= 0; i--)
        {
            if (i < 2) time += ":";
            int detailTime = (int)Mathf.Pow(60, i);
            int t = duration / detailTime;
            duration = duration % detailTime;
            if (t > 9)
            {
                time += t.ToString();
                continue;
            }
            if (t > 0)
            {
                time += "0" + t.ToString();
                continue;
            }
            time += "00";
        }
        return time;
    }

    #region Set Bool
    public static void SetBool(string key, bool state)
    {
        PlayerPrefs.SetInt(key, state ? 1 : 0);
    }
    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key) == 1;
    }

    // public static bool isBool
    // {
    //   get{ return PlayerPrefs.GetInt("key") == 1;}
    //   set{ PlayerPrefs.SetInt("key", value ? 1 : 0);}
    // }
    
    #endregion

    #endregion
}
