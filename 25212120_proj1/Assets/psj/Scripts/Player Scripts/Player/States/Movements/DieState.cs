using UnityEngine;

public class DieState : BaseState<PlayerStateType>
{
    private Animator animator;
    private PlayerInputManager playerInputManager;

    public DieState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Animator animator) : base(key, stateManager)
    {
        this.animator = animator;
        this.playerInputManager = inputManager;
    }

    public override void EnterState()
    {
        playerInputManager.isDead = true;
        playerInputManager.enabled = false;
        animator.SetTrigger("dead");
        playerInputManager.gameObject.layer = LayerMask.NameToLayer("Dead");
        playerInputManager.GetComponent<Collider>().enabled = false;
        Debug.Log("Player Dead");
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
    }
}