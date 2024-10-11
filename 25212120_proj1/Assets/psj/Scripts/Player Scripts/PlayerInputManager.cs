using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private Animator playerAnimator;
    private PlayerMovement playerInput;

    public Vector2 moveInput;
    public bool leftShiftKey_Pressed = false;

    [Header("Player Action Handlers")]
    public bool isRolling = false;
    public bool isGrounded = true;
    public bool isJumping = false;

    private StateManager<PlayerStateType> stateManager;
    private Rigidbody rb;
    private Animator animator;
    private Transform playerTransform;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = new PlayerMovement();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();

        stateManager = GetComponent<StateManager<PlayerStateType>>();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.PlayerMove.Move.performed += OnMovePerformed;
        playerInput.PlayerMove.Move.canceled += OnMoveCanceled;

        playerInput.PlayerMove.Sprint.performed += OnSprintPerformed;
        playerInput.PlayerMove.Sprint.canceled += OnSprintCanceled;

        playerInput.PlayerAction.Dash.performed += OnDashPerformed;

        playerInput.PlayerAction.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.PlayerMove.Move.performed -= OnMovePerformed;
        playerInput.PlayerMove.Move.canceled -= OnMoveCanceled;

        playerInput.PlayerMove.Sprint.performed -= OnSprintPerformed;
        playerInput.PlayerMove.Sprint.canceled -= OnSprintCanceled;

        playerInput.PlayerAction.Dash.performed -= OnDashPerformed;

        playerInput.PlayerAction.Jump.performed -= OnJumpPerformed;
    }

    private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        playerAnimator.SetBool("moveInput", true);
    }

    private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        playerAnimator.SetBool("moveInput", false);
    }

    private void OnSprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        leftShiftKey_Pressed = true;
        playerAnimator.SetBool("leftShiftKey_Pressed", true);
    }

    private void OnSprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        leftShiftKey_Pressed = false;
        playerAnimator.SetBool("leftShiftKey_Pressed", false);
    }

    private void OnDashPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!isRolling && isGrounded && !isJumping)
        {
            isRolling = true;
            stateManager.PushState(PlayerStateType.Dash);
        }
    }

    public void SetIsRolling(bool value)
    {
        isRolling = value;
    }

    public void SetIsGrounded(bool value)
    {
        isGrounded = value;
    }

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!isJumping && !isRolling && isGrounded)
        {
            isJumping = true;
            stateManager.PushState(PlayerStateType.Jump);
        }
    }

    public void SetIsJumping(bool value)
    {
        isJumping = value;
    }
}
