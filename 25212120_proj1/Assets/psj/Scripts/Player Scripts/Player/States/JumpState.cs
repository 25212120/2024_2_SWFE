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
        animator.SetTrigger("spaceKey_Pressed");
        animator.SetBool("isInAir", true);
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

        GroundCheck();

        if (playerInputManager.isGrounded == false)
        {
            MaintainHorizontalVelocity();
        }


    }

    public override void ExitState()
    {
        playerInputManager.SetIsJumping(false);
        playerInputManager.SetIsGrounded(true);
        animator.ResetTrigger("spaceKey_Pressed");
        animator.ResetTrigger("finishedJumping");
        animator.SetBool("isInAir", false);
    }

    public override void CheckTransitions()
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (playerInputManager.isGrounded && !stateInfo.IsName("JumpEnd_SwordShield"))  animator.SetTrigger("finishedJumping");

        if (stateInfo.IsName("JumpEnd_SwordShield") && stateInfo.normalizedTime >= 1.0f)    stateManager.PopState();
    }


    private Vector3 jumpDirection;
    private float jumpForce = 40f;

    // 땅 체크 ( 중앙관리? )
    private void GroundCheck()
    {
        RaycastHit hit;
        float rayDistance = 0.3f;
        Vector3 origin = playerTransform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                if (playerInputManager.isGrounded == false)
                {
                    playerInputManager.SetIsGrounded(true);
                    animator.SetBool("isInAir", false);
                }
            }
        }
        else
        {
            if (playerInputManager.isGrounded)
            {
                playerInputManager.SetIsGrounded(false);
                animator.SetBool("isInAir", true);
            }
        }
    }

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