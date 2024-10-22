using UnityEngine;

public class DashState : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerCoolDownManager playerCoolDownmanager;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public DashState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator, Transform playerTransform, PlayerCoolDownManager playerCoolDownmanager)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerCoolDownmanager = playerCoolDownmanager; ;
        this.playerInputManager = inputManager;
        this.rb = rb;
    }


    public override void EnterState()
    {
        playerInputManager.SetIsDashing(true);
        SetDashDirection();
        RotatePlayer();
        animator.SetTrigger("Q_Key_Pressed");
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {

    }

    public override void ExitState()
    {
        playerInputManager.SetIsDashing(false);
        animator.SetTrigger("finishedDashing");
    }

    public override void CheckTransitions()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Dash") && stateInfo.normalizedTime >= 1.0f)
        {
            stateManager.PopState();
        }
    }

    // DashState Logic

    private Vector3 dashDirection;
    private float dashForce = 30f;
    private float rotationSpeed = 100f;

    private void SetDashDirection()
    {
        float horizontalInput = playerInputManager.moveInput.x;
        float verticalInput = playerInputManager.moveInput.y;
        Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (inputDirection == Vector3.zero) dashDirection = playerTransform.forward;
        else
        {
            Quaternion rotation = Quaternion.Euler(0f, -45f, 0f);
            dashDirection = (rotation * inputDirection).normalized;
        }
    }

    void RotatePlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(dashDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
