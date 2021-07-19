using UnityEngine;

public class CameraScaleForSize : MonoBehaviour
{
    public enum TypeCamera
    {
        Perspective = 0,
        Orthograhics = 1
    }

    public enum FixAxisCamera
    {
        Horizontal = 0,
        Vertical = 1
    }

    public FixAxisCamera fixAxisCamera = FixAxisCamera.Vertical;
    public TypeCamera typeCamera = TypeCamera.Perspective;

    public float cameraPerspectiveFovdefault = 70F;
    public float cameraOrthograhicsFovdefault = 15F;
    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;

        switch(fixAxisCamera)
        {
            case FixAxisCamera.Horizontal:
                {
                    SetRatioCameraHorizontal();
                    break;
                }
            case FixAxisCamera.Vertical:
                {
                    SetRatioCameraVertical();
                    break;
                }
        }
    }

    public void SetRatioCameraHorizontal()
    {
        float num = (float)Screen.width / (float)Screen.height;
        float ratio = (Screen.height / (float)720F) / (Screen.width / (float)1280F);

        if (num > 1.3F)
        {
            switch (typeCamera)
            {
                case TypeCamera.Perspective:
                    {
                        mainCamera.fieldOfView = cameraPerspectiveFovdefault * ratio;
                        break;
                    }
                case TypeCamera.Orthograhics:
                    {
                        mainCamera.orthographicSize = cameraOrthograhicsFovdefault * ratio;
                        break;
                    }
            }
        }
    }

    public void SetRatioCameraVertical()
    {
        float num = (float)Screen.height / (float)Screen.width;
        float ratio = (Screen.height / 1280f) / (Screen.width / 720f);

        if (num > 1.8f)
        {
            switch (typeCamera)
            {
                case TypeCamera.Perspective:
                    {
                        mainCamera.fieldOfView = cameraPerspectiveFovdefault * ratio;
                        break;
                    }
                case TypeCamera.Orthograhics:
                    {
                        mainCamera.orthographicSize = cameraOrthograhicsFovdefault * ratio;
                        break;
                    }
            }
        }
    }
}
