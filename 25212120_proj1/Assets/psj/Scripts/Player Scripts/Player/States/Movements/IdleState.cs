using UnityEngine;

public class IdleState : BaseState<PlayerStateType>
{
    private Animator animator;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public IdleState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator) : base(key, stateManager)
    {
        this.playerInputManager = inputManager;
        this.rb = rb;
        this.animator = animator;
    }

    public override void EnterState()
    {
        rb.velocity = Vector3.zero;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
    }

    public override void CheckTransitions()
    {
        if (playerInputManager.moveInput != Vector2.zero)
        {
            stateManager.ChangeState(PlayerStateType.Walk);
        }

    }
}