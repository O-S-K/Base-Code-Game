using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    /// <summary>
    /// Amount of Shake
    /// </summary>
    public Vector3 Amount = new Vector3(1f, 1f, 0);

    /// <summary>
    /// Duration of Shake
    /// </summary>
    public float Duration = 1;

    /// <summary>
    /// Shake Speed
    /// </summary>
    public float Speed = 10;

    /// <summary>
    /// Amount over Lifetime [0,1]
    /// </summary>
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    /// <summary>
    /// Set it to true: The camera position is set in reference to the old position of the camera
    /// Set it to false: The camera position is set in absolute values or is fixed to an object
    /// </summary>
    public bool DeltaMovement = true;

    protected Camera Camera;
    protected float time = 0;
    protected Vector3 lastPos;
    protected Vector3 nextPos;
    protected float lastFoV;
    protected float nextFoV;
    protected bool destroyAfterPlay;

    /// <summary>
    /// awake
    /// </summary>
    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    /// <summary>
    /// Do the shake
    /// </summary>
    public static void ShakeOnce(float duration = 1f, float speed = 10f, Vector3? amount = null, Camera camera = null, bool deltaMovement = true, AnimationCurve curve = null)
    {
        //set data
        var instance = ((camera != null) ? camera : Camera.main).gameObject.AddComponent<CameraEffects>();
        instance.Duration = duration;
        instance.Speed = speed;
        if (amount != null)
            instance.Amount = (Vector3)amount;
        if (curve != null)
            instance.Curve = curve;
        instance.DeltaMovement = deltaMovement;

        //one time
        instance.destroyAfterPlay = true;
        instance.Shake();
    }

    /// <summary>
    /// Do the shake
    /// </summary>
    public void Shake()
    {
        ResetCam();
        time = Duration;
    }

    private void LateUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shake();
        }

        if (time > 0)
        {
            //do something
            time -= Time.deltaTime;
            if (time > 0)
            {
                //next position based on perlin noise
                nextPos = (Mathf.PerlinNoise(time * Speed, time * Speed * 2) - 0.5f) * Amount.x * transform.right * Curve.Evaluate(1f - time / Duration) +
                          (Mathf.PerlinNoise(time * Speed * 2, time * Speed) - 0.5f) * Amount.y * transform.up * Curve.Evaluate(1f - time / Duration);
                nextFoV = (Mathf.PerlinNoise(time * Speed * 2, time * Speed * 2) - 0.5f) * Amount.z * Curve.Evaluate(1f - time / Duration);

                Camera.fieldOfView += (nextFoV - lastFoV);
                Camera.transform.Translate(DeltaMovement ? (nextPos - lastPos) : nextPos);

                lastPos = nextPos;
                lastFoV = nextFoV;
            }
            else
            {
                //last frame
                ResetCam();
                if (destroyAfterPlay)
                    Destroy(this);
            }
        }
    }

    private void ResetCam()
    {
        //reset the last delta
        Camera.transform.Translate(DeltaMovement ? -lastPos : Vector3.zero);
        Camera.fieldOfView -= lastFoV;

        //clear values
        lastPos = nextPos = Vector3.zero;
        lastFoV = nextFoV = 0f;
    }
}
