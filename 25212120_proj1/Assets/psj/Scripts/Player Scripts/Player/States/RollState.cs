using UnityEngine;

public class RollState : BaseState<PlayerStateType>
{
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public RollState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb)
            : base(key, stateManager)
    {
        this.playerInputManager = inputManager;
        this.rb = rb;
    }

    public override void EnterState()
    {
        Debug.Log("Entered RollState");
        // Roll �ִϸ��̼� ����
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        Debug.Log("Exiting RollState");
        // Roll �ִϸ��̼� ����
    }
    public override void CheckTransitions()
    {
        
    }
}