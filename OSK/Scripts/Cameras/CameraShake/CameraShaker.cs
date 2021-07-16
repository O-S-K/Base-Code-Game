using System;
using UnityEngine;

// if (CameraShaker.instance != null) 
//     CameraShaker.instance.InitShake(0.07f, 1f);

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker instance;
    Transform trans;

    Vector3 lastRealPos;
    Vector3 defPos = new Vector3(0f, 0f, 0f);

    float shakeTime;
    float shakePwr;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        trans = base.transform;
    }

    void Update()
    {
        if (shakeTime > 0f)
        {
            var value = UnityEngine.Random.insideUnitCircle.normalized * shakePwr;
            trans.localPosition = defPos + new Vector3(value.x, value.y, 0f);
            shakeTime -= Time.deltaTime;
            if (shakeTime <= 0f)
            {
                shakePwr = 0f;
                trans.localPosition = defPos;
            }
        }
    }

    public void InitShake(float time, float pwr)
    {
        if (pwr >= shakePwr && time >= shakeTime)
        {
            shakeTime = time;
            shakePwr = pwr;
        }
    }
}
