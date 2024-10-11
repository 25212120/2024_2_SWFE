using UnityEngine;
using UnityEngine.EventSystems;

public class DashState : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public DashState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator, Transform playerTransform)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerInputManager = inputManager;
        this.rb = rb;
    }


    public override void EnterState()
    {
        playerInputManager.SetIsRolling(true);
        SetRollDirection();
        RotatePlayer();
        animator.SetTrigger("F_Key_Pressed");
        rb.AddForce(rollDirection * rollForce, ForceMode.VelocityChange);
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {

    }

    public override void ExitState()
    {
        playerInputManager.SetIsRolling(false);
        animator.SetTrigger("finishedDashing");
    }

    public override void CheckTransitions()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Dash_SwordShield") && stateInfo.normalizedTime >= 1.0f)
        {
            stateManager.PopState();
        }
    }

    private Vector3 rollDirection;
    private float rollForce = 40f;
    private float rotationSpeed = 100f;

    private void SetRollDirection()
    {
        float horizontalInput = playerInputManager.moveInput.x;
        float verticalInput = playerInputManager.moveInput.y;
        Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (inputDirection == Vector3.zero) rollDirection = playerTransform.forward;
        else
        {
            Quaternion rotation = Quaternion.Euler(0f, -45f, 0f);
            rollDirection = (rotation * inputDirection).normalized;
        }
    }

    void RotatePlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(rollDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
