using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerControls handles detecting and responding to all input from the player.
/// </summary>
public class PlayerControls : MonoBehaviour
{
    [System.Serializable]
    public class CameraControlSettings
    {
        public Transform pivot;
        [Range(0.0f, 5.0f)]
        public float sensitivity = 1.0f;
        [Range(0.0f, 180.0f)]
        public float range = 90;
    }
    public CameraControlSettings cameraLeftRightSettings;
    public CameraControlSettings cameraUpDownSettings;

    public Transform objectFocusPoint;
    public float _focusDistance = 2.5f;
    protected float _objectFocusDist;
    protected bool _isFocused = false;

    public LayerMask selectionLayerMask;
    public ISelectable<PlayerControls> _targetedObject;

    protected float _deltaLeftRightRotation = 0;
    public float deltaLeftRightRotation
    {
        get { return _deltaLeftRightRotation; }
    }
    protected float _leftRightRotation = 0;
    public float leftRightRotation
    {
        get { return _leftRightRotation; }
    }

    protected float _deltaUpDownRotation = 0;
    public float deltaUpDownRotation
    {
        get { return _deltaUpDownRotation; }
    }
    protected float _upDownRotation = 0;
    public float upDownRotation
    {
        get { return _upDownRotation; }
    }

    protected Rect _cursorRect;

    protected void Awake()
    {
        _cursorRect = new Rect(Screen.width * 0.5f - 10, Screen.height * 0.5f - 10, 20, 20);
    }

    protected void Start()
    {
        Screen.lockCursor = true;
    }

	protected void Update () {
        HandleCameraControls();
        HandleObjectTargeting();

        if (Input.GetMouseButtonDown(0))
        {
            Screen.lockCursor = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
	}

    protected void HandleCameraControls()
    {
        // Left Right Rotation
        _deltaLeftRightRotation = Input.GetAxis("Mouse X") * cameraLeftRightSettings.sensitivity;
        _leftRightRotation += _deltaLeftRightRotation;
        _leftRightRotation = Mathf.Clamp(_leftRightRotation, -cameraLeftRightSettings.range, cameraLeftRightSettings.range);
        cameraLeftRightSettings.pivot.localEulerAngles = new Vector3(0, _leftRightRotation, 0);

        // Up Down Rotation
        _deltaUpDownRotation = -Input.GetAxis("Mouse Y") * cameraUpDownSettings.sensitivity;
        _upDownRotation += _deltaUpDownRotation; 
        _upDownRotation = Mathf.Clamp(_upDownRotation, -cameraUpDownSettings.range, cameraUpDownSettings.range);
        cameraUpDownSettings.pivot.localEulerAngles = new Vector3(_upDownRotation, 0, 0);
    }

    protected void HandleObjectTargeting()
    {
        // handle deselecting
        if (HeadGestureRecognizer.Instance.JustGestured(HeadGesture.shake))
        {
            TargetObject();
        }

        // handle bringing the object closer/moving it back
        if (HeadGestureRecognizer.Instance.JustGestured(HeadGesture.nod))
        {
            if (_isFocused)
            {
                objectFocusPoint.position = Camera.main.transform.position + Camera.main.transform.forward * _objectFocusDist;
            }
            else
            {
                objectFocusPoint.position = Camera.main.transform.position + Camera.main.transform.forward * _focusDistance;
            }
            _isFocused = !_isFocused;
        }

        // handle selecting the object
        if (_targetedObject == null || !_targetedObject.IsSelected())
        {
            Ray camRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(camRay, out hit, float.MaxValue, selectionLayerMask))
            {
                ISelectable<PlayerControls> obj = hit.collider.GetComponent(typeof(ISelectable<PlayerControls>)) as ISelectable<PlayerControls>;
                TargetObject(obj);
                objectFocusPoint.position = hit.point;
                _objectFocusDist = Vector3.Distance(Camera.main.transform.position, hit.point);
                _isFocused = false;
            }
            else
            {
                TargetObject();
            }
        }
    }

    protected void TargetObject(ISelectable<PlayerControls> obj = null)
    {
        // deselect the previous object
        if (obj != _targetedObject && _targetedObject != null)
        {
            _targetedObject.Deselect();
        }

        _targetedObject = obj;

        if (_targetedObject != null)
        {
            _targetedObject.Select(this);
        }
    }

    protected void OnGUI()
    {
        GUI.Label(_cursorRect, "x");
    }
}

