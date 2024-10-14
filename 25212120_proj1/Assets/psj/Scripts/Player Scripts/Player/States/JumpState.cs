using UnityEngine;

public class JumpState : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public JumpState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator, Transform playerTransform)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerInputManager = inputManager;
        this.rb = rb;
    }

    private bool forceApplied;

    public override void EnterState()
    {
        // animation parameters
        animator.SetTrigger("spaceKey_Pressed");
        animator.SetBool("isInAir", true);

        // script variables
        playerInputManager.SetIsJumping(true);
        SetJumpDirection();


        forceApplied = false;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        if (forceApplied == false)
        {
            rb.AddForce(jumpDirection, ForceMode.VelocityChange);
            forceApplied = true;
        }

        if (playerInputManager.isGrounded == false)
        {
            MaintainHorizontalVelocity();
        }
    }

    public override void ExitState()
    {
        playerInputManager.SetIsJumping(false);
        animator.ResetTrigger("spaceKey_Pressed");
    }

    public override void CheckTransitions()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (playerInputManager.isGrounded) animator.SetBool("isInAir", false);

        if (stateInfo.IsName("JumpEnd_SwordShield") && stateInfo.normalizedTime >= 0.9f)
        {
            animator.SetBool("finishedJumping", true);
            stateManager.PopState();
        }
    }


    // JumpState Logic

    private Vector3 jumpDirection;
    private float jumpForce = 40f;
    private Vector3 initialHorizontalVelocity;

    // 점프 직전 방향으로 방향 설정
    private void SetJumpDirection()
    {
        Vector3 currentVelocity = rb.velocity;
        currentVelocity.y = 0;

        initialHorizontalVelocity = currentVelocity;

        jumpDirection = Vector3.up * jumpForce;
    }

    // 점프 직전 수평 이동속도 유지
    private void MaintainHorizontalVelocity()
    {
        Vector3 currentVelocity = rb.velocity;
        currentVelocity.y = 0;

        Vector3 desiredVelocity = initialHorizontalVelocity;

        Vector3 velocityDifference = desiredVelocity - currentVelocity;

        rb.AddForce(velocityDifference, ForceMode.VelocityChange);
    }

}