using UnityEngine;

[RequireComponent(typeof(MatrixBlender))]
public class PerspectiveSwitcher : MonoBehaviour
{
    public enum TYPE_CAMERA
    {
        Perspective,
        Orthographic
    }

    public TYPE_CAMERA typeCamera;
    public float fov = 60f, orthographicSize = 50f;
    [Space]
    public float near = .1f, far = 100f;

    private Matrix4x4 ortho, perspective;
    private MatrixBlender blender;
    private Camera m_camera;

    private float aspect;
    private bool startingCamOthographic;

    void Start()
    {
        StartingCamDefaul();
    }

    void StartingCamDefaul()
    {
        aspect = (float)Screen.width / (float)Screen.height;

        float horizontal = orthographicSize * aspect;
        float verticle = orthographicSize;

        ortho = Matrix4x4.Ortho(-horizontal, horizontal, -verticle, verticle, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        m_camera = GetComponent<Camera>();

        if (typeCamera == TYPE_CAMERA.Orthographic)
        {
            m_camera.projectionMatrix = ortho;
            startingCamOthographic = true;
        }
        else if (typeCamera == TYPE_CAMERA.Perspective)
        {
            m_camera.projectionMatrix = perspective;
            startingCamOthographic = false;
        }

        blender = GetComponent<MatrixBlender>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startingCamOthographic = !startingCamOthographic;
            if (startingCamOthographic)
            {
                blender.BlendToMatrix(ortho, 1f, 8, true);
            }
            else
            {
                blender.BlendToMatrix(perspective, 1f, 8, false);
            }
        }
    }
}
