using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    // ChangeState인 경우 (  )Key_Pressed 변수를 설정하고 is(   )는 State 스크립트 내부적으로 변경
    [Header("Player Movement Inputs")]
    public Vector2 moveInput;
    public bool leftShiftKey_Pressed = false;

    [Header("Player Action Inputs")]
    public bool leftButton_Pressed = false;
    public bool F_Key_Pressed = false;

    // PushState인 경우 is(    )만 만들고 Stata 스크립트 내부적으로 변경
    [Header("Player Action Handlers")]
    public bool isDashing = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool isAttacking = false;
    public bool isInteracting = false;
    // 이후 isPerformingAction으로 묶어버릴 생각임


    private Rigidbody rb;
    private Animator animator;
    private Transform playerTransform;
    private PlayerMovement playerInput;
    private PlayerCoolDownManager playerCoolDown;

    private StateManager<PlayerStateType> stateManager;

    private void Awake()
    {
        playerInput = new PlayerMovement();

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerTransform = GetComponent<Transform>();
        playerCoolDown = GetComponent<PlayerCoolDownManager>();

        stateManager = GetComponent<StateManager<PlayerStateType>>();
    }

    private void Update()
    {
        leftButton_Pressed = false;
        F_Key_Pressed = false;
        Debug.Log(isAttacking);
    }
    private void FixedUpdate()
    {
        GroundCheck();
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

        playerInput.PlayerAction.Attack.performed += OnAttackPerformed;

        playerInput.PlayerAction.Interaction.performed += OnInteractionPerformed;
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

        playerInput.PlayerAction.Attack.performed -= OnAttackPerformed;

        playerInput.PlayerAction.Interaction.performed -= OnInteractionPerformed;
    }
    private void GroundCheck()
    {
        RaycastHit hit;
        float rayDistance = 0.2f;
        Vector3 origin = playerTransform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayDistance))
        {
            // 땅에 닿음
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                if (isGrounded == false)
                {
                    SetIsGrounded(true);
                    animator.SetBool("isInAir", false);
                }
            }
        }
        else
        {
            if (isGrounded)
            {
                SetIsGrounded(false);
                animator.SetBool("isInAir", true);
            }
        }
    }
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        animator.SetBool("moveInput", true);
    }
    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        animator.SetBool("moveInput", false);
    }
    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        leftShiftKey_Pressed = true;
        animator.SetBool("leftShiftKey_Pressed", true);
    }
    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        leftShiftKey_Pressed = false;
        animator.SetBool("leftShiftKey_Pressed", false);
    }
    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        if (!isDashing && playerCoolDown.CanDash() && isGrounded && !isJumping && !isAttacking && !isInteracting)
        {
            stateManager.PushState(PlayerStateType.Dash);
        }
    } 
    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (!isJumping && !isDashing && isGrounded && !isAttacking && !isInteracting)
        {
            stateManager.PushState(PlayerStateType.Jump);
        }
    }
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (!isJumping && !isDashing && isGrounded && !isInteracting)
        {
            leftButton_Pressed = true;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (isAttacking == false)
            {
                stateManager.PushState(PlayerStateType.Attack);
                animator.SetTrigger("leftButton_Pressed");
            }
            else
            {
                if (stateInfo.IsName("Combo01_SwordShield")) animator.SetTrigger("NextCombo");
                else if (stateInfo.IsName("Combo02_SwordShield")) animator.SetTrigger("NextCombo");
            }
        }
    }
    private void OnInteractionPerformed(InputAction.CallbackContext ctx)
    {
        if (!isDashing && isGrounded && !isJumping && !isAttacking && !isInteracting)
        {
            stateManager.PushState(PlayerStateType.Interaction);
        }
    }
    public void SetIsDashing(bool value)
    {
        isDashing = value;
    }
    public void SetIsJumping(bool value)
    {
        isJumping = value;
    }
    public void SetIsGrounded(bool value)
    {
        isGrounded = value;
    }
    public void SetIsInteracting(bool value)
    {
        isInteracting = value;
    }
}
