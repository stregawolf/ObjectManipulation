using UnityEngine;
using System.Collections;

/// <summary>
/// ManipulatableObject handles movement and feedback for an object affected by the player.
/// Users a simple state machine to handle all of its behaviour
/// </summary>
[RequireComponent(typeof(OutlineManipulator), typeof(Rigidbody))]
public class ManipulatableObject : MonoBehaviour, ISelectable<PlayerControls> {
    public enum ManipulationState
    {
        unselected = 0,
        selecting,
        selected,
    }
    public ManipulationState _currentState = ManipulationState.unselected;

    public OutlineManipulator outlineManipulator;
    [Range(0.0f, 0.03f)]
    public float maxOutlineWidth = 0.01f;

    public float selectionTime = 0.5f;
    protected float _selectionTimer = 0.0f;
    protected PlayerControls _selector;

    public float followFactor = 1.0f;

    protected void Start()
    {
        // gather dependencies
        if (outlineManipulator == null)
        {
            outlineManipulator = GetComponentInChildren<OutlineManipulator>();
        }
    }

    protected void Update()
    {
        // create state update handlers
        switch (_currentState)
        {
            case ManipulationState.unselected:
                UpdateUnselected();
                break;
            case ManipulationState.selecting:
                UpdateSelecting();
                break;
            case ManipulationState.selected:
                UpdateSelected();
                break;
        }
    }


    protected void UpdateUnselected()
    {
        // deselects and reduces outline effect
        if (_selectionTimer > 0.0f)
        {
            _selectionTimer -= Time.deltaTime;
            UpdateSelectionOutline();
        }
    }

    protected void UpdateSelecting()
    {
        // selects the object and increases outline effect
        if (_selectionTimer < selectionTime)
        {
            _selectionTimer += Time.deltaTime;
            UpdateSelectionOutline();
            if (_selectionTimer > selectionTime)
            {
                _selectionTimer = selectionTime;
                SwitchState(ManipulationState.selected);
            }
        }
    }

    protected void UpdateSelected()
    {
        // moves the object towards the specified focus point using a simple lerp effect for smooth, predictable motion
        transform.position = Vector3.Lerp(transform.position, _selector.objectFocusPoint.position, followFactor*Time.deltaTime);
    }

    public void SwitchState(ManipulationState newState)
    {
        // handle state transitions
        switch (newState)
        {
            case ManipulationState.selected:
                rigidbody.useGravity = false;
                PulseOutline();
                break;
        }

        _currentState = newState;
    }

    protected void UpdateSelectionOutline()
    {
        UpdateOutline(Mathf.Clamp01(_selectionTimer / selectionTime));
    }

    protected LTDescr PulseOutline(float pulseTime = 1f)
    {
        return LeanTween.value(gameObject, UpdateOutline, 1.1f, 0.5f, pulseTime).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
    }

    protected void UpdateOutline(float percentage)
    {
        outlineManipulator.SetOutlineWidth(maxOutlineWidth * percentage);
        outlineManipulator.SetOutlineColor(Color.Lerp(Color.blue, Color.green, percentage));
    }

    #region ISelectable Methods
    public bool IsSelected()
    {
        return _currentState >= ManipulationState.selected;
    }

    public void Select(PlayerControls selector)
    {
        if (_currentState == ManipulationState.unselected && _selectionTimer <= 0.0f)
        {
            _selector = selector;
            SwitchState(ManipulationState.selecting);
        }
    }

    public void Deselect()
    {
        LeanTween.cancel(gameObject);
        rigidbody.useGravity = true;
        rigidbody.velocity = Vector3.zero;
        _selectionTimer *= 0.5f;
        SwitchState(ManipulationState.unselected);
    }
    #endregion
}
