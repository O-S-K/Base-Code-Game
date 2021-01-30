/*
    /// Call method when reset loop level
    public void UpgradeInfoEnemy()
        {
            /// value is Percent
            UpgradeInfoEnemyWhenResetLevel.UpgradeDamage = percentUpgradeDamageEnemyWhenResetLevel;
            UpgradeInfoEnemyWhenResetLevel.ValueHealthEnemy = percentUpgradeHealthEnemyWhenResetLevel;
            UpgradeInfoEnemyWhenResetLevel.SpeedEnemy = percentUpgradeSpeedEnemyWhenResetLevel;
            UpgradeInfoEnemyWhenResetLevel.SpeedBulletEnemy = percentUpgradeSpeedBulletEnemyWhenResetLevel;
        }

*/

using UnityEngine;

public static class UpgradeInfoEnemyWhenResetLevel
{
    // Atrributes Info Object
    public static float UpgradeDamage
    {
        get { return PlayerPrefs.GetFloat("UpgradeDamage", 0);}
        set { PlayerPrefs.SetFloat("UpgradeDamage", UpgradeDamage + value);}
    }
    public static float ValueHealthEnemy
    {
        get {return PlayerPrefs.GetFloat("ValueHealthEnemy", 0);}
        set { PlayerPrefs.SetFloat("ValueHealthEnemy", ValueHealthEnemy + value);}
    }
    public static float SpeedEnemy
    {
        get { return PlayerPrefs.GetFloat("SpeedEnemy", 0);}
        set {  PlayerPrefs.SetFloat("SpeedEnemy", SpeedEnemy + value);}
    }
    public static float SpeedBulletEnemy
    {
        get {  return PlayerPrefs.GetFloat("SpeedBulletEnemy", 0);}
        set {  PlayerPrefs.SetFloat("SpeedBulletEnemy", SpeedBulletEnemy + value);}
    }


    public static void UpgradeInfoEnemy(ref float _damageEnemy, ref float _valuehealthEnemy, ref float _speedEnemy, ref float _speedBulletEnemy)
    {
        // clamp max value upgrade
        if (UpgradeDamage > 100 || ValueHealthEnemy > 100 || SpeedBulletEnemy > 100 || SpeedBulletEnemy > 100) return;

        // upgrade atrributes for pecent
        _damageEnemy += _damageEnemy * (float)(UpgradeDamage / 100F);
        _valuehealthEnemy += _valuehealthEnemy * (float)(ValueHealthEnemy / 100F);
        _speedEnemy += _speedEnemy * (float)(SpeedEnemy / 100F);
        _speedBulletEnemy += _speedBulletEnemy * (float)(SpeedBulletEnemy / 100F);

#if UNITY_EDITOR
        Debug.Log("UPGRADE ATTRIBUTES ENEMY WHEN LOOP LEVEL");
        Debug.Log("Upgrade DamageEnemy: " + _damageEnemy);
        Debug.Log("Upgrade HealthEnemy: " + _valuehealthEnemy);
        Debug.Log("Upgrade SpeedEnemy: " + _speedEnemy);
        Debug.Log("Upgrade SpeedBulletEnemy: " + _speedBulletEnemy);
#endif
    }
}
