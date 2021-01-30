using UnityEngine;

public static class DataConfigFirebase
{
    #region Data PlayerPrefs Config Firebase
    // stage
    public static readonly string MONEY_DATA_CONFIG_FIREBASE = "MONEY_DATA_CONFIG_FIREBASE_";
    public static readonly string CURRENT_LEVEL_PASS_CONFIG_FIREBASE = "CURRENT_LEVEL_PASS_CONFIG_FIREBASE_";
    public static readonly string TOTAL_LEVEL_PASS_CONFIG_FIREBASE = "TOTAL_LEVEL_PASS_CONFIG_FIREBASE_";
    public static readonly string COUNT_DIE_PLAYER_CONFIG_FIBASE = "COUNT_DIE_PLAYER_CONFIG_FIBASE_";

    // count player die
    public static readonly string TOTAL_COUNT_DIE_PLAYER_VERY_EASY = "TOTAL_COUNT_DIE_PLAYER_VERY_EASY_";
    public static readonly string TOTAL_COUNT_DIE_PLAYER_EASY = "TOTAL_COUNT_DIE_PLAYER_EASY_";
    public static readonly string TOTAL_COUNT_DIE_PLAYER_MEDIUM = "TOTAL_COUNT_DIE_PLAYER_MEDIUM_";
    public static readonly string TOTAL_COUNT_DIE_PLAYER_HARD = "TOTAL_COUNT_DIE_PLAYER_HARD_";
    public static readonly string TOTAL_COUNT_DIE_PLAYER_VERY_HARD = "TOTAL_COUNT_DIE_PLAYER_VERY_HARD_";

    // percent type player
    public static readonly string PERCENT_TYPE_PLAYER_VERY_EASY = "PERCENT_TYPE_PLAYER_VERY_EASY_";
    public static readonly string PERCENT_TYPE_PLAYER_EASY = "PERCENT_TYPE_PLAYER_EASY_";
    public static readonly string PERCENT_TYPE_PLAYER_MEDIUM = "PERCENT_TYPE_PLAYER_MEDIUM_";
    public static readonly string PERCENT_TYPE_PLAYER_HARD = "PERCENT_TYPE_PLAYER_HARD_";
    public static readonly string PERCENT_TYPE_PLAYER_VERY_HARD = "PERCENT_TYPE_PLAYER_VERY_HARD_";

    // value info enemy
    public static readonly string VALUE_HEALTH_ENEMY_DATA_CONFIG_FIREBASE = "VALUE_HEALTH_ENEMY_DATA_CONFIG_FIREBASE_";
    public static readonly string VALUE_DAMAGE_ENEMY_DATA_CONFIG_FIREBASE = "VALUE_DAMAGE_ENEMY_DATA_CONFIG_FIREBASE_";
    public static readonly string VALUE_SPEED_ENEMY_DATA_CONFIG_FIREBASE = "VALUE_SPEED_ENEMY_DATA_CONFIG_FIREBASE_";
    public static readonly string VALUE_SPEED_WEAPON_ENEMY_DATA_CONFIG_FIREBASE = "VALUE_SPEED_WEAPON_ENEMY_DATA_CONFIG_FIREBASE";
    #endregion

    // No Editing Firebase

    #region Count Die Player Firebase
    public static int CountDiePlayerFirebase
    {
        get { return PlayerPrefs.GetInt(COUNT_DIE_PLAYER_CONFIG_FIBASE, 0); }
        set { PlayerPrefs.SetInt(COUNT_DIE_PLAYER_CONFIG_FIBASE, value); }
    }
    #endregion

    #region Value Health Enemy Firebase
    public static float ValueHealthEnemyConfigFirebase
    {
        get { return PlayerPrefs.GetFloat(VALUE_HEALTH_ENEMY_DATA_CONFIG_FIREBASE, 0); }
        set { PlayerPrefs.SetFloat(VALUE_HEALTH_ENEMY_DATA_CONFIG_FIREBASE, value); }
    }
    #endregion
    #region Value Damage Enemy Firebase
    public static float ValueDamageEnemyConfigFirebase
    {
        get { return PlayerPrefs.GetFloat(VALUE_DAMAGE_ENEMY_DATA_CONFIG_FIREBASE, 0); }
        set { PlayerPrefs.SetFloat(VALUE_DAMAGE_ENEMY_DATA_CONFIG_FIREBASE, value); }
    }
    #endregion
    #region Value Speed Enemy Firebase
    public static float ValueSpeedEnemyConfigFirebase
    {
        get { return PlayerPrefs.GetFloat(VALUE_SPEED_ENEMY_DATA_CONFIG_FIREBASE, 0F); }
        set { PlayerPrefs.SetFloat(VALUE_SPEED_ENEMY_DATA_CONFIG_FIREBASE, value); }
    }
    #endregion
    #region Value Speed Bullet Weapon Enemy Firebase
    public static float ValueSpeedWeaponEnemyConfigFirebase
    {
        get { return PlayerPrefs.GetFloat(VALUE_SPEED_WEAPON_ENEMY_DATA_CONFIG_FIREBASE, 0F); }
        set { PlayerPrefs.SetFloat(VALUE_SPEED_WEAPON_ENEMY_DATA_CONFIG_FIREBASE, value); }
    }
    #endregion

    #region Current Level Pass Firebase
    public static int CurrentLevelPassFirebase
    {
        get { return PlayerPrefs.GetInt(CURRENT_LEVEL_PASS_CONFIG_FIREBASE, 0); }
        set { PlayerPrefs.SetInt(CURRENT_LEVEL_PASS_CONFIG_FIREBASE, value); }
    }
    #endregion

    // Edited Firebase
    #region Value Money Firebase
    public static float TotalMoneyFirebase
    {
        get { return PlayerPrefs.GetFloat(MONEY_DATA_CONFIG_FIREBASE, 0); }
        set { PlayerPrefs.SetFloat(MONEY_DATA_CONFIG_FIREBASE, value); }
    }
    #endregion

    #region Total number count player die very easy 
    public static float TotalCountPlayerDieVeryEasy
    {
        get { return PlayerPrefs.GetFloat(TOTAL_COUNT_DIE_PLAYER_VERY_EASY, 0); }
        set { PlayerPrefs.SetFloat(TOTAL_COUNT_DIE_PLAYER_VERY_EASY, value); }
    }
    #endregion
    #region Total number count player die easy 
    public static float TotalCountPlayerDieEasy
    {
        get { return PlayerPrefs.GetFloat(TOTAL_COUNT_DIE_PLAYER_EASY, 1); }
        set { PlayerPrefs.SetFloat(TOTAL_COUNT_DIE_PLAYER_EASY, value); }
    }
    #endregion
    #region Total number count player die medium 
    public static float TotalCountPlayerDieMedium
    {
        get { return PlayerPrefs.GetFloat(TOTAL_COUNT_DIE_PLAYER_MEDIUM, 2); }
        set { PlayerPrefs.SetFloat(TOTAL_COUNT_DIE_PLAYER_MEDIUM, value); }
    }
    #endregion
    #region Total number count player die hard 
    public static float TotalCountPlayerDieHard
    {
        get { return PlayerPrefs.GetFloat(TOTAL_COUNT_DIE_PLAYER_HARD, 3); }
        set { PlayerPrefs.SetFloat(TOTAL_COUNT_DIE_PLAYER_HARD, value); }
    }

    #endregion
    #region Total number count player die very hard 
    public static float TotalCountPlayerDieVeryHard
    {
        get { return PlayerPrefs.GetFloat(TOTAL_COUNT_DIE_PLAYER_VERY_HARD, 4); }
        set { PlayerPrefs.SetFloat(TOTAL_COUNT_DIE_PLAYER_VERY_HARD, value); }
    }
    #endregion

    #region Percent type very easy
    public static float PercentTypePlayerVeryEasy
    {
        get { return PlayerPrefs.GetFloat(PERCENT_TYPE_PLAYER_VERY_EASY, 0); }
        set { PlayerPrefs.SetFloat(PERCENT_TYPE_PLAYER_VERY_EASY, value); }
    }
    #endregion
    #region Percent type easy 
    public static float PercentTypePlayerEasy
    {
        get { return PlayerPrefs.GetFloat(PERCENT_TYPE_PLAYER_EASY, 0); }
        set { PlayerPrefs.SetFloat(PERCENT_TYPE_PLAYER_EASY, value); }
    }

    #endregion
    #region Percent type medium 
    public static float PercentTypePlayerMedium
    {
        get { return PlayerPrefs.GetFloat(PERCENT_TYPE_PLAYER_MEDIUM, 0); }
        set { PlayerPrefs.SetFloat(PERCENT_TYPE_PLAYER_MEDIUM, value); }
    }
    #endregion
    #region Percent type hard 
    public static float PercentTypePlayerHard
    {
        get { return PlayerPrefs.GetFloat(PERCENT_TYPE_PLAYER_HARD, 0); }
        set { PlayerPrefs.SetFloat(PERCENT_TYPE_PLAYER_HARD, value); }
    }
    #endregion
    #region Percent type very hard 

    public static float PercentTypePlayerVeryHard
    {
        get { return PlayerPrefs.GetFloat(PERCENT_TYPE_PLAYER_VERY_HARD, 0); }
        set { PlayerPrefs.SetFloat(PERCENT_TYPE_PLAYER_VERY_HARD, value); }
    }
    #endregion

    #region Total Level Pass Firebase
    public static int TotalLevelPassFirebase
    {
        get { return PlayerPrefs.GetInt(TOTAL_LEVEL_PASS_CONFIG_FIREBASE, 3); }
        set { PlayerPrefs.SetInt(TOTAL_LEVEL_PASS_CONFIG_FIREBASE, value); }
    }
    #endregion
}

public static class ConfigLevelOfDifficult
{
    public enum TypeOfPlayer
    {
        VeryEasy,
        Easy,
        Medium,
        Hard,
        VeryHard
    }

    static TypeOfPlayer typeOfPlayer = TypeOfPlayer.Medium;

    public static void SetCountPlayerDieInLevel()
    {
        if (DataConfigFirebase.CurrentLevelPassFirebase > DataConfigFirebase.TotalLevelPassFirebase) // total level pass
        {
            if (DataConfigFirebase.CountDiePlayerFirebase <= DataConfigFirebase.TotalCountPlayerDieVeryEasy)
            {
                typeOfPlayer = TypeOfPlayer.VeryEasy;
                SetupInfoConfigFirebase(typeOfPlayer);
            }
            else if (DataConfigFirebase.CountDiePlayerFirebase == DataConfigFirebase.TotalCountPlayerDieEasy)
            {
                typeOfPlayer = TypeOfPlayer.Easy;
                SetupInfoConfigFirebase(typeOfPlayer);
            }
            else if (DataConfigFirebase.CountDiePlayerFirebase == DataConfigFirebase.TotalCountPlayerDieMedium)
            {
                typeOfPlayer = TypeOfPlayer.Medium;
                SetupInfoConfigFirebase(typeOfPlayer);
            }
            else if (DataConfigFirebase.CountDiePlayerFirebase == DataConfigFirebase.TotalCountPlayerDieHard)
            {
                typeOfPlayer = TypeOfPlayer.Hard;
                SetupInfoConfigFirebase(typeOfPlayer);
            }
            else if (DataConfigFirebase.CountDiePlayerFirebase >= DataConfigFirebase.TotalCountPlayerDieVeryHard)
            {
                typeOfPlayer = TypeOfPlayer.VeryHard;
                SetupInfoConfigFirebase(typeOfPlayer);
            }

            DataConfigFirebase.CountDiePlayerFirebase = 0;
            DataConfigFirebase.CurrentLevelPassFirebase = 0;
        }

#if UNITY_EDITOR
        Debug.Log("<color=yellow>CountDiePlayerFirebase: </color>" + DataConfigFirebase.CountDiePlayerFirebase);
        Debug.Log("<color=yellow>CurrentLevelPassFirebase: </color>" + DataConfigFirebase.CurrentLevelPassFirebase);
#endif
    }

    public static void SetupInfoConfigFirebase(TypeOfPlayer typeOfPlayer)
    {
#if UNITY_EDITOR
        Debug.Log("TypeOfPlayer: " + typeOfPlayer);
#endif

        switch (typeOfPlayer)
        {
            case TypeOfPlayer.VeryEasy: SetupInfo(DataConfigFirebase.PercentTypePlayerVeryEasy); break;
            case TypeOfPlayer.Easy:     SetupInfo(DataConfigFirebase.PercentTypePlayerEasy);     break;
            case TypeOfPlayer.Medium:   SetupInfo(DataConfigFirebase.PercentTypePlayerMedium);   break;
            case TypeOfPlayer.Hard:     SetupInfo(DataConfigFirebase.PercentTypePlayerHard);     break;
            case TypeOfPlayer.VeryHard: SetupInfo(DataConfigFirebase.PercentTypePlayerVeryHard); break;
        }
    }

    public static void SetupInfo(float percent)
    {
        DataConfigFirebase.ValueDamageEnemyConfigFirebase = percent;
        DataConfigFirebase.ValueHealthEnemyConfigFirebase= percent;
        DataConfigFirebase.ValueSpeedEnemyConfigFirebase= percent;
        DataConfigFirebase.ValueSpeedWeaponEnemyConfigFirebase= percent;
    }

    public static void GetInfoConfigFirebase(ref float _damageEnemy, ref float _valuehealthEnemy, ref float _speedEnemy, ref float _speedBulletEnemy)
    {
        _damageEnemy += _damageEnemy * (float)(DataConfigFirebase.ValueDamageEnemyConfigFirebase / 100F);
        _valuehealthEnemy += _valuehealthEnemy * (float)(DataConfigFirebase.ValueHealthEnemyConfigFirebase / 100F);
        _speedEnemy += _speedEnemy * (float)(DataConfigFirebase.ValueSpeedEnemyConfigFirebase / 100F);
        _speedBulletEnemy += _speedBulletEnemy * (float)(DataConfigFirebase.ValueSpeedWeaponEnemyConfigFirebase / 100F);

#if UNITY_EDITOR
        Debug.Log("<color=yellow>UPGRADE INFO ENEMIES</color>");
        Debug.Log("Upgrade_DamageEnemy: " + _damageEnemy);
        Debug.Log("Upgrade_HealthEnemy: " + _valuehealthEnemy);
        Debug.Log("Upgrade_SpeedEnemy: " + _speedEnemy);
        Debug.Log("Upgrade_SpeedBulletEnemy: " + _speedBulletEnemy);
        Debug.Log("\n");
#endif
    }
}