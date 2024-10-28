using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] public GameObject[] rightHand_Weapons;
    [SerializeField] public GameObject[] leftHand_Weapons;
    [SerializeField] public RuntimeAnimatorController[] player_animControllers;
    public int IndexSwapTo = 0;
    public int currentRightHandIndex;
    public int currentLeftHandIndex;
    public int previousRightHandIndex;
    public int previousLeftHandIndex;

    public bool wantToCheckGround = true;

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
    public bool isSwapping = false;
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

        currentRightHandIndex = 0;
        currentLeftHandIndex = 0;
    }

    private void Update()
    {
        leftButton_Pressed = false;
        F_Key_Pressed = false;
    }
    private void FixedUpdate()
    {
        if (wantToCheckGround)
        {
            GroundCheck();
        }
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

        playerInput.PlayerAction.WeaponSkill.performed += OnWeaponSkillPerformed;

        playerInput.PlayerAction.Interaction.performed += OnInteractionPerformed;

        playerInput.WeaponSwap.SwordAndShield.performed += OnSwapToSwordAndSheildPerformed;
        playerInput.WeaponSwap.SingleTwoHandeSword.performed += OnSwapToSingleTwoHandeSwordPerformed;
        playerInput.WeaponSwap.DoubleSwords.performed += OnSwapToDoubleSwordsPerformed;
        playerInput.WeaponSwap.BowAndArrow.performed += OnSwapToBowAndArrowPerformed;
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

        playerInput.WeaponSwap.SwordAndShield.performed -= OnSwapToSwordAndSheildPerformed;
        playerInput.WeaponSwap.SingleTwoHandeSword.performed -= OnSwapToSingleTwoHandeSwordPerformed;
        playerInput.WeaponSwap.DoubleSwords.performed -= OnSwapToDoubleSwordsPerformed;
        playerInput.WeaponSwap.BowAndArrow.performed -= OnSwapToSwordAndSheildPerformed;
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
        if (!isDashing && playerCoolDown.CanDash() && isGrounded && !isJumping && !isAttacking && !isInteracting && !isSwapping)
        {
            stateManager.PushState(PlayerStateType.Dash);
        }
    } 
    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (!isJumping && !isDashing && isGrounded && !isAttacking && !isInteracting && !isSwapping)
        {
            stateManager.PushState(PlayerStateType.Jump);
        }
    }

    public Queue<string> inputQueue = new Queue<string>();
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {

        if (!isJumping && !isDashing && isGrounded && !isInteracting && !isSwapping)
        {
            leftButton_Pressed = true;

            if (!isAttacking)
            {
                stateManager.PushState(PlayerStateType.Attack);
                animator.SetTrigger("leftButton_Pressed");
            }
            else if (inputQueue.Count < 1)
            {
                inputQueue.Enqueue("NextCombo");
            }
        }
    }
    private void OnWeaponSkillPerformed(InputAction.CallbackContext ctx)
    {
        if (playerCoolDown.CanUseWeaponSkill(currentRightHandIndex) && !isJumping && !isDashing && isGrounded && !isInteracting && !isSwapping && !isAttacking)
        {
            stateManager.PushState(PlayerStateType.WeaponSkill);
            animator.SetTrigger("rightButton_Pressed");
        }
    }
    private void OnInteractionPerformed(InputAction.CallbackContext ctx)
    {
        if (!isDashing && isGrounded && !isJumping && !isAttacking && !isInteracting && !isSwapping)
        {
            stateManager.PushState(PlayerStateType.Interaction);
        }
    }

    private void OnSwapToSwordAndSheildPerformed(InputAction.CallbackContext ctx)
    {
        if (!isDashing && isGrounded && !isJumping && !isAttacking && !isInteracting && !isSwapping)
        {
            IndexSwapTo = 0;
            stateManager.PushState(PlayerStateType.WeaponSwap);
        }
    }
    private void OnSwapToSingleTwoHandeSwordPerformed(InputAction.CallbackContext ctx)
    {
        if (!isDashing && isGrounded && !isJumping && !isAttacking && !isInteracting && !isSwapping)
        {
            IndexSwapTo = 1;
            stateManager.PushState(PlayerStateType.WeaponSwap);
        }
    }
    private void OnSwapToDoubleSwordsPerformed(InputAction.CallbackContext ctx)
    {
        if (!isDashing && isGrounded && !isJumping && !isAttacking && !isInteracting && !isSwapping)
        {
            IndexSwapTo = 2;
            stateManager.PushState(PlayerStateType.WeaponSwap);
        }
    }
    private void OnSwapToBowAndArrowPerformed(InputAction.CallbackContext ctx)
    {
        if (!isDashing && isGrounded && !isJumping && !isAttacking && !isInteracting && !isSwapping)
        {
            IndexSwapTo = 3;
            stateManager.PushState(PlayerStateType.WeaponSwap);
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
