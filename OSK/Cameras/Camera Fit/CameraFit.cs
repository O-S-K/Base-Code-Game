using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
[ExecuteInEditMode]
public class CameraFit : MonoBehaviour
{
    public enum CameraType { Orthographic, Perspective }
    [SerializeField]
    private CameraType _ct;

    public enum Mode { FixedHeight, FixedWidth, Dynamic }
    [SerializeField]
    private Mode _adjustMode = Mode.FixedHeight;
    private Mode _prevMode;

    private float _prevWidth;
    [SerializeField]
    private float _mainAreaWidth;
    public float mainAreaWidth
    {
        get { return _mainAreaWidth; }
        set
        {
            float tvalue = value;
            if (tvalue < 1)
            {
                tvalue = 1;
            }
            _mainAreaWidth = tvalue;
            _prevWidth = _mainAreaWidth;
            verticalSize = _verticalSize;
            verticalFOV = _verticalFOV;
        }
    }

    private float _prevHeight;
    [SerializeField]
    private float _mainAreaHeight;
    public float mainAreaHeight
    {
        get { return _mainAreaHeight; }
        set
        {
            float tvalue = value;
            if (tvalue < 1)
            {
                tvalue = 1;
            }
            _mainAreaHeight = tvalue;
            _prevHeight = _mainAreaHeight;
            horizontalSize = _horizontalSize;
            horizontalFOV = _horizontalFOV;
        }
    }

    private float mainAreaAspectRatio
    {
        set
        {
            if (value > 1)
            {
                _mainAreaWidth = value;
                _mainAreaHeight = 1;
            }
            else
            {
                _mainAreaWidth = 1;
                _mainAreaHeight = 1/value;
            }
            _prevWidth = _mainAreaWidth;
            _prevHeight = _mainAreaHeight;
        }
        get
        {
            return _mainAreaWidth / _mainAreaHeight;
        }
    }

    private Camera _aspectCamera = null;
    private float screenAspect
    {
        get
        {
            return (float)_aspectCamera.pixelWidth / _aspectCamera.pixelHeight;
        }
    }

    private float _prevVFOV;
    [SerializeField, Range(1f, 179f)]
    private float _verticalFOV;
    public float verticalFOV
    {
        get
        {
            return _verticalFOV;
        }
        set
        {
            float tvalue = value;
            if (value < 1f)
            {
                tvalue = 1f;
            }
            if (value > 179f)
            {
                tvalue = 179f;
            }
            switch (_adjustMode)
            {
                case Mode.Dynamic:
                    float horizontalt = AngleConversionByRatio(tvalue, mainAreaAspectRatio);
                    if (horizontalt >= 1f && horizontalt <= 179f)
                    {
                        _verticalFOV = tvalue;
                        _horizontalFOV = horizontalt;
                    }
                    else if (horizontalt < 1)
                    {
                        _horizontalFOV = 1f;
                        _verticalFOV = AngleConversionByRatio(_horizontalFOV, 1 / mainAreaAspectRatio);
                    }
                    else if (horizontalt > 179f)
                    {
                        _horizontalFOV = 179f;
                        _verticalFOV = AngleConversionByRatio(_horizontalFOV, 1 / mainAreaAspectRatio);
                    }
                    break;
                default:
                    _verticalFOV = tvalue;
                    break;
            }
            _prevHFOV = _horizontalFOV;
            _prevVFOV = _verticalFOV;
            ChangeViewForPerspectiveCamera();
        }
    }

    private float _prevHFOV;
    [SerializeField, Range(1f, 179f)]
    private float _horizontalFOV;
    public float horizontalFOV
    {
        get
        {
            return _horizontalFOV;
        }
        set
        {
            float tvalue = value;
            if (value< 1f)
            {
                tvalue = 1f;
            }
            if (value > 179f)
            {
                tvalue = 179f;
            }
            switch (_adjustMode)
            {
                case Mode.Dynamic:
                    float verticalt = AngleConversionByRatio(tvalue, 1 / mainAreaAspectRatio);
                    if (verticalt >= 1 && verticalt <= 179)
                    {
                        _horizontalFOV = tvalue;
                        _verticalFOV = verticalt;
                    }
                    else if (verticalt < 1)
                    {
                        _verticalFOV = 1f;
                        _horizontalFOV = AngleConversionByRatio(_verticalFOV, mainAreaAspectRatio);
                    }
                    else if (verticalt > 179f)
                    {
                        _verticalFOV = 179f;
                        _horizontalFOV = AngleConversionByRatio(_verticalFOV, mainAreaAspectRatio);
                    }
                    break;
                default:
                    _horizontalFOV = tvalue;
                    break;
            }
           
            _prevVFOV = _verticalFOV;
            _prevHFOV = _horizontalFOV;
            ChangeViewForPerspectiveCamera();
        }
    }

    private float _prevVSize;
    [SerializeField]
    private float _verticalSize;
    public float verticalSize
    {
        get { return _verticalSize; }
        set
        {
            if (value != 0)
            {
                _verticalSize = value;
                if (_adjustMode == Mode.Dynamic)
                {
                    _horizontalSize = _verticalSize * mainAreaAspectRatio;
                }
                _prevVSize = _verticalSize;
                _prevHSize = _horizontalSize;
                ChangeViewForOrthographicCamera();
            }
        }
    }

    private float _prevHSize;
    [SerializeField]
    private float _horizontalSize;
    public float horizontalSize
    {
        get { return _horizontalSize; }
        set
        {
            if (value != 0)
            {
                _horizontalSize = value;
                if (_adjustMode == Mode.Dynamic)
                {
                    _verticalSize = _horizontalSize / mainAreaAspectRatio;
                }
                _prevVSize = _verticalSize;
                _prevHSize = _horizontalSize;
                ChangeViewForOrthographicCamera();
            }
        }
    }

    private float _cameraPrevSize;
    private float _cameraPrevFOV;
    private float _cameraPrevPixelWidth;
    private float _cameraPrevPixelHeight;


    public void Reset()
    {
        ResetPublicFields();
    }


    void ResetPublicFields()
    {
        mainAreaAspectRatio = screenAspect;
        switch (_adjustMode)
        {
            case Mode.Dynamic:
                verticalSize = _aspectCamera.orthographicSize;
                verticalFOV = _aspectCamera.fieldOfView;
                break;
            case Mode.FixedHeight:
                verticalSize = _aspectCamera.orthographicSize;
                verticalFOV = _aspectCamera.fieldOfView;
                break;
            case Mode.FixedWidth:
                horizontalSize =  _aspectCamera.orthographicSize * mainAreaAspectRatio;
                horizontalFOV = AngleConversionByRatio(_aspectCamera.fieldOfView, mainAreaAspectRatio);
                break;
        }
        _prevMode = _adjustMode;
    }


    public void Awake()
    {
        _aspectCamera = GetComponent<Camera>();
        if (_aspectCamera.orthographic)
        {
            _ct = CameraType.Orthographic;
        }
        else
        {
            _ct = CameraType.Perspective;
        }

        _prevMode = _adjustMode;
        _prevWidth = _mainAreaWidth;
        _prevHeight = _mainAreaHeight;
        _prevHSize = horizontalSize;
        _prevVSize = verticalSize;
        _prevHFOV = horizontalFOV;
        _prevVFOV = verticalFOV;

        _cameraPrevPixelWidth = _aspectCamera.pixelWidth;
        _cameraPrevPixelHeight = _aspectCamera.pixelHeight;
    }


    public void Start()
    {
        ChangeViewForCamera();
    }


    public void Update()
    {
        if ((_cameraPrevPixelWidth != _aspectCamera.pixelWidth) || (_cameraPrevPixelHeight != _aspectCamera.pixelHeight))
        {
            _cameraPrevPixelWidth = _aspectCamera.pixelWidth;
            _cameraPrevPixelHeight = _aspectCamera.pixelHeight;
            ChangeViewForCamera();
        }
        if (_aspectCamera.orthographic)
        {
            if (_ct == CameraType.Perspective)
            {
                _ct = CameraType.Orthographic;
                ChangeViewForCamera();
            }
            if (_cameraPrevSize != _aspectCamera.orthographicSize)
            {
                if (_cameraPrevSize == verticalSize)
                {
                    verticalSize = _aspectCamera.orthographicSize;
                }
                else
                {
                    verticalSize = _aspectCamera.orthographicSize * screenAspect / mainAreaAspectRatio;
                }
            }
        }
        else
        {
            if (_ct == CameraType.Orthographic)
            {
                _ct = CameraType.Perspective;
                ChangeViewForCamera();
            }
            if (_cameraPrevFOV != _aspectCamera.fieldOfView)
            {
                if (_cameraPrevFOV == verticalFOV)
                {
                    verticalFOV = _aspectCamera.fieldOfView;
                }
                else
                {
                    verticalFOV = AngleConversionByRatio(_aspectCamera.fieldOfView, screenAspect / mainAreaAspectRatio);
                }
            }
        }
    }


    float AngleConversionByRatio(float degreeAngle, float ratio)
    {
        return 2f * (Mathf.Atan(Mathf.Tan(0.5f * degreeAngle * Mathf.Deg2Rad) * ratio)) * Mathf.Rad2Deg;
    }


    public void OnValidate()
    {
        if (_aspectCamera == null)
        {
            _aspectCamera = GetComponent<Camera>();
        }
        if(_aspectCamera != null && _aspectCamera.pixelHeight != 0 && _aspectCamera.pixelWidth != 0)
        {
            if (_prevVSize != verticalSize)
            {
                verticalSize = _verticalSize;
            }
            else if (_prevHSize != horizontalSize)
            {
                horizontalSize = _horizontalSize;
            }
            else if (_prevHeight != _mainAreaHeight)
            {
                mainAreaHeight = _mainAreaHeight;
            }
            else if (_prevWidth != _mainAreaWidth)
            {
                mainAreaWidth = _mainAreaWidth;
            }
            else if (_prevHFOV != _horizontalFOV)
            {
                horizontalFOV = _horizontalFOV;
            }
            else if (_prevVFOV != _verticalFOV)
            {
                verticalFOV = _verticalFOV;
            }
            else if(_prevMode != _adjustMode)
            {
                ResetPublicFields();
            }
            else
            {
                ChangeViewForCamera();
            }
        }
    }


    private void ChangeViewForCamera()
    {
        if (_aspectCamera.orthographic)
        {
            ChangeViewForOrthographicCamera();
        }
        else
        {
            ChangeViewForPerspectiveCamera();
        }    
    }


    private void ChangeViewForOrthographicCamera()
    {
        switch (_adjustMode)
        {
            case Mode.FixedHeight:
                _aspectCamera.orthographicSize = verticalSize;
                break;
            case Mode.FixedWidth:
                _aspectCamera.orthographicSize = horizontalSize / screenAspect;
                break;
            case Mode.Dynamic:
                _aspectCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize / screenAspect);
                break;
        }
        _cameraPrevSize = _aspectCamera.orthographicSize;
    }

    
    private void ChangeViewForPerspectiveCamera()
    {
        float aspectDifference;
        switch (_adjustMode)
        {
            case Mode.FixedHeight:
                _aspectCamera.fieldOfView = verticalFOV;
                break;
            case Mode.FixedWidth:
                _aspectCamera.fieldOfView = AngleConversionByRatio(horizontalFOV, 1/screenAspect);
                break;
            case Mode.Dynamic:
                aspectDifference = Mathf.Min(1f, screenAspect / mainAreaAspectRatio);
                _aspectCamera.fieldOfView = AngleConversionByRatio(verticalFOV, 1 / aspectDifference);
                break;
        }
        _cameraPrevFOV = _aspectCamera.fieldOfView;
    }
}