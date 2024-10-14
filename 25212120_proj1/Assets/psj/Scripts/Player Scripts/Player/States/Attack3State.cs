using UnityEngine;

public class Attack3State : BaseState<PlayerStateType>
{
    private Animator animator;
    private Transform playerTransform;
    private PlayerInputManager playerInputManager;
    private Rigidbody rb;

    public Attack3State(PlayerStateType key, StateManager<PlayerStateType> stateManager, PlayerInputManager inputManager, Rigidbody rb, Animator animator, Transform playerTransform)
            : base(key, stateManager)
    {
        this.playerTransform = playerTransform;
        this.animator = animator;
        this.playerInputManager = inputManager;
        this.rb = rb;
    }


    public override void EnterState()
    {
        animator.SetTrigger("NextCombo");
        animator.ResetTrigger("finishedAttacking");
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {

    }

    public override void ExitState()
    {
        animator.ResetTrigger("NextCombo");
    }

    public override void CheckTransitions()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1;

        if (normalizedTime >= 0.95f)
        {
            playerInputManager.isAttacking = false;
            animator.SetTrigger("finishedAttacking");
            stateManager.PopState();
        }
    }
}