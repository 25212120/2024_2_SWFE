using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.EventSystems;

public class WalkState : BaseState<PlayerStateType>
{
    private PlayerStateMachine player;
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private Rigidbody rb;

    public WalkState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator) : base(key, stateManager)
    {
        this.playerInputManager = inputManager;
        this.rb = rb;
        this.animator = animator;
    }

    public override void EnterState()
    {
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        MovePlayer();
        RotatePlayer();
    }

    public override void ExitState()
    {
    }

    public override void CheckTransitions()
    {
        if (playerInputManager.moveInput == Vector2.zero)
        {
            stateManager.ChangeState(PlayerStateType.Idle);
        }
        else if (playerInputManager.leftShiftKey_Pressed == true)
        {
            stateManager.ChangeState(PlayerStateType.Sprint);
        }
    }


    // WalkState Logic

    private float walkSpeed = 70f;
    private float rotationSpeed = 7f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 inputDirection;
    private Vector3 moveDirection;


    void MovePlayer()
    {
        float horizontalInput = playerInputManager.moveInput.x;
        float verticalInput = playerInputManager.moveInput.y;
        inputDirection = new Vector3(horizontalInput, 0, verticalInput);
        Quaternion rotation = Quaternion.Euler(0f, -45f, 0f);
        moveDirection = rotation * inputDirection;

        rb.AddForce(moveDirection.normalized * walkSpeed);
    }

    void RotatePlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
