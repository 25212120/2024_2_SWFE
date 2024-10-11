using UnityEngine;

public class IdleState : BaseState<PlayerStateType>
{
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public IdleState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb) : base(key, stateManager)
    {
        this.playerInputManager = inputManager;
        this.rb = rb;
    }

    public override void EnterState()
    {
        Debug.Log("Entered IdleState");
        // Idle �ִϸ��̼� ����
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        Debug.Log("Exiting IdleState");
        // Idle �ִϸ��̼� ����
    }

    public override void CheckTransitions()
    {
        if (playerInputManager.moveInput != Vector2.zero)
        {
            stateManager.ChangeState(PlayerStateType.Walk);
        }
    }
}