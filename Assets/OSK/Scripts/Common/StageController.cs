/*
GAME COMPLETED
    /// Current level > maxlevel -> change stage else add level
    StageController = StageController.GetCurrentLevel() < maxLevelReset ? AddLevel : AddStage;
GAME OVER
    /// Reset level to first
    StageController.ResetLevel();
*/

using System;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public static Action OnAddStage = delegate { };
    public static string STAGEDATA = "STAGE_DATA";
    public static int maxLevelReset = 5;

    private static StageData currentStageData
    {
        get
        {
            if (PlayerPrefs.HasKey(STAGEDATA))
            {
                string json = PlayerPrefs.GetString(STAGEDATA);
                var stage = JsonUtility.FromJson<StageData>(json);
                return stage;
            }
            else
            {
                var stage = new StageData();
                stage.stage = 1;
                stage.level = 0;
                string json = JsonUtility.ToJson(stage);
                PlayerPrefs.SetString(STAGEDATA, json);
                return stage;

            }
        }
    }

    private void SaveData() { StageData temp = currentStageData;}
    public static int GetCurrentLevel() {return currentStageData.level;}
    public static int GetCurrentStage() { return currentStageData.stage; }

    public static int GetTotalLevelPass()
    {
        var current = (currentStageData.stage - 1) * maxLevelReset;
        return currentStageData.level + current;
    }

    public static void AddLevel()
    {
        var temp = currentStageData;
        if (temp.level < maxLevelReset)
        {
            temp.level++;
        }
        else
        {
            Debug.Log("Error AddLevel");
        }
        string json = JsonUtility.ToJson(temp);
        PlayerPrefs.SetString(STAGEDATA, json);
    }

    public static void RemoveLevel()
    {
        var temp = currentStageData;
        if (temp.level < maxLevelReset)
        {
            temp.level--;
        }
        else
        {
            Debug.Log("Error RemoveLevel");
        }
        PlayerPrefs.SetString(STAGEDATA, JsonUtility.ToJson(temp));
    }

    public static void AddStage()
    {
        var temp = currentStageData;
        temp.stage++;
        temp.level = 1;
        PlayerPrefs.SetString(STAGEDATA, JsonUtility.ToJson(temp));
        OnAddStage();
    }

    public static void RemoveStage()
    {
        var temp = currentStageData;
        temp.stage--;
        temp.level = 1;
        PlayerPrefs.SetString(STAGEDATA, JsonUtility.ToJson(temp));
    }

    public static void ResetLevel()
    {
        var temp = currentStageData;
        temp.level = 1;
        PlayerPrefs.SetString(STAGEDATA, JsonUtility.ToJson(temp));
    }

    public static void ResetStage()
    {
        var temp = currentStageData;
        temp.level = 1;
        temp.stage = 1;
        PlayerPrefs.SetString(STAGEDATA, JsonUtility.ToJson(temp));
    }
}

[SerializeField]
public class StageData
{
    public int stage = 1;
    public int level = 1;
}
