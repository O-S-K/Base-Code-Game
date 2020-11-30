using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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

    private static int lastRandomNumber;
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
        if (money>=1000000000)
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

    public static int GetSecondElalsed(DateTime prevDate)
    {
        DateTime now = DateTime.Now;
        TimeSpan timeDiffernce = now.Subtract(prevDate);
        return timeDiffernce.Seconds;
    }

    public static string CutString(string text,int MaxLength)
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

    #endregion
 }
