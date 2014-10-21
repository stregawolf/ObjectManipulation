using UnityEngine;
using System.Collections.Generic;

public enum HeadGesture
{
    none,
    shake,
    nod,
}

/// <summary>
/// HeadGestureRecognizer is a singleton responsible for storing head motion data and detecting head gestures
/// </summary>
[RequireComponent(typeof(PlayerControls))]
public class HeadGestureRecognizer : Singleton<HeadGestureRecognizer>
{
    protected const int MaxMotionHistory = 10;

    public PlayerControls playerControls;

    public float motionDeltaThreshold = 1.0f;

    protected List<MotionData> _motionHistory = new List<MotionData>(MaxMotionHistory);
    protected MotionData _currentData;

    public float gestureDurationThreshold = 0.25f;
    protected List<KeyValuePair<HeadGesture, List<MotionData>>> _headGesturePatterns = new List<KeyValuePair<HeadGesture, List<MotionData>>>();
    protected HeadGesture _lastGesture = HeadGesture.none;
    protected HeadGesture _currentGesture = HeadGesture.none;

    public bool displayDebugInfo = false;

    protected void Awake()
    {
        // Setup head gesture patterns
        List<MotionData> shakePatternLeft = new List<MotionData>();
        shakePatternLeft.Add(new MotionData(MotionDirection.left, gestureDurationThreshold));
        shakePatternLeft.Add(new MotionData(MotionDirection.none, gestureDurationThreshold));
        shakePatternLeft.Add(new MotionData(MotionDirection.right, gestureDurationThreshold));
        shakePatternLeft.Add(new MotionData(MotionDirection.none, gestureDurationThreshold));
        shakePatternLeft.Add(new MotionData(MotionDirection.left, gestureDurationThreshold));
        _headGesturePatterns.Add(new KeyValuePair<HeadGesture, List<MotionData>>(HeadGesture.shake, shakePatternLeft));

        List<MotionData> shakePatternRight = new List<MotionData>();
        shakePatternRight.Add(new MotionData(MotionDirection.right, gestureDurationThreshold));
        shakePatternRight.Add(new MotionData(MotionDirection.none, gestureDurationThreshold));
        shakePatternRight.Add(new MotionData(MotionDirection.left, gestureDurationThreshold));
        shakePatternRight.Add(new MotionData(MotionDirection.none, gestureDurationThreshold));
        shakePatternRight.Add(new MotionData(MotionDirection.right, gestureDurationThreshold));
        _headGesturePatterns.Add(new KeyValuePair<HeadGesture, List<MotionData>>(HeadGesture.shake, shakePatternRight));
        
        List<MotionData> nodPatternUp = new List<MotionData>();
        nodPatternUp.Add(new MotionData(MotionDirection.up, gestureDurationThreshold));
        nodPatternUp.Add(new MotionData(MotionDirection.none, gestureDurationThreshold));
        nodPatternUp.Add(new MotionData(MotionDirection.down, gestureDurationThreshold));
        nodPatternUp.Add(new MotionData(MotionDirection.none, gestureDurationThreshold));
        nodPatternUp.Add(new MotionData(MotionDirection.up, gestureDurationThreshold));
        _headGesturePatterns.Add(new KeyValuePair<HeadGesture, List<MotionData>>(HeadGesture.nod, nodPatternUp));
        /*
        List<MotionData> nodPatternDown = new List<MotionData>();
        nodPatternDown.Add(new MotionData(MotionDirection.down, gestureDurationThreshold));
        nodPatternDown.Add(new MotionData(MotionDirection.none, gestureDurationThreshold));
        nodPatternDown.Add(new MotionData(MotionDirection.up, gestureDurationThreshold));
        nodPatternDown.Add(new MotionData(MotionDirection.none, gestureDurationThreshold));
        nodPatternDown.Add(new MotionData(MotionDirection.down, gestureDurationThreshold));
        _headGesturePatterns.Add(new KeyValuePair<HeadGesture, List<MotionData>>(HeadGesture.nod, nodPatternDown));
         */
    }

    protected void Start()
    {
        // gather dependencies
        if (playerControls == null)
        {
            playerControls = GetComponentInChildren<PlayerControls>();
        }

        // initialize data
        _currentData = new MotionData();
        _motionHistory.Add(_currentData);
    }

    protected void Update()
    {
        _lastGesture = _currentGesture;

        _currentGesture = HeadGesture.none;
        _currentData.duration += Time.deltaTime;

        MotionDirection currentDirection = MotionDirection.none;

        // Detect dominate movement direction this frame
        float absLeftRight = Mathf.Abs(playerControls.deltaLeftRightRotation);
        float absUpDown = Mathf.Abs(playerControls.deltaUpDownRotation);
        if (absLeftRight > motionDeltaThreshold || absUpDown > motionDeltaThreshold)
        {
            if (absLeftRight > absUpDown)
            {
                if (playerControls.deltaLeftRightRotation > 0)
                {
                    currentDirection = MotionDirection.right;
                }
                else if (playerControls.deltaLeftRightRotation < 0)
                {
                    currentDirection = MotionDirection.left;
                }
            }
            else if (absUpDown > absLeftRight)
            {
                if (playerControls.deltaUpDownRotation > 0)
                {
                    currentDirection = MotionDirection.down;
                }
                else if (playerControls.deltaUpDownRotation < 0)
                {
                    currentDirection = MotionDirection.up;
                }
            }
        }

        if (currentDirection != _currentData.direction)
        {
            // store motion data
            if (_motionHistory.Count == MaxMotionHistory)
            {
                _motionHistory.RemoveAt(0);
            }
            _currentData = new MotionData(currentDirection);
            _motionHistory.Add(_currentData);

            // look for any recognized gestures
            _currentGesture = DetectHeadGestures();
        }
    }

    public bool JustGestured(HeadGesture gesture)
    {
        return _currentGesture == gesture && _lastGesture != gesture;
    }

    protected HeadGesture DetectHeadGestures()
    {
        foreach (KeyValuePair<HeadGesture, List<MotionData>> pattern in _headGesturePatterns)
        {
            int patternIndex = 0;
            int patternSize = pattern.Value.Count;
            int motionIndex = _motionHistory.Count - 1;
            bool patternMatches = true;

            // attempt to match known patterns with motion history
            while (motionIndex >= 0 && patternIndex < patternSize)
            {
                MotionData motionData = _motionHistory[motionIndex];
                MotionData patternData = pattern.Value[patternIndex];

                if (motionData.direction != patternData.direction || motionData.duration > patternData.duration)
                {
                    patternMatches = false;
                    break;
                }

                motionIndex--;
                patternIndex++;
            }

            if (patternMatches)
            {
                return pattern.Key;
            }
        }
        return HeadGesture.none;
    }

    protected void OnGUI()
    {
        if (displayDebugInfo)
        {
            for (int i = 0, n = _motionHistory.Count; i < n; ++i)
            {
                GUI.Label(new Rect(10, 10 + i * 10, 500, 20), string.Format("{0} - {1:N2}", _motionHistory[i].direction.ToString(), _motionHistory[i].duration));
            }
        }
    }
}
