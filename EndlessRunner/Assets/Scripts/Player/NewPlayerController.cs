using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewPlayerController : MonoBehaviour
{
    [Header("AnimationRelated")]
    protected Animator animator;
    protected float AnimatorMoveSpeed = 0f;

    [Header("Speeds")]
    const float MaxSpeed = 30f;
    [Tooltip("Velocity magnitude")]
    public float TargetMoveSpeed 
    { 
        get { return _targetMoveSpeed; } 
        set { _targetMoveSpeed = value; } 
    }
    protected float _targetMoveSpeed;
    public Feedbacks SpeedupFeedbacks;
    public Feedbacks SpeeddownFeedbacks;
    public float RotationRate 
    {
        get; set;
    }

    public enum RotateType
    {
        FreeRotate,
        FiveDirections,
        SevenDirections,
        TwelveDirections
    }
    [Header("Rotation Related")]
    [SerializeField] protected RotateType RotationType = RotateType.FreeRotate;
    protected const float RotateCoolDown = 0.12f;
    protected const float WrongTurnCoolDown = 0.3f;
    protected int RotationCDFrameBased = 30;
    protected int TurnFrameCount = 0;
    protected float BeforeNextTurnInput = 0.9f;
    protected float RotateTimer = 0f;
    protected bool CanRotate = true;
    protected bool CanChangeSpeed = false;
    [SerializeField] protected float RotationSpeed = 0f;
    [Tooltip("0 to 5 recommended")]
    [Range(0f, 10f)] public float NoiseLevel;
    public float RandomHeadingChance { get; set; }
    public bool CanTurnWrongTwiceInARow = false;
    [Tooltip("If Last Turn Was A Wrong Turn, don't Turn Wrong this time")]
    protected bool bLastTurnWasWrong = false;
    protected float RotateDeg = 0f;
    protected float CurrRotateDeg = 0f;
    protected float PrevDeg = 0f;
    protected float _timeLastTurn;
    protected float _selfAdjustTime = 3f;
    protected float[] _straightDirections = new float[] { 0f, 90f, 180f, -90f, -180f };

    public float CurrMoveSpeed
    {
        get { return _currMoveSpeed; }
        set
        {
            if (_currMoveSpeed != value)
            {
                SpeedDisplay.Current.UpdateDisplayValue(value);
                _currMoveSpeed = value;
            }
        }
    }
    protected float _currMoveSpeed = 0f;
    protected Coroutine _lowerSpeedCoroutine;
    protected float SpeedBeforePause = 0f;
    [SerializeField] protected ScoreDisplay scoreDisplay;
    public bool CanSelfPace = false;
    protected int PaceLevels = 7;
    protected float OneSpeedLevel = 0f;

    [Header("Side Walk Related")]
    public Feedbacks OnSideWalkFeedback;
    protected Coroutine StopSideWalkCoroutine = null;
    protected int enterCount = 0;

    [Header("StateRelated")]
    protected MovementState _currState = MovementState.WalkForward;
    public MovementState CurrentState
    {
        get { return _currState; }
        set
        {
            if (value != _currState)
            {
                _currState = value;
                switch (_currState)
                {
                    case MovementState.WalkForward:
                        OnEnterWalkState();
                        break;
                    case MovementState.Stumble:
                        OnEnterStumbleState();
                        break;
                    case MovementState.LeftTurn:
                        bool MakeAWrongTurn = false;
                        _timeLastTurn = Time.time;
                        float randVal = Random.Range(0f, 100f);
                        if (randVal < RandomHeadingChance) MakeAWrongTurn = true;
                        TurningsPressed++; // for metrics
                        if ((!CanTurnWrongTwiceInARow && bLastTurnWasWrong) || !MakeAWrongTurn)
                        {
                            bLastTurnWasWrong = false;
                            OnEnterLeftTurn();
                        }
                        else CurrentState = MovementState.WrongRight;
                        break;
                    case MovementState.RightTurn:
                        MakeAWrongTurn = false;
                        _timeLastTurn = Time.time;
                        randVal = Random.Range(0f, 100f);
                        if (randVal < RandomHeadingChance) MakeAWrongTurn = true;
                        TurningsPressed++; // for metrics
                        if ((!CanTurnWrongTwiceInARow && bLastTurnWasWrong) || !MakeAWrongTurn)
                        {
                            bLastTurnWasWrong = false;
                            OnEnterRightTurn();
                        }
                        else CurrentState = MovementState.WrongLeft;
                        break;
                    case MovementState.WrongLeft:
                        RandomHeadingsOccurred++; // for metrics
                        bLastTurnWasWrong = true;
                        OnEnterWrongLeft();
                        break;
                    case MovementState.WrongRight:
                        RandomHeadingsOccurred++; // for metrics
                        bLastTurnWasWrong = true;
                        OnEnterWrongRight();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public enum MovementState
    {
        WalkForward,
        Stumble,
        LeftTurn,
        RightTurn,
        WrongLeft,
        WrongRight
    }

    [Header("Events")]
    public UnityAction OnStumble;

    [Header("Other Utilities")]
    protected Rigidbody _rb;
    protected float timer = 0f;

    private float _timeBeginSideWalk = 0f;

    /// "for METRICS purposes"
    #region FOR_METRICS
    public int TurningsPressed { get; set; }
    public int RandomHeadingsOccurred { get; set; }
    public float DistanceTraveled { get; set; }
    public int MaxMoveSpeed 
    {
        get
        {
            if (Speeds == null || Speeds.Count == 0) return 0;
            else
            {
                Speeds.Sort();
                return Speeds[Speeds.Count - 1];
            }
        }
    }
    public int MinMoveSpeed 
    {
        get
        {
            if (Speeds == null || Speeds.Count == 0) return 0;
            else
            {
                Speeds.Sort();
                return Speeds[0];
            }
        }
    }
    public float AvgMoveSpeed
    {
        get { return (float)(sum / count); }
    }
    int count = 0;
    double sum = 0d;
    public int MedianMoveSpeed
    {
        get
        {
            if (Speeds == null || Speeds.Count == 0) return 0;
            else
            {
                Speeds.Sort();
                if (Speeds.Count%2 == 0)
                {
                    return Mathf.FloorToInt((Speeds[((Speeds.Count - 1) / 2)] + Speeds[((Speeds.Count - 1) / 2 + 1)]) / 2f);
                }
                else
                {
                    return Speeds[(Speeds.Count - 1) / 2];
                }
            }
        }
    }
    public List<int> Speeds = new List<int>();

    public float AverageRandomHeadingChances
    {
        get
        {
            RandomHeadingChances.Sort();
            float sum = 0f;
            foreach(float num in RandomHeadingChances)
            {
                sum += num;
            }
            return sum / RandomHeadingChances.Count;
        }
    }
    public List<float> RandomHeadingChances = new List<float>();
    public int NumSpeedChanges { get; set; }
    #endregion

    private void OnEnable()
    {
        PlayerManager.OnGameStarted += OnStartGame;
        PlayerManager.OnPauseGame += OnPauseGame;
        PlayerManager.OnResumeGame += OnResumeGame;
    }

    private void OnDisable()
    {
        PlayerManager.OnGameStarted -= OnStartGame;
        PlayerManager.OnPauseGame -= OnPauseGame;
        PlayerManager.OnResumeGame -= OnResumeGame;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (scoreDisplay == null)
        {
            scoreDisplay = FindObjectOfType<ScoreDisplay>();
            if (scoreDisplay == null) Debug.LogWarning("Missing a <ScoreDisplay> class");
        }
        animator = GetComponentInChildren<Animator>();
        CurrentState = MovementState.WalkForward;
        TurningsPressed = 0;
        RandomHeadingsOccurred = 0;
        DistanceTraveled = 0f;
        NumSpeedChanges = 0;
        Speeds = new List<int>();
        //Input.gyro.enabled = true;
    }

    private void Update()
    {
        if (!PlayerManager.isGameStarted || PlayerManager.isGamePaused || PlayerManager.isGameOver)
        {
            return;
        }

        CurrMoveSpeed = Mathf.Lerp(CurrMoveSpeed, TargetMoveSpeed, 3f * Time.deltaTime);
        AnimatorMoveSpeed = CurrMoveSpeed / MaxSpeed;

        if (CurrentState != MovementState.Stumble)
        {
            if (CanRotate)
            {
                if (LeftInput() && !RightInput())
                {
                    CurrentState = MovementState.LeftTurn;
                }
                else if (RightInput() && !LeftInput())
                {
                    CurrentState = MovementState.RightTurn;
                }
                else if (!GUIManager.Current.DirectionButtonPressed)
                {
                    CurrentState = MovementState.WalkForward;
                }
                SelfAjust();
            }

            if (CanSelfPace && SpeedSlider.CanChangeSpeed)
            {
                if (UpInput() && !DownInput())
                {
                    // InCrease Speed
                    float oldTargetSpeed = TargetMoveSpeed;
                    TargetMoveSpeed = Mathf.Min(TargetMoveSpeed + OneSpeedLevel, Loader.MaxSpeed);
                    SpeedSlider.Current.UpdateSliderValue(TargetMoveSpeed);
                    CalculatedWeightedChanceWrongHeading();
                    if (oldTargetSpeed != TargetMoveSpeed && SpeedupFeedbacks) SpeedupFeedbacks.PlayFeedbacks();

                    // for metrics
                    NumSpeedChanges++;
                    Speeds.Add(Mathf.FloorToInt(TargetMoveSpeed));
                }
                else if (DownInput() && !UpInput())
                {
                    // Decrease Speed
                    float oldTargetSpeed = TargetMoveSpeed;
                    TargetMoveSpeed = Mathf.Max(TargetMoveSpeed - OneSpeedLevel, Loader.MinSpeed);
                    SpeedSlider.Current.UpdateSliderValue(TargetMoveSpeed);
                    CalculatedWeightedChanceWrongHeading();
                    if (oldTargetSpeed != TargetMoveSpeed && SpeeddownFeedbacks) SpeeddownFeedbacks.PlayFeedbacks();

                    // for metrics
                    NumSpeedChanges++;
                    Speeds.Add(Mathf.FloorToInt(TargetMoveSpeed));
                }
            }
        }
        else if (RotationType == RotateType.FreeRotate)
        {
            RotateDeg = 0f;
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (!PlayerManager.isGameStarted || PlayerManager.isGamePaused || PlayerManager.isGameOver)
        {
            return;
        }

        _rb.velocity = transform.forward * CurrMoveSpeed;
        DistanceTraveled += _rb.velocity.magnitude * Time.fixedDeltaTime;
        Vector3 eulerAng = _rb.rotation.eulerAngles;
        if (RotationType == RotateType.FreeRotate)
        {
            _rb.MoveRotation(Quaternion.Euler(eulerAng.x, eulerAng.y + Time.fixedDeltaTime * RotateDeg, eulerAng.z));
        }
        else
        {
            CurrRotateDeg = Mathf.LerpAngle(PrevDeg, RotateDeg, RotateTimer / (RotateCoolDown * 0.8f));
            _rb.MoveRotation(Quaternion.Euler(eulerAng.x, CurrRotateDeg, eulerAng.z));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            Debug.Log("Trip");
            AudioManager.Current.PlaySound("Trip"); /// stumble = trip
            CurrentState = MovementState.Stumble;
        }
        else if (other.transform.CompareTag("SideWalk"))
        {
            enterCount++;
            StopCoroutine(StopOnSideWalkCoroutine());
            if (enterCount == 1 && OnSideWalkFeedback)
            {
                _timeBeginSideWalk = Time.time;
                OnSideWalkFeedback.PlayFeedbacks();
                scoreDisplay.InWarningState();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("SideWalk"))
        {
            StartCoroutine(StopOnSideWalkCoroutine());
        }
    }

    // self adjust to walk in straight line
    protected void SelfAjust()
    {
        if (Time.time - _timeLastTurn > _selfAdjustTime)
        {
            foreach (float targetAng in _straightDirections)
            {
                if (targetAng == RotateDeg) return;
            }
            foreach (float targetAng in _straightDirections)
            {
                float difference = targetAng - RotateDeg;
                if (Mathf.Abs(difference) <= 15f)
                {
                    if (difference > 0f) OnEnterRightTurn();
                    else if (difference < 0f) OnEnterLeftTurn();
                    _timeLastTurn = Time.time;
                    break;
                }
            }
        }
    }

    protected void UpdateAnimation()
    {
        animator.SetFloat("MoveSpeed", AnimatorMoveSpeed);
    }

    #region Referenced By Animator Controller
    public void LowerSpeed()
    {
        if (_lowerSpeedCoroutine != null)
        {
            StopCoroutine(_lowerSpeedCoroutine);
        }
        _lowerSpeedCoroutine = StartCoroutine(LowerSpeedCoroutine());
    }

    public void ResetMoveSpeed()
    {
        if (_lowerSpeedCoroutine != null)
        {
            StopCoroutine(_lowerSpeedCoroutine);
            _lowerSpeedCoroutine = null;
        }
    }

    #endregion

    #region Movement Related
    public void StopMovement()
    {
        CurrMoveSpeed = 0f;
        TargetMoveSpeed = 0f;
        _rb.velocity = Vector3.zero;
        AnimatorMoveSpeed = CurrMoveSpeed / MaxSpeed;
        UpdateAnimation();
    }

    protected IEnumerator LowerSpeedCoroutine()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            CurrMoveSpeed = Mathf.Lerp(CurrMoveSpeed, 0f, 10f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Determines the amount of speed to change each time in self-paced levels
    /// </summary>
    public void SetOneSpeedLevel()
    {
        OneSpeedLevel = (Loader.MaxSpeed - Loader.MinSpeed) / PaceLevels;
    }

    #endregion
    protected IEnumerator BalanceSpeedCoroutine()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            CurrMoveSpeed = TargetMoveSpeed;
        }
    }

    public void OnStartGame()
    {
        StartCoroutine(TrackAverageSpeed());
    }

    public void OnPauseGame()
    {
        Time.timeScale = 0; //pauses the game?
        SpeedBeforePause = CurrMoveSpeed;
        AudioManager.Current.PauseSounds();
    }

    public void OnResumeGame()
    {
        Time.timeScale = 1; //pauses the game?
        CurrMoveSpeed = SpeedBeforePause;
        AudioManager.Current.ResumeSounds();
    }

    #region StateTransitions
    void OnEnterWalkState()
    {
        animator.SetTrigger("WalkForward");
        if (RotationType == RotateType.FreeRotate) RotateDeg = 0f;
    }

    void OnEnterStumbleState()
    {
        animator.SetTrigger("Stumble");
        if (OnStumble != null) OnStumble();
    }

    public void ExitStumble()
    {
        animator.ResetTrigger("Stumble");
        CurrentState = MovementState.WalkForward;
    }

    void OnEnterLeftTurn()
    {
        if (RotationType == RotateType.FiveDirections) RotateDeg -= 45f;
        else if (RotationType == RotateType.SevenDirections) RotateDeg -= 30f;
        else if (RotationType == RotateType.FreeRotate) RotateDeg = -RotationSpeed;
        else if (RotationType == RotateType.TwelveDirections) RotateDeg -= 15f;

        if (RotationType != RotateType.FreeRotate)
        {
            //animator.SetTrigger("LeftSideWalk");
            StartCoroutine(CountDownCD(RotateCoolDown));
        }
    }

    void OnEnterRightTurn()
    {
        if (RotationType == RotateType.FiveDirections) RotateDeg += 45f;
        else if (RotationType == RotateType.SevenDirections) RotateDeg += 30f;
        else if (RotationType == RotateType.FreeRotate) RotateDeg = RotationSpeed;
        else if (RotationType == RotateType.TwelveDirections) RotateDeg += 15f;

        if (RotationType != RotateType.FreeRotate)
        {
            //animator.SetTrigger("RightSideWalk");
            StartCoroutine(CountDownCD(RotateCoolDown));
        }
    }

    void OnEnterWrongLeft()
    {
        if (RotationType == RotateType.FiveDirections) RotateDeg -= 45f;
        else if (RotationType == RotateType.SevenDirections) RotateDeg -= 30f;
        else if (RotationType == RotateType.TwelveDirections) RotateDeg -= 15f;

        StartCoroutine(CountDownCD(WrongTurnCoolDown));

        animator.SetTrigger("WrongLeftTurn");
        AudioManager.Current.PlaySound("WrongTurn");
    }

    void OnEnterWrongRight()
    {
        if (RotationType == RotateType.FiveDirections) RotateDeg += 45f;
        else if (RotationType == RotateType.SevenDirections) RotateDeg += 30f;
        else if (RotationType == RotateType.TwelveDirections) RotateDeg += 15f;

        StartCoroutine(CountDownCD(WrongTurnCoolDown));

        animator.SetTrigger("WrongRightTurn");
        AudioManager.Current.PlaySound("WrongTurn");
    }

    #endregion

    #region Inputs
    protected bool LeftInput()
    {
        /*if (RotationType == RotateType.FreeRotate) */return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        //return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
    }

    protected bool RightInput()
    {
        /*if (RotationType == RotateType.FreeRotate) */return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        //return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
    }

    // Disable Phone Tilting
    //private int GyroInput()
    //{
    //    Vector3 rot = Input.gyro.rotationRate;
    //    if(rot.x > RotationRate)
    //    {
    //        return -1;
    //    }else if (rot.x < -RotationRate)
    //    {
    //        return 1;
    //    }else
    //    {
    //        return 0;
    //    }
    //}

    protected bool UpInput()
    {
        return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || SwipeManager.swipeUp;
    }

    protected bool DownInput()
    {
        return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || SwipeManager.swipeDown;
    }

    public void TurnLeft()
    {
        if (CanRotate && CurrentState != MovementState.Stumble)
        {
            CurrentState = MovementState.LeftTurn;
        }
    }

    public void TurnRight()
    {
        if (CanRotate && CurrentState != MovementState.Stumble)
        {
            CurrentState = MovementState.RightTurn;
        }
    }
    #endregion

    protected IEnumerator CountDownCD(float CD)
    {
        CanRotate = false;
        while (RotateTimer < CD)
        {
            yield return new WaitForFixedUpdate();
            RotateTimer += Time.fixedDeltaTime;
        }
        PrevDeg = RotateDeg;
        RotateTimer = 0f;
        CanRotate = true;
        CurrentState = MovementState.WalkForward;
        if (CD == RotateCoolDown)
        {
            animator.ResetTrigger("LeftSideWalk");
            animator.ResetTrigger("RightSideWalk");
        }
        else if (CD == WrongTurnCoolDown)
        {
            animator.ResetTrigger("WrongLeftTurn");
            animator.ResetTrigger("WrongRightTurn");
        }
    }

    protected IEnumerator StopOnSideWalkCoroutine()
    {
        yield return new WaitForSeconds(0.6f);
        enterCount--;
        if (OnSideWalkFeedback && enterCount == 0)
        {
            OnSideWalkFeedback.StopFeedbacks();
            scoreDisplay.ResetWarningColor();
            scoreDisplay.AddToOffPathTimer(Time.time - _timeBeginSideWalk);
        }
    }

    public static void CalculatedWeightedChanceWrongHeading()
    {
        NewPlayerController controller = FindObjectOfType<NewPlayerController>();
        controller.RandomHeadingChance = controller.NoiseLevel + controller.NoiseLevel * (controller.TargetMoveSpeed - Loader.MinSpeed) / (Loader.MaxSpeed - Loader.MinSpeed);
    }

    protected IEnumerator TrackAverageSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (PlayerManager.isGameStarted && !PlayerManager.isGamePaused)
            {
                count++;
                sum += CurrMoveSpeed;
            }
        }
    }
}
