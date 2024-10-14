using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    // ChangeState�� ��� (  )Key_Pressed ������ �����ϰ� is(   )�� State ��ũ��Ʈ ���������� ����
    [Header("Player Movement Inputs")]
    public Vector2 moveInput;
    public bool leftShiftKey_Pressed = false;

    [Header("Player Action Inputs")]
    public bool leftButton_Pressed = false;

    // PushState�� ��� is(    )�� ����� Stata ��ũ��Ʈ ���������� ����
    [Header("Player Action Handlers")]
    public bool isDashing = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool isAttacking = false;
    // ���� isPerformingAction���� ������� ������


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
        Debug.Log(isAttacking);
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
        if (!isDashing && playerCoolDown.CanDash() && isGrounded && !isJumping && !isAttacking)
        {
            stateManager.PushState(PlayerStateType.Dash);
        }
    }
    public void SetIsDashing(bool value)
    {
        isDashing = value;
    }
    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (!isJumping && !isDashing && isGrounded && !isAttacking)
        {
            stateManager.PushState(PlayerStateType.Jump);
        }
    }
    public void SetIsJumping(bool value)
    {
        isJumping = value;
    }
    private void FixedUpdate()
    {
        GroundCheck();
    }
    public void SetIsGrounded(bool value)
    {
        isGrounded = value;
    }
    private void GroundCheck()
    {
        RaycastHit hit;
        float rayDistance = 0.2f;
        Vector3 origin = playerTransform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayDistance))
        {
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
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (!isJumping && !isDashing && isGrounded)
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
                if      (stateInfo.IsName("Combo01_SwordShield"))   animator.SetTrigger("NextCombo");
                else if (stateInfo.IsName("Combo02_SwordShield"))   animator.SetTrigger("NextCombo");
            }
        }
    }
}
