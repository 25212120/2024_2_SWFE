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
            playerInputManager.SetIsJumping(true);
            rb.AddForce(jumpDirection, ForceMode.Impulse);
            animator.SetBool("isInAir", true);
            forceApplied = true;
        }

        GroundCheck();
    }

    public override void ExitState()
    {
        playerInputManager.SetIsJumping(false);
        animator.SetBool("isInAir", false);
    }

    public override void CheckTransitions()
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("JumpEnd_SwordShield") && stateInfo.normalizedTime >= 1.0f)
        {
            animator.SetTrigger("finishedJumping");
            stateManager.PopState();
        }
    }


    private bool isGrounded = false;
    private Vector3 jumpDirection;
    private float jumpForce = 30f;
    private float moveJumpForce = 5f;
    private void GroundCheck()
    {
        RaycastHit hit;
        float rayDistance = 0.2f;
        Vector3 origin = playerTransform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayDistance) == true)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void SetJumpDirection()
    {
        if (playerInputManager.moveInput == Vector2.zero)
            jumpDirection = Vector3.up * jumpForce;
        else
        {
            Vector3 moveDirection = new Vector3(playerInputManager.moveInput.x, 0, playerInputManager.moveInput.y);
            jumpDirection = (Vector3.up * jumpForce) + (moveDirection * moveJumpForce);
        }
    }
}