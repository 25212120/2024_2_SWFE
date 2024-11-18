using System.Collections;
using UnityEngine;

public class HitState : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;

    public HitState(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Animator animator, Transform playerTransform) : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerInputManager = inputManager;
    }

    public override void EnterState()
    {
        if(playerInputManager.isDefending == true)
        {
            animator.SetTrigger("getHitWhileDefending");
            animator.ResetTrigger("getHitWhileDefending");
        }
        else
        {
            animator.SetTrigger("getHit");
            animator.ResetTrigger("getHit");
        }
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
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1.0f)
        {
            if (stateInfo.IsTag("GetHit"))
            {
                stateManager.PopState();
            }
        }
    }
}
