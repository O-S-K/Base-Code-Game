using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MatrixBlender : MonoBehaviour
{
    Camera m_camera;
    private void Start()
    {
        m_camera = GetComponent<Camera>();
    }

    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        var ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
        {
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        }
        return ret;
    }

    private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration, float ease, bool reverse)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float step;
            if (reverse) step = 1 - Mathf.Pow(1 - (Time.time - startTime) / duration, ease);
            else step = Mathf.Pow((Time.time - startTime) / duration, ease);
            m_camera.projectionMatrix = MatrixLerp(src, dest, step);
            yield return 1;
        }
        m_camera.projectionMatrix = dest;
    }

    public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration, float ease, bool reverse)
    {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(m_camera.projectionMatrix, targetMatrix, duration, ease, reverse));
    }
}
