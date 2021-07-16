using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum TypeCamera
    {
        Perspective = 0,
        Orthograhics = 1
    }

    public TypeCamera typeCamera = TypeCamera.Perspective;
    public float cameraPerspectiveFovdefault = 70F;
    public float cameraOrthograhicsFovdefault = 15F;

    bool isFirstStart = false;

    Camera mainCamera;

    void Start() => mainCamera = Camera.main;

    void Update()
    {
#if !UNITY_EDITOR
        if (isFirstStart) return;
        isFirstStart = true;
#endif

        float num = (float)Screen.height / (float)Screen.width;
        float ratio = Screen.height / 1280f / (Screen.width / 720f);

    //if (num > 1.8f)
    //{
        switch (typeCamera)
        {
            case TypeCamera.Perspective: mainCamera.fieldOfView = cameraPerspectiveFovdefault * ratio; break;
            case TypeCamera.Orthograhics: mainCamera.orthographicSize = cameraOrthograhicsFovdefault * ratio; break;
        }
    //}
    }
}
