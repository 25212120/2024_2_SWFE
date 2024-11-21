using UnityEngine;

public class SprintState : BaseState<PlayerStateType>
{ 
    private PlayerInputManager playerInputManager;
    private Animator animator;
    private Rigidbody rb;

    public SprintState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator) : base(key, stateManager)
    {
        this.playerInputManager = inputManager;
        this.animator = animator;
        this.rb = rb;
    }

    public override void EnterState()
    {
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
        SprintPlayer();
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
        else if (playerInputManager.leftShiftKey_Pressed == false)
        {
            stateManager.ChangeState(PlayerStateType.Walk);
        }
    }

    // SprintState Logic
    private float maxSpeed = 10f;

    private float sprintSpeed = 100f;
    private float rotationSpeed = 10f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 inputDirection;
    private Vector3 moveDirection;

    void SprintPlayer()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            MovePlayer();
        }
        else
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void MovePlayer()
    {
        float horizontalInput = playerInputManager.moveInput.x;
        float verticalInput = playerInputManager.moveInput.y;
        inputDirection = new Vector3(horizontalInput, 0, verticalInput);
        Quaternion rotation = Quaternion.Euler(0f, -45f, 0f);
        moveDirection = rotation * inputDirection;

        rb.AddForce(moveDirection.normalized * sprintSpeed);
    }

    void LimitVelocity()
    {
        Vector3 velocity = rb.velocity;
        if(velocity.magnitude > maxSpeed)
        {
            rb.velocity = velocity.normalized * maxSpeed;
        }
    }

    void RotatePlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

}