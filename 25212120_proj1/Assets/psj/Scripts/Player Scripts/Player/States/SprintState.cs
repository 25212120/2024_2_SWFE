using UnityEngine;

public class SprintState : BaseState<PlayerStateType>
{ 
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public SprintState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb) : base(key, stateManager)
    {
        this.playerInputManager = inputManager;
        this.rb = rb;
    }

    public override void EnterState()
    {
        Debug.Log("Entered SprintState");
        // Sprint �ִϸ��̼� ����
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
        MovePlayer();
    }

    public override void ExitState()
    {
        Debug.Log("Exiting SprintState");
        // Sprint �ִϸ��̼� ����
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

    private float sprintSpeed = 50f;

    void MovePlayer()
    {
        float horizontalInput = playerInputManager.moveInput.x;
        float verticalInput = playerInputManager.moveInput.y;
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        rb.AddForce(movement.normalized * sprintSpeed);
    }

}