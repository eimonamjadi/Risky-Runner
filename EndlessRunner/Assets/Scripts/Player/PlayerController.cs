using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Range(-1, 1)] private int desiredLane;
    [Tooltip("Left and Right Offset")]
    protected Vector3 CurrOffset = Vector3.zero;
    protected Vector3 TargetOffset = Vector3.zero;
    protected Vector3 OffsetLastFrame = Vector3.zero;
    protected Vector3 OffsetVelocity;
    [Header("AnimationRelated")]
    protected Animator animator;

    [Header("Speeds")]
    const float MaxSpeed = 30f;
    [Range(0, MaxSpeed)] public float TargetMoveSpeed;
    public float CurrMoveSpeed = 0f;
    public float AnimatorMoveSpeed = 0f;
    public float laneWidth = 2.8f;
    protected Vector3 PosLastFrame;
    protected Vector3 CartVelocity;
    public CinemachineDollyCart Cart;
    Vector3 TargetPosition = Vector3.zero;

    [Tooltip("JumpRelated")]
    public Transform _groundCheck;
    private bool _bIsGrounded = false;
    public bool IsGrounded { get { return _bIsGrounded; } }
    private float _groundDist = 0.8f;
    public LayerMask GroundLayer;
    public float JumpForce = 6f;
    private Coroutine _lowerSpeedCoroutine;
    protected bool bInvulnerable = false;
    protected float InvulnerableTime = 0.3f;

    protected Coroutine balanceSpeed;

    protected float SpeedBeforePause = 0f;

    [Header("StateRelated")]
    protected MovementState _prevState = MovementState.WalkForward;
    public MovementState PreviousState
    {
        get { return _prevState; }
    }
    protected MovementState _currState = MovementState.WalkForward;
    public MovementState CurrentState
    {
        get { return _currState; }
        set
        {
            if (value != _currState)
            {
                _prevState = _currState;
                _currState = value;
                switch (_prevState)
                {
                    case MovementState.LeftSideWalk:
                        OnExitLeftSideWalkState();
                        break;
                    case MovementState.RightSideWalk:
                        OnExitRightSideWalkState();
                        break;
                    case MovementState.Jump:
                        OnExitJumpState();
                        break;
                    default:
                        break;
                }
                switch (_currState)
                {
                    case MovementState.LeftSideWalk:
                        OnEnterLeftSideWalkState();
                        break;
                    case MovementState.RightSideWalk:
                        OnEnterRightSideWalkState();
                        break;
                    case MovementState.Jump:
                        OnEnterJumpState();
                        break;
                    case MovementState.WalkForward:
                        OnEnterWalkState();
                        break;
                    case MovementState.Stumble:
                        OnEnterStumbleState();
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
        LeftSideWalk,
        RightSideWalk,
        Jump,
        Stumble
    }

    public UnityAction OnStumble;

    private void OnEnable()
    {
        PlayerManager.OnPauseGame += OnPauseGame;
        PlayerManager.OnResumeGame += OnResumeGame;
    }

    private void OnDisable()
    {
        PlayerManager.OnPauseGame -= OnPauseGame;
        PlayerManager.OnResumeGame -= OnResumeGame;
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    	desiredLane = 0;
        TargetPosition = new Vector3(Cart.transform.position.x, transform.position.y, Cart.transform.position.z);
        CurrentState = MovementState.WalkForward;
        PosLastFrame = Cart.transform.position;
    }

    void Update()
    {
        if(!PlayerManager.isGameStarted || PlayerManager.isGamePaused || PlayerManager.isGameOver) {
            return;
        }
        else
        {
            if (_bIsGrounded && balanceSpeed == null) CurrMoveSpeed = Mathf.Lerp(CurrMoveSpeed, TargetMoveSpeed, 3f * Time.deltaTime);
            Cart.m_Speed = CurrMoveSpeed;
            AnimatorMoveSpeed = CurrMoveSpeed / MaxSpeed;
        }

        _bIsGrounded = Physics.CheckSphere(_groundCheck.position, _groundDist, GroundLayer);

        if (_bIsGrounded && (SwipeManager.swipeUp || Input.GetKeyDown(KeyCode.W)))
        {
            Jump();
        }
        
        /// This is a realistic game, you can't swip left and right unless you are on the ground
        if (_bIsGrounded && CurrentState != MovementState.Stumble)
        {
            if ((SwipeManager.swipeRight || Input.GetKeyDown(KeyCode.D)) && desiredLane < 1)
            {
                /// start moving to the right
                desiredLane++;
                CurrentState = MovementState.RightSideWalk;
                StopBalanceSpeedCoroutine();
                balanceSpeed = StartCoroutine(BalanceSpeedCoroutine());
            }

            else if ((SwipeManager.swipeLeft || Input.GetKeyDown(KeyCode.A)) && desiredLane > -1)
            {
                desiredLane--;
                CurrentState = MovementState.LeftSideWalk;
                StopBalanceSpeedCoroutine();
                balanceSpeed = StartCoroutine(BalanceSpeedCoroutine());
            }
        }

        UpdateAnimation();
    }

    private void FixedUpdate() {
        if(!PlayerManager.isGameStarted || PlayerManager.isGamePaused || PlayerManager.isGameOver) {
            return;
        }

        transform.rotation = Cart.transform.rotation;

        TargetPosition = new Vector3(Cart.transform.position.x, transform.position.y, Cart.transform.position.z);

        if (desiredLane == 0)
        {
            TargetOffset = Vector3.zero;
        }
        else
        {
            TargetOffset = desiredLane * transform.right * laneWidth;
        }

        OffsetLastFrame = CurrOffset;
        CurrOffset = Vector3.MoveTowards(CurrOffset, TargetOffset, TargetMoveSpeed * 0.2f * Time.fixedDeltaTime);
        OffsetVelocity = (CurrOffset - OffsetLastFrame) / Time.fixedDeltaTime;

        TargetPosition += CurrOffset;
        transform.position = TargetPosition;

        /// this means the target is reached
        if (CurrentState != MovementState.WalkForward && Vector3.Distance(CurrOffset, TargetOffset) < 0.8f)
        {
            CurrentState = MovementState.WalkForward;
        }
    }

    private void Jump() {
        if (CurrentState != MovementState.Stumble)
        {
            CurrentState = MovementState.Jump;
            GetComponent<Rigidbody>().AddForce(transform.up * JumpForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!bInvulnerable && other.transform.CompareTag("Obstacle"))
        {
            Debug.Log("Trip");
            AudioManager.Current.PlaySound("Trip"); /// stumble = trip
            CurrentState = MovementState.Stumble;
        }
    }

    private void UpdateAnimation() {
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

    public void ExitStumble()
    {
        animator.ResetTrigger("Stumble");
        if (_prevState == MovementState.LeftSideWalk) CurrentState = MovementState.LeftSideWalk;
        else if (_prevState == MovementState.RightSideWalk) CurrentState = MovementState.RightSideWalk;
        else CurrentState = MovementState.WalkForward;
    }

#endregion
    public void StopMovement()
    {
        CurrMoveSpeed = 0f;
        TargetMoveSpeed = 0f;
        Cart.m_Speed = CurrMoveSpeed;
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

    protected IEnumerator BalanceSpeedCoroutine()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            CurrMoveSpeed = TargetMoveSpeed - OffsetVelocity.magnitude;
        }
    }

    protected void StopBalanceSpeedCoroutine()
    {
        if (balanceSpeed != null) StopCoroutine(balanceSpeed);
        balanceSpeed = null;
    }

    protected IEnumerator SetInvulnerable()
    {
        float startTime = Time.time;
        bInvulnerable = true;
        while(Time.time - startTime <= InvulnerableTime)
        {
            yield return new WaitForEndOfFrame();
        }
        bInvulnerable = false;
    }

    public void OnPauseGame()
    {
        Time.timeScale = 0; //pauses the game?
        SpeedBeforePause = CurrMoveSpeed;
        Cart.m_Speed = CurrMoveSpeed;
        AudioManager.Current.PauseSounds();
    }

    public void OnResumeGame()
    {
        Time.timeScale = 1; //pauses the game?
        CurrMoveSpeed = SpeedBeforePause;
        Cart.m_Speed = CurrMoveSpeed;
        AudioManager.Current.ResumeSounds();
    }

#region StateTransitions
    void OnEnterLeftSideWalkState()
    {
        animator.SetTrigger("LeftSideWalk");
        animator.ResetTrigger("WalkForward");
    }

    void OnEnterRightSideWalkState()
    {
        animator.SetTrigger("RightSideWalk");
        animator.ResetTrigger("WalkForward");
    }

    void OnEnterWalkState()
    {
        animator.SetTrigger("WalkForward");
    }

    void OnExitLeftSideWalkState()
    {
        animator.ResetTrigger("LeftSideWalk");
        animator.SetTrigger("WalkForward");
    }

    void OnExitRightSideWalkState()
    {
        animator.ResetTrigger("RightSideWalk");
        animator.SetTrigger("WalkForward");
    }

    void OnEnterJumpState()
    {
        animator.SetTrigger("Jump");
    }

    void OnExitJumpState()
    {
        if (_prevState == MovementState.LeftSideWalk) animator.SetTrigger("LeftSideWalk");
        else if (_prevState == MovementState.RightSideWalk) animator.SetTrigger("RightSideWalk");
    }

    void OnEnterStumbleState()
    {
        animator.SetTrigger("Stumble");
        StartCoroutine(SetInvulnerable());
        if (OnStumble != null) OnStumble();
    }
#endregion
}




